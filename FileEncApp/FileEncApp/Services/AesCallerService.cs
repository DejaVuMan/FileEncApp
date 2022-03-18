using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using FileEncApp.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace FileEncApp.Services
{
    class AesCallerService
    {
        private ExportService exportInfo;
        public async Task EncryptFile(string filePath, string pass, string fileName)
        {
            bool PermissionGranted = await CheckPermission();
            if(!PermissionGranted)
            {
                await Application.Current.MainPage.DisplayAlert("Writing File Data", "You didn't give the proper permissions. Please provide them and try again.", "OK");
            }

            if(exportInfo == null)
            {
                exportInfo = new ExportService();
            }

            bool worked = exportInfo.CreateExportData(filePath, pass, fileName);

            if(worked)
            {
                if(DeviceInfo.Version.Major < 10)
                {
                    await Application.Current.MainPage.DisplayAlert("Writing File Data", "File was successfully Encrypted in root directory under folder name: Encrypted Files", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Writing File Data", "File was successfully Encrypted in the Documents directory under folder name: Encrypted Files", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Writing File Data", "An error occurred during enryption. No changes were made.", "OK");
            }
        }

        public async Task DecryptFile(string filePath, string pass, string fileName)
        {
            bool PermissionGranted = await CheckPermission();
            if(!PermissionGranted)
            {
                await Application.Current.MainPage.DisplayAlert("Reading File Data", "You didn't give the proper permissions. Please provide them and try again.", "OK");
            }

            if (exportInfo == null)
            {
                exportInfo = new ExportService();
            }

            bool worked = exportInfo.ReadImportData(filePath, pass, fileName);

            if(worked)
            {
                if(DeviceInfo.Version.Major < 10)
                {
                    await Application.Current.MainPage.DisplayAlert("Reading File Data", "File was successfully Decrypted in root directory under folder name: Decrypted Files. Don't forget to remove the original file!", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Writing File Data", "File was successfully Decrypted in Documents directory under folder name: Decrypted Files. Don't forget to remove the original file!", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Reading File Data", "An error occurred during decryption. No changes were made.", "OK");
            }
        }

        private async Task<bool> CheckPermission()
        {
            bool permissionNeeded = DependencyService.Get<IFileWrite>().IsPermissionNeeded();
            if(!permissionNeeded)
            {
                return true;
            }

            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            if(Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                await Application.Current.MainPage.DisplayAlert("Writing\\Reading File Data", "Without this permission, we can't create an encrypted file.", "OK");
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
