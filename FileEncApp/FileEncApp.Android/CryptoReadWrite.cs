using Android.Annotation;
using System;
using System.IO;
using FileEncApp.Interfaces;
using Xamarin.Essentials;
using Android.Content;
using Plugin.CurrentActivity;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(FileEncApp.Droid.CryptoReadWrite))]
namespace FileEncApp.Droid
{
    public class CryptoReadWrite : IFileWrite
    {
        /// <summary>
        /// Version 10 (API 29) cannot write directly to a directory outside of the immediate app sandbox, so permissions aren't required.
        /// Before that, permission is needed.
        /// </summary>
        /// <returns></returns>
        public bool IsPermissionNeeded()
        {
            Version version = DeviceInfo.Version;

            if (version.Major < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Write file to app directory.
        /// </summary>
        /// <param name="filePath">Location of original file which we pass as input to AES functions</param>
        /// <param name="pass">password which AES function will work on</param>
        /// <param name="fileName">File name of original file which we append .aes to and use as
        /// part of file creation parameters for the final encrypted/decrypted file</param>
        /// <returns>Worked</returns>
        public bool ExportFile(string filePath, string pass, string fileName)
        {
            Version version = DeviceInfo.Version;

            // After version 10 (API 29), we are not allowed to write directly to a public directory (DCIM, Download, etc).
            // We must utilize the MediaStore API due to security changes and scoped file access.
            // Version 11 (API 30) seems to revert some of these requirements and allows writing only inside of public dirs.

            if (version.Major < 10)
            {
                return WriteAPI28AndBelow(filePath, pass, fileName);
            }
            else
            {
                return WriteAPI29(filePath, pass, fileName);
            }

        }

        [TargetApi(Value = 28)]
        private bool WriteAPI28AndBelow(string filePath, string pass, string fileName)
        {
            string directory;

#pragma warning disable CS0618 // Type or member is obsolete - disable because this will target API 28 and below
            directory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePathDir = Path.Combine(directory, "Encrypted Files");
            if (!File.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
            }
#pragma warning restore CS0618

            string filespec = Path.Combine(filePathDir, fileName + ".aes");

            FileInfo newFile = new FileInfo(filespec);

            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete(); //overwrite file with existing file name - TODO: iterate with appended (i) to end until unique
                }
                catch
                {
                    return false;
                }
            }
            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.Encryptor(filePath, pass, filespec);
            return result;
        }

        private bool WriteAPI29(string filePath, string pass, string fileName)
        {
            ContentValues contentValues = new ContentValues();
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName + ".aes");
            Console.WriteLine(Android.OS.Environment.DirectoryDocuments);
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath,"Documents/Encrypted Files");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "application/octet-stream");

            Context context = Android.App.Application.Context;

            Android.Net.Uri uri = context.ContentResolver.Insert(Android.Provider.MediaStore.Files.GetContentUri("external"), contentValues);
            Stream outputStream = context.ContentResolver.OpenOutputStream(uri);

            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.ScopedStorageEncryptor(filePath, pass, outputStream);
            outputStream.Close();

            return result;
        }

        public bool ImportFile(string filePath, string pass, string fileName)
        {
            Version version = DeviceInfo.Version;

            // After version 10 (API 29), we are not allowed to write directly to a public directory (DCIM, Download, etc).
            // We must utilize the MediaStore API due to security changes and scoped file access.

            if (version.Major < 10)
            {
                return ReadAPI28AndBelow(filePath, pass, fileName);
            }
            else
            {
                return ReadAPI29(filePath, pass, fileName);
            }
        }

        private bool ReadAPI28AndBelow(string filePath, string pass, string fileName)
        {
            string directory;
#pragma warning disable CS0618 // Type or member is obsolete - disable because this will target API 28 and below
            directory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePathDir = Path.Combine(directory, "Decrypted Files");
            if (!File.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
            }
#pragma warning restore CS0618

            fileName = fileName.Remove(fileName.LastIndexOf('.'));

            string filespec = Path.Combine(filePathDir, fileName);

            FileInfo newFile = new FileInfo(filespec);

            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete(); //overwrite file with existing file name - TODO: iterate with appended (i) to end until unique
                }
                catch
                {
                    return false;
                }
            }
            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.Decryptor(filePath, pass, filespec);
            return result;
        }

        private bool ReadAPI29(string filePath, string pass, string fileName)
        {
            fileName = fileName.Remove(fileName.LastIndexOf('.')); // remove ".AES" from end of file
            ContentValues contentValues = new ContentValues();
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName);
            Console.WriteLine(Android.OS.Environment.DirectoryDocuments);
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "Documents/Decrypted Files");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "application/octet-stream");

            Context context = Android.App.Application.Context;

            Android.Net.Uri uri = context.ContentResolver.Insert(Android.Provider.MediaStore.Files.GetContentUri("external"), contentValues);
            Stream outputStream = context.ContentResolver.OpenOutputStream(uri);

            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.ScopedStorageDecryptor(filePath, pass, outputStream);
            outputStream.Close();
            return result;
        }
    }
}