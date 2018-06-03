using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Filter
{
    [DisplayName("Split")]
    [Description("Split the string and take one substring from the result.")]
    class SplitFilter : Filter
    {
        public override string Info
        {
            get
            {
                return $"Split string : \"{SplitCharacter}\", Element at : {ListIndex}.";
            }
        }

        public override string Execute(string input)
        {
            var split = input.Split(SplitCharacter.ToCharArray());
            var index = ListIndex;
            index = input.Length <= index ? input.Length-1 : index;
            return base.Execute(split[index]);
        }

        public override bool Validate(ref string ErrorMsg)
        {
            ErrorMsg = "";

            if (string.IsNullOrEmpty(SplitCharacter))
            {
                ErrorMsg = "Split filter required character to works.";
            }
            else
            {
                return base.Validate(ref ErrorMsg);
            }

            return false;
        }

        [DisplayName("Split Character")]
        [Description("List character used to split the string without any space.")]
        public string SplitCharacter { get; set; }

        [DisplayName("List Index")]
        [Description("Index of the string selected from the splitted result.")]
        public int ListIndex { get; set; }

    }
}
