using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace EZAutoclickerWPF.Logging
{
    public static class CreateLogs
    {
        //A custom method that is use in another project (not out yet)
        //but cut down a bit here
        public static void MakeLog(string filename, string start_Close_text)
        {
            //TODO: Remake this whole method

            string path = @"EZAutoclickerLOG__";
            string fileend = ".txt";
            string name = filename;
            string time = DateTime.Now.ToString("yyyy/MM/dd_HH/mm");
            var os = RuntimeInformation.OSDescription;
            string assemblyversion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            try
            {
                File.WriteAllText(path
                    + time
                    + name
                    + fileend, start_Close_text
                    + time
                    + "\nWith: "
                    + "\nOs version: "
                    + os
                    + "\nEZAutoclicker version: "
                    + assemblyversion);
            }
            catch
            {
                MessageBox.Show("Could not create logs", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}