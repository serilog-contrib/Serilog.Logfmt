using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Serilog.Logfmt
{
    public class LogfmtOptions
    {

        internal bool NormalizeCase { get; private set; }
        internal bool GrafanaLevels { get; private set; }

        internal LogExceptionOptions ExceptionOptions {get;}

        internal Func<string, bool> PropertyKeyFilter { get; private set; }

        public LogfmtOptions()
        {
            NormalizeCase = true;
            GrafanaLevels = true;
            PropertyKeyFilter = k => false;
            ExceptionOptions = new LogExceptionOptions();
        }

        public LogfmtOptions PreserveCase()
        {
            NormalizeCase = false;
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

    }
}
