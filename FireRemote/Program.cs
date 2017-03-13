using System;
using System.Diagnostics;
using System.IO;

namespace FireRemote
{
    class Program
    {
        static void Main(string[] args)
        {            
            Process adb = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\con-4\\AppData\\Local\\Android\\sdk\\platform-tools\\adb.exe",
                    Arguments = "kill-server && start-server",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            try
            {
                adb.Start();
                Console.WriteLine("Attempting to start server");
                while (!adb.StandardOutput.EndOfStream)
                {
                    string line = adb.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }
                Console.WriteLine("Server started");
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                Console.WriteLine("ADB not found: {0}", e);
            }

            
            string ipAdd = "192.168.1.156";
            Console.WriteLine("Trying IP Address: {0}", ipAdd);
            int attempts = 0;
            bool connected = false;

            while (attempts < 3 && !connected)
            {
                adb.StartInfo.Arguments = "connect " + ipAdd;
                try
                {
                    adb.Start();
                    Console.WriteLine("Attempting to connnect to device...");
                    connected = true;
                    while (!adb.StandardOutput.EndOfStream)
                    {
                        string line = adb.StandardOutput.ReadLine();
                        if (line.IndexOf("unable to connect") != -1) // If you find "unable to connect" set connection status to false
                            connected = false;                                                                    
                        Console.WriteLine(line);
                    }
                    if (connected) //Otherwise you are connected
                    {
                        Console.WriteLine("Connected! You may use the keyboard to control FireStick.\nIf the firestick is off press P to power it on.");
                    }
                    else
                    {
                        ++attempts;
                        Console.Write("Enter IP address: ");
                        ipAdd = Console.ReadLine();
                    }
                }
                catch (SystemException)
                {

                }
            }
            ConsoleKeyInfo keyinfo;     
            do
            {
                string baseArgs = "-s " + ipAdd + " shell input keyevent ";
                adb.StartInfo.Arguments = baseArgs;
                keyinfo = Console.ReadKey();
                if(keyinfo.Key == ConsoleKey.RightArrow)
                    adb.StartInfo.Arguments = baseArgs + "22";
                else if(keyinfo.Key == ConsoleKey.LeftArrow)
                    adb.StartInfo.Arguments = baseArgs + "21";
                else if (keyinfo.Key == ConsoleKey.UpArrow)
                    adb.StartInfo.Arguments = baseArgs + "19";
                else if (keyinfo.Key == ConsoleKey.DownArrow)
                    adb.StartInfo.Arguments = baseArgs + "20";
                else if (keyinfo.Key == ConsoleKey.Enter)
                    adb.StartInfo.Arguments = baseArgs + "23";
                else if (keyinfo.Key == ConsoleKey.Home)
                    adb.StartInfo.Arguments = baseArgs + "3";
                else if (keyinfo.Key == ConsoleKey.Backspace) // Back
                    adb.StartInfo.Arguments = baseArgs + "4";
                else if (keyinfo.Key == ConsoleKey.P) // Power on
                    adb.StartInfo.Arguments = baseArgs + "26";
                else if (keyinfo.Key == ConsoleKey.Applications) // Menu button
                    adb.StartInfo.Arguments = baseArgs + "82";
                else if (keyinfo.Key == ConsoleKey.MediaNext) // Fast Forward
                    adb.StartInfo.Arguments = baseArgs + "90";
                else if (keyinfo.Key == ConsoleKey.MediaPrevious) // Rewind
                    adb.StartInfo.Arguments = baseArgs + "89";
                else if (keyinfo.Key == ConsoleKey.MediaPlay) // PlayPause
                    adb.StartInfo.Arguments = baseArgs + "85";

                //Todo: add alphabet functionality
                if (adb.StartInfo.Arguments != baseArgs) {
                    try
                    {
                        adb.Start();
                        Console.WriteLine(keyinfo.Key);
                        while (!adb.StandardOutput.EndOfStream)
                        {
                            string line = adb.StandardOutput.ReadLine();
                            Console.WriteLine(line);
                        }
                    }
                    catch (SystemException)
                    {

                    }
                }

            }
            while (keyinfo.Key != ConsoleKey.Escape);
        }
    }
}
