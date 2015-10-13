// Author: Adrian Hum
// Project: TVDBSharp/Status.cs
// 
// Created : 2015-03-06  16:06 
// Modified: 2015-04-10 16:05)

namespace TVDBSharp.Models.Enums {
    /// <summary>
    ///     Describes the current status of a show.
    /// </summary>
    public enum Status {
        /// <summary>
        ///     No more episodes are being released.
        /// </summary>
        Ended,

        /// <summary>
        ///     The show is ongoing.
        /// </summary>
        Continuing,

        /// <summary>
        ///     Default value if no status is specified.
        /// </summary>
        Unknown
    }
}