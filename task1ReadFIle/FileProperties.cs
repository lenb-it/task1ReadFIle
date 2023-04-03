namespace task1ReadFIle
{
    internal class FileProperties
    {
        public FileProperties(string fileName, long fileSize)
        {
            Words = new();
            Letters = new();
            FileName = fileName;
            FileSize = fileSize;
        }

        public string FileName { get; }

        public long FileSize { get; }

        public int LetterCount { get; private set; }

        public Dictionary<char, int> Letters { get; set; }

        public int WordsCount { get; private set; }

        public Dictionary<string, int> Words { get; set; }

        public int LinesCount { get; set; }

        public int DigitsCount { get; set; }

        public int NumbersCount { get; set; }

        public string LongestWord   
        {
            get => Words.Aggregate(string.Empty, (a, b) => a.Length < b.Key.Length ? b.Key : a);
        }

        public int WordsWithHyphen { get; set; }

        public int Punctuations { get; set; }

        public void AddLetter(char letter)
        {
            LetterCount++;

            if (Letters.ContainsKey(letter))
            {
                Letters[letter]++;
                return;
            }

            Letters.Add(letter, 1);
        }

        public void AddWord(string word)
        {
            WordsCount++;

            if (Words.ContainsKey(word))
            {
                Words[word]++;
                return;
            }

            Words.Add(word, 1);
        }
    }
}
