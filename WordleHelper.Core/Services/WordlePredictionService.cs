namespace WordleHelper.Core.Services;

public interface IWordlePredictionService
{
    Task<List<string>> GetPredictions(char[] known, char[][] wrongPositions, char[] mustInclude, char[] blacklist);
}

public sealed class WordlePredictionService(IWordStorageService wordStorageService) : IWordlePredictionService
{
    private readonly IWordStorageService _wordStorageService = wordStorageService ?? throw new ArgumentNullException(nameof(wordStorageService));

    public async Task<List<string>> GetPredictions(char[] known, char[][] wrongPositions, char[] mustInclude, char[] blacklist)
    {
        if (known.Length != 5 || wrongPositions.Length != 5 || mustInclude.Length > 5)
        {
            throw new ArgumentException("Invalid input lengths.");
        }

        string[] words = await _wordStorageService.GetWordsAsync();

        List<string> filterLookuptable = words
            .Where(e => blacklist.All(c => e.Contains(c) == false))
            .Where(e => mustInclude.All(c => e.Contains(c)))
            .Where(e => IsNotMatch(e[0], wrongPositions[0]) && IsNotMatch(e[1], wrongPositions[1]) && IsNotMatch(e[2], wrongPositions[2]) && IsNotMatch(e[3], wrongPositions[3]) && IsNotMatch(e[4], wrongPositions[4]))
            .Where(e => IsMatch(e[0], known[0]) && IsMatch(e[1], known[1]) && IsMatch(e[2], known[2]) && IsMatch(e[3], known[3]) && IsMatch(e[4], known[4]))
            .ToList() ?? [];

        return filterLookuptable;
    }

    private static bool IsMatch(char a, char b) => b == ' ' || b == a;

    private static bool IsNotMatch(char a, char[] b) => b.Contains(a) == false;
}