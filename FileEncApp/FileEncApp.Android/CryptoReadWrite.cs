using Android.Annotation;
using System;
using System.IO;
using FileEncApp.Interfaces;
using Xamarin.Essentials;

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

            if (version.Major < 10)
            {
                return WriteAPI28AndBelow(filePath, pass, fileName);
            }
            else
            {
                return WriteAPI29AndAbove(filePath, pass, fileName);
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
                    System.Diagnostics.Debug.WriteLine("Existing file was deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to delete existing filespec: " + fileName);
                    System.Diagnostics.Debug.WriteLine("The following exception occurred: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }
            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.Encryptor(filePath, pass, filespec);
            return result;
        }

        private bool WriteAPI29AndAbove(string filePath, string pass, string fileName)
        {
            // In Android 29, we are not allowed to create a new folder in the root public directory - we will store files in a folder w/in app directory instead.
            var filePathDir = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "Encrypted Files"); // get Private Public folder

            if (!File.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
                System.Diagnostics.Debug.WriteLine("New file directory created: " + filePathDir);
                System.Diagnostics.Debug.Flush();
            }

            string filespec = Path.Combine(filePathDir, fileName + ".aes");

            FileInfo newFile = new FileInfo(filespec);

            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete(); //overwrite file with existing file name - TODO: iterate with appended (i) to end until unique

                    System.Diagnostics.Debug.WriteLine("Existing file was deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to delete existing filespec: " + fileName);
                    System.Diagnostics.Debug.WriteLine("The following exception occurred: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }
            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.Encryptor(filePath, pass, filespec);
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
                return ReadAPI29AndAbove(filePath, pass, fileName);
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
                    System.Diagnostics.Debug.WriteLine("Existing file was deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to delete existing filespec: " + fileName);
                    System.Diagnostics.Debug.WriteLine("The following exception occurred: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }
            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.Decryptor(filePath, pass, filespec);
            return result;
        }
        private bool ReadAPI29AndAbove(string filePath, string pass, string fileName)
        {
            // In Android 29, we are not allowed to create a new folder in the root public directory - we will store files in a folder w/in app directory instead.
            var filePathDir = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "Decrypted Files"); // get Private Public folder

            if (!File.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
                System.Diagnostics.Debug.WriteLine("New file directory created: " + filePathDir);
                System.Diagnostics.Debug.Flush();
            }

            fileName = fileName.Remove(fileName.LastIndexOf('.'));

            string filespec = Path.Combine(filePathDir, fileName);

            FileInfo newFile = new FileInfo(filespec);

            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete(); //overwrite file with existing file name - TODO: iterate with appended (i) to end until unique

                    System.Diagnostics.Debug.WriteLine("Existing file was deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to delete existing filespec: " + fileName);
                    System.Diagnostics.Debug.WriteLine("The following exception occurred: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }
            var aesMagic = new CryptKeeper(); // call on it from CryptKeeper class
            bool result = aesMagic.Decryptor(filePath, pass, filespec);
            return result;
        }
        // implement logic in UI side
    }
}