using Serilog.Data;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Serilog.Logfmt
{
    class LogfmtValueFormatter : LogEventPropertyValueVisitor<TextWriter, bool>
    {
        private readonly LogfmtOptions _options;
        private List<string> _keys;


        public LogfmtValueFormatter(LogfmtOptions options)
        {

            _options = options;
            _keys = new List<string>(capacity: 16);

        }

        public void Format(string key, LogEventPropertyValue value, TextWriter output)
        {
            _keys.Add(key);
            Visit(output, value);
            output.Write(" ");
            _keys.RemoveAt(_keys.Count - 1);

        }

        protected override bool VisitDictionaryValue(TextWriter state, DictionaryValue dictionary)
        {
            return false;
        }

        protected override bool VisitScalarValue(TextWriter state, ScalarValue scalar)
        {
            var keyName = GetFullKeyName();
            var svalue = scalar.Value?.ToString() ?? "\"\"";
            state.Write("{0}=", keyName);
            var needQuotes = svalue.Contains(" ");
            if (needQuotes)
            {
                switch (_options.DoubleQuotesAction)
                {
                    case DoubleQuotesAction.ConvertToSingle:
                        svalue = svalue.Replace('"', '\'');
                        break;
                    case DoubleQuotesAction.Remove:
                        svalue = svalue.Replace(@"""", "");
                        break;
                    case DoubleQuotesAction.Escape:
                        svalue = svalue.Replace(@"""", @"\""");
                        break;
                    default: break;
                }
            }
            state.Write("{1}{0}{1}", svalue, needQuotes ? "\"" : "");
            return true;
        }

        private string GetFullKeyName()
        {
            var sb = new StringBuilder();
            for (var idx = 0; idx < _keys.Count; idx++)
            {
                var key = _keys[idx];
                sb.Append(key);
                if (idx < _keys.Count -1 && _keys[idx + 1][0] != '[')           // Keys that are indexes of Sequence start with [ and in this case we don't want to use the separator
                {
                    sb.Append(_options.ComplexPropertySeparator);
                }
            }
            return sb.ToString();

        }

        protected override bool VisitSequenceValue(TextWriter state, SequenceValue sequence)
        {
            var idx = 0;
            foreach (var value in sequence.Elements)
            {
                Format($"[{idx}]", value, state);
                idx++;
            }

            return false;
        }

        protected override bool VisitStructureValue(TextWriter state, StructureValue structure)
        {
            foreach (var prop in structure.Properties)
            {
                var name = _options.NormalizeCase ? prop.Name.ToLowerInvariant() : prop.Name;
                Format(name, prop.Value, state);
            }

            return true;
        }
    }
}
