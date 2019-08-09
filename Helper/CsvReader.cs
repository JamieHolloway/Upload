using System.IO;
using System.Text.RegularExpressions;

namespace Helper
{
    public sealed class CsvReader : System.IDisposable
    {
        public CsvReader(string fileName) : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
        }

        public CsvReader(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        public System.Collections.IEnumerable RowEnumerator
        {
            get
            {
                if (null == reader)
                    throw new System.ApplicationException("No input file provided.");

                RowIndex = 0;
                string sLine;

                while (null != (sLine = reader.ReadLine()))
                {
                    string sNextLine;
                    while (RexRunOnLine.IsMatch(sLine) && null != (sNextLine = reader.ReadLine()))
                        sLine += "\n" + sNextLine;

                    RowIndex++;
                    var values = RexCsvSplitter.Split(sLine);

                    for (var i = 0; i < values.Length; i++)
                        values[i] = Csv.Unescape(values[i]);

                    yield return values;
                }

                reader.Close();
            }
        }

        public long RowIndex { get; private set; } = 0;

        public void Dispose()
        {
            reader?.Dispose();
        }

        private readonly TextReader reader;
        private static readonly Regex RexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private static readonly Regex RexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");
    }

    public static class Csv
    {
        public static string Escape(string s)
        {
            if (s.Contains(Quote))
                s = s.Replace(Quote, EscapedQuote);

            if (s.IndexOfAny(CharactersThatMustBeQuoted) > -1)
                s = Quote + s + Quote;

            return s;
        }

        public static string Unescape(string s)
        {
            if (s.StartsWith(Quote) && s.EndsWith(Quote))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(EscapedQuote))
                    s = s.Replace(EscapedQuote, Quote);
            }

            return s;
        }

        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] CharactersThatMustBeQuoted = {',', '"', '\n'};
    }
}
