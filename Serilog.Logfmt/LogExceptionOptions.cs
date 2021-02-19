using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Logfmt
{
    public class LogExceptionOptions
    {
        internal Func<Exception, bool> ExceptionFilter { get; private set; }
        internal LogfmtStackTraceFormat StackTraceFormat { get; private set; }
        
        internal LogfmtExceptionDataFormat ExceptionDataFormat { get; private set; }

        public LogExceptionOptions()
        {
            ExceptionFilter = e => false;
            StackTraceFormat = LogfmtStackTraceFormat.None;
            ExceptionDataFormat = LogfmtExceptionDataFormat.Type | LogfmtExceptionDataFormat.Message  | LogfmtExceptionDataFormat.Level;
        }

        public LogExceptionOptions IgnoreAll()
        {
            ExceptionFilter = e => true;
            return this;
        }

        public LogExceptionOptions IgnoreWhen(Func<Exception, bool> filter)
        {
            ExceptionFilter = filter;
            return this;
        }

        public LogExceptionOptions LogStackTrace(LogfmtStackTraceFormat format)
        {
            StackTraceFormat = format;
            return this;
        }

        public LogExceptionOptions LogExceptionData(LogfmtExceptionDataFormat format)
        {
            ExceptionDataFormat = format;
            return this;
        }


    }
}
