namespace discotext.Utils;

public class CommandMatcher
{
    private readonly Dictionary<string, HashSet<string>> _commandSynonyms;
    private readonly GameText _gameText;
    private readonly int _maxDistanceThreshold;

    public CommandMatcher(GameText gameText, int maxDistanceThreshold = 2)
    {
        _gameText = gameText;
        _maxDistanceThreshold = maxDistanceThreshold;
        
        _commandSynonyms = new Dictionary<string, HashSet<string>>
        {
            { "look", new HashSet<string> { "l", "see", "view" } },
            { "go", new HashSet<string> { "move", "walk", "g" } },
            { "examine", new HashSet<string> { "x", "inspect", "check" } },
            { "take", new HashSet<string> { "grab", "get", "pick" } },
            { "inventory", new HashSet<string> { "i", "inv", "items" } },
            { "use", new HashSet<string> { "utilize", "activate" } },
            { "status", new HashSet<string> { "stats", "condition" } },
            { "help", new HashSet<string> { "h", "commands", "?" } },
            { "quit", new HashSet<string> { "exit", "end", "q" } }
        };
    }

    public string MatchCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        string normalizedInput = input.ToLower().Trim();
        foreach (var entry in _commandSynonyms)
        {
            if (entry.Key == normalizedInput || entry.Value.Contains(normalizedInput))
                return entry.Key;
        }

        var bestMatches = new List<(string Command, int Distance)>();
        
        foreach (var entry in _commandSynonyms)
        {
            int distance = TextMatching.LevenshteinDistance(normalizedInput, entry.Key);
            if (distance <= _maxDistanceThreshold)
                bestMatches.Add((entry.Key, distance));
            
            foreach (var synonym in entry.Value)
            {
                distance = TextMatching.LevenshteinDistance(normalizedInput, synonym);
                if (distance <= _maxDistanceThreshold)
                    bestMatches.Add((entry.Key, distance));
            }
        }

        if (bestMatches.Count > 0)
        {
            var bestMatch = bestMatches.OrderBy(m => m.Distance).First();
            _gameText.DisplayMessage($"I understand that as '{bestMatch.Command}'.");
            return bestMatch.Command;
        }

        return null;
    }
}