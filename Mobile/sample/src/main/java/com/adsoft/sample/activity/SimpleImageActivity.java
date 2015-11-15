/*******************************************************************************
 * Copyright 2014 Sergey Tarasevich
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

import android.content.ContentResolver;
import android.content.Context;
import android.database.Cursor;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.widget.Toast;

import com.adsoft.sample.ProfileObject;
import com.adsoft.sample.R;
import com.adsoft.sample.fragment.ImageGridFragment;
import com.adsoft.sample.provider.ProfileProvider;

import static android.widget.Toast.makeText;

/**
 * @author Sergey Tarasevich (nostra13[at]gmail[dot]com)
 */
public class SimpleImageActivity extends FragmentActivity {
    int type = 0;
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if (getIntent().getCharSequenceExtra("email") != null) {
			String email = getIntent().getCharSequenceExtra("email").toString();
            String password = getIntent().getCharSequenceExtra("password").toString();
            type = getIntent().getIntExtra("type", 0);
			ImageGridFragment fr;
			String tag = ImageGridFragment.class.getSimpleName();
			int titleRes;
			fr = (ImageGridFragment)getSupportFragmentManager().findFragmentByTag(tag);
			if (fr == null) {
				fr = new ImageGridFragment();
				fr.email = email;
                fr.password = password;
                fr.startType = type;
			}

			titleRes = R.string.ac_name_image_grid;
			setTitle(titleRes);
			getSupportFragmentManager().beginTransaction().replace(android.R.id.content, fr, tag).commit();
		} else {
			makeText(SimpleImageActivity.this, "Can not get Email from extra", Toast.LENGTH_LONG).show();
			finish();
		}

	}

	public static ProfileObject getProfile(Context context, String email) {
		ContentResolver resolver = context.getContentResolver();
		String[] projection = new String[]{ProfileProvider._ID, ProfileProvider.EMAIL, ProfileProvider.IMAGES_INDEX
				, ProfileProvider.NAME, ProfileProvider.PASSWORD};
		Cursor cursor =
				resolver.query(ProfileProvider.CONTENT_URI,
						projection,
						"email='" + email + "'",
						null,
						null);
		ProfileObject object = null;
		if (cursor.moveToFirst()) {
			object = new ProfileObject();
			do {
				object.Email = cursor.getString(1);
				object.ImageIndex = cursor.getString(2);

				// do something meaningful
			} while (cursor.moveToNext());
		}

		return object;
	}


}