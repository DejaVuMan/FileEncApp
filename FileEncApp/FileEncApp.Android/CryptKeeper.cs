using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Android.Util;

namespace FileEncApp.Droid
{
    public class CryptKeeper
    {
        /// <remarks>
        /// The count of iterations used in a PKDF2-based encryption for a key consistently rises as computational throughput increases.
        /// As of September 2000, the RFC2898 documentation reccomended 1000+ iterations.
        /// in February 2005, AES in Kerberos 5 has a default value of 4096 rounds with SHA-1 according to RFC3962
        /// iOS 4.x used roughly 10,000 iterations in 2010.
        /// While some organizations, such as StableBit, use over 200,000 iterations (!) with SHA-512, for now this application will use 20,000.
        /// This makes it more difficult for the average hacker to gain access to data while still preserving run-time performance.
        /// 
        /// A relevant formula for this: v * 2^(n-1) > f * p
        /// v = time to verify a single password, f is the power a potential hacker can gather,
        /// n is average entropy of password, p is the "patience" of the hacker.
        /// </remarks>
        int iterations = 20000;
        public bool Encryptor(string filePath, string pass, string filespec)
        {
            try
            {
                using (FileStream outputFs = new FileStream(filespec, FileMode.Create))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] passBytes = Encoding.UTF8.GetBytes(pass); // used for creation of Salt\IV for PBKDF2 and AES
                        byte[] iv = MD5.Create().ComputeHash(passBytes); // create 128 bit IV
                        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pass, iv))// generate PBKDF2 key for use in AES function here
                        {
                            pbkdf2.IterationCount = iterations;

                            byte[] aesKey = SHA256Managed.Create().ComputeHash(pbkdf2.GetBytes(256)); // create 256 bit key from PBKDF2 key bytes
                            
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
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Whoops! An error occurred in FileEncApp: " + e);
                return false;
            }
            return true;
        }

        public bool ScopedStorageEncryptor(String filePath, string pass, Stream fileStream)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] passBytes = Encoding.UTF8.GetBytes(pass); // used for creation of Salt\IV for PBKDF2 and AES
                    byte[] iv = MD5.Create().ComputeHash(passBytes); // create 128 bit IV
                    using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pass, iv))// generate PBKDF2 key for use in AES function here
                    {
                        pbkdf2.IterationCount = iterations;

                        byte[] aesKey = SHA256Managed.Create().ComputeHash(pbkdf2.GetBytes(256)); // create 256 bit key from PBKDF2 key bytes

                        aes.Key = aesKey;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        using (var outStreamEncrypted =
                            new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Whoops! An error occurred in FileEncApp: " + e);
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
                        byte[] iv = MD5.Create().ComputeHash(passBytes); // create 128 bit IV

                        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pass, iv))// generate PBKDF2 key for use in AES function here
                        {
                            pbkdf2.IterationCount = iterations;

                            byte[] aesKey = SHA256Managed.Create().ComputeHash(pbkdf2.GetBytes(256)); // create 256 bit key

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
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Whoops! An error occurred in FileEncApp: " + e); // error most likely not here
                return false;
            }
            return true;
        }

        public bool ScopedStorageDecryptor(String filePath, string pass, Stream fileStream)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] passBytes = Encoding.UTF8.GetBytes(pass); // used for creation of Salt\IV for PBKDF2 and AES
                    byte[] iv = MD5.Create().ComputeHash(passBytes); // create 128 bit IV

                    using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pass, iv))// generate PBKDF2 key for use in AES function here
                    {
                        pbkdf2.IterationCount = iterations;

                        byte[] aesKey = SHA256Managed.Create().ComputeHash(pbkdf2.GetBytes(256)); // create 256 bit key from PBKDF2 key bytes

                        aes.Key = aesKey;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        using (var outStreamDecrypted =
                                   new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Whoops! An error occurred in FileEncApp: " + e); // error most likely not here
                return false;
            }
            return true;
        }
    }
}