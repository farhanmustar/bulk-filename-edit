using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Filter
{
    [DisplayName("Regex")]
    [Description("Match the target string base on regex and take one substring from the result.")]
    class RegexFilter : Filter
    {
        public override string Info
        {
            get
            {
                return $"Regex pattern : \"{RegexPattern}\", Element at : {ListIndex}.";
            }
        }

        [DisplayName("Regex Pattern")]
        [Description("Regex formatted string to match target.")]
        public string RegexPattern { get; set; }

        [DisplayName("List Index")]
        [Description("Index of the string selected from the matched result.")]
        public int ListIndex { get; set; }

        public override string Execute(string input)
        {
            Regex rgx = new Regex(RegexPattern, RegexOptions.None);
            MatchCollection matches = rgx.Matches(input);

            if (matches.Count <= 0)
            {
                return base.Execute("");
            }

            int index = matches.Count > ListIndex ? ListIndex : matches.Count - 1;
            return base.Execute(matches[index].Value);
        }

        public override bool Validate(ref string ErrorMsg)
        {
            ErrorMsg = "";
            if (string.IsNullOrEmpty(RegexPattern))
            {
                ErrorMsg = "Regex required pattern to works.";
                return false;
            }

            try
            {
                Regex.Match("", RegexPattern);
            }
            catch (ArgumentException)
            {
                ErrorMsg = "Regex pattern is invalid";
                return false;
            }

            return base.Validate(ref ErrorMsg);
        }
    }
}
