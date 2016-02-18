namespace TestCurl
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class Program
    {
        // Specific city info
        private const string CityName = "SF";
        private const string StateName = "CA";

        // USPS API configure
        static readonly string UspsApiPrefixUrl = ConfigurationManager.AppSettings["UspsApiPrefixUrl"];
        static readonly string UspsUserId = ConfigurationManager.AppSettings["UspsUserId"];

        static readonly string InputFileName = ConfigurationManager.AppSettings["InputFileName"];
        static readonly string OutputFileName = ConfigurationManager.AppSettings["OutputFileName"];
        static readonly string TargetElementNameToFind = ConfigurationManager.AppSettings["TargetElementNameToFind"];
        static readonly string TargetElementAddressToFind = ConfigurationManager.AppSettings["TargetElementAddressToFind"];

        public static void Main(string[] args)
        {
            // create new file
            File.WriteAllText(OutputFileName, "");

            var streetNames = File.ReadAllLines(InputFileName);

            // set a watchTimer for recording time
            var watchTimer = Stopwatch.StartNew();

            // execute validate
            CycleValidateStreetNames(streetNames);

            Console.WriteLine("");
            Console.WriteLine("Succeeded.");
            watchTimer.Stop();
            var elapsedSeconds = (double)watchTimer.ElapsedMilliseconds / (double)1000;
            var averageValidationSeconds = (double)elapsedSeconds / (double)streetNames.Length;

            Console.WriteLine($"Total time eplased: {elapsedSeconds.ToString("F")} s");
            Console.WriteLine($"Average street name validation time: {averageValidationSeconds.ToString("F")} s");
        }

        // TODO: this cycle validate method only supply quintuple street names as an union
        public static void CycleValidateStreetNames(string[] streetNames)
        {
            var succeededCount = 0;
            var totalCount = streetNames.Length;
            const int stepLong = 5;

            for (var i = 0; i < streetNames.Length; i += stepLong)
            {
                var countTime = i + 5;

                if (countTime >= streetNames.Length)
                {
                    break;
                }

                var quinStreetNames = new List<string>
                {
                    streetNames[i],
                    streetNames[i + 1],
                    streetNames[i + 2],
                    streetNames[i + 3],
                    streetNames[i + 4]
                };

                var xmlStringResult = ConcatenateUrlString(quinStreetNames);

                var succeededResultCount = RetriveXml(OutputFileName, xmlStringResult);
                succeededCount += succeededResultCount;

                Show(countTime, totalCount, succeededCount);
            }
        }

        // Concatenate Url String
        public static string ConcatenateUrlString(IReadOnlyList<string> inputStreetNames)
        {
            var streetInfo =
                new XElement("ZipCodeLookupRequest",
                    new XElement("Address",
                            new XElement("Address2", inputStreetNames[0]),
                            new XElement("City", CityName),
                            new XElement("State", StateName),
                            new XAttribute("ID", "0")
                        ),
                    new XElement("Address",
                            new XElement("Address2", inputStreetNames[1]),
                            new XElement("City", CityName),
                            new XElement("State", StateName),
                            new XAttribute("ID", "1")
                        ),
                    new XElement("Address",
                            new XElement("Address2", inputStreetNames[2]),
                            new XElement("City", CityName),
                            new XElement("State", StateName),
                            new XAttribute("ID", "2")
                        ),
                    new XElement("Address",
                            new XElement("Address2", inputStreetNames[3]),
                            new XElement("City", CityName),
                            new XElement("State", StateName),
                            new XAttribute("ID", "3")
                        ),
                    new XElement("Address",
                            new XElement("Address2", inputStreetNames[4]),
                            new XElement("City", CityName),
                            new XElement("State", StateName),
                            new XAttribute("ID", "4")
                        ),
                    new XAttribute("USERID", UspsUserId)
                    ).ToString();
            ;
            var xmlStringFirstPart = "&XML=" + streetInfo;

            return UspsApiPrefixUrl + xmlStringFirstPart;
        }

        // retrive XML response
        public static int RetriveXml(string outputFileName, string xmlStringResult)
        {
            try
            {
                // slow load
                var xmlInMemoryDoc = XDocument.Load(xmlStringResult);
                
                var zipCodeResult =
                   (
                      from e in xmlInMemoryDoc.Descendants()
                      where e.Name == TargetElementNameToFind
                      select e
                   ).ToList();

                var addressResult =
                    (
                      from e in xmlInMemoryDoc.Descendants()
                      where e.Name == TargetElementAddressToFind
                      select e
                   ).ToList();

                // exist not clear error
                if (zipCodeResult.Count != addressResult.Count)
                {
                    return 0;
                }

                var recordCount = zipCodeResult.Count;

                using (var streamWriter = File.AppendText(outputFileName))
                {
                    for (var i = 0; i < recordCount; i++)
                    {
                        streamWriter.WriteLine(addressResult[i].Value + ", " + CityName + " " + StateName + " " + zipCodeResult[i].Value);
                    }
                }

                return recordCount;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // show procudure during running
        public static void Show(int countTime, int totalCountTime, int succeededCount)
        {
            // display the procedure
            if (countTime % 17 == 0)
            {
                var persentNum = (double)countTime / (double)totalCountTime;
                var succfulRatio = (double) succeededCount / (double) countTime;

                Console.WriteLine(" Successful calculated: " + succeededCount + "\n Successful ratio: " +
                                  succfulRatio.ToString("P") + "\n (" + countTime + "/" + totalCountTime + ")  " +
                                  persentNum.ToString("P"));
            }

            countTime += 5;
        }

        // below is not used code, keep it maybe be used later
        //public static string DownLoadResponse(string inputStreetName, string outputFileName)
        //{
        //    const string url = "http://production.shippingapis.com/ShippingAPI.dll?API=ZipCodeLookup";
        //    const string xmlStringFirstPart = "&XML=<ZipCodeLookupRequest USERID=\"192NEXT07990\"><Address ID='0'><Address1></Address1><Address2>";
        //    const string xmlStringLastPart = "</Address2><City>San Francisco</City><State>CA</State></Address></ZipCodeLookupRequest>";

        //    var fullUrl = url + xmlStringFirstPart + inputStreetName + xmlStringLastPart;

        //    try
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create(fullUrl);
        //        request.Method = "GET";
        //        _response = request.GetResponse();
        //        _reader = new StreamReader(_response.GetResponseStream(), Encoding.UTF8);
        //        _result = _reader.ReadToEnd();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        _reader?.Close();
        //        _response?.Close();
        //    }

        //    return _result;
        //    //// Create a file to write to. 
        //    //var createText = _result + Environment.NewLine;
        //    //File.WriteAllText(DowanLoadFileName, createText);
        //}

        //public static void RetriveHtml(string inputStreetName, string outputFileName)
        //{
        //    const string firstPartUrl =
        //        "https://tools.usps.com/go/ZipLookupResultsAction!input.action?resultMode=1&companyName=&address1=";
        //    const string lastPartUrl = "&address2=&city=San+Francisco&state=CA&urbanCode=&postalCode=&zip=";

        //    // retrive zip value from it's span tag
        //    var fullUrl = firstPartUrl + HttpUtility.UrlEncode(inputStreetName) + lastPartUrl;
        //    var html = Client.DownloadString(fullUrl);

        //    var doc = new HtmlDocument();
        //    doc.LoadHtml(html);

        //    try
        //    {
        //        var itemList =
        //            doc.DocumentNode.SelectNodes("//span[@class='zip']")
        //                //this xpath selects all span tag having its class as hidden first
        //                .Select(p => p.InnerText)
        //                .ToList();

        //        foreach (var finalText in itemList.Select(item => inputStreetName + "    Zip Code is: " + item))
        //        {
        //            using (var streamWriter = File.AppendText(outputFileName))
        //            {
        //                streamWriter.WriteLine(finalText);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // Debug
        //        // var finalText = inputStreetName + "    Is Not Valid" + Environment.NewLine;
        //        // File.AppendAllText(outputFileName, finalText);
        //    }
        //}
    }
}
