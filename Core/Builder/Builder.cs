using BulkFIlenameEdit.Core.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using  TFilter = BulkFIlenameEdit.Core.Filter.Filter;

namespace BulkFIlenameEdit.Core.Builder
{
    abstract public class Builder : IFormValidator
    {
        /// <summary>
        /// User friendly name of the filter
        /// </summary>
        public string TypeName { get { return GetFriendlyName(); } }
        private string GetFriendlyName()
        {
            var type = this.GetType();
            string name = type.Name;
            var attr = type.GetCustomAttribute(typeof(DisplayNameAttribute));
            if (attr != null)
            {
                var nameAttr = attr as DisplayNameAttribute;
                if (nameAttr != null)
                {
                    name = nameAttr.DisplayName;
                }
            }
            return name;
        }
        
        [Description("Name your builder settings.")]
        public string Name { get; set; }
        abstract public string Info { get;}

        //TODO: this should receive string builder instead.
        abstract public string Execute(string input);

        //TODO: use event maybe make more sense here.
        public virtual bool Validate(ref string ErrorMsg)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMsg = "Builder must have a name.";
            }
            else
            {
                return true;
            }
            return false;
        }
    }
    abstract public class BuilderFromSource : Builder
    {
        [DisplayName("Source")]
        [Description("Input string.")]
        public virtual TFilter Source { get; set; }

        override public bool Validate(ref string ErrorMsg)
        {
            if (Source == null)
            {
                ErrorMsg = "Builder must have a string source.";
            }
            else
            {
                return base.Validate(ref ErrorMsg);
            }

            return false;
        }
    }
}
