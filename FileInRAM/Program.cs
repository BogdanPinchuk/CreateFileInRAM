using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace FileInRAM
{
    class Program
    {
        /// <summary>
        /// Txt-file for testing
        /// </summary>
        private static readonly string path = "test.txt";

        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            Console.WriteLine($"Is file upsent? Answer: {File.Exists(path)}");
            Console.WriteLine($"File name: {new FileInfo(path).FullName}");
            Console.WriteLine("Continue...\n");
            //Console.ReadKey(true);

            var size = new FileInfo(path).Length;
            var str = new StringBuilder();

            using (var mmFile = MemoryMappedFile.CreateFromFile(path, FileMode.OpenOrCreate, null,
                size, MemoryMappedFileAccess.Read))
            using (var stream = mmFile.CreateViewStream(0, size, MemoryMappedFileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                // read info in file
                str.Append(reader.ReadToEnd().Trim());
                
                Console.WriteLine("Result of file (data):\n");
                Console.WriteLine(str);

                Console.ReadKey(true);
            }

            str.Append($"\nRecord MMF: {DateTime.Now}");

            using (var mmFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, str.Length))
            using (var stream = mmFile.CreateViewStream())
            using (var writer = new StreamWriter(stream))
            {
                // write data in file
                writer.WriteLine(str);
            }

            Console.WriteLine("\nFinished!");
            Console.ReadKey(true);
        }
    }
}
