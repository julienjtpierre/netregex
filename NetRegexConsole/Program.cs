using NetRegex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetRegexConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RegexExecution r = new RegexExecution
            {
                Pattern = @"App\d{2}",
                Text = "App01",
                Replace = null,
                Options = RegexOptions.None,
                MatchMultipleStrings = false,
                IgnoreMaximumMatches = false
            };

            r.Execute();
        }
    }
}
