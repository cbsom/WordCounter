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
        private static Encoding _encoding = Encoding.GetEncoding(1255);

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
                    DetectFileEncoding(args[0]);
                    Console.WriteLine("Encoding detected: {0}", _encoding.BodyName);
                    var list = GetFrequencyTable(args[0]);
                    if (list.Count > 0)
                    {
                        OutputCsv(args[1], list);
                        Console.WriteLine("Completed creating file {0}", args[1]);
                    }
                    else
                    {
                        Console.WriteLine("There was an issue getting word frequency count - list is empty");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Failed to create {0} from {1}\n{2}\n{3}",
                        args[1], args[0], ex.Message, ex.StackTrace);
                }

            }
            Console.WriteLine("Press <ENTER> to exit...");
            Console.ReadLine();
        }

        static void DetectFileEncoding(string filePath)
        {
            var Utf8EncodingVerifier = Encoding.GetEncoding("utf-8",
                new EncoderExceptionFallback(), new DecoderExceptionFallback());
            Stream fs = File.OpenRead(filePath);
            using (var reader = new StreamReader(fs, Utf8EncodingVerifier,
                   detectEncodingFromByteOrderMarks: true, leaveOpen: true, bufferSize: 1024))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                    }
                    _encoding = reader.CurrentEncoding;
                }
                catch
                {
                    return;
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        static List<KeyValuePair<string, long>> GetFrequencyTable(string wordListPath)
        {
            var dict = new Dictionary<string, long>();
            foreach (string line in File.ReadLines(wordListPath, _encoding))
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
            return list;
        }

        private static void OutputCsv(string outputPath, List<KeyValuePair<string, long>> list)
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            using (var file = File.OpenWrite(outputPath))
            {
                using (var sw = new StreamWriter(file, _encoding))
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
