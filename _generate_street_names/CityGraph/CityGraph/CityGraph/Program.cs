namespace CityGraph
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;

    public class Program
    {
        const string InputFileName = "cities.txt";
        const string GoogleGeoCodingUrl = "https://maps.googleapis.com/maps/api/geocode/xml?address=";
        const string GoogleDirectionUrl = "https://maps.googleapis.com/maps/api/directions/xml?";
        const string GoogleApiKey = "&key=AIzaSyBaVMaAW_bjSd5V-USdDTlrpAJ7-rUzrBI";

        static void Main(string[] args)
        {
            var streetNames = File.ReadAllLines(InputFileName);

            Array.Sort(streetNames);

            var streetMap = new Dictionary<Tuple<string, string>, double>();

            for (var i = 0; i < streetNames.Length; ++i)
            {
                var nearestCityList = new List<Tuple<string, double>>();

                for (var j = 0; j < streetNames.Length; ++j)
                {
                    if (i == j) continue;

                    var originLocation = streetNames[i];
                    var destinationLocation = streetNames[j];

                    var directionUrl = ConcatenateDirectionUrlString(originLocation, destinationLocation);
                    var result = GetDirections(directionUrl);

                    if (result == null) continue;

                    streetMap.Add(Tuple.Create(streetNames[i], streetNames[j]), result.Item1);
                    nearestCityList.Add(Tuple.Create(streetNames[j], result.Item1));
                }

                nearestCityList.Sort();

                Console.WriteLine("City " + streetNames[i] + ": ");
                Console.WriteLine("nearest cities: ");
                foreach (var city in nearestCityList)
                {
                    Console.WriteLine(city);
                }
            }

            //foreach (var streetPair in streetMap)
            //{
            //    Console.WriteLine(streetPair.Key.Item1 + " to " + streetPair.Key.Item2 + " is: " + streetPair.Value);
            //}

        }

        // Concatenate Url String
        public static string ConcatenateGeoCodingUrlString(string cityName)
        {
            var cityNameEncode = HttpUtility.UrlEncode(cityName);
            
            return GoogleGeoCodingUrl + cityName + GoogleApiKey;
        }

        public static string ConcatenateDirectionUrlString(string origin, string destination)
        {
            var originEncode = HttpUtility.UrlEncode(origin);
            var destionationEncode = HttpUtility.UrlEncode(destination);

            var originUrl = "origin=" + originEncode;
            var destionationUrl = "&destination=" + destionationEncode;

            return GoogleDirectionUrl + originUrl + destionationUrl + GoogleApiKey;
        }

        // retrive XML response
        public static Tuple<double, double> GetGeoLocation(string xmlString)
        {
            try
            {
                var xmlInMemoryDoc = XDocument.Load(xmlString);

                var status =
                    (
                        from e in xmlInMemoryDoc.Descendants()
                        where e.Name == "status"
                        select e
                        ).ToList();

                if (status.FirstOrDefault()?.Value.Equals("ok", StringComparison.OrdinalIgnoreCase) == false)
                {
                    return null;
                }

                var geolocation =
                    (
                        from e in xmlInMemoryDoc.Descendants()
                        where e.Name == "location"
                        select e
                        ).ToList();

                var latitude = geolocation.Elements("lat").FirstOrDefault()?.Value;

                double latitudeValue = 0;
                if (latitude != null)
                {
                    latitudeValue = double.Parse(latitude);
                }

                var longitude = geolocation.Elements("lng").FirstOrDefault()?.Value;

                double longitudeValue = 0;
                if (longitude != null)
                {
                    longitudeValue = double.Parse(longitude);
                }

                Console.WriteLine(latitude + " " + longitude);

                return new Tuple<double, double>(latitudeValue, longitudeValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Tuple<double, string> GetDirections(string xmlString)
        {
            try
            {
                var xmlInMemoryDoc = XDocument.Load(xmlString);

                var status =
                    (
                        from e in xmlInMemoryDoc.Descendants()
                        where e.Name == "status"
                        select e
                        ).ToList();

                if (status.FirstOrDefault()?.Value.Equals("ok", StringComparison.OrdinalIgnoreCase) == false)
                {
                    return null;
                }

                var distanceList =
                    (
                        from e in xmlInMemoryDoc.Descendants()
                        where e.Name == "distance"
                        select e
                        ).ToList();

                var metersList =
                    (
                        from e in distanceList.Descendants()
                        where e.Name == "value"
                        select e
                        ).ToList();

                var milesList =
                    (
                        from e in distanceList.Descendants()
                        where e.Name == "text"
                        select e
                        ).ToList();

                var meters = double.Parse(metersList.Last().Value);
                var miles = milesList.Last().Value;

                return Tuple.Create(meters, miles);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error url: " + xmlString);
                Console.WriteLine(ex);
                return null;
            }
        }

        public static double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
        {
            double distance = 0;

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(lat1 / 180 * Math.PI) * Math.Cos(lat2 / 180 * Math.PI)
                        * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            //Calculate distance in meters.
            distance = radius * c;

            return distance; // distance in meters
        }
    }
}
