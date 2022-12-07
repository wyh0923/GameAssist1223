package com.imgui.android.exampleapp;
import android.app.Activity;
import android.os.Bundle;
import android.widget.Toast;
import android.opengl.GLSurfaceView;
import android.util.Log;
import android.app.ActivityManager;
import android.content.Context;
import android.content.pm.ConfigurationInfo;

public class ImguiExampleActivity extends Activity
{
    private GLSurfaceView glSurfaceView;
    private boolean rendererSet=false;
	static{
		System.loadLibrary("ImguiAndroidWrapper");
	}

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        ActivityManager activityManager
                = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
        ConfigurationInfo confInfo = activityManager.getDeviceConfigurationInfo();
        if (confInfo.reqGlEsVersion >= 0x20000) {
            Log.w("App2", "This device support OpenGL ES " + Integer.toHexString(confInfo.reqGlEsVersion));
            glSurfaceView = new GLSurfaceView(this);
            glSurfaceView.setEGLContextClientVersion(2);
            glSurfaceView.setRenderer(new RendererWrapper());
            rendererSet=true;
            setContentView(glSurfaceView);
        } else {
            Toast.makeText(this, "This device does not support OpenGL ES 2.0.",Toast.LENGTH_LONG).show();
            return;
        }
    }

	@Override
    protected void onPause() {
        super.onPause();

        if (rendererSet) {
            glSurfaceView.onPause();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();

        if (rendererSet) {
            glSurfaceView.onResume();
        }
    }
};
