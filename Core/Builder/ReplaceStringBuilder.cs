using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Builder
{
    [DisplayName("Replace String")]
    [Description("Search and replaced matched string.")]
    class ReplaceStringBuilder : Builder
    {
        public override string Info
        {
            get
            {
                return $"Replace From: \"{FromStr}\", To: \"{ToStr}\".";
            }
        }

        public override string Execute(string input)
        {
            return input.Replace(FromStr, ToStr);
        }

        public override bool Validate(ref string ErrorMsg)
        {
            if (string.IsNullOrEmpty(FromStr))
            {
                ErrorMsg = "From string cannot be empty.";
            }
            else if (string.IsNullOrEmpty(ToStr))
            {
                ErrorMsg = "To string cannot be empty.";
            }
            else
            {
                return base.Validate(ref ErrorMsg);
            }
            return false;
        }

        [DisplayName("From string")]
        [Description("Source string to replace.")]
        public string FromStr { get; set; }
        [DisplayName("To string")]
        [Description("String to replace to.")]
        public string ToStr { get; set; }
    }
}
