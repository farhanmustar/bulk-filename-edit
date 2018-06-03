using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Core.Forms
{
    public interface IFormValidator 
    {
        bool Validate(ref string ErrorMsg);
    }
}
