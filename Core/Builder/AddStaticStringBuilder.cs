using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Builder
{
    [DisplayName("Add Static String")]
    [Description("Add pre defined string into the build list.")]
    class AddStaticStringBuilder : Builder
    {
        public override string Info
        {
            get
            {
                return $"Add String: \"{this.String}\".";
            }
        }

        public override string Execute(string input)
        {
            return input + this.String;
        }

        public override bool Validate(ref string ErrorMsg)
        {
            if (string.IsNullOrEmpty(this.String))
            {
                ErrorMsg = "Static string cannot be empty.";
            }
            else
            {
                return base.Validate(ref ErrorMsg);
            }
            return false;
        }

        [DisplayName("Static String")]
        [Description("String to be added.")]
        public string @String { get; set; }
    }
}
