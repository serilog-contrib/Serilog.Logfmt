using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Logfmt
{
    public interface IDoubleQuotesOptions
    {
        void Escape();
        void ConvertToSingle();
        void Preserve();
        void Remove();
    }
}
