// Author: Adrian Hum
// Project: MyMovies.Model/Fileset.cs
// 
// Created : 2015-10-10  07:14 
// Modified: 2015-10-10 07:47)

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace MyMovies.Model
{
    public class Extension
    {
        [Key]
        public string Name { get; set; }

        public bool Ignore { get; set; }

        public bool DoNotHash { get; set; }

        public ICollection<Fileset> OwnedFiles { get; set; }
    }

    public class Fileset
    {
        public Fileset(string s)
        {
            Filename = System.IO.Path.GetFileName(s);
            FilePath = System.IO.Path.GetDirectoryName(s);
            Extension = System.IO.Path.GetExtension(s);
        }

        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Filename { get; set; }

        [StringLength(255)]
        public string FilePath { get; set; }

        public string Hash { get; set; }

        [StringLength(15)]
        public string Extension { get; set; }

        [StringLength(15)]
        public string Type { get; set; }
        [NotMapped]
        public string Fullname { get { return Path.Combine(FilePath, Filename); } }

        public Extension FileExtension { get; set; }
    }
}