package com.santit.movil;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.MatrixCursor;
import android.net.Uri;
import android.os.ParcelFileDescriptor;
import android.provider.OpenableColumns;

import java.io.File;
import java.io.FileNotFoundException;

/**
 * Entrega a otra app (WhatsApp, Gmail...) el PDF que la web armo, como ARCHIVO adjunto.
 * Solo lectura, solo archivos de cache/compartir, y solo a quien recibio el permiso en el Intent
 * (exported=false + grantUriPermissions). Sin dependencias (no usa androidx FileProvider).
 */
public class ArchivosCompartidos extends ContentProvider {

    static String autoridad(Context c) {
        return c.getPackageName() + ".archivos";
    }

    static File carpeta(Context c) {
        File d = new File(c.getCacheDir(), "compartir");
        //noinspection ResultOfMethodCallIgnored
        d.mkdirs();
        return d;
    }

    static Uri uriDe(Context c, File f) {
        return new Uri.Builder().scheme("content").authority(autoridad(c)).appendPath(f.getName()).build();
    }

    private File archivo(Uri uri) throws FileNotFoundException {
        String nombre = uri.getLastPathSegment();
        if (nombre == null || nombre.contains("/") || nombre.contains("\\") || nombre.startsWith(".")) throw new FileNotFoundException();
        File f = new File(carpeta(getContext()), nombre);
        if (!f.isFile()) throw new FileNotFoundException();
        return f;
    }

    @Override
    public boolean onCreate() {
        return true;
    }

    @Override
    public ParcelFileDescriptor openFile(Uri uri, String modo) throws FileNotFoundException {
        if (modo != null && !modo.equals("r")) throw new FileNotFoundException("solo lectura");
        return ParcelFileDescriptor.open(archivo(uri), ParcelFileDescriptor.MODE_READ_ONLY);
    }

    @Override
    public String getType(Uri uri) {
        String n = uri.getLastPathSegment();
        return n != null && n.toLowerCase().endsWith(".pdf") ? "application/pdf" : "application/octet-stream";
    }

    // nombre y tamaño: WhatsApp los muestra en el adjunto
    @Override
    public Cursor query(Uri uri, String[] columnas, String sel, String[] args, String orden) {
        File f;
        try { f = archivo(uri); } catch (FileNotFoundException e) { return null; }
        String[] cols = columnas != null ? columnas : new String[]{OpenableColumns.DISPLAY_NAME, OpenableColumns.SIZE};
        MatrixCursor c = new MatrixCursor(cols, 1);
        Object[] fila = new Object[cols.length];
        for (int i = 0; i < cols.length; i++) {
            if (OpenableColumns.DISPLAY_NAME.equals(cols[i])) fila[i] = f.getName();
            else if (OpenableColumns.SIZE.equals(cols[i])) fila[i] = f.length();
        }
        c.addRow(fila);
        return c;
    }

    @Override
    public Uri insert(Uri uri, ContentValues v) {
        throw new UnsupportedOperationException();
    }

    @Override
    public int delete(Uri uri, String s, String[] a) {
        throw new UnsupportedOperationException();
    }

    @Override
    public int update(Uri uri, ContentValues v, String s, String[] a) {
        throw new UnsupportedOperationException();
    }
}
