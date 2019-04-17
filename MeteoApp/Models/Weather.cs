using System;
using Xamarin.Forms;

namespace MeteoApp
{
    public class Weather
    {
        public Location Location { get; set; }
        public double Temp { get; set; }
        public string IconURL { get; set; }
        public UriImageSource Image { get; set; }

        public void LoadIcon()
        {
            Image = new UriImageSource
            {
                Uri = new Uri(IconURL),
                CachingEnabled = true,
                // For 2 days
                CacheValidity = new TimeSpan(2, 0, 0, 0)
            };
        }
    }
}