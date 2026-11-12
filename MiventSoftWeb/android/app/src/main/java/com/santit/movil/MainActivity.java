package com.santit.movil;

import android.Manifest;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.DownloadManager;
import android.content.ActivityNotFoundException;
import android.content.ClipData;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Looper;
import android.os.SystemClock;
import android.print.PrintAttributes;
import android.print.PrintManager;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.view.inputmethod.EditorInfo;
import android.webkit.CookieManager;
import android.webkit.JavascriptInterface;
import android.webkit.JsResult;
import android.webkit.SslErrorHandler;
import android.webkit.URLUtil;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;
import android.webkit.WebResourceError;
import android.webkit.WebResourceRequest;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.net.http.SslError;
import android.webkit.RenderProcessGoneDetail;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URISyntaxException;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 * Sanbit movil: contenedor de Sanbit web (WebView) con la IP/puerto del servidor configurables.
 *
 * Puente JS "SantitApp" (lo usa la web en src/global/appMovil.js):
 *   configurarServidor(), reintentar(), servidorActual(), version(),
 *   imprimir(html, titulo, horizontal), guardarArchivo(nombre, contenido, mime), obtenerUbicacion(id)
 */
public class MainActivity extends Activity {

    private static final int REQ_ARCHIVO = 10;
    private static final int REQ_GUARDAR = 11;
    private static final int REQ_UBICACION = 12;
    private static final long TIEMPO_MAX_CONEXION_MS = 25000;
    private static final long TIEMPO_MAX_UBICACION_MS = 15000;
    private static final float PRECISION_BUENA_M = 30f;
    private static final String BASE_LOCAL = "file:///android_asset/";

    private final Handler ui = new Handler(Looper.getMainLooper());
    private final ExecutorService fondo = Executors.newSingleThreadExecutor();

    private WebView web;
    private ProgressBar barra;
    private volatile Servidor servidor;
    private String plantillaLocal;

    private boolean paginaLocal;       // se muestra inicio.html (bienvenida / sin conexion)
    private boolean limpiarHistorial;  // al cargar el servidor desde cero, "atras" no vuelve a paginas viejas
    private boolean webVisible;
    private long ultimoAtras;

    private AlertDialog dialogoServidor;
    private ValueCallback<Uri[]> archivosCallback;
    private String guardarContenido;
    private WebView webImpresion;

    private String ubicacionId;
    private LocationListener ubicacionListener;
    private Location mejorUbicacion;

    private final Runnable vencioConexion = () -> {
        if (paginaLocal || isFinishing()) return;
        web.stopLoading();
        mostrarLocal("Sin conexión con el servidor",
                "El servidor no respondió a tiempo. Verifique que la PC esté encendida, que Sanbit esté iniciado y que el teléfono esté en la misma red Wi-Fi.");
    };

    private final Runnable vencioUbicacion = () -> {
        if (ubicacionId == null) return;
        if (mejorUbicacion != null) responderUbicacion(mejorUbicacion);
        else responderErrorUbicacion(3, "No se pudo obtener la ubicación. Intente al aire libre o cerca de una ventana.");
    };

    // ------------------------------------------------------------------ ciclo de vida

    @Override
    protected void onCreate(Bundle estado) {
        super.onCreate(estado);

        FrameLayout raiz = new FrameLayout(this);
        web = new WebView(this);
        web.setVisibility(View.INVISIBLE); // hasta que pinte, se ve la pantalla de arranque
        raiz.addView(web, new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));

