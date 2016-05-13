namespace IPChanger
{
    public class InterfaceInformation
    {
        public string Name { get; set; }
        public bool IsDHCP { get; set; }
        public string IPAddress { get; set; }
        public string IPMask { get; set; }
        public string Gateway { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}