using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BigFileHasher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"g:\hashfile.txt"))
            {

                var queue = new Queue<string>(new[] { @"g:\" });
                while (queue.Any())
                {
                    try
                    {
                        var a = queue.Dequeue();
                        Console.WriteLine(a);
                        foreach (var aa in Directory.EnumerateDirectories(a)) queue.Enqueue(aa);
                        foreach (var aa in Directory.GetFiles(a))
                        {


                            Console.Write("\t{0} --- ", aa);

                            var fileStream = new FileStream(aa, FileMode.Open, FileAccess.Read);
                            var hash = GetSha512Buffered(fileStream);

                            file.Write(string.Format("{0}||{1}", aa, hash));

                            //Console.WriteLine(aa.Hash);

                        }
                    }
                    catch
                    {
                        //
                    }
                }
            }
        }

        public static
            string GetSha512Buffered
            (Stream
                streamIn)
        {
            if ((streamIn.Length/1000000) < 50)
            {
                Console.WriteLine("No Hash");
                return "No Hash";
            }
            var csrleft = Console.CursorLeft;
            Process process = Process.GetCurrentProcess();
            const int bufferSizeForMd5Hash = 1024 * 1024 * 8; // 8MB
            string hashString;
            using (var md5Prov = new SHA256Managed())
            {
                int readCount;
                long bytesTransfered = 0;
                var buffer = new byte[bufferSizeForMd5Hash];
                while ((readCount = streamIn.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // Need to figure out if this is final block
                    if (bytesTransfered + readCount == streamIn.Length)
                    {
                        md5Prov.TransformFinalBlock(buffer, 0, readCount);
                    }
                    else
                    {
                        md5Prov.TransformBlock(buffer, 0, bufferSizeForMd5Hash, buffer, 0);
                    }
                    bytesTransfered += readCount;
                    Console.CursorLeft = csrleft;
                    Console.Write(" {0}MB/{1}MB.   Memory Used: {2}MB      ",
                        bytesTransfered / 1000000,
                        streamIn.Length / 1000000,
                        process.PrivateMemorySize64 / 1000000);
                }

                hashString = BitConverter.ToString(md5Prov.Hash).Replace("-", String.Empty);
                Console.WriteLine(hashString);
                md5Prov.Clear();
            }
            return hashString;
        }
    }
}