        barra = new ProgressBar(this, null, android.R.attr.progressBarStyleHorizontal);
        barra.setMax(100);
        barra.setIndeterminate(false);
        barra.setProgressTintList(ColorStateList.valueOf(getColor(R.color.marca)));
        barra.setProgressBackgroundTintList(ColorStateList.valueOf(Color.TRANSPARENT));
        barra.setVisibility(View.GONE);
        raiz.addView(barra, new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, dp(4), Gravity.TOP));

        setContentView(raiz);
        configurarWeb();

        servidor = Servidor.leer(this);
        if (servidor == null) {
            mostrarBienvenida();
            abrirDialogoServidor();
        } else {
            cargarServidor();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (web != null) web.onResume();
    }

    @Override
    protected void onPause() {
        if (web != null) web.onPause();
        CookieManager.getInstance().flush();
        super.onPause();
    }

    @Override
    protected void onDestroy() {
        detenerUbicacion();
        ui.removeCallbacksAndMessages(null);
        fondo.shutdownNow();
        if (dialogoServidor != null) dialogoServidor.dismiss();
        if (webImpresion != null) webImpresion.destroy();
        if (web != null) {
            if (web.getParent() instanceof ViewGroup) ((ViewGroup) web.getParent()).removeView(web);
            web.destroy();
            web = null;
        }
        super.onDestroy();
    }

    @Override
    public void onBackPressed() {
        if (dialogoServidor != null && dialogoServidor.isShowing()) {
            dialogoServidor.dismiss();
            return;
        }
        if (web == null) {
            super.onBackPressed();
            return;
        }
        if (!paginaLocal && web.canGoBack()) {
            web.goBack();
            return;
        }
        long ahora = SystemClock.elapsedRealtime();
        if (ahora - ultimoAtras < 2000) {
            super.onBackPressed();
            return;
        }
        ultimoAtras = ahora;
        aviso("Presione atrás otra vez para salir");
    }

    // ------------------------------------------------------------------ WebView

    private void configurarWeb() {
        WebSettings s = web.getSettings();
        s.setJavaScriptEnabled(true);
        s.setDomStorageEnabled(true);
        s.setAllowFileAccess(false);
        s.setSupportZoom(false);
        s.setBuiltInZoomControls(false);
        s.setDisplayZoomControls(false);
        s.setTextZoom(100); // la letra grande del sistema no desarma las pantallas
        s.setUseWideViewPort(true);
        s.setLoadWithOverviewMode(false);
        s.setJavaScriptCanOpenWindowsAutomatically(false); // window.open devuelve null: la web imprime por el puente
        s.setSupportMultipleWindows(false);
        s.setMediaPlaybackRequiresUserGesture(true);
        s.setMixedContentMode(WebSettings.MIXED_CONTENT_COMPATIBILITY_MODE);
        s.setUserAgentString(s.getUserAgentString() + " SantitApp/" + BuildConfig.VERSION_NAME);

        CookieManager.getInstance().setAcceptCookie(true);
        WebView.setWebContentsDebuggingEnabled(BuildConfig.DEBUG);

        web.setOverScrollMode(View.OVER_SCROLL_NEVER);
        web.setBackgroundColor(Color.WHITE);
        web.addJavascriptInterface(new Puente(), "SantitApp");
        web.setWebViewClient(new Navegacion());
        web.setWebChromeClient(new Chrome());
        web.setDownloadListener(this::descargar);
    }

    private void cargarServidor() {
        if (servidor == null) {
            mostrarBienvenida();
            abrirDialogoServidor();
            return;
        }
        paginaLocal = false;
        limpiarHistorial = true;
        ui.removeCallbacks(vencioConexion);
        ui.postDelayed(vencioConexion, TIEMPO_MAX_CONEXION_MS);
        web.loadUrl(servidor.url());
    }

    private void mostrarBienvenida() {
        mostrarLocal("Bienvenido", "Para comenzar, configure la IP y el puerto de la PC donde corre Sanbit.");
    }

    /** Pagina local (assets/inicio.html): bienvenida o error de conexion, con Reintentar / Configurar servidor. */
    private void mostrarLocal(String titulo, String mensaje) {
        ui.removeCallbacks(vencioConexion);
        paginaLocal = true;
        Servidor s = servidor;
        String html = plantilla()
                .replace("{{TITULO}}", esc(titulo))
                .replace("{{MENSAJE}}", esc(mensaje))
                .replace("{{SERVIDOR}}", s == null ? "" : esc(s.url()))
                .replace("{{CLASE_SERVIDOR}}", s == null ? "oculto" : "")
                .replace("{{CLASE_REINTENTAR}}", s == null ? "oculto" : "")
                .replace("{{CLASE_CONFIGURAR}}", s == null ? "pri" : "sec");
        web.loadDataWithBaseURL(BASE_LOCAL, html, "text/html", "UTF-8", null);
    }

    private String plantilla() {
        if (plantillaLocal == null) {
            try (InputStream in = getAssets().open("inicio.html")) {
                ByteArrayOutputStream out = new ByteArrayOutputStream();
                byte[] buf = new byte[4096];
                int n;
                while ((n = in.read(buf)) > 0) out.write(buf, 0, n);
                plantillaLocal = new String(out.toByteArray(), StandardCharsets.UTF_8);
            } catch (IOException e) {
                plantillaLocal = "<html><body style='font-family:sans-serif;padding:24px'><h2>{{TITULO}}</h2><p>{{MENSAJE}}</p>"
                        + "<p>{{SERVIDOR}}</p><button onclick='SantitApp.reintentar()' class='{{CLASE_REINTENTAR}}'>Reintentar</button> "
                        + "<button onclick='SantitApp.configurarServidor()' class='{{CLASE_CONFIGURAR}} {{CLASE_SERVIDOR}}'>Configurar servidor</button></body></html>";
            }
        }
        return plantillaLocal;
    }

    private void mostrarWeb() {
        if (webVisible) return;
        webVisible = true;
        web.setVisibility(View.VISIBLE);
        getWindow().setBackgroundDrawable(new ColorDrawable(Color.WHITE));
    }

    private final class Navegacion extends WebViewClient {
        @Override
        public boolean shouldOverrideUrlLoading(WebView v, WebResourceRequest r) {
            return abrirFuera(r.getUrl());
        }

        @Override
        public void onPageCommitVisible(WebView v, String url) {
            if (!paginaLocal) ui.removeCallbacks(vencioConexion);
            mostrarWeb();
        }

        @Override
        public void onPageFinished(WebView v, String url) {
            mostrarWeb();
            if (!paginaLocal && servidor != null && servidor.esDelServidor(url)) {
                ui.removeCallbacks(vencioConexion);
                if (limpiarHistorial) {
                    limpiarHistorial = false;
                    v.clearHistory();
                }
            }
        }

        @Override
        public void onReceivedError(WebView v, WebResourceRequest r, WebResourceError e) {
            if (!r.isForMainFrame() || paginaLocal) return;
            mostrarLocal("Sin conexión con el servidor", motivoError(e.getErrorCode(), String.valueOf(e.getDescription())));
        }

        @Override
        public void onReceivedSslError(WebView v, SslErrorHandler h, SslError error) {
            h.cancel(); // nunca se aceptan certificados invalidos
            if (!paginaLocal) {
                mostrarLocal("Certificado no válido",
                        "El certificado HTTPS del servidor no es de confianza. Use un certificado válido o desmarque \"Usar HTTPS\".");
            }
        }

        @Override
        public boolean onRenderProcessGone(WebView v, RenderProcessGoneDetail detalle) {
            // El motor de la web se cerro (falta de memoria): se reinicia la pantalla en lugar de cerrar la app
            if (v.getParent() instanceof ViewGroup) ((ViewGroup) v.getParent()).removeView(v);
            v.destroy();
            if (v == webImpresion) webImpresion = null;
            if (v == web) {
                web = null;
                recreate();
            }
            return true;
        }
    }

    private String motivoError(int codigo, String descripcion) {
        String destino = servidor == null ? "el servidor" : servidor.mostrar();
        switch (codigo) {
            case WebViewClient.ERROR_HOST_LOOKUP:
                return "No se encontró " + destino + ". Verifique la IP configurada.";
            case WebViewClient.ERROR_CONNECT:
                return "No se pudo conectar con " + destino + ". Verifique que Sanbit esté iniciado, el puerto y que el teléfono esté en la misma red Wi-Fi.";
            case WebViewClient.ERROR_TIMEOUT:
                return destino + " no responde. Verifique que la PC esté encendida y en la misma red.";
            default:
                return "No se pudo abrir " + destino + " (" + descripcion + ").";
        }
    }

    /** Enlaces a otros sitios (WhatsApp, mapas, telefono...) se abren con la app del telefono. */
    private boolean abrirFuera(Uri u) {
        if (servidor != null && servidor.esDelServidor(u)) return false;
        String esquema = u.getScheme() == null ? "" : u.getScheme().toLowerCase(Locale.ROOT);
        if (esquema.equals("about")) return false;
        if (esquema.equals("javascript") || esquema.equals("file") || esquema.equals("content")
                || esquema.equals("data") || esquema.equals("blob")) return true;
        try {
            Intent i;
            if (esquema.equals("intent")) {
                i = Intent.parseUri(u.toString(), Intent.URI_INTENT_SCHEME);
                i.setComponent(null);
                i.setSelector(null);
            } else {
                i = new Intent(Intent.ACTION_VIEW, u);
            }
            i.addCategory(Intent.CATEGORY_BROWSABLE);
            startActivity(i);
        } catch (ActivityNotFoundException | URISyntaxException e) {
            aviso("No hay una aplicación para abrir este enlace");
        }
        return true;
    }

    private final class Chrome extends WebChromeClient {
        @Override
        public void onProgressChanged(WebView v, int progreso) {
            barra.setProgress(progreso);
            barra.setVisibility(progreso < 100 && !paginaLocal ? View.VISIBLE : View.GONE);
        }

        @Override
        public boolean onShowFileChooser(WebView v, ValueCallback<Uri[]> callback, FileChooserParams params) {
            if (archivosCallback != null) archivosCallback.onReceiveValue(null);
            archivosCallback = callback;
            try {
                startActivityForResult(Intent.createChooser(intentArchivos(params), "Seleccionar archivo"), REQ_ARCHIVO);
            } catch (ActivityNotFoundException e) {
                archivosCallback = null;
                callback.onReceiveValue(null);
                aviso("No hay una aplicación para elegir archivos");
            }
            return true;
        }

        // alert / confirm de la web con dialogos de la app (sin "La pagina en http://... dice")
        @Override
        public boolean onJsAlert(WebView v, String url, String mensaje, JsResult r) {
            new AlertDialog.Builder(MainActivity.this)
                    .setMessage(mensaje)
                    .setPositiveButton("Aceptar", (d, w) -> r.confirm())
                    .setOnCancelListener(d -> r.cancel())
                    .show();
            return true;
        }

        @Override
        public boolean onJsConfirm(WebView v, String url, String mensaje, JsResult r) {
            new AlertDialog.Builder(MainActivity.this)
                    .setMessage(mensaje)
                    .setPositiveButton("Aceptar", (d, w) -> r.confirm())
                    .setNegativeButton("Cancelar", (d, w) -> r.cancel())
                    .setOnCancelListener(d -> r.cancel())
                    .show();
            return true;
        }
    }

    private static Intent intentArchivos(WebChromeClient.FileChooserParams params) {
        List<String> tipos = new ArrayList<>();
        for (String t : params.getAcceptTypes()) {
            if (t == null) continue;
            for (String x : t.split(",")) {
                x = x.trim().toLowerCase(Locale.ROOT);
                if (x.contains("/")) tipos.add(x);
            }
        }
        boolean soloImagenes = !tipos.isEmpty();
        for (String t : tipos) soloImagenes &= t.startsWith("image/");

        Intent i = new Intent(Intent.ACTION_GET_CONTENT);
        i.addCategory(Intent.CATEGORY_OPENABLE);
        i.setType(soloImagenes ? "image/*" : "*/*");
        if (!tipos.isEmpty()) i.putExtra(Intent.EXTRA_MIME_TYPES, tipos.toArray(new String[0]));
        if (params.getMode() == WebChromeClient.FileChooserParams.MODE_OPEN_MULTIPLE) i.putExtra(Intent.EXTRA_ALLOW_MULTIPLE, true);
        return i;
    }

    // ------------------------------------------------------------------ popup del servidor

    private void abrirDialogoServidor() {
        if (dialogoServidor != null && dialogoServidor.isShowing()) return;

        View vista = getLayoutInflater().inflate(R.layout.dialogo_servidor, null);
        EditText txtIp = vista.findViewById(R.id.txtIp);
        EditText txtPuerto = vista.findViewById(R.id.txtPuerto);
        CheckBox chkHttps = vista.findViewById(R.id.chkHttps);
        ProgressBar progreso = vista.findViewById(R.id.progreso);
        TextView lblEstado = vista.findViewById(R.id.lblEstado);

        Servidor actual = servidor;
        if (actual != null) {
            txtIp.setText(actual.host);
            txtPuerto.setText(String.valueOf(actual.puerto));
            chkHttps.setChecked(actual.https);
        } else {
            txtPuerto.setText(String.valueOf(Servidor.PUERTO_DEFECTO));
        }
        txtIp.setSelection(txtIp.length());
        txtIp.setImeOptions(EditorInfo.IME_ACTION_NEXT);
        txtPuerto.setImeOptions(EditorInfo.IME_ACTION_DONE);

        AlertDialog d = new AlertDialog.Builder(this)
                .setTitle("Configurar servidor")
                .setView(vista)
                .setPositiveButton("Guardar", null)
                .setNegativeButton("Cancelar", null)
                .setNeutralButton("Guardar igual", null)
                .create();

        d.setOnShowListener(x -> {
            Button btnGuardar = d.getButton(AlertDialog.BUTTON_POSITIVE);
            Button btnIgual = d.getButton(AlertDialog.BUTTON_NEUTRAL);
            btnIgual.setVisibility(View.GONE); // solo si la prueba de conexion falla

            btnGuardar.setOnClickListener(b -> {
                Servidor nuevo;
                try {
                    nuevo = Servidor.desdeTexto(txtIp.getText().toString(), txtPuerto.getText().toString(), chkHttps.isChecked());
                } catch (IllegalArgumentException e) {
                    estadoDialogo(lblEstado, e.getMessage(), true);
                    return;
                }
                btnGuardar.setEnabled(false);
                btnIgual.setVisibility(View.GONE);
                progreso.setVisibility(View.VISIBLE);
                estadoDialogo(lblEstado, "Probando conexión con " + nuevo.url() + " ...", false);
                fondo.execute(() -> {
                    String error = nuevo.probar();
                    ui.post(() -> {
                        if (!d.isShowing()) return;
                        progreso.setVisibility(View.GONE);
                        btnGuardar.setEnabled(true);
                        if (error == null) {
                            d.dismiss();
                            aplicarServidor(nuevo);
                        } else {
                            estadoDialogo(lblEstado, error, true);
                            btnIgual.setVisibility(View.VISIBLE);
                        }
                    });
                });
            });

            btnIgual.setOnClickListener(b -> {
                try {
                    Servidor nuevo = Servidor.desdeTexto(txtIp.getText().toString(), txtPuerto.getText().toString(), chkHttps.isChecked());
                    d.dismiss();
                    aplicarServidor(nuevo);
                } catch (IllegalArgumentException e) {
                    estadoDialogo(lblEstado, e.getMessage(), true);
                }
            });

            txtPuerto.setOnEditorActionListener((t, accion, ev) -> {
                if (accion != EditorInfo.IME_ACTION_DONE) return false;
                if (btnGuardar.isEnabled()) btnGuardar.performClick();
                return true;
            });
            txtIp.requestFocus();
        });
        d.setOnDismissListener(x -> dialogoServidor = null);
        if (d.getWindow() != null) d.getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE);
        dialogoServidor = d;
        d.show();
    }

    private void estadoDialogo(TextView lbl, String texto, boolean error) {
        lbl.setText(texto);
        lbl.setTextColor(error ? Color.parseColor("#B91C1C") : Color.parseColor("#5B6472"));
    }

    private void aplicarServidor(Servidor nuevo) {
        nuevo.guardar(this);
        servidor = nuevo;
        aviso("Servidor: " + nuevo.mostrar());
        cargarServidor();
    }

    // ------------------------------------------------------------------ puente JS

    private final class Puente {
        @JavascriptInterface
        public void configurarServidor() {
            enUi(MainActivity.this::abrirDialogoServidor);
        }

        @JavascriptInterface
        public void reintentar() {
            enUi(MainActivity.this::cargarServidor);
        }

        @JavascriptInterface
        public String servidorActual() {
            Servidor s = servidor;
            return s == null ? "" : s.mostrar();
        }

        @JavascriptInterface
        public String version() {
            return BuildConfig.VERSION_NAME;
        }

        @JavascriptInterface
        public void imprimir(String html, String titulo, boolean horizontal) {
            enUi(() -> imprimirHtml(html, titulo, horizontal));
        }

        @JavascriptInterface
        public void guardarArchivo(String nombre, String contenido, String mime) {
            enUi(() -> pedirGuardar(nombre, contenido, mime));
        }

        @JavascriptInterface
        public void obtenerUbicacion(String id) {
            enUi(() -> pedirUbicacion(id));
        }
    }

    /** Los metodos del puente solo actuan si la pagina actual es la local o la del servidor configurado. */
    private void enUi(Runnable accion) {
        ui.post(() -> {
            if (isFinishing() || isDestroyed() || web == null) return;
            if (!paginaLocal && (servidor == null || !servidor.esDelServidor(web.getUrl()))) return;
            accion.run();
        });
    }

    // ------------------------------------------------------------------ imprimir

    /** Imprime (o guarda en PDF) el documento que arma la web, con el servicio de impresion de Android. */
    private void imprimirHtml(String html, String titulo, boolean horizontal) {
        if (html == null || html.isEmpty()) return;
        if (webImpresion != null) webImpresion.destroy();
        String nombre = titulo == null || titulo.trim().isEmpty() ? "Sanbit" : titulo.trim();

        WebView w = new WebView(this);
        w.getSettings().setJavaScriptEnabled(false);
        w.getSettings().setAllowFileAccess(false);
        w.setWebViewClient(new WebViewClient() {
            private boolean enviado;

            @Override
            public boolean shouldOverrideUrlLoading(WebView v, WebResourceRequest r) {
                return true;
            }

            @Override
            public void onPageFinished(WebView v, String url) {
                if (enviado || isFinishing()) return;
                enviado = true;
                PrintManager pm = (PrintManager) getSystemService(PRINT_SERVICE);
                PrintAttributes atributos = new PrintAttributes.Builder()
                        .setMediaSize(horizontal ? PrintAttributes.MediaSize.NA_LETTER.asLandscape() : PrintAttributes.MediaSize.NA_LETTER.asPortrait())
                        .setMinMargins(PrintAttributes.Margins.NO_MARGINS)
                        .build();
                try {
                    pm.print(nombre, v.createPrintDocumentAdapter(nombre), atributos);
                } catch (RuntimeException e) {
                    aviso("No se pudo abrir la impresión: " + e.getMessage());
                }
            }
        });
        webImpresion = w; // se mantiene viva mientras dura el trabajo de impresion
        Servidor s = servidor;
        w.loadDataWithBaseURL(s == null ? null : s.url(), html, "text/html", "UTF-8", null);
    }

    // ------------------------------------------------------------------ archivos

    /** Exportar CSV: el usuario elige donde guardarlo (sin permisos de almacenamiento). */
    private void pedirGuardar(String nombre, String contenido, String mime) {
        if (contenido == null) return;
        String tipo = mime == null || mime.trim().isEmpty() ? "text/plain" : mime.split(";")[0].trim();
        Intent i = new Intent(Intent.ACTION_CREATE_DOCUMENT);
        i.addCategory(Intent.CATEGORY_OPENABLE);
        i.setType(tipo);
        i.putExtra(Intent.EXTRA_TITLE, nombre == null || nombre.trim().isEmpty() ? "archivo" : nombre.trim());
        guardarContenido = contenido;
        try {
            startActivityForResult(i, REQ_GUARDAR);
        } catch (ActivityNotFoundException e) {
            guardarContenido = null;
            aviso("No se puede guardar archivos en este teléfono");
        }
    }

    @Override
    protected void onActivityResult(int req, int resultado, Intent datos) {
        if (req == REQ_ARCHIVO) {
            if (archivosCallback == null) return;
            Uri[] uris = null;
            if (resultado == RESULT_OK && datos != null) {
                ClipData clip = datos.getClipData();
                if (clip != null && clip.getItemCount() > 0) {
                    uris = new Uri[clip.getItemCount()];
                    for (int k = 0; k < uris.length; k++) uris[k] = clip.getItemAt(k).getUri();
                } else if (datos.getData() != null) {
                    uris = new Uri[]{datos.getData()};
                }
            }
            archivosCallback.onReceiveValue(uris);
            archivosCallback = null;
            return;
        }
        if (req == REQ_GUARDAR) {
            String contenido = guardarContenido;
            guardarContenido = null;
            if (resultado != RESULT_OK || datos == null || datos.getData() == null || contenido == null) return;
            try (OutputStream os = getContentResolver().openOutputStream(datos.getData(), "wt")) {
                if (os == null) throw new IOException("destino no disponible");
                os.write(contenido.getBytes(StandardCharsets.UTF_8));
                aviso("Archivo guardado");
            } catch (IOException | SecurityException e) {
                aviso("No se pudo guardar el archivo: " + e.getMessage());
            }
            return;
        }
        super.onActivityResult(req, resultado, datos);
    }

    /** Descargas http(s) del servidor (con la sesion) a la carpeta Descargas. */
    private void descargar(String url, String agente, String disposicion, String mime, long largo) {
        Uri u = Uri.parse(url);
        String esquema = u.getScheme() == null ? "" : u.getScheme().toLowerCase(Locale.ROOT);
        if (!esquema.equals("http") && !esquema.equals("https")) {
            aviso("Esta descarga no es compatible con la app");
            return;
        }
        String nombre = URLUtil.guessFileName(url, disposicion, mime);
        try {
            DownloadManager.Request r = new DownloadManager.Request(u);
            String cookies = CookieManager.getInstance().getCookie(url);
            if (cookies != null && servidor != null && servidor.esDelServidor(u)) r.addRequestHeader("Cookie", cookies);
            r.addRequestHeader("User-Agent", agente);
            r.setMimeType(mime);
            r.setTitle(nombre);
            r.setNotificationVisibility(DownloadManager.Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
                r.setDestinationInExternalPublicDir(Environment.DIRECTORY_DOWNLOADS, nombre);
            } else {
                r.setDestinationInExternalFilesDir(this, Environment.DIRECTORY_DOWNLOADS, nombre);
            }
            ((DownloadManager) getSystemService(DOWNLOAD_SERVICE)).enqueue(r);
            aviso("Descargando " + nombre);
        } catch (RuntimeException e) {
            aviso("No se pudo descargar: " + e.getMessage());
        }
    }

    // ------------------------------------------------------------------ ubicacion (GPS)
    // En http el WebView bloquea navigator.geolocation; la web lo reemplaza por este puente.

    private void pedirUbicacion(String id) {
        if (ubicacionId != null) responderErrorUbicacion(3, "Se inició otra lectura de ubicación");
        ubicacionId = id;
        if (tienePermiso(Manifest.permission.ACCESS_FINE_LOCATION) || tienePermiso(Manifest.permission.ACCESS_COARSE_LOCATION)) {
            iniciarUbicacion();
        } else {
            requestPermissions(new String[]{Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_COARSE_LOCATION}, REQ_UBICACION);
        }
    }

    @Override
    public void onRequestPermissionsResult(int req, String[] permisos, int[] resultados) {
        if (req != REQ_UBICACION) {
            super.onRequestPermissionsResult(req, permisos, resultados);
            return;
        }
        boolean concedido = false;
        for (int r : resultados) concedido |= r == PackageManager.PERMISSION_GRANTED;
        if (concedido) iniciarUbicacion();
        else responderErrorUbicacion(1, "Permiso de ubicación denegado. Puede habilitarlo en Ajustes > Aplicaciones > Sanbit.");
    }

    private void iniciarUbicacion() {
        if (ubicacionId == null) return;
        LocationManager lm = (LocationManager) getSystemService(LOCATION_SERVICE);
        List<String> proveedores = new ArrayList<>();
        if (tienePermiso(Manifest.permission.ACCESS_FINE_LOCATION) && lm.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
            proveedores.add(LocationManager.GPS_PROVIDER);
        }
        if (lm.isProviderEnabled(LocationManager.NETWORK_PROVIDER)) proveedores.add(LocationManager.NETWORK_PROVIDER);
        if (proveedores.isEmpty()) {
            responderErrorUbicacion(2, "Active la ubicación (GPS) del teléfono e intente de nuevo.");
            return;
        }

        mejorUbicacion = null;
        ubicacionListener = new LocationListener() {
            @Override
            public void onLocationChanged(Location l) {
                if (mejorUbicacion == null || l.getAccuracy() < mejorUbicacion.getAccuracy()) mejorUbicacion = l;
                if (l.getAccuracy() <= PRECISION_BUENA_M) responderUbicacion(l);
            }

            // En Android 7 a 10 estos metodos son abstractos: deben implementarse
            @Override
            public void onStatusChanged(String proveedor, int estado, Bundle extras) { }

            @Override
            public void onProviderEnabled(String proveedor) { }

            @Override
            public void onProviderDisabled(String proveedor) { }
        };
        try {
            for (String p : proveedores) lm.requestLocationUpdates(p, 1000, 0, ubicacionListener, Looper.getMainLooper());
        } catch (SecurityException | IllegalArgumentException e) {
            responderErrorUbicacion(2, "No se pudo leer la ubicación: " + e.getMessage());
            return;
        }
        ui.postDelayed(vencioUbicacion, TIEMPO_MAX_UBICACION_MS);
    }

    private void detenerUbicacion() {
        ui.removeCallbacks(vencioUbicacion);
        if (ubicacionListener != null) {
            ((LocationManager) getSystemService(LOCATION_SERVICE)).removeUpdates(ubicacionListener);
            ubicacionListener = null;
        }
        mejorUbicacion = null;
    }

    private void responderUbicacion(Location l) {
        String id = ubicacionId;
        ubicacionId = null;
        detenerUbicacion();
        if (id == null || web == null) return;
        web.evaluateJavascript("window.__santitUbicacion&&window.__santitUbicacion(" + JSONObject.quote(id) + ",0,"
                + l.getLatitude() + "," + l.getLongitude() + "," + l.getAccuracy() + ",'')", null);
    }

    private void responderErrorUbicacion(int codigo, String mensaje) {
        String id = ubicacionId;
        ubicacionId = null;
        detenerUbicacion();
        aviso(mensaje);
        if (id == null || web == null) return;
        web.evaluateJavascript("window.__santitUbicacion&&window.__santitUbicacion(" + JSONObject.quote(id) + "," + codigo
                + ",0,0,0," + JSONObject.quote(mensaje) + ")", null);
    }

    // ------------------------------------------------------------------ utilidades

    private boolean tienePermiso(String permiso) {
        return checkSelfPermission(permiso) == PackageManager.PERMISSION_GRANTED;
    }

    private void aviso(String texto) {
        Toast.makeText(this, texto, Toast.LENGTH_LONG).show();
    }

    private int dp(int valor) {
        return Math.round(valor * getResources().getDisplayMetrics().density);
    }

    private static String esc(String s) {
        if (s == null) return "";
        StringBuilder b = new StringBuilder(s.length());
        for (char c : s.toCharArray()) {
            switch (c) {
                case '&': b.append("&amp;"); break;
                case '<': b.append("&lt;"); break;
                case '>': b.append("&gt;"); break;
                case '"': b.append("&quot;"); break;
                case '\'': b.append("&#39;"); break;
                default: b.append(c);
            }
        }
        return b.toString();
    }
}
