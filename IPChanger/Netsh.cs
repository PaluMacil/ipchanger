using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IPChanger
{
    public static class Netsh
    {
        internal static Process CreateNetShProcess(string arguments)
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            return p;
        }

        internal static List<InterfaceInformation> GetAllInterfaceInformation()
        {
            string interfaces = "";
            List<InterfaceInformation> interfaceList = new List<InterfaceInformation>();
            Process p = CreateNetShProcess("interface ipv4 show interfaces");
            p.Start();

            string output = "";

            while (!output.Contains("---"))
            {
                output = p.StandardOutput.ReadLine();
            }

            //next lines will be the interfaces

            while (!p.StandardOutput.EndOfStream)
            {
                string[] interfaceParts = p.StandardOutput.ReadLine().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (interfaceParts.Length >= 5)
                {
                    //likely a valid interface
                    string interfaceName = "";
                    for (int i = 4; i < interfaceParts.Length; i++)
                    {
                        interfaceName += interfaceParts[i] + " ";
                    }
                    interfaceName = interfaceName.Substring(0, interfaceName.Length - 1);
                    interfaceList.Add(GetInterfaceInformation(interfaceName));
                    interfaces += interfaceName + ",";
                }
            }
            interfaces = interfaces.Substring(0, interfaces.Length - 1);

            return interfaceList;
        }

        internal static InterfaceInformation GetInterfaceInformation(string interfaceName)
        {
            Process p = CreateNetShProcess("interface ipv4 show addresses \"" + interfaceName + "\"");
            p.Start();

            string output = "";

            while (!output.Contains("Configuration for"))
            {
                output = p.StandardOutput.ReadLine();
            }

            //next line will be interface information
            InterfaceInformation information = new InterfaceInformation();
            information.Name = interfaceName;

            output = p.StandardOutput.ReadLine();

            //always confirm we're reading the correct lines
            if (output == null && p.StandardOutput.EndOfStream)
            {
                return information;
            }
            if (output.Contains("DHCP"))
            {
                information.IsDHCP = output.Contains("Yes");
            }

            //next line IP
            output = p.StandardOutput.ReadLine();
            if (output == null && p.StandardOutput.EndOfStream)
            {
                return information;
            }
            if (output.Contains("IP Address"))
            {
                string[] ipSplit = output.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                //split parts should be "IP", "Address", and the actual IP address

                information.IPAddress = ipSplit[2];
            }

            output = p.StandardOutput.ReadLine();
            if (output == null && p.StandardOutput.EndOfStream)
            {
                return information;
            }
            //next line MASK
            if (output.Contains("Subnet Prefix"))
            {
                string[] maskSplit = output.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                information.IPMask = maskSplit[4].Substring(0, maskSplit[4].Length - 1); //remove trailing parenthesis
            }

            output = p.StandardOutput.ReadLine();
            if (output == null && p.StandardOutput.EndOfStream)
            {
                return information;
            }
            //next line gateway
            if (output.Contains("Default Gateway"))
            {
                string[] gatewaySplit = output.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                information.Gateway = gatewaySplit[2];
            }

            return information;
        }

        /// <summary>
        /// Sets the interface given the specified information.
        /// </summary>
        /// <param name="savedInterface">interface information to be set</param>
        /// <returns>Returns true if successful</returns>
        internal static bool SetInterface(SavedInterface savedInterface)
        {
            if (savedInterface.IsDHCP)
            {
                string arguments = string.Format("interface ipv4 set address name=\"{0}\" source=dhcp", savedInterface.Name);
                Process p = CreateNetShProcess(arguments);
                p.Start();

                //need to wait for results to update in netsh.
                System.Threading.Thread.Sleep(5000);
                InterfaceInformation newInformation = GetInterfaceInformation(savedInterface.Name);
                if (newInformation.IsDHCP)
                {
                    return true;
                }
            }
            else
            {
                string arguments = string.Format("interface ipv4 set address name=\"{0}\" sour=static address={1} mask={2} gateway={3}", savedInterface.Name, savedInterface.IPAddress, savedInterface.IPMask, savedInterface.Gateway);

                Process p = CreateNetShProcess(arguments);
                p.Start();

                //need to wait for results to update in netsh.
                System.Threading.Thread.Sleep(5000);
                //need to confirm the change worked
                InterfaceInformation newInformation = GetInterfaceInformation(savedInterface.Name);
                if (newInformation.IPAddress.Equals(savedInterface.IPAddress))
                {
                    return true;
                }
            }
            return false;
        }
    }
}