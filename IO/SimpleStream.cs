using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using static System.Console;
namespace IO
{
    public class SimpleStream
    {
        public static void ReadFileUsingFileStream(string fileName)
        {
            const int BUFFERSIZE = 256;
            using (var stream = new FileStream(fileName, FileMode.Open,
            FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[BUFFERSIZE];
                bool completed = false;
                do
                {
                    int nread = stream.Read(buffer, 0, BUFFERSIZE);
                    if (nread == 0) completed = true;
                    if (nread < BUFFERSIZE)
                    {
                        System.Array.Clear(buffer, nread, BUFFERSIZE - nread);
                    }
                    WriteLine($"read {nread} bytes = {Encoding.UTF8.GetString(buffer, 0, nread)}");
                } while (!completed);
            }
        }

        public static void WriteFileUsingSteam(string filename)
        {
            using (var stream = File.OpenWrite(filename))
            {
                byte[] buffer = Encoding.UTF8.GetBytes("hello");
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        // via SteamReader/SteamWriter
        public static void ReadFileUsingReader(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    WriteLine(reader.ReadLine());
                }
            }
        }

        public static void WriteFileUsingWriter(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Write))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.WriteLine("changed!");
            }
        }

        public static void CreateZipFile(string directory, string zipFile)
        {
            FileStream zipStream = File.OpenWrite(zipFile);
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                IEnumerable<string> files = Directory.EnumerateFiles(
                        directory,
                        "*", 
                        SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(file));
                    using (FileStream inputStream = File.OpenRead(file))
                    using (Stream outputStream = entry.Open())
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }
        }
    }
}