using System;
using System.IO;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace FileEncApp.Views
{
    [Preserve(AllMembers = true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        static string filePath;
        static string fileName;

        async void Button_Clicked(object sender, EventArgs e) // normally async void is BAD
        {
            // await operator will suspend evaluation of async until operand completes
            var result = await FilePicker.PickAsync(
                new PickOptions
                {
                PickerTitle = "Select a file"
                });

            var fileTypes = FilePickerFileType.Images;

            if (result != null) // if something is indeed chosen
            {
                var name = result.FileName;
                var fullPath = result.FullPath;

                String[] imageEndsWith = {".tif", ".tiff", ".bmp", ".jpg", ".jpeg", ".gif", ".png"};

                bool isImage = false;

                foreach(string img in imageEndsWith)
                {
                    if(name.EndsWith(img, StringComparison.OrdinalIgnoreCase))
                    {
                        isImage = true;
                        break;
                    }
                    isImage = false;
                }

                if (isImage)
                {
                    var stream = await result.OpenReadAsync();
                    MemoryStream ImageStream = new MemoryStream();
                    stream.CopyTo(ImageStream);
                    stream.Dispose(); // bypass issue where async stream can spontaneously dispose

                    ImageStream.Position = 0;
                    var byteArray = ImageStream.ToArray();
                    resultImage.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
                    ImageStream.Dispose();
                }
                else if (name.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
                {
                    resultImage.Source = ImageSource.FromFile("text_icon.png"); // load different image if dark background
                }
                else if(name.EndsWith("aes", StringComparison.OrdinalIgnoreCase))
                {
                    resultImage.Source = ImageSource.FromFile("encrypted_icon.png");
                }
                else
                {
                    resultImage.Source = ImageSource.FromFile("question_icon.png");
                }

                resultFName.Text = name;
                pickButton.Text = "Pick different file";
                if(name.Contains(".aes")) // we will encrypt files with .aes to know which file is which
                {
                    EncDecButton.Text = "Decrypt File";
                    EncDecButton.IsVisible = true;
                    filePath = fullPath;
                    fileName = name;
                    return;
                }
                else
                {
                    EncDecButton.Text = "Encrypt File";
                    EncDecButton.IsVisible = true;
                    filePath = fullPath;
                    fileName = name;
                    return;
                }
            }
        }

        async void EncDecButton_Clicked(object sender, EventArgs e)
        {
            FileEncApp.Services.AesCallerService aesHelper = new FileEncApp.Services.AesCallerService(); // call on aesHelper functions from external file
            if (filePath.Contains(".aes"))
            {
                string rawPass = await DisplayPromptAsync("Decryption Password", "Please enter the password you used for decryption.");
                await aesHelper.DecryptFile(filePath, rawPass, fileName); // accepts String pass and String file path -- await result of encryption
            }
            else
            {
                string rawPass = await DisplayPromptAsync("Encryption Password", "Please enter your password you will use for encryption. Do not forget it or your data will be lost!");
                await aesHelper.EncryptFile(filePath, rawPass, fileName); // accepts String pass and String file path -- await result of encryption
            }
        }
    }
}