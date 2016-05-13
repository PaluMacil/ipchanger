using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IPChanger
{
    public class SavedInterface
    {

        public string SavedInterfaceName { get; set; }
        public string Name { get; set; }
        public bool IsDHCP { get; set; }
        public string IPAddress { get; set; }
        public string IPMask { get; set; }
        public string Gateway { get; set; }

        public override string ToString()
        {
            return SavedInterfaceName;
        }

        public static void Serialize(List<SavedInterface> info)
        {
            List<SavedInterface> allNotNullInterfaces = new List<SavedInterface>();
            foreach(SavedInterface intf in info)
            {
                if(intf != null)
                {
                    allNotNullInterfaces.Add(intf);
                }
            }

            var serializer = new XmlSerializer(allNotNullInterfaces.GetType());
            if (!Directory.Exists(@"C:\programdata\riversquid"))
            {
                Directory.CreateDirectory(@"C:\programdata\riversquid");
            }
            using (var writer = XmlWriter.Create(@"C:\programdata\riversquid\savedInterfaces.xml"))
            {
                serializer.Serialize(writer, allNotNullInterfaces);
            }
        }

        public static List<SavedInterface> Deserialize()
        {
            var serializer = new XmlSerializer(typeof(List<SavedInterface>));
            List<SavedInterface> savedInterfaces = new List<SavedInterface>();
            if (File.Exists(@"C:\programdata\riversquid\savedInterfaces.xml"))
            {
                using (var reader = XmlReader.Create(@"C:\programdata\riversquid\savedInterfaces.xml"))
                {
                    savedInterfaces = (List<SavedInterface>)serializer.Deserialize(reader);
                }
            }
            return savedInterfaces;
        }

        public static SavedInterface ConvertInterfacetoSavedInterface(InterfaceInformation intf, string savedName)
        {
            SavedInterface newInt = new SavedInterface();
            newInt.SavedInterfaceName = savedName;
            newInt.Name = intf.Name;
            newInt.IPAddress = intf.IPAddress;
            newInt.IPMask = intf.IPMask;
            newInt.Gateway = intf.Gateway;
            newInt.IsDHCP = intf.IsDHCP;

            return newInt;
        }

    }
}
