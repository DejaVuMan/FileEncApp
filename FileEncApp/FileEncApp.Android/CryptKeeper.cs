using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using System.Security.Cryptography;

namespace FileEncApp.Droid
{
    public class CryptKeeper
    {
        public bool Encryptor(string filePath, string pass, string filespec)
        {
            try
            {
                using (FileStream outputFs = new FileStream(filespec, FileMode.Create))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(pass); // testing purposes - we will pass password here in the future
                        byte[] aesKey = SHA256Managed.Create().ComputeHash(passBytes); // create 256 bit key
                        byte[] iv = MD5.Create().ComputeHash(passBytes); // create 128 bit IV
                                                                         // consider PKDF2 implementation in the future
                        aes.Key = aesKey;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        using (var outStreamEncrypted =
                            new CryptoStream(outputFs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            // Encrypt a Chunk of Data at a time to save memory and accomodate large files
                            int count = 0;
                            int offset = 0;

                            //blockSizeBytes can be any value technically
                            int blockSizeBytes = aes.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];
                            int bytesRead = 0;

                            using (var inputFs = new FileStream(filePath, FileMode.Open))
                            {
                                do
                                {
                                    count = inputFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamEncrypted.Write(data, 0, count); // writing and calculating could be very CPU and I/O Intensive
                                    bytesRead += blockSizeBytes;
                                } while (count > 0);
                            }
                            outStreamEncrypted.FlushFinalBlock();
                            File.Delete(filePath); // delete original file only if encryption was successful.
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("Encrypted file creation succeeded. filePath: " + filespec);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed file write. Passed in filePath: " + filePath + ". Full filespec: " + filespec);
                System.Diagnostics.Debug.WriteLine("The following Exception occurred: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }
            return true;
        }

        public bool Decryptor(string filePath, string pass, string filespec)
        {
            try
            {
                using (FileStream outputFs = new FileStream(filespec, FileMode.Create))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(pass); // testing purposes - we will pass password here in the future
                        byte[] aesKey = SHA256Managed.Create().ComputeHash(passBytes); // create 256 bit key
                        byte[] iv = MD5.Create().ComputeHash(passBytes); // create 128 bit IV

                        aes.Key = aesKey;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        using (var outStreamDecrypted =
                                new CryptoStream(outputFs, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            // Encrypt a Chunk of Data at a time to save memory and accomodate large files
                            int count = 0;
                            int offset = 0;

                            //blockSizeBytes can be any value technically
                            int blockSizeBytes = aes.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];
                            int bytesRead = 0;

                            using (var inputFs = new FileStream(filePath, FileMode.Open))
                            {
                                do
                                {
                                    count = inputFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamDecrypted.Write(data, 0, count); // writing and calculating could be very CPU and I/O Intensive
                                    bytesRead += blockSizeBytes;
                                } while (count > 0);
                            }
                            outStreamDecrypted.FlushFinalBlock();
                            File.Delete(filePath); // delete original file only if encryption was successful.
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("Decrypted file creation succeeded. filePath: " + filespec);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed file write. Passed in filePath: " + filePath + ". Full filespec: " + filespec);
                System.Diagnostics.Debug.WriteLine("The following Exception occurred: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }
            return true;
        }
    }
}