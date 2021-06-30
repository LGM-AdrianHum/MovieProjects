using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AccessPirateBayByUrl
{
    class Program
    {
        static void Main(string[] args) {
            var searchterm = "hudson";
            var orderbyint = (int)TorData.OrderByOptions.Uploaded;
            var cat = 0;
            
            Console.Clear();
            Console.WriteLine(sdata);
            Console.ReadLine();
        }

        
    }
}
