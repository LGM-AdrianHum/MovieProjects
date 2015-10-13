// Author: Adrian Hum
// Project: TVDBSharp/Interval.cs
// 
// Created : 2015-03-06  16:06 
// Modified: 2015-04-10 16:05)

using System;

namespace TVDBSharp.Models.Enums {
    public enum Interval {
        Day,
        Week,
        Month,
        All
    }

    public static class IntervalHelpers {
        public static string Print(Interval interval)
        {
            switch (interval)
            {
                case Interval.Day:
                    return "day";
                case Interval.Week:
                    return "week";
                case Interval.Month:
                    return "month";
                case Interval.All:
                    return "all";
                default:
                    throw new ArgumentException("Unsupported interval enum: " + interval);
            }
        }
    }
}