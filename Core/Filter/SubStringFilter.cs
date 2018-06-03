using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Filter
{
    [DisplayName("Sub String")]
    [Description("Take substring based on starting index and the max string length.")]
    class SubStringFilter : Filter
    {
        public override string Info
        {
            get
            {
                return $"Start Index = {StartIndex}, Length = {Length}.";
            }
        }

        public override string Execute(string input)
        {
            if (StartIndex < 0)
            {
                StartIndex = input.Length - StartIndex;
            }
            if (Length == 0)
            {
                return input.Substring(StartIndex);
            }

            var result = input.Substring(StartIndex, Length);
            return base.Execute(result);
        }

        public override bool Validate(ref string ErrorMsg)
        {
            if (Length <= 0)
            {
                ErrorMsg = "Length must be more than 0.";
            }
            else
            {
                return base.Validate(ref ErrorMsg);
            }
            return false;
        }

        [DisplayName("Start Index")]
        [Description("Negative value to start from end character.")]
        public int StartIndex { get; set; }
        [Description("Maximum length of the string.")]
        public int Length { get; set; }

    }
}
