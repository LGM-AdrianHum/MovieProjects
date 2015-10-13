namespace Movies {
    public static class Helper
    {
        public static string ToSafeFilename(this string s)
        {
            return s.Replace(":", ",")
                .Replace("/", " ")
                .Replace("\\", " ")
                .Replace("*", " ")
                .Replace("?", ".")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", " ")
                .Replace("  ", " ");
        }
    }
}