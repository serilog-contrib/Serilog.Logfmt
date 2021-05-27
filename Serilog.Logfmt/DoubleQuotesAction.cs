using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Logfmt
{
    enum DoubleQuotesAction
    {
        None,
        ConvertToSingle,
        Escape,
        Remove
    }
}
