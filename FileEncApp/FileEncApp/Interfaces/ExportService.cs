using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.IO;
using System.Security.Cryptography;

namespace FileEncApp.Interfaces
{
    public class ExportService
    {
        public bool CreateExportData(string filePath, string pass, string fileName)
        {
            return DependencyService.Get<IFileWrite>().ExportFile(filePath, pass, fileName);
        }

        public bool ReadImportData(string filePath, string pass, string fileName)
        {
            return DependencyService.Get<IFileWrite>().ImportFile(filePath, pass, fileName);
        }
    }
}
