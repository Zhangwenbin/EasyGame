package com.longriver.onesdklib;

        import android.content.Intent;
        import android.net.Uri;
        import android.os.Build;
        import android.support.annotation.RequiresApi;
        import android.support.v4.content.FileProvider;
        import android.util.Log;

        import com.unity3d.player.UnityPlayer;

        import java.io.File;
        import java.io.FileOutputStream;
        import java.io.InputStream;
        import java.net.HttpURLConnection;
        import java.net.URL;

// import androidx.core.app.ActivityCompat;
      //  import androidx.core.content.ContextCompat;
     //   import androidx.core.content.FileProvider;
public class DownloadUtil {
    private  static  String filepath;
    public static void downloadAPK(final String _url,final String _filepath,final String authority) {//(View v) {
        filepath = _filepath;

        //准备用于保存APK文件的File对象 : /storage/sdcard/Android/package_name/files/xxx.apk

        //2). 启动分线程, 请求下载APK文件, 下载过程中显示下载进度
        new Thread(new Runnable() {

            @Override
            public void run() {
                try {
                    //1. 得到连接对象
                    String path = _url;
                    URL url = new URL(path);
                    HttpURLConnection connection = (HttpURLConnection) url.openConnection();
                    //2. 设置
                    //connection.setRequestMethod("GET");
                    connection.setConnectTimeout(5000);
                    connection.setReadTimeout(10000);
                    //3. 连接
                    connection.connect();
                    //4. 请求并得到响应码200
                    int responseCode = connection.getResponseCode();
                    if (responseCode == 200) {
                        //设置dialog的最大进度
//                                dialog.setMax(connection.getContentLength());
                        float max = connection.getContentLength();

                        //5. 得到包含APK文件数据的InputStream
                        InputStream is = connection.getInputStream();
                        //6. 创建指向apkFile的FileOutputStream
                        Log.i("gsc", _filepath);
                        File apkFile = new File(_filepath);

                        FileOutputStream fos = new FileOutputStream(apkFile);
                        //7. 边读边写
                        byte[] buffer = new byte[1024];
                        int len = -1;
                        int lencount = 0;
                        while ((len = is.read(buffer)) != -1) {
                            fos.write(buffer, 0, len);
                            //8. 显示下载进度
//                                    dialog.incrementProgressBy(len);
                            lencount += len;
                            Log.i("lib_APK", "Progress:" + lencount / max);
                            //休息一会(模拟网速慢)
                            //Thread.sleep(50);
//                                    SystemClock.sleep(50);
                        }

                        fos.close();
                        is.close();
                    }
                    //9. 下载完成, 关闭, 进入3)
                    connection.disconnect();

                    //3). 主线程, 移除dialog, 启动安装
                    UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
//                                    dialog.dismiss();
//                                    installAPK();
                            InstallApk(authority);
                        }
                    });

                } catch (Exception e) {
                    e.printStackTrace();
                } finally {
                }
            }
        }).start();

        //09-05 12:59:20.553: I/ActivityManager(1179): Displayed com.android.packageinstaller/.PackageInstallerActivity: +282ms
    }

    /**
     * 启动安装APK
     */
    @RequiresApi(api = Build.VERSION_CODES.LOLLIPOP)
    public static void InstallApk(String authority) {
        String apkPath = filepath;
        System.out.println("Android下载地址："+apkPath);
        File file = new File(apkPath);
        Intent intent = new Intent(Intent.ACTION_VIEW);

        if(Build.VERSION.SDK_INT>=24) { //Android 7.0及以上
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            // 参数2 清单文件中provider节点里面的authorities ; 参数3  共享的文件,即apk包的file类
            Log.i("unity", "InstallApk: "+authority);
            Uri apkUri = FileProvider.getUriForFile(UnityPlayer.currentActivity.getBaseContext(), authority, file);//记住修改包名
            // 对目标应用临时授权该Uri所代表的文件
            intent.setDataAndType(apkUri, "application/vnd.android.package-archive");

        }else{

            intent.setDataAndType(Uri.fromFile(file), "application/vnd.android.package-archive");
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        }

        UnityPlayer.currentActivity.startActivity(intent);
    }


}