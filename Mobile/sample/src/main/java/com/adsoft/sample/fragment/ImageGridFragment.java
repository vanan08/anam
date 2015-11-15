/*******************************************************************************
 * Copyright 2011-2014 Sergey Tarasevich
 * <p/>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p/>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p/>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/
package com.adsoft.sample.fragment;

import android.app.ProgressDialog;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.BaseAdapter;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.Toast;

import com.adsoft.sample.Constants;
import com.adsoft.sample.ProfileObject;
import com.adsoft.sample.activity.HomeActivity;
import com.adsoft.sample.activity.LoginActivity;
import com.adsoft.sample.activity.SignupActivity;
import com.adsoft.sample.activity.SimpleImageActivity;
import com.adsoft.sample.provider.ProfileProvider;
import com.adsoft.sample.webservice.UserImage;
import com.adsoft.sample.webservice.Webservice;
import com.nostra13.universalimageloader.core.DisplayImageOptions;
import com.nostra13.universalimageloader.core.ImageLoader;
import com.nostra13.universalimageloader.core.assist.FailReason;
import com.nostra13.universalimageloader.core.listener.ImageLoadingProgressListener;
import com.nostra13.universalimageloader.core.listener.SimpleImageLoadingListener;
import com.adsoft.sample.R;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.List;

/**
 * @author Sergey Tarasevich (nostra13[at]gmail[dot]com)
 */
public class ImageGridFragment extends AbsListViewBaseFragment {

    public static final int INDEX = 1;

    public String Type = "";
    public String email = "";
    public String password = "";
    public int startType = 0;
    public String sPosition = "";
    int count = 1;


    private static String convertInputStreamToString(InputStream inputStream) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
        String line = "";
        String result = "";
        while ((line = bufferedReader.readLine()) != null)
            result += line;

        inputStream.close();
        return result;

    }

    public String register(String imageIndexs, String email, String password) {

        String SPHERE_URL = Constants.SPHERE_URL+"/Account/Register";

        StringBuffer chaine = new StringBuffer("");
        try {
            URL url = new URL(SPHERE_URL);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestProperty("User-Agent", "");
            connection.setRequestMethod("POST");
            connection.setDoInput(true);
            Uri.Builder builder = new Uri.Builder()
                    .appendQueryParameter("Fullname", imageIndexs)
                    .appendQueryParameter("ImageIndexs", imageIndexs)
                    .appendQueryParameter("Email", email)
                    .appendQueryParameter("Password", password);

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

            if (chaine.toString().toLowerCase().contains("Fail")) {
                return null;
            }

            Log.d("", "nvan: " + chaine.toString());
        } catch (Exception e) {
            getActivity().getContentResolver().delete(ProfileProvider.CONTENT_URI, null, null);
            // writing exception to log
            e.printStackTrace();
        }

        return chaine.toString();
    }



    // Async Task Class
    class GetImageFromInternet extends AsyncTask<String, String, List<UserImage>> {
        int type = 0;
        GridView gridView;
        int _count = 0;
        String ImageIndexs;
        String Email;
        String password;
        final ProgressDialog progressDialog = new ProgressDialog(getActivity(), R.style.AppTheme_Dark_Dialog);

        public GetImageFromInternet(GridView gridView, int type, int count, String ImageIndexs, String Email, String password) {
            this.gridView = gridView;
            this.type = type;
            this._count = count;
            this.ImageIndexs = ImageIndexs;
            this.Email = Email;
            this.password = password;
        }

        // Show Progress bar before downloading Music
        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            progressDialog.setIndeterminate(true);
//            progressDialog.setCanceledOnTouchOutside(false);
            progressDialog.setMessage("Authenticating...");
            if(progressDialog != null && !progressDialog.isShowing())
                progressDialog.show();
        }

        // Download Music File from Internet
        @Override
        protected List<UserImage> doInBackground(String... f_url) {
            try {
                return Webservice.getImages(_count, type, ImageIndexs, Email, password);
            } catch (Exception ex) {
                Log.e("", ex.getMessage(), ex);
            }
            return null;
        }

        // While Downloading Music File
        protected void onProgressUpdate(String... progress) {
            // Set progress percentage
        }

        // Once Music File is downloaded
        @Override
        protected void onPostExecute(List<UserImage> lsImage) {
            try{
                if(progressDialog.isShowing())
                    progressDialog.dismiss();
            }catch (Exception ex){}

            if (lsImage == null && type == 0) {
                Toast.makeText(getActivity(), "Can't get image!.", Toast.LENGTH_LONG).show();
                Intent intent = new Intent(getActivity(), LoginActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                startActivity(intent);
                getActivity().finish();
            } else if (lsImage == null && type != 0) {
                Toast.makeText(getActivity(), "Can't not register.", Toast.LENGTH_LONG).show();
                Intent intent = new Intent(getActivity(), SignupActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                startActivity(intent);
                getActivity().finish();
            }else if(lsImage.size() == 0) {
                new LoginTask().execute(ImageIndexs, email);

            }else {
                ImageAdapter adapter = new ImageAdapter(getActivity(), lsImage);
                gridView.setAdapter(adapter);
                adapter.notifyDataSetChanged();
            }
        }
    }

    private class LoginTask extends AsyncTask<String, Void, Boolean> {
        final ProgressDialog progressDialog = new ProgressDialog(getActivity(), R.style.AppTheme_Dark_Dialog);
        @Override
        protected Boolean doInBackground(String... params) {
            try{
                String ImageIndexs = params[0];
                String Email = params[1];
                return Webservice.login(ImageIndexs, Email);
            }catch (Exception ex){return false;}
        }

        @Override
        protected void onPostExecute(Boolean result) {
            Intent intent = null;
            try{
                if(progressDialog.isShowing())
                    progressDialog.dismiss();
            }catch (Exception ex){}
            if(result.booleanValue()) {
                intent = new Intent(getActivity(), HomeActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
            }else{
                Toast.makeText(getActivity(), "Login fail!.", Toast.LENGTH_LONG).show();
                intent = new Intent(getActivity(), LoginActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
            }

            startActivity(intent);
            getActivity().finish();
        }

        @Override
        protected void onPreExecute() {
            progressDialog.setIndeterminate(true);
            progressDialog.setMessage("Loging...");
            if(progressDialog != null && !progressDialog.isShowing())
                progressDialog.show();
        }

        @Override
        protected void onProgressUpdate(Void... values) {}
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fr_image_grid, container, false);
        listView = (GridView) rootView.findViewById(R.id.grid);


        new GetImageFromInternet((GridView) listView, startType, count, "1;2;3", email, password).execute();

        listView.setOnItemClickListener(new OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                UserImage _userImage = (UserImage)listView.getAdapter().getItem(position);
                ProfileObject object = SimpleImageActivity.getProfile(getActivity(), email);

                ContentValues values = new ContentValues();
                values.put("email", email);
                values.put("password", password);
                values.put("name", "Test");
                if (object != null) {
                    sPosition += object.ImageIndex.equals("") ? String.valueOf(_userImage.Index) : ";" + String.valueOf(_userImage.Index);
                    values.put("image_index", sPosition );
                    getActivity().getContentResolver().update(ProfileProvider.CONTENT_URI, values, "email='" + email + "'", null);
                } else {
                    sPosition = String.valueOf(_userImage.Index);
                    values.put("image_index", sPosition);

                    getActivity().getContentResolver().insert(ProfileProvider.CONTENT_URI, values);
                }

                if (count == 3 && startType != 0) {
                    //TODO: Register user
                    object = SimpleImageActivity.getProfile(getActivity(), email);
                    new RegisterAsyncTask(object.ImageIndex, email, password).execute();
                } else if (count == 3 && startType == 0) {
                    ProfileObject profileObject = SimpleImageActivity.getProfile(getActivity(), email);
                    new LoginTask().execute(profileObject.ImageIndex == null ? sPosition : profileObject.ImageIndex, email);
                }{
                    count++;
                    ProfileObject profileObject = SimpleImageActivity.getProfile(getActivity(), email);
                    new GetImageFromInternet((GridView) listView, startType, count, profileObject.ImageIndex == null ? sPosition : profileObject.ImageIndex, email, password).execute();
                }
            }
        });
        return rootView;
    }

    private static class ImageAdapter extends BaseAdapter {

        private List<UserImage> lsUserImage;

        private LayoutInflater inflater;

        private DisplayImageOptions options;

        ImageAdapter(Context context, List<UserImage> lsUserImage) {
            this.lsUserImage = lsUserImage;
            inflater = LayoutInflater.from(context);

            options = new DisplayImageOptions.Builder()
                    .showImageOnLoading(R.drawable.ic_stub)
                    .showImageForEmptyUri(R.drawable.ic_empty)
                    .showImageOnFail(R.drawable.ic_error)
                    .cacheInMemory(true)
                    .cacheOnDisk(true)
                    .considerExifParams(true)
                    .bitmapConfig(Bitmap.Config.RGB_565)
                    .build();
        }

        @Override
        public int getCount() {
            return lsUserImage.size();
        }

        @Override
        public Object getItem(int position) {
            return lsUserImage.get(position);
        }

        @Override
        public long getItemId(int position) {
            return position;
        }

        @Override
        public View getView(int position, View convertView, ViewGroup parent) {
            final ViewHolder holder;
            View view = convertView;
            if (view == null) {
                view = inflater.inflate(R.layout.item_grid_image, parent, false);
                holder = new ViewHolder();
                assert view != null;
                holder.imageView = (ImageView) view.findViewById(R.id.image);
                holder.progressBar = (ProgressBar) view.findViewById(R.id.progress);
                view.setTag(holder);
            } else {
                holder = (ViewHolder) view.getTag();
            }

            ImageLoader.getInstance()
                    .displayImage(lsUserImage.get(position).ImageUrl, holder.imageView, options, new SimpleImageLoadingListener() {
                        @Override
                        public void onLoadingStarted(String imageUri, View view) {
                            holder.progressBar.setProgress(0);
                            holder.progressBar.setVisibility(View.VISIBLE);
                        }

                        @Override
                        public void onLoadingFailed(String imageUri, View view, FailReason failReason) {
                            holder.progressBar.setVisibility(View.GONE);
                        }

                        @Override
                        public void onLoadingComplete(String imageUri, View view, Bitmap loadedImage) {
                            holder.progressBar.setVisibility(View.GONE);
                        }
                    }, new ImageLoadingProgressListener() {
                        @Override
                        public void onProgressUpdate(String imageUri, View view, int current, int total) {
                            holder.progressBar.setProgress(Math.round(100.0f * current / total));
                        }
                    });

            return view;
        }
    }

    static class ViewHolder {
        ImageView imageView;
        ProgressBar progressBar;
    }

    // Async Task Class
    class RegisterAsyncTask extends AsyncTask<String, String, String> {

        String imageIndexs;
        String email;
        String password;

        final ProgressDialog progressDialog = new ProgressDialog(getActivity(), R.style.AppTheme_Dark_Dialog);

        public RegisterAsyncTask(String imageIndexs, String email, String password) {
            this.imageIndexs = imageIndexs;
            this.email = email;
            this.password = password;
        }

        // Show Progress bar before downloading Music
        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            progressDialog.setIndeterminate(true);
//            progressDialog.setCanceledOnTouchOutside(false);
            progressDialog.setMessage("Authenticating...");
            progressDialog.show();
        }

        // Download Music File from Internet
        @Override
        protected String doInBackground(String... f_url) {
            try {
                return register(imageIndexs, email, password);
            } catch (Exception ex) {
                Log.e("", ex.getMessage(), ex);
            }
            return null;
        }

        // While Downloading Music File
        protected void onProgressUpdate(String... progress) {
            // Set progress percentage
        }

        // Once Music File is downloaded
        @Override
        protected void onPostExecute(String result) {

            Intent intent = new Intent(getActivity(), HomeActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
            startActivity(intent);
            getActivity().finish();
            try{
                if(progressDialog != null && progressDialog.isShowing())
                    progressDialog.dismiss();
            }catch (Exception e){}

        }
    }
}