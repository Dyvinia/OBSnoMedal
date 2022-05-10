using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace OBSnoMedal {
    internal class Program {

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MAXIMIZE = 0xF030;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args) {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Do Not Manually Close This Window!");
            Console.WriteLine("----------------------------------");

            string OBSPath;
            string medalPath;

            using (RegistryKey OBSKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\OBS Studio"))
                if (OBSKey != null) {
                    OBSPath = (string)OBSKey.GetValue("");
                    OBSPath += "\\bin\\64bit\\obs64.exe";
                } else OBSPath = null;

            medalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            medalPath += "\\Medal\\Medal.exe";

            if ((medalPath != null || OBSPath != null) && File.Exists(medalPath) && File.Exists(OBSPath)) {
                if (Process.GetProcessesByName("Medal").Length > 0) {
                    Console.WriteLine("Killing Medal");
                    foreach (var process in Process.GetProcessesByName("Medal")) {
                        process.Kill();
                    }
                    Console.WriteLine("Killed Medal");
                }
                else Console.WriteLine("Medal not currently running");

                Console.WriteLine("Starting OBS");
                Process OBS = new Process();
                OBS.StartInfo.FileName = OBSPath;
                OBS.StartInfo.WorkingDirectory = Path.GetDirectoryName(OBSPath);
                OBS.Start();
                Console.WriteLine("Started OBS");
                Console.WriteLine("Waiting for OBS to close");
                OBS.WaitForExit();
                Console.WriteLine("OBS Exited");

                Console.WriteLine("Starting Medal");
                Process medal = new Process();
                medal.StartInfo.FileName = medalPath;
                medal.StartInfo.WorkingDirectory = Path.GetDirectoryName(medalPath);
                medal.Start();

                Thread.Sleep(2000);
            }
            else if (medalPath == null) {
                Console.WriteLine("Medal path not found");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
            else if (OBSPath == null) {
                Console.WriteLine("OBS path not found");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
            else if (!File.Exists(medalPath)) {
                Console.WriteLine($"Path not valid: {medalPath} ");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
            else if (!File.Exists(OBSPath)) {
                Console.WriteLine($"Path not valid: {OBSPath} ");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }
    }
}
