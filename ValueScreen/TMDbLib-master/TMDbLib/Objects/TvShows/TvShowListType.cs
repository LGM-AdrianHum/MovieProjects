﻿using TMDbLib.Utilities;

namespace TMDbLib.Objects.Movies
{
    public enum TvShowListType
    {
        [EnumValue("on_the_air")]
        OnTheAir,
        [EnumValue("airing_today")]
        AiringToday,
        [EnumValue("top_rated")]
        TopRated,
        [EnumValue("popular")]
        Popular
    }
}