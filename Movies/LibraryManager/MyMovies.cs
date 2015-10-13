namespace LibraryManager
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MyMovies : DbContext
    {
        public MyMovies()
            : base("name=MyMovies")
        {
        }

        public virtual DbSet<Fileset> Filesets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fileset>()
                .Property(e => e.extension)
                .IsUnicode(false);

            modelBuilder.Entity<Fileset>()
                .Property(e => e.type)
                .IsUnicode(false);
        }
    }
}
