package com.santit.movil;

import android.content.Context;
import android.content.SharedPreferences;
import android.net.Uri;

import java.io.IOException;
import java.io.InputStream;
import java.net.ConnectException;
import java.net.HttpURLConnection;
import java.net.SocketTimeoutException;
import java.net.URL;
import java.net.UnknownHostException;
import java.nio.charset.StandardCharsets;
import java.util.Locale;

import javax.net.ssl.SSLException;

/** Direccion del servidor de Sanbit (IP/nombre + puerto), guardada en el telefono. */
final class Servidor {
    static final int PUERTO_DEFECTO = 3001;
    private static final String PREFS = "servidor";
    private static final int TIEMPO_PRUEBA_MS = 6000;

    final String host;
    final int puerto;
    final boolean https;

    private Servidor(String host, int puerto, boolean https) {
        this.host = host;
        this.puerto = puerto;
        this.https = https;
    }

    /** Configuracion guardada, o null si todavia no se configuro. */
    static Servidor leer(Context c) {
        SharedPreferences p = c.getSharedPreferences(PREFS, Context.MODE_PRIVATE);
        String h = p.getString("host", "");
        if (h == null || h.isEmpty() || !esHostValido(h)) return null;
        int puerto = p.getInt("puerto", PUERTO_DEFECTO);
        if (puerto < 1 || puerto > 65535) puerto = PUERTO_DEFECTO;
        return new Servidor(h, puerto, p.getBoolean("https", false));
    }

    void guardar(Context c) {
        c.getSharedPreferences(PREFS, Context.MODE_PRIVATE).edit()
                .putString("host", host).putInt("puerto", puerto).putBoolean("https", https).apply();
    }

    String url() {
        return (https ? "https" : "http") + "://" + host + ":" + puerto + "/";
    }

    /** Texto corto para mostrar en el login: 192.168.1.10:3001 */
    String mostrar() {
        return host + ":" + puerto + (https ? " (HTTPS)" : "");
    }

    /** true si la URL pertenece a este servidor (mismo esquema, host y puerto). */
    boolean esDelServidor(Uri u) {
        if (u == null || u.getHost() == null) return false;
        String esquema = https ? "https" : "http";
        if (!esquema.equalsIgnoreCase(u.getScheme())) return false;
        int p = u.getPort() == -1 ? (https ? 443 : 80) : u.getPort();
        return p == puerto && host.equalsIgnoreCase(u.getHost());
    }

    boolean esDelServidor(String url) {
        return url != null && esDelServidor(Uri.parse(url));
    }

    /**
     * Normaliza lo que escribe el usuario: acepta "192.168.1.10", "192.168.1.10:3001",
     * "http://192.168.1.10:3001/". Lanza IllegalArgumentException con el mensaje para mostrar.
     */
    static Servidor desdeTexto(String ipTexto, String puertoTexto, boolean https) {
        String t = ipTexto == null ? "" : ipTexto.trim();
        String min = t.toLowerCase(Locale.ROOT);
        if (min.startsWith("https://")) {
            https = true;
            t = t.substring(8);
        } else if (min.startsWith("http://")) {
            t = t.substring(7);
        }
        int barra = t.indexOf('/');
        if (barra >= 0) t = t.substring(0, barra);

        String pTxt = puertoTexto == null ? "" : puertoTexto.trim();
        int dosPuntos = t.lastIndexOf(':');
        if (dosPuntos >= 0) {
            String enIp = t.substring(dosPuntos + 1);
            t = t.substring(0, dosPuntos);
            if (!enIp.isEmpty()) pTxt = enIp;
        }

        if (t.isEmpty()) throw new IllegalArgumentException("Ingrese la IP del servidor.");
        if (!esHostValido(t)) throw new IllegalArgumentException("\"" + t + "\" no es una IP o nombre válido. Ejemplo: 192.168.1.10");

        int puerto;
        if (pTxt.isEmpty()) {
            puerto = PUERTO_DEFECTO;
        } else {
            try {
                puerto = Integer.parseInt(pTxt);
            } catch (NumberFormatException e) {
                throw new IllegalArgumentException("El puerto debe ser un número.");
            }
        }
        if (puerto < 1 || puerto > 65535) throw new IllegalArgumentException("El puerto debe estar entre 1 y 65535.");
        return new Servidor(t.toLowerCase(Locale.ROOT), puerto, https);
    }

    static boolean esHostValido(String h) {
        if (h.matches("[0-9.]+")) {
            String[] partes = h.split("\\.", -1);
            if (partes.length != 4) return false;
            for (String p : partes) {
                if (p.isEmpty() || p.length() > 3 || Integer.parseInt(p) > 255) return false;
            }
            return true;
        }
        return h.length() <= 253
                && h.matches("(?i)[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?(\\.[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?)*");
    }

    /**
     * Prueba de conexion (llamar en un hilo de fondo).
     * @return null si el servidor responde como Sanbit; si no, el motivo para mostrar al usuario.
     */
    String probar() {
        HttpURLConnection con = null;
        try {
            con = (HttpURLConnection) new URL(url()).openConnection();
            con.setConnectTimeout(TIEMPO_PRUEBA_MS);
            con.setReadTimeout(TIEMPO_PRUEBA_MS);
            con.setInstanceFollowRedirects(true);
            con.setUseCaches(false);
            int codigo = con.getResponseCode();
            if (codigo >= 400) return "El servidor respondió con error HTTP " + codigo + ".";
            String inicio = leerInicio(con.getInputStream());
            // "Santit": servidores web anteriores al cambio de nombre a Sanbit
            if (!inicio.contains("Sanbit") && !inicio.contains("Santit")) {
                return "Hay un servicio en " + host + ":" + puerto + ", pero no parece Sanbit. Verifique el puerto.";
            }
            return null;
        } catch (SocketTimeoutException e) {
            return "El servidor no responde. Verifique la IP, que la PC esté encendida y que el teléfono esté en la misma red Wi-Fi.";
        } catch (ConnectException e) {
            return "Conexión rechazada. Verifique el puerto, que Sanbit esté iniciado y que el firewall de la PC lo permita.";
        } catch (UnknownHostException e) {
            return "No se encontró el servidor \"" + host + "\".";
        } catch (SSLException e) {
            return "Error de certificado HTTPS: " + e.getMessage();
        } catch (IOException | RuntimeException e) {
            return "No se pudo conectar: " + e.getMessage();
        } finally {
            if (con != null) con.disconnect();
        }
    }

    private static String leerInicio(InputStream in) throws IOException {
        try (InputStream is = in) {
            byte[] buf = new byte[8192];
            int total = 0, n;
            while (total < buf.length && (n = is.read(buf, total, buf.length - total)) > 0) total += n;
            return new String(buf, 0, total, StandardCharsets.UTF_8);
        }
    }
}
