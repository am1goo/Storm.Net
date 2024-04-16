using System;

namespace Ston
{
    public static class StonExtensions
    {
        public static string Substring(this string text, int startIndex, int length, SubstringOptions options)
        {
            if (options.HasFlag(SubstringOptions.Trimmed))
            {
                var firstIndex = IndexOfAnySymbol(text);
                if (firstIndex < 0)
                    return string.Empty;

                var newIndex = Math.Max(startIndex, firstIndex);
                var delta = newIndex - startIndex;
                startIndex = newIndex;

                var lastIndex = LastIndexOfAnySymbol(text, Math.Min(text.Length, startIndex + (length - delta)));
                if (lastIndex < 0)
                    return string.Empty;

                length = Math.Min(length, lastIndex + 1 - startIndex);
            }
            return text.Substring(startIndex, length);
        }

        public static int IndexOfAnySymbol(this string text, int startIndex = 0)
        {
            var length = text.Length;
            for (int i = startIndex; i < length; ++i)
            {
                var c = text[i];
                if (char.IsWhiteSpace(c))
                    continue;

                return i;
            }

            return -1;
        }

        public static int LastIndexOfAnySymbol(this string text, int startIndex = -1)
        {
            var length = text.Length;
            if (startIndex < 0)
                startIndex = length;

            for (int i = startIndex - 1; i >= 0; --i)
            {
                var c = text[i];
                if (char.IsWhiteSpace(c))
                    continue;

                return i;
            }

            return -1;
        }

        public enum SubstringOptions
        {
            Trimmed = 1 << 0,
        }
    }
}
