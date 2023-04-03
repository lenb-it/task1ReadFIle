using System.Text;
using Newtonsoft.Json;

namespace task1ReadFIle
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            if (args is null || !File.Exists(args.FirstOrDefault()))
            {
                Console.WriteLine($"File {args?.FirstOrDefault()} not found");
                Console.ReadLine();

                return;
            }

            Console.WriteLine("Please wait.");

            string newFile = await SaveFilePropertiesAsync(args.First());

            Console.WriteLine($"Properties save to file: {newFile}");
            Console.ReadLine();
        }

        private static async Task<string> SaveFilePropertiesAsync(string fileName)
        {
            var file = new FileInfo(fileName);
            var properties = await ReadFileAsync(fileName, file.Length);
            var resultFileName = $"{file.Name.Substring(0, file.Name.LastIndexOf(Constants.Dot))}.json";

            var data = GetSortedJsonData(properties);
            await WriteToFileAsync(resultFileName, data);

            return resultFileName;
        }

        private static string GetSortedJsonData(FileProperties properties)
        {
            properties.Words = properties.Words.OrderByDescending(w => w.Value).ToDictionary(w => w.Key, w => w.Value);
            properties.Letters = properties.Letters.OrderByDescending(l => l.Value).ToDictionary(l => l.Key, l => l.Value);

            var jsonData = JsonConvert.SerializeObject(properties, Formatting.Indented);

            return jsonData;
        }

        private static async Task WriteToFileAsync(string fileName, string data)
        {
            using var stream = new StreamWriter(fileName, append: false);
            await stream.WriteAsync(data);
        }

        private static async Task<FileProperties> ReadFileAsync(string fileName, long length)
        {
            var properties = new FileProperties(fileName, length);
            using var stream = new StreamReader(fileName, Encoding.UTF8);
            var currentWord = new StringBuilder();

            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                properties.LinesCount++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                DataReadingImplementation(properties, currentWord, line);
            }

            return properties;
        }

        private static void DataReadingImplementation(FileProperties properties, StringBuilder currentWord, string line)
        {
            currentWord.Clear();
            var isNumber = false;

            for (var i = 0; i < line.Length; ++i)
            {
                if (char.IsLetter(line[i]))
                {
                    currentWord.Append(line[i]);
                    properties.AddLetter(line[i]);
                }
                else if (char.IsDigit(line[i]))
                {
                    properties.DigitsCount++;
                    isNumber = true;
                }
                else if (line[i] == Constants.Hyphen)
                {
                    if ((i - 1) >= 0 && (i + 1) < line.Length && 
                        char.IsLetter(line[i - 1]) && char.IsLetter(line[i + 1]))
                    {
                        if (!currentWord.ToString().Contains(Constants.Hyphen))
                            properties.WordsWithHyphen++;

                        currentWord.Append(line[i]);
                    }
                    else if (i + 1 < line.Length && char.IsDigit(line[i + 1]))
                    {
                        isNumber = true;
                    }
                    else
                    {
                        properties.Punctuations++;
                    }
                }
                else if (char.IsPunctuation(line[i]))
                {
                    properties.Punctuations++;
                }

                if (!char.IsLetter(line[i]) && line[i] != Constants.Hyphen)
                {
                    if (currentWord.Length > 1)
                        properties.AddWord(currentWord.ToString());

                    currentWord.Clear();
                }

                if (!char.IsDigit(line[i]) && isNumber)
                {
                    properties.NumbersCount++;
                    isNumber = false;
                }
            }

            if (currentWord.Length > 0)
            {
                properties.AddWord(currentWord.ToString());
                currentWord.Clear();
            }
        }
    }
}