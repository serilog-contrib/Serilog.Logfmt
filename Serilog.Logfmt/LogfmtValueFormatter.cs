using Serilog.Data;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Serilog.Logfmt
{
    class LogfmtValueFormatter : LogEventPropertyValueVisitor<TextWriter, bool>
    {
        private readonly LogfmtOptions _options;

        public LogfmtValueFormatter(LogfmtOptions options)
        {
            _options = options;
        }

        public void Format(LogEventPropertyValue value, TextWriter output)
        {
            Visit(output, value);
        }

        protected override bool VisitDictionaryValue(TextWriter state, DictionaryValue dictionary)
        {
            return false;
        }

        protected override bool VisitScalarValue(TextWriter state, ScalarValue scalar)
        {
            var svalue = scalar.Value?.ToString() ?? "";
            var needQuotes = svalue.Contains(" ");
            state.Write("{1}{0}{1}", svalue, needQuotes ? "\"" : "");
            return false;
        }

        protected override bool VisitSequenceValue(TextWriter state, SequenceValue sequence)
        {
            return false;
        }

        protected override bool VisitStructureValue(TextWriter state, StructureValue structure)
        {
            return false;
        }
    }
}
