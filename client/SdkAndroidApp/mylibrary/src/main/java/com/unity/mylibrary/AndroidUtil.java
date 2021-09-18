package com.unity.mylibrary;

import android.app.Activity;
import android.content.Context;

public class AndroidUtil {
    public static String GetChannel(Context context){
        return context.getString(R.string.lg_app_channel);
    }
}
