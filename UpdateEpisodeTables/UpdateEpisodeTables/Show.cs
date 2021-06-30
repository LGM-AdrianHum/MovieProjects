namespace UpdateEpisodeTables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Show
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TvDbId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}