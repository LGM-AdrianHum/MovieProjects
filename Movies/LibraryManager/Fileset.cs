namespace LibraryManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Fileset
    {
        public int id { get; set; }

        [StringLength(255)]
        public string filename { get; set; }

        [StringLength(255)]
        public string path { get; set; }

        public long? hash { get; set; }

        [StringLength(15)]
        public string extension { get; set; }

        [StringLength(15)]
        public string type { get; set; }
    }
}
