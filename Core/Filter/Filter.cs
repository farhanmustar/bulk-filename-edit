using BulkFIlenameEdit.Core.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Filter
{
    abstract public class Filter : IFormValidator
    {
        /// <summary>
        /// User friendly name of the filter
        /// </summary>
        public string TypeName { get { return GetFriendlyName();  } } 
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

        [Description("Name your filter settings.")]
        public string Name { get; set; }

        /// <summary>
        /// User friendly display string of the current filter configuration
        /// </summary>
        abstract public string Info { get; } 
        virtual public string Execute(string input)
        {
            Result = input;
            return input;
        }

        public string Result { get; private set; }


        public virtual bool Validate(ref string ErrorMsg)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMsg = "Filter must have a name.";
            }
            else
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
