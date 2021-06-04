using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Serilog.Logfmt
{
    public class LogfmtOptions : IDoubleQuotesOptions
    {

        internal bool NormalizeCase { get; private set; }
        internal bool GrafanaLevels { get; private set; }

        internal LogExceptionOptions ExceptionOptions {get;}

        internal DoubleQuotesAction DoubleQuotesAction { get; private set; }

        internal Func<string, bool> PropertyKeyFilter { get; private set; }

        internal string ComplexPropertySeparator { get; private set; }

        public LogfmtOptions()
        {
            NormalizeCase = true;
            GrafanaLevels = true;
            PropertyKeyFilter = k => false;
            ExceptionOptions = new LogExceptionOptions();
            DoubleQuotesAction = DoubleQuotesAction.ConvertToSingle;
            ComplexPropertySeparator = ".";
        }

        public LogfmtOptions PreserveCase()
        {
            NormalizeCase = false;
            return this;
        }

        public LogfmtOptions UseComplexPropertySeparator(string separator)
        {
            ComplexPropertySeparator = separator;
            return this;
        }

        public LogfmtOptions OnDoubleQuotes(Action<IDoubleQuotesOptions> optionsAction)
        {
            optionsAction?.Invoke(this);
            return this;
        }

        public LogfmtOptions PreserveSerilogLogLevels()
        {
            GrafanaLevels = false;
            return this;
        }

        public LogfmtOptions IncludeAllProperties()
        {
            PropertyKeyFilter = k => true;
            return this;
        }
        
        public LogfmtOptions OnException(Action<LogExceptionOptions> optionsAction)
        {
            optionsAction?.Invoke(ExceptionOptions);
            return this;
        }

        void IDoubleQuotesOptions.Escape()
        {
            DoubleQuotesAction = DoubleQuotesAction.Escape;
        }

        void IDoubleQuotesOptions.ConvertToSingle()
        {
            DoubleQuotesAction = DoubleQuotesAction.ConvertToSingle;
        }
        void IDoubleQuotesOptions.Preserve()
        {
            DoubleQuotesAction = DoubleQuotesAction.None;
        }
        void IDoubleQuotesOptions.Remove()
        {
            DoubleQuotesAction = DoubleQuotesAction.Remove;
        }
    }
}
