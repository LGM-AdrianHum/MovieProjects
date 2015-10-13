// Author: Adrian Hum
// Project: MyMovies.Updater/Program.cs
// 
// Created : 2015-10-10  07:25 
// Modified: 2015-10-10 07:48)

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using MyMovies.Model;

namespace MyMovies.Updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var db = new Model.MyMovies();
            var dict = db.Extensions.ToDictionary(x => x.Name, y => y);
            var queue = new Queue<string>(new[] { @"\\admin-pc\g" });
            while (queue.Any())
            {
                try
                {
                    var a = queue.Dequeue();
                    Console.WriteLine(a);
                    foreach (var aa in Directory.EnumerateDirectories(a)) queue.Enqueue(aa);
                    foreach (var aa in Directory.GetFiles(a).Select(x => new Fileset(x)))
                    {


                        Console.Write("\t{0} --- ", aa.Filename);
                        
                        aa.FileExtension = dict[aa.Extension];
                        if (!aa.FileExtension.DoNotHash)
                        {
                            var fileStream = new FileStream(aa.Fullname, FileMode.Open, FileAccess.Read);
                            aa.Hash = GetSha512Buffered(fileStream);
                        }
                        else Console.WriteLine("No Hash");



                        //Console.WriteLine(aa.Hash);
                        db.Filesets.Add(aa);

                    }
                }
                catch
                {
                    //
                }
                db.SaveChanges();
            }
        }

        public static string GetSha512Buffered(Stream streamIn)
        {
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