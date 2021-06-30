// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TorData.cs" >
//   This file is part of the AccessPirateBayByUrl distribution
// 
//   Copyright (c) 2019
// 
//   This program is distributed WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//   FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   <developer>Casandra/Adrian Hum</developer>
//   <solution>AccessPirateBayByUrl/AccessPirateBayByUrl/TorData.cs</solution>
//   <created>-- </created>
//   <modified>2019-12-06 3:00 PM</modified>
// </summary>
//  --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace AccessPirateBayByUrl {
    public class TorData
    {

        public enum OrderByOptions
        {
            Name = 1,
            Uploaded = 3,
            Size = 5,
            Uploader = 11,
            Seeders = 8,
            Leechers = 9,
            Type = 13
        }
        private static string GetPage(string url)
        {
            var request = WebRequest.Create(url);
            // Set the Method property of the request to POST.  
            request.Method = "GET";

            var response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            string responseFromServer;
            // Get the stream containing content returned by the server.  
            // The using block ensures the stream is automatically closed.
            using (var dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                var reader = new StreamReader(dataStream);
                // Read the content.  
                responseFromServer = reader.ReadToEnd();
                // Display the content.  
            }

            response.Close();
            return responseFromServer;
        }

        private static IEnumerable<TorData> ProcessPage(string responseFromServer)
        {
            var res = new List<TorData>();
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(responseFromServer);

            var titleNodes = htmlDoc.DocumentNode.SelectNodes("//table[@id=\"searchResult\"]//tr");
            for (var i = 1; i < titleNodes.Count; i++)
            {
                var node = titleNodes[i];

                var ll = node.SelectSingleNode(".//div[@class=\"detName\"]");
                if (ll == null) continue;
                var aa = ll.SelectSingleNode(".//a[@class=\"detLink\"]");
                var pa = ll.ParentNode.SelectSingleNode(".//a[starts-with(@href, 'magnet:')]").Attributes
                             .FirstOrDefault(x => x.Name == "href")?.Value ?? "";
                var status = ll.ParentNode.SelectSingleNode(".//font[@class='detDesc']").InnerText
                    .Replace("&nbsp;", " ").Split(',').Select(x => x.Trim()).ToArray();
                var seedleech = ll.ParentNode.ParentNode.SelectNodes(".//td[@align='right']")
                    .Select(x => x.InnerText.Trim()).ToArray();
                var cat = ll.ParentNode.ParentNode.SelectNodes(".//td[@class='vertTh']//a")
                    .Select(x => x.InnerText.Trim()).ToArray();
                var tordata = new TorData
                {
                    Href = aa.Attributes.FirstOrDefault(x => x.Name == "href")?.Value,
                    Title = aa.Attributes.FirstOrDefault(x => x.Name == "title")?.Value,
                    Magnet = pa,
                    StatusUploaded = status[0].Replace("Uploaded", "").Trim(),
                    StatusSize = status[1].Replace("Size", "").Trim(),
                    StatusUploader = status[2].Replace("ULed by", "").Trim(),
                    Seed = int.TryParse(seedleech[0], out var i1) ? i1 : 0,
                    Leech = int.TryParse(seedleech[1], out var i2) ? i2 : 0,
                    Cat = cat[0],
                    SubCat = cat[1]
                };
                var rx = new Regex(@"([Ss]?)(?<season>[0-9]{1,2})(?<episode>([eE\.\-])([0-9]{1,2})){1,3}");

                var mc = rx.Matches(tordata?.Title);
                if (mc.Count <= 0) continue;
                tordata.Season = mc[0].Groups["season"].Value;
                var sb = new List<string>();
                for (var index = 0; index < mc.Count; index++)
                {
                    var m = mc[index];
                    sb.AddRange(from object ep in m.Groups["episode"].Captures select ep.ToString().Replace("E", "").Replace("e", ""));
                }

                tordata.Episodes = sb;
                res.Add(tordata);
            }

            return res;
        }

        public string Href { get; set; }
        public string Title { get; set; }
        public string Magnet { get; set; }
        public string Status { get; set; }
        public string StatusUploaded { get; set; }
        public string StatusSize { get; set; }
        public string StatusUploader { get; set; }
        public int Leech { get; set; }
        public int Seed { get; set; }
        public string SubCat { get; set; }
        public string Cat { get; set; }
        public string Season { get; set; }
        public List<string> Episodes { get; set; }
    }
}