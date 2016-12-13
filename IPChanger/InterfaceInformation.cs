using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
namespace IPChanger
{
    public class InterfaceInformation
    {
        public string Name { get; set; }
        public bool IsDHCP { get; set; }
        public string IPAddress { get; set; }
        public string IPMask { get; set; }
        public string Gateway { get; set; }

        public event EventHandler CablePluggedIn = delegate { };

        private bool enableAutoPlay;
        public bool EnableAutoPlay {
            get { return enableAutoPlay; }
            set { enableAutoPlay = value; }
        }

        private bool mediaConnected = true;
        public bool MediaConnected
        {
            get { return mediaConnected; }
            set {
                if (mediaConnected != value) 
                { 
                    mediaConnected = value;
                    if(mediaConnected)
                    {
                        CablePluggedIn(this, new EventArgs());
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static void Serialize(List<InterfaceInformation> interfaces)
        {
            List<InterfaceInformation> allNotNullInterfaces = new List<InterfaceInformation>();
            foreach (InterfaceInformation intf in interfaces)
            {
                if (intf != null)
                {
                    allNotNullInterfaces.Add(intf);
                }
            }

            var serializer = new XmlSerializer(allNotNullInterfaces.GetType());
            if (!Directory.Exists(@"C:\programdata\riversquid"))
            {
                Directory.CreateDirectory(@"C:\programdata\riversquid");
            }
            using (var writer = XmlWriter.Create(@"C:\programdata\riversquid\Interfaces.xml"))
            {
                serializer.Serialize(writer, allNotNullInterfaces);
            }
        }

        public static List<InterfaceInformation> Deserialize()
        {
            var serializer = new XmlSerializer(typeof(List<InterfaceInformation>));
            List<InterfaceInformation> savedInterfaces = new List<InterfaceInformation>();
            if (File.Exists(@"C:\programdata\riversquid\Interfaces.xml"))
            {
                using (var reader = XmlReader.Create(@"C:\programdata\riversquid\Interfaces.xml"))
                {
                    savedInterfaces = (List<InterfaceInformation>)serializer.Deserialize(reader);
                }
            }
            return savedInterfaces;
        }
    }
}