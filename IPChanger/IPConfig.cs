using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPChanger
{
    public class IPConfig
    {

        private static Process CreateIPConfigProcess()
        {
            Process p = new Process();
            p.StartInfo.FileName = "ipconfig.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            return p;
        }

        internal static void SetInterfaceState(List<InterfaceInformation> interfaces)
        {
            Process p = CreateIPConfigProcess();
            p.Start();

            string output = "";

            while (!p.StandardOutput.EndOfStream)
            {
                while (!output.Contains("adapter") && !p.StandardOutput.EndOfStream)
                {
                    output = p.StandardOutput.ReadLine();
                }
                string interfaceName = output.Substring(output.IndexOf("adapter") + 7);
                interfaceName = interfaceName.Substring(0, interfaceName.Length - 1).Trim(); //remove : at end 
                InterfaceInformation interfaceInfo = interfaces.Find(inf => inf.Name.Equals(interfaceName) && inf.EnableAutoPlay);
                if (interfaceInfo != null)
                {
                    while(!output.Contains("Media State") && !(output.Contains("Connection-specific")))
                    {
                        output = p.StandardOutput.ReadLine();
                    }

                    if(output.Contains("Connection-specific"))
                    {
                        interfaceInfo.MediaConnected = true;
                    }
                    else
                    {
                        interfaceInfo.MediaConnected = false;
                    }
                }
                output = p.StandardOutput.ReadLine();
            }
        }
    }
}
