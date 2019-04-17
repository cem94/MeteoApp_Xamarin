
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeteoApp
{
    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
    }
}