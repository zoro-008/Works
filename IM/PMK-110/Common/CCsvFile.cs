using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace COMMON
{
    public class CCsvFile
    {
        public static string ToCsv<T>(string separator, IEnumerable<T> objectlist)
        {
            Type t = typeof(T);
            FieldInfo[] fields = t.GetFields();

            string header = String.Join(separator, fields.Select(f => f.Name).ToArray());

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var o in objectlist)
                csvdata.AppendLine(ToCsvFields(separator, fields, o));

            return csvdata.ToString();
        }

        public static string ToCsvFields(string separator, FieldInfo[] fields, object o)
        {
            StringBuilder linie = new StringBuilder();

            foreach (var f in fields)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                    linie.Append(x.ToString());
            }

            return linie.ToString();
        }

        public static bool SaveStringToCsv(string _sFilePath , string _sText)
        {
            //해당 패스 만들고.
            if (!Directory.Exists(Path.GetDirectoryName(_sFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_sFilePath));
            }

            FileInfo fileinfo = new FileInfo(_sFilePath);
            FileStream fs = new FileStream(_sFilePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine(_sText);

            sw.Close();

            return true ;
        }
    }
}
