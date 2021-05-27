using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Serilog.Logfmt
{
    static class StringWriterExtensions
    {
        public static string ToLogfmtQuotedString(this StringWriter sw, DoubleQuotesAction action)
        {
            var sb = sw.GetStringBuilder();
            switch (action)
            {
                case DoubleQuotesAction.ConvertToSingle:
                    sb.Replace('"', '\'');
                    break;
                case DoubleQuotesAction.Escape:
                    sb.Replace(@"""", @"\""");
                    break;
                case DoubleQuotesAction.Remove:
                    sb.Replace(@"""", "");
                    break;
                default:            // None
                    break;
            }

            return sb.ToString();
        }

        // From https://github.com/serilog/serilog/issues/936 for avoiding extra quotes on strings
        public static void WriteMessage(this TextWriter tw, LogEvent logEvent)
        {
            Predicate<LogEventPropertyValue> isString = pv =>
            {
                var sv = pv as ScalarValue;
                return (sv != null) && (sv.Value as string) != null;
            };
            foreach (var t in logEvent.MessageTemplate.Tokens)
            {
                var pt = t as PropertyToken;
                LogEventPropertyValue propVal;
                if (pt != null &&
                   logEvent.Properties.TryGetValue(pt.PropertyName, out propVal) &&
                   isString(propVal))
                {
                    tw.Write(((ScalarValue)propVal).Value);
                }
                else
                {
                    t.Render(logEvent.Properties, tw, null);
                }
            }
        }
    }
}
