using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.Logic
{
    public static class StringUtils
    {
        public static ICollection<string> MergeByTrimSize(ICollection<string> list, int trimSize)
        {
            return MergeByTrimSize(list, trimSize);
        }

        /// <summary>
        /// Gibt ein IEnumberable zurück mit der übergebenen ICollection aneinander gestringt in der Maximallänge der trimsize.
        /// </summary>
        /// <param name="list">Collection an Elementen die so lange zusammengestringt werden bis die trimsize übertroffen wird.</param>
        /// <param name="trimSize">Maximallänge für die Ausgabezeile.</param>
        /// <param name="line">Aktuelle Zeile.</param>
        /// <param name="lines">Ausgabe Collection.</param>
        /// <returns>IEnumerable mit list Elementen aneinander gestringt die nicht länger als die TrimeSize sind.</returns>
        private static ICollection<string> MergeByTrimSize(ICollection<string> list, int trimSize, string line = "", ICollection<string> lines = null)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "List is null!");
            }

            if (trimSize < 1)
            {
                throw new ArgumentException("TrimSize should be over 1!", "trimSize");
            }

            if (line == null)
            {
                line = string.Empty;
            }

            if (line.Length > trimSize)
            {
                throw new ArgumentException("Line is longer than trimesize!", "line");
            }

            if (lines == null)
            {
                lines = new List<string>();
            }

            if (list.Count > 0)
            {
                if (line.Length + list.First().Length > trimSize)
                {
                    lines.Add(line);
                    return MergeByTrimSize(list.Skip(1).ToList(), trimSize, list.First(), lines);
                }
                else
                {
                    return MergeByTrimSize(list.Skip(1).ToList(), trimSize, line += list.First(), lines);
                }
            }
            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(line);
            }
            return lines;
        }

        public static string[] GetStringListOutOfString(string item, Action<List<string>> action = null)
        {
            if (item == null)
            {
                throw new NullReferenceException("item");
            }

            var list = item.Replace("\r\n", "\n").Split('\n').ToList();

            if (action != null)
            {
                action.Invoke(list);
            }

            return list.ToArray();
        }
    }
}
