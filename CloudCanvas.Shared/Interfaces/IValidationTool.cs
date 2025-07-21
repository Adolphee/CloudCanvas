using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Interfaces
{
    public interface IValidationTool
    {
        void ValidateString(string paramName, string paramValue);
        void ValidatePositiveNumber(string paramName, double paramValue, double min, double max);

    }
}
