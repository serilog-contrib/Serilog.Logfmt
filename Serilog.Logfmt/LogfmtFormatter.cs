using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Serilog.Logfmt
{
    public class LogfmtFormatter : ITextFormatter
    {
        private readonly LogfmtValueFormatter _formatter;
        private readonly LogfmtOptions _options;
        private Func<string, bool> _propertyKeyFilter;

        public LogfmtFormatter(Action<LogfmtOptions> configOptions = null)
        {
            _options = new LogfmtOptions();
            configOptions?.Invoke(_options);
            _formatter = new LogfmtValueFormatter(_options);
            _propertyKeyFilter = _options.PropertyKeyFilter;
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));
            output.Write("ts={0} ", logEvent.Timestamp.UtcDateTime.ToString("o"));


            output.Write("level={0} ", _options.GrafanaLevels ?  GrafanaLevelValue(logEvent.Level) : logEvent.Level.ToString());
            foreach (var property in logEvent.Properties.Where(p => _propertyKeyFilter(p.Key)))
            {
                var key = _options.NormalizeCase ? GetNormalizedKeyCase(property.Key) : property.Key;
                output.Write("{0}=", key);
                _formatter.Format(property.Value, output);
                output.Write(" ");
            }
            output.Write("msg=");

            var msg = "";
            using (var sw = new StringWriter())
            {
                logEvent.RenderMessage(sw);
                msg = sw.ToString();
            }

            output.WriteLine("{1}{0}{1} ", msg, msg.Contains(" ") ? "\"" :  "");

            if (logEvent.Exception != null)
            {
                LogException(logEvent, output);
            }

        }

        private void LogException(LogEvent logEvent, TextWriter output)
        {
            var exception = logEvent.Exception;
            var dataOptions = _options.ExceptionOptions.ExceptionDataFormat;
            output.Write("ts={0} ", logEvent.Timestamp.UtcDateTime.ToString("o"));
            if (dataOptions != LogfmtExceptionDataFormat.None)
            {
                
                if (dataOptions.HasFlag(LogfmtExceptionDataFormat.Level))
                {
                    output.Write("level={0} ", _options.GrafanaLevels ? "err" : LogEventLevel.Error.ToString());
                }
                if (dataOptions.HasFlag(LogfmtExceptionDataFormat.Type))
                {
                    output.Write("exception={0} ", exception.GetType().Name);
                }
                if (dataOptions.HasFlag(LogfmtExceptionDataFormat.Message))
                {
                    output.Write("err=\"{0}\" ", exception.Message);
                }
            }
            switch (_options.ExceptionOptions.StackTraceFormat)
            {
                case LogfmtStackTraceFormat.All:
                    output.Write(exception.StackTrace);
                    break;
                case LogfmtStackTraceFormat.SingleLine:
                    output.Write(exception.StackTrace.Replace("\n", "").Replace("\r",""));
                    break;
                default: break;
            }
            output.WriteLine();
        }

        private string GrafanaLevelValue(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug: return "debug";
                case LogEventLevel.Error: return "err";
                case LogEventLevel.Information: return "info";
                case LogEventLevel.Fatal: return "critical";
                case LogEventLevel.Warning: return "warn";
                case LogEventLevel.Verbose: return "trace";
                default: return "unknown";
            }
        } 

        private string GetNormalizedKeyCase(string key)
        {
            var sb = new StringBuilder(key.Length);
            var first = true;
            foreach (var c in key)
            {
                if (char.IsUpper(c))
                {
                    if (!first)
                    {
                        sb.Append('_');
                    }
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
                first = false;
            }

            return sb.ToString();
        }
    }
}
