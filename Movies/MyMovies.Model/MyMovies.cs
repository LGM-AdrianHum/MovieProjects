// Author: Adrian Hum
// Project: MyMovies.Model/MyMovies.cs
// 
// Created : 2015-10-10  07:14 
// Modified: 2015-10-10 07:16)

using System.Data.Entity;
using System.IO;

namespace MyMovies.Model
{
    public class MyMovies : DbContext
    {
        public MyMovies()
            : base("name=MyMovies") { }

        public virtual DbSet<Fileset> Filesets { get; set; }
        public virtual DbSet<Extension> Extensions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fileset>()
                .Property(e => e.Extension)
                .IsUnicode(false);

            modelBuilder.Entity<Fileset>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<Extension>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Fileset>()
            .HasRequired(c => c.FileExtension)
            .WithMany(p => p.OwnedFiles)
            .HasForeignKey(c => c.Extension);


        }
    }
}