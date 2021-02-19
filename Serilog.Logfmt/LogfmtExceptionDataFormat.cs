using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Logfmt
{
    [Flags]
    public enum LogfmtExceptionDataFormat
    {
        None = 0,
        Type = 1,
        Message = 2,
        Level = 4
    }
}
