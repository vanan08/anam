package com.adsoft.sample.webservice;

import android.net.Uri;
import android.util.Log;

import com.adsoft.sample.Constants;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;


/**
 * Created by nvan on 11/12/2015.
 */
public class Webservice {
    public static List<UserImage> getImages(int times, int type, String ImageIndexs, String Email, String password) {

        String SPHERE_URL = Constants.SPHERE_URL+"/Account/GetRegisterImages";
        if (type == 0) {
            SPHERE_URL = Constants.SPHERE_URL+"/Account/GetLoginImages";
        }

        List<UserImage> lsImages = new ArrayList<UserImage>();

        StringBuffer chaine = new StringBuffer();
        try {
            URL url = new URL(SPHERE_URL);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestProperty("User-Agent", "");
            connection.setRequestMethod("POST");
            connection.setDoInput(true);

            Uri.Builder builder = new Uri.Builder()
                    .appendQueryParameter("email", Email)
                    .appendQueryParameter("Password", password)
                    .appendQueryParameter("ImageIndexs", ImageIndexs)
                    .appendQueryParameter("Fullname", "Test")
                    .appendQueryParameter("Times", String.valueOf(times));

            String query = builder.build().getEncodedQuery();

            OutputStream os = connection.getOutputStream();
            BufferedWriter writer = new BufferedWriter(
                    new OutputStreamWriter(os, "UTF-8"));
            writer.write(query);
            writer.flush();
            writer.close();
            os.close();
            connection.connect();

            InputStream inputStream = connection.getInputStream();

            BufferedReader rd = new BufferedReader(new InputStreamReader(inputStream));
            String line = "";
            while ((line = rd.readLine()) != null) {
                chaine.append(line);
            }

            if (chaine.toString().toLowerCase().replace("\"", "").contains("fail")) {
                return null;
            }

            if(chaine.toString().toLowerCase().replace("\"", "").contains("ok")){
                return lsImages;
            }
            String res = "{'Images':" + chaine.toString() + "}";

                JSONObject obj = new JSONObject(res);
                JSONArray jsas = obj.getJSONArray("Images");
                UserImage userImage = null;

                for (int i = 0; i < jsas.length(); i++) {
                    userImage = new UserImage();
                    JSONObject message = jsas.getJSONObject(i);
                    String ImageUrl = message.getString("ImageUrl");
                    String Index = message.getString("Index");
                    userImage.ImageUrl = ImageUrl;
                    userImage.Index = Index;
                    lsImages.add(userImage);
                    Log.d("", "nvan ImageUrl: " + ImageUrl);
                    Log.d("", "nvan Index: " + Index);
                }

            Log.d("", "nvan: " + res);
        } catch (Exception e) {
            // writing exception to log
            e.printStackTrace();
            return null;
        }

        return lsImages;
    }

