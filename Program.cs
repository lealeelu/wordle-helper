using System.Text.RegularExpressions;
using WordleHelper;

partial class Program
{
    static void Main(string[] args)
    {
        var _wordleHelperController = new WordleHelperController();
        switch (args[0])
        {
            case "-p":
                WordleHelperController.FilterDictionary();
                break;
            case "-lf":
                _wordleHelperController.letterFrequency();
                break;
            case "-s":
                _wordleHelperController.Solve();
                break;
            default:
                Console.WriteLine("Missing arg p, lf, s");
                break;
        }
    }

    
}