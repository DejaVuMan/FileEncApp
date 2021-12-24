using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace FileEncApp.Interfaces
{
    public interface IFileWrite
    {
        bool IsPermissionNeeded();

        bool ExportFile(string filePath, string pass, string fileName); // get path of source file, password to encrypt file with, fileName to append to final path
        bool ImportFile(string filePath, string pass, string fileName); // get path of source file, password to decrypt file with, fileName to append to final path
    }
}
