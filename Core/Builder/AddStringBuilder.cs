using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkFIlenameEdit.Core.Filter;
using TFilter = BulkFIlenameEdit.Core.Filter.Filter;

namespace BulkFIlenameEdit.Core.Builder
{
    [DisplayName("Add String")]
    [Description("Take result string from filter and add it into builder list.")]
    class AddStringBuilder : BuilderFromSource
    {
        public override string Info
        {
            get
            {
                return $"Add { (IsTrim? "(trim) " : "") }From: {Source}.";
            }
        }

        public override string Execute(string input)
        {
            var source = Source.Result;
            source = IsTrim ? source.Trim() : source;
            return input + source;
        }
        [DisplayName("Trim source")]
        [Description("Used to trim whitespace in the beginning and end fo file.")]
        public bool IsTrim { get; set; }

        //TODO: this does not work. cannot get parent atrribute. but base type can. ref name prop
        [DisplayName("Source")]
        [Description("Select filter that will supply input string for this builder.")]
        public override TFilter Source
        {
            get
            {
                return base.Source;
            }

            set
            {
                base.Source = value;
            }
        }
    }
}
