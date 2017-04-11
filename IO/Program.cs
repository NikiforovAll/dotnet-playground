using System;
using System.IO;
namespace IO
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = GetDefaultFilePath("test.txt");
            switch (args[0])
            {
                case "1":
                    SimpleStream.WriteFileUsingSteam(fileName);
                    SimpleStream.ReadFileUsingFileStream(fileName);
                break;
                case "2":
                    SimpleStream.WriteFileUsingWriter(fileName);
                    SimpleStream.ReadFileUsingReader(fileName);
                break;
                case "3":
                    SimpleStream.CreateZipFile(GetDefaultFilePath("test"), "test.zip");
                break;
            }
        }

        public static string GetDefaultFilePath(string fileName){
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            return  Path.Combine(directory, fileName);

        }
    }
}
