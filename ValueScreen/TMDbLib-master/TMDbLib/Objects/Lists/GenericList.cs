﻿using System.Collections.Generic;
using Newtonsoft.Json;
using TMDbLib.Objects.General;

namespace TMDbLib.Objects.Lists
{
    public class GenericList : List
    {
        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty("items")]
        public List<MovieResult> Items { get; set; }
    }
}