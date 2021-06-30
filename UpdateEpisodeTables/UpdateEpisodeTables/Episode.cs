namespace UpdateEpisodeTables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Episode
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string ShowName { get; set; }

        public int? SeasonNumber { get; set; }

        public int? EpisodeNumber { get; set; }

        public int? TvDbId { get; set; }

        public int? EpisodeId { get; set; }

        public DateTime? FirstAired { get; set; }

        [StringLength(512)]
        public string Filename { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

    }
}