    public static boolean login(String ImageIndexs, String Email) {

        String SPHERE_URL = Constants.SPHERE_URL+"/Account/Login";

        List<UserImage> lsImages = new ArrayList<UserImage>();

        StringBuffer chaine = new StringBuffer();
        try {
            URL url = new URL(SPHERE_URL);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestProperty("User-Agent", "");
            connection.setRequestMethod("POST");
            connection.setDoInput(true);

            Uri.Builder builder = new Uri.Builder()
                    .appendQueryParameter("email", Email)
                    .appendQueryParameter("Password", "password")
                    .appendQueryParameter("ImageIndexs", ImageIndexs)
                    .appendQueryParameter("Fullname", "Test")
                    .appendQueryParameter("Times", String.valueOf(0));

            String query = builder.build().getEncodedQuery();

            OutputStream os = connection.getOutputStream();
            BufferedWriter writer = new BufferedWriter(
                    new OutputStreamWriter(os, "UTF-8"));
            writer.write(query);
            writer.flush();
            writer.close();
            os.close();
            connection.connect();

            InputStream inputStream = connection.getInputStream();

            BufferedReader rd = new BufferedReader(new InputStreamReader(inputStream));
            String line = "";
            while ((line = rd.readLine()) != null) {
                chaine.append(line);
            }

            if (chaine.toString().toLowerCase().replace("\"", "").contains("fail")) {
                return false;
            }

            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    public static String sendFileToServer(String filename, String targetUrl) {
        String response = "error";
        Log.e("Image filename", filename);
        Log.e("url", targetUrl);
        HttpURLConnection connection = null;
        DataOutputStream outputStream = null;
        // DataInputStream inputStream = null;

        String pathToOurFile = filename;
        String urlServer = targetUrl;
        String lineEnd = "\r\n";
        String twoHyphens = "--";
        String boundary = "*****";
        DateFormat df = new SimpleDateFormat("yyyy_MM_dd_HH:mm:ss");

        int bytesRead, bytesAvailable, bufferSize;
        byte[] buffer;
        int maxBufferSize = 1 * 1024;
        try {
            FileInputStream fileInputStream = new FileInputStream(new File(
                    pathToOurFile));

            URL url = new URL(urlServer);
            connection = (HttpURLConnection) url.openConnection();

            // Allow Inputs & Outputs
            connection.setDoInput(true);
            connection.setDoOutput(true);
            connection.setUseCaches(false);
            connection.setChunkedStreamingMode(1024);
            // Enable POST method
            connection.setRequestMethod("POST");

            connection.setRequestProperty("Connection", "Keep-Alive");
            connection.setRequestProperty("Content-Type",
                    "multipart/form-data;boundary=" + boundary);

            outputStream = new DataOutputStream(connection.getOutputStream());
            outputStream.writeBytes(twoHyphens + boundary + lineEnd);

            String connstr = null;
            connstr = "Content-Disposition: form-data; name=\"uploads\";filename=\""
                    + pathToOurFile + "\"" + lineEnd;
            Log.i("Connstr", connstr);

            outputStream.writeBytes(connstr);
            outputStream.writeBytes(lineEnd);

            bytesAvailable = fileInputStream.available();
            bufferSize = Math.min(bytesAvailable, maxBufferSize);
            buffer = new byte[bufferSize];

            // Read file
            bytesRead = fileInputStream.read(buffer, 0, bufferSize);
            Log.e("Image length", bytesAvailable + "");
            try {
                while (bytesRead > 0) {
                    try {
                        outputStream.write(buffer, 0, bufferSize);
                    } catch (OutOfMemoryError e) {
                        e.printStackTrace();
                        response = "outofmemoryerror";
                        return response;
                    }
                    bytesAvailable = fileInputStream.available();
                    bufferSize = Math.min(bytesAvailable, maxBufferSize);
                    bytesRead = fileInputStream.read(buffer, 0, bufferSize);
                }
            } catch (Exception e) {
                e.printStackTrace();
                response = "error";
                return response;
            }
            outputStream.writeBytes(lineEnd);
            outputStream.writeBytes(twoHyphens + boundary + twoHyphens
                    + lineEnd);

            // Responses from the server (code and message)
            int serverResponseCode = connection.getResponseCode();
            String serverResponseMessage = connection.getResponseMessage();
            Log.i("Server Response Code ", "" + serverResponseCode);
            Log.i("Server Response Message", serverResponseMessage);

            if (serverResponseCode == 200) {
                response = "true";
            }

            String CDate = null;
            Date serverTime = new Date(connection.getDate());
            try {
                CDate = df.format(serverTime);
            } catch (Exception e) {
                e.printStackTrace();
                Log.e("Date Exception", e.getMessage() + " Parse Exception");
            }
            Log.i("Server Response Time", CDate + "");

            filename = CDate
                    + filename.substring(filename.lastIndexOf("."),
                    filename.length());
            Log.i("File Name in Server : ", filename);

            fileInputStream.close();
            outputStream.flush();
            outputStream.close();
            outputStream = null;
        } catch (Exception ex) {
            // Exception handling
            response = "error";
            Log.e("Send file Exception", ex.getMessage() + "");
            ex.printStackTrace();
        }
        return response;
    }
}
