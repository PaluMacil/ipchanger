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
            var serializer = new XmlSerializer(info.GetType());
            if (!Directory.Exists(@"C:\programdata\riversquid"))
            {
                Directory.CreateDirectory(@"C:\programdata\riversquid");
            }
            using (var writer = XmlWriter.Create(@"C:\programdata\riversquid\savedInterfaces.xml"))
            {
                serializer.Serialize(writer, info);
            }
        }

        public static List<SavedInterface> Deserialize()
        {
            var serializer = new XmlSerializer(typeof(List<SavedInterface>));
            List<SavedInterface> savedInterfaces = null;
            if (File.Exists(@"C:\programdata\riversquid\savedInterfaces.xml"))
            {
                using (var reader = XmlReader.Create(@"C:\programdata\riversquid\savedInterfaces.xml"))
                {
                    savedInterfaces = (List<SavedInterface>)serializer.Deserialize(reader);
                }
            }
            return savedInterfaces;
        }

    }
}
