using System.Text.RegularExpressions;

namespace Stimulsoft.Data.Expressions.Antlr.Runtime.Misc
{
#if PORTABLE
    using System;
#endif

    internal static class RegexOptionsHelper
    {
        public static readonly RegexOptions Compiled;

        static RegexOptionsHelper()
        {
#if !PORTABLE
            Compiled = RegexOptions.Compiled;
#else
            if (!Enum.TryParse("Compiled", out Compiled))
                Compiled = RegexOptions.None;
#endif
        }
    }
}
