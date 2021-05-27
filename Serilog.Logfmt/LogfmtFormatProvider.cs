using System;

namespace Serilog.Logfmt
{
    internal class LogfmtFormatProvider : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }
    }
}