namespace UpdateEpisodeTables
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TvShows : DbContext
    {
        public TvShows()
            : base("name=TvShows")
        {
        }

        public virtual DbSet<Episode> Episodes { get; set; }
        public virtual DbSet<Show> Shows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
