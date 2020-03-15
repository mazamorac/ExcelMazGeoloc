using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using System.Text.RegularExpressions;
using System.Reflection;

// ToDo: implement versioning from code instead of project properties
// [assembly: AssemblyVersion("1.2.0.1")]

namespace ExcelMazGeoloc
{
    public class Geoloc
    {
        private const string c_CacheFileName = "LatLonAddInCacheFile.tsv"; // Default name of the geolocation cache file
        private const double c_CacheSaveBatch = 100;                       // Save the cache to disk every N number of entries
        private const double c_CacheStaleDuration = 365 / 2;               // Age out data from the cache after six months - NOT IMPLEMENTED YET
        private const double c_CacheMaxSize = 10000;                       // Max number of items in cache                 - NOT IMPLEMENTED YET
        private const string c_DefaultApi = "G";                           // Default API to query

        public struct LatLon
        {
            // We're using IEEE negative zeroes as a kind of null to indicate "unset value"
            public LatLon(double lat = -0d, double lon = -0d)
            {
                Lat = lat;
                Lon = lon;
            }
            public double Lat { get; }
            public double Lon { get; }
            public override string ToString() => $"{Lat},{Lon}";
        }

        public struct LatLonSquare
        {
            // We're using IEEE negative zeroes as a kind of null to indicate "unset value"
            public LatLonSquare(double southlat = -0d, double westlon = -0d, double northlat = -0d, double eastlon = -0d)
            {
                Southwest = new LatLon(southlat, westlon);
                Northeast = new LatLon(northlat, eastlon);
            }
            public LatLon Southwest { get; }
            public LatLon Northeast { get; }
            public override string ToString() => $"{Southwest.Lat},{Southwest.Lon}|{Northeast.Lat},{Northeast.Lon}";
        }

        public struct GeoData
        {
            public GeoData(string name, string location, string api = c_DefaultApi,
                // We're using IEEE negative zeroes as a kind of null to indicate "unset value"
                double viewsouthlat = -0d, double viewwestlon = -0d, double viewnorthlat = -0d, double vieweastlon = -0d)
            {
                Name = name;                                           // Name of Geodata
                Location = location;                                       // Location of geodata: the string for API query
                Api = String.IsNullOrEmpty(api) ? c_DefaultApi : api; // Which API to use for lookup
                Viewport = new LatLonSquare(viewsouthlat, viewwestlon, viewnorthlat, vieweastlon); // Viewport to bias API query
                LatLon = new LatLon();                                   // Still haven't queried, so it's unset
                Timestamp = DateTime.Now;
            }

            public string Name { get; }
            public string Location { get; }
            public string Api { get; }
            public LatLonSquare Viewport { get; }
            public LatLon LatLon { get; }
            public DateTime Timestamp { get; }

            public override string ToString() => $"({Name},[{Location}],{Api},{Viewport},{LatLon},{Timestamp})";
        }


        public bool b_CacheInitialized = false;
        public Dictionary<string, GeoData> d_CacheLatLonData = new Dictionary<string, GeoData>();
        private readonly double i_CacheDirtyCount = 0;



    }
}
