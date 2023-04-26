using System.Text.RegularExpressions;

partial class Program
{
    static void Main(string[] args) {
        Dictionary<(char, int), int> letterBuckets = new();
        switch (args[0])
        {
            case "-p":
                FilterDictionary();
                break;
            case "-lf":
                letterFrequency(letterBuckets);
                PrintResultLetters(letterBuckets);
                break;
            case "-s":
                Solve(GetWordList(), "-----", new List<char>(), new List<char>());
                break;
            default:
                Console.WriteLine("Missing arg p, lf, s");
                break;
        }
        
        
        
    }

    private static void Solve(List<string> wordList, string wordTemplate, List<char> grayLetters, List<char> orangeLetters)
    {
        if (!wordTemplate.Contains('-') || wordList.Count < 2 || wordTemplate == "done") return;
        Console.WriteLine("Enter word Template: ");
        wordTemplate = Console.ReadLine();
        Console.WriteLine("Enter gray letters");
        grayLetters.AddRange(Console.ReadLine().ToCharArray());
        Console.WriteLine("Enter Orange letters");
        orangeLetters.AddRange(Console.ReadLine().ToCharArray());
        wordList = wordList.Where(word => TemplateMatchWord(word, wordTemplate)
                                                && orangeLetters.All(word.Contains)
                                                && grayLetters.All(grayLetter => !word.Contains(grayLetter))).ToList();
        wordList.ToList().ForEach(word => Console.WriteLine(word));
        Solve(wordList, wordTemplate, grayLetters, orangeLetters);
    }

    private static bool TemplateMatchWord(string testWord, string templateWord)
    {
        for (var i = 0; i < 5; i++)
        {
            if (!TemplateMatchChar(testWord[i], templateWord[i])) return false;
        }

        return true;
    }

    private static bool TemplateMatchChar(char a, char b)
    {
        return a == b || b == '-';
    }
    
    static void letterFrequency(IDictionary<(char, int), int> letterBucket)
    {
        try
        {
            using var sr = new StreamReader("C:/Users/lmlee/RiderProjects/WordleHelper/resources/wordleDictionary.txt");
            while (sr.ReadLine() is { } line)
            {
                for (var i = 0; i < 5; i++)
                {
                    AddToLetterBucket(letterBucket, line[i], i);
                }
            }
            sr.Close();
        } catch (Exception e) {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    private static void AddToLetterBucket(IDictionary<(char, int), int> letterBucket, char letter, int index)
    {
        if (letterBucket.TryGetValue((letter, index), out var value))
        {
            letterBucket[(letter, index)] = value + 1;
        }
        else
        {
            letterBucket[(letter, index)] = 1;
        }
    }

    private static void PrintResultLetters(Dictionary<(char, int), int> letterBucket)
    {
        for (var i = 0; i < 5; i++)
        {
            Console.WriteLine($"WordIndex {i}");
            PrintIndexResult(letterBucket, i);
            Console.WriteLine();
        }
        Console.WriteLine();
        letterBucket.GroupBy(pair => pair.Key.Item1)
            .Select(result => new { letter = result.First().Key.Item1, count = result.Sum(c => c.Value) })
            .OrderBy(result => -result.count)
            .ToList()
            .ForEach(result => Console.Write($"{result.letter.ToString()} {result.count}, "));
    }

    private static void PrintIndexResult(Dictionary<(char, int), int> letterBucket, int wordIndex)
    {
        letterBucket.Where(pair => pair.Key.Item2 == wordIndex)
            .Select(pair => new { letter = pair.Key.Item1, count = pair.Value })
            .OrderBy(pair => -pair.count).ToList()
            .ForEach(obj => Console.Write($" {obj.letter}, {obj.count} |"));
    }

    static List<string> GetWordList()
    {
        List<string> wordList = new();
        try
        {
            using var sr = new StreamReader("C:/Users/lmlee/RiderProjects/WordleHelper/resources/wordleDictionary.txt");
            while (sr.ReadLine() is { } line)
            {
                wordList.Add(line);
            }
            sr.Close();
        } catch (Exception e) {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        return wordList;
    }
    
    static void FilterDictionary()
    {
        var _regex = MyRegex();

        try
        {
            using var sr = new StreamReader("C:/Users/lmlee/RiderProjects/WordleHelper/resources/words.txt");
            using var sw = new StreamWriter("C:/Users/lmlee/RiderProjects/WordleHelper/resources/wordleDictionary.txt");
            while (sr.ReadLine() is { } line) {
                if (_regex.IsMatch(line))
                {
                    sw.WriteLine(line);
                }
            }
            sr.Close();
            sw.Close();
        } catch (Exception e) {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    [GeneratedRegex("^[a-z]{4}[^s]$")]
    private static partial Regex MyRegex();
}