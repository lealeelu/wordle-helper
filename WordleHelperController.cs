using System.Text.RegularExpressions;

namespace WordleHelper;

public partial class WordleHelperController
{
    readonly Dictionary<(char, int), int> letterBuckets = new();

    public static void Solve()
    {
        var wordList = GetWordList();
        var clues = new List<Clue>();
        while (wordList.Count > 1)
        {
            Console.WriteLine("Enter word Template: ");
            var wordTemplate = Console.ReadLine();
            if (wordTemplate == "") return;
            clues.AddRange(ParseWordTemplate(wordTemplate));
            wordList = wordList.Where(word => TemplateMatchWord(word, clues)).ToList(); 
            wordList.Where(word => TemplateMatchWord(word, clues)).ToList().ForEach(Console.WriteLine);
            Console.WriteLine($"'Count: {wordList.Count}");
        }
    }

    private static IEnumerable<Clue> ParseWordTemplate(string wordTemplate)
    {
        return wordTemplate.Split(',').Select((clue, index) => clue.Length > 1
            ? new Clue{ type = GetClueType(clue[0]), letter = clue[1], index = index }
            : new Clue{ type = ClueType.Positive, letter = clue[0], index = index });
    }

    private static ClueType GetClueType(char clue)
    {
        return clue switch
        {
            '!' => ClueType.NotIncluded,
            '~' => ClueType.IncludedWrongSpot,
            _ => ClueType.Unknown
        };
    }
    
    private static bool TemplateMatchWord(string testWord, IEnumerable<Clue> clues)
    {
        return clues.All(clue =>
        {
            var testLetter = testWord[clue.index];
            return clue.type switch
            {
                ClueType.NotIncluded => !testWord.Contains(clue.letter),
                ClueType.IncludedWrongSpot => testLetter != clue.letter && testWord.Contains(clue.letter),
                ClueType.Positive => testLetter == clue.letter,
            };
        });
    }

    public void letterFrequency()
    {
        FillLetterBucket();
        PrintResultLetters();
    }

    private void FillLetterBucket()
    {
        try
        {
            using var sr = new StreamReader("C:/Users/lmlee/RiderProjects/WordleHelper/resources/wordleDictionary.txt");
            while (sr.ReadLine() is { } line)
            {
                for (var i = 0; i < 5; i++)
                {
                    AddToLetterBucket(letterBuckets, line[i], i);
                }
            }

            sr.Close();
        }
        catch (Exception e)
        {
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

    private void PrintResultLetters()
    {
        for (var i = 0; i < 5; i++)
        {
            Console.WriteLine($"WordIndex {i}");
            PrintIndexResult(letterBuckets, i);
            Console.WriteLine();
        }

        Console.WriteLine();
        letterBuckets.GroupBy(pair => pair.Key.Item1)
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

    private static List<string> GetWordList()
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
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        return wordList;
    }

    public static void FilterDictionary()
    {
        var _regex = MyRegex();

        try
        {
            using var sr = new StreamReader("C:/Users/lmlee/RiderProjects/WordleHelper/resources/words.txt");
            using var sw = new StreamWriter("C:/Users/lmlee/RiderProjects/WordleHelper/resources/wordleDictionary.txt");
            while (sr.ReadLine() is { } line)
            {
                if (_regex.IsMatch(line))
                {
                    sw.WriteLine(line);
                }
            }

            sr.Close();
            sw.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    [GeneratedRegex("^[a-z]{4}[^s]$")]
    private static partial Regex MyRegex();
    
    private class Clue
    {
        public ClueType type;
        public char letter;
        public int index;
    }
    
    private enum ClueType
    {
        NotIncluded, //gray
        IncludedWrongSpot, //orange
        Positive, //green
        Unknown,
    }
}