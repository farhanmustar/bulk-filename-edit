using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Filter
{
    [DisplayName("Original Name")]
    [Description("Will return the original filename.")]
    class RawFilenameFilter : Filter
    {
        public override string Info
        {
            get
            {
                return "Original Name.";
            }
        }

        public override string Execute(string input)
        {
            return base.Execute(input);
        }
    }
}
