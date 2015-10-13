using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var AllFileInfos = new List<FileInfo>();
            var AllExtensions = new List<string>();
            var wq = new Queue<string>();
            wq.Enqueue(@"\\admin-pc\g");
            while (wq.Any())
            {
                var a = wq.Dequeue();
                Console.WriteLine(a);
                foreach (var d in Directory.EnumerateDirectories(a))
                {
                    wq.Enqueue(d);
                }
                AllFileInfos.AddRange(Directory.GetFiles(a).Select(x=>new FileInfo(x)));
                
            }
            AllExtensions = AllFileInfos.Select(x => x.Extension).Distinct().ToList();
            Console.WriteLine(AllExtensions.Count);
            Console.ReadLine();
        }
    }
}
