using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Java.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileEncApp.Droid.FileHelperServiceAndroid))]
namespace FileEncApp.Droid
{
    public class FileHelperServiceAndroid : IFileHelperService
    {
        public void Save(string fileName)
        {
            //if (MainActivity.Instance.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Granted)
            //{
            //    // Depracated - Use in things before API 29
            //    string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath; // deprecated?
            //    string filePath = System.IO.Path.Combine(path, fileName);
            //    FileOutputStream fileOutputStream = new FileOutputStream(new Java.IO.File(filePath));
            //    fileOutputStream.Close();
            //}
            //else
            //{
            //    MainActivity.Instance.RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 0);
            //}
        }
    }
}