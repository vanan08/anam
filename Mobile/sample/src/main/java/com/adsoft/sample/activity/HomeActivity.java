/*******************************************************************************
 * Copyright 2011-2013 Sergey Tarasevich
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/
package com.adsoft.sample.activity;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;

import com.adsoft.sample.Constants;
import com.adsoft.sample.R;
import com.adsoft.sample.ext.RealPathUtil;
import com.adsoft.sample.webservice.Webservice;
import com.android.volley.RequestQueue;
import com.nostra13.universalimageloader.core.ImageLoader;


/**
 * @author Sergey Tarasevich (nostra13[at]gmail[dot]com)
 */
public class HomeActivity extends AppCompatActivity {
	private static final int SELECT_PHOTO = 100;
    private RequestQueue mQueue;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main_layout);
		Button btnUpload = (Button)findViewById(R.id.btnUpload);

		btnUpload.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent photoPickerIntent = new Intent(Intent.ACTION_PICK);
				photoPickerIntent.setType("image/*");
				startActivityForResult(photoPickerIntent, SELECT_PHOTO);
			}
		});

	}

	@Override
	protected void onActivityResult(int requestCode, int resCode, Intent data) {
		if(resCode == Activity.RESULT_OK && data != null){
			String realPath;
			// SDK < API11
			if (Build.VERSION.SDK_INT < 11)
				realPath = RealPathUtil.getRealPathFromURI_BelowAPI11(this, data.getData());

				// SDK >= 11 && SDK < 19
			else if (Build.VERSION.SDK_INT < 19)
				realPath = RealPathUtil.getRealPathFromURI_API11to18(this, data.getData());

				// SDK > 19 (Android 4.4)
			else
				realPath = RealPathUtil.getRealPathFromURI_API19(this, data.getData());

			String url = Constants.SPHERE_URL+"/Uploading/Post";
            new UploadTask().execute(realPath, url);
        }

	}



	@Override
	public void onBackPressed() {
		ImageLoader.getInstance().stop();
		super.onBackPressed();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.main_menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
			case R.id.item_clear_memory_cache:
				ImageLoader.getInstance().clearMemoryCache();
				return true;
			case R.id.item_clear_disc_cache:
				ImageLoader.getInstance().clearDiskCache();
				return true;
			default:
				return false;
		}
	}

    private class UploadTask extends AsyncTask<String, Void, String> {
        final ProgressDialog progressDialog = new ProgressDialog(HomeActivity.this, R.style.AppTheme_Dark_Dialog);
        @Override
        protected String doInBackground(String... params) {
            String realPath = params[0];
            String url = params[1];
            Webservice.sendFileToServer(realPath, url);
            return "Executed";
        }

        @Override
        protected void onPostExecute(String result) {
            try{
                if(progressDialog.isShowing())
                    progressDialog.dismiss();
            }catch (Exception ex){}
        }

        @Override
        protected void onPreExecute() {
            progressDialog.setIndeterminate(true);
            progressDialog.setMessage("Uploading...");
            if(progressDialog != null && !progressDialog.isShowing())
                progressDialog.show();
        }

        @Override
        protected void onProgressUpdate(Void... values) {}
    }
}