using System;
using System.Runtime.CompilerServices;

namespace BazaarBot
{
    internal static class StringExtensions
    {
        #region Comparisons

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsOrdinal(this string strA, string strB)
            => string.CompareOrdinal(strA, strB) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsOrdinalIgnoreCase(this string strA, string strB)
            => string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsIgnoreCase(this string source, string query)
            => source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// Quicker than <see cref="string.Equals"/>.
        /// </summary>
        /// <exception cref="NullReferenceException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool QuickEquals(this string source, string query)
            => source.Length == query.Length && source.QuickStartsWith(query);

        /// <summary>
        /// Quicker than <see cref="String.EndsWith(string)"/>. Prefer this for 
        /// non-localized strings because it compares the byte value rather than 
        /// the character represented by the value. 
        /// </summary>
        /// <remarks>https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html#:~:text=Inefficient%20built%2Din%20string%20APIs</remarks>
        public static bool QuickEndsWith(this string a, string b)
        {
            int ap = a.Length - 1;
            int bp = b.Length - 1;

            while (ap >= 0 && bp >= 0 && a[ap] == b[bp])
            {
                ap--;
                bp--;
            }

            return (bp < 0);
        }

        /// <summary>
        /// Quicker than <see cref="String.StartsWith(string)"/>.
        /// non-localized strings because it compares the byte value rather than 
        /// the character represented by the value. 
        /// </summary>
        /// <remarks>https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html#:~:text=Inefficient%20built%2Din%20string%20APIs</remarks>
        public static bool QuickStartsWith(this string a, string b)
        {
            int aLen = a.Length;
            int bLen = b.Length;

            int ap = 0;
            int bp = 0;

            while (ap < aLen && bp < bLen && a[ap] == b[bp])
            {
                ap++;
                bp++;
            }

            return (bp == bLen);
        }

        #endregion Comparisons
    }
}
