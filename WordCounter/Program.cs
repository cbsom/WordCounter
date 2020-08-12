using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordCounter
{
    class Program
    {
        private static readonly Encoding _hebrewEncoding = Encoding.GetEncoding(1255);
       
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: WordCounter inFilePath outFilePath");
            }
            else
            {
                try
                {
                    getTable(args[0], args[1]);
                    Console.WriteLine("Completed creating file {0}", args[1]);
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine("Failed to create {0} from {1}\n{2}\n{3}", 
                        args[1], args[0], ex.Message, ex.StackTrace);
                }
                
            }
            Console.WriteLine("Press <ENTER> to exit...");
            Console.ReadLine();
        }

        static void getTable(string wordListPath, string outputPath)
        {
            var dict = new Dictionary<string, long>();
            foreach (string line in File.ReadLines(wordListPath, _hebrewEncoding))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                foreach (string word in line.Split(new char[] { ' ' }, 
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        var cleaned = word.Trim().Replace(",", "");
                        if (dict.ContainsKey(cleaned))
                        {
                            dict[cleaned]++;
                        }
                        else
                        {
                            dict.Add(cleaned, 1);
                        }
                    }
                }
            }

            var list = dict.ToList();
            list.Sort((kvp1, kvp2) => kvp2.Value.CompareTo(kvp1.Value));
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            using (var file = File.OpenWrite(outputPath))
            {
                using (var sw = new StreamWriter(file, _hebrewEncoding))
                {
                    sw.WriteLine("word,instances");
                    foreach (var kvp in list)
                    {
                        sw.WriteLine(kvp.Key + "," + kvp.Value.ToString());
                    }
                }
            }
        }
    }
}
