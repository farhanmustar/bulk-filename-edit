using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Filter
{
    [DisplayName("In Between")]
    [Description("Match the target string base on regex and take one substring from the result.")]
    class InBetweenSubStringFilter : Filter
    {
        public override string Info
        {
            get
            {
                return $"Prefix : \"{Prefix}\", Suffix : \"{Suffix}\", Element at : {ListIndex}.";
            }
        }

	// '\' char must be the first one due to the algo.
	private const string regex_special_char="\\.+*?[^]$(){}=!<>|:-";

        [DisplayName("Prefix")]
        [Description("Starting string before the target string.")]
        public string Prefix { get; set; }

        [DisplayName("Suffix")]
        [Description("Ending string after the target string.")]
        public string Suffix { get; set; }

        [DisplayName("List Index")]
        [Description("Index of the string selected from the matched result.")]
        public int ListIndex { get; set; }

        public override string Execute(string input)
        {
	    string prefix = Prefix;
	    string suffix = Suffix;
	    foreach(char c in regex_special_char) {
		    prefix = prefix.Replace(c.ToString(), $"\\{c}");
		    suffix = suffix.Replace(c.ToString(), $"\\{c}");
	    }

            Regex rgx = new Regex($"(?<={prefix})(.*?)(?={suffix})", RegexOptions.None);
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
            if (string.IsNullOrEmpty(Prefix) || string.IsNullOrEmpty(Suffix))
            {
                ErrorMsg = "Prefix and suffix cannot be empty.";
                return false;
            }

            return base.Validate(ref ErrorMsg);
        }
    }
}
