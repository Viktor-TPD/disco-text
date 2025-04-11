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
            { "look", new HashSet<string> { "l", "see", "view", "observe" } },
            { "go", new HashSet<string> { "move", "walk", "g", "travel", "head" } },
            { "examine", new HashSet<string> { "x", "inspect", "check", "study" } },
            { "take", new HashSet<string> { "grab", "get", "pick", "pick up", "collect" } },
            { "inventory", new HashSet<string> { "i", "inv", "items", "bag", "pockets" } },
            { "use", new HashSet<string> { "utilize", "activate", "interact", "operate" } },
            { "status", new HashSet<string> { "stats", "condition", "health", "self" } },
            { "help", new HashSet<string> { "h", "commands", "?", "tutorial", "guide" } },
            { "quit", new HashSet<string> { "exit", "end", "q", "leave game" } },
            
            // Flavour
            { "die", new HashSet<string> { "concede", "suicide", "end it all", "kill myself", "give up", "surrender" } },
            { "dance", new HashSet<string> { "boogie", "groove", "bust a move", "disco" } },
            { "sing", new HashSet<string> { "karaoke", "perform", "belt out", "serenade" } },
            { "scream", new HashSet<string> { "yell", "shout", "cry out", "howl" } }
        };
    }

    public string MatchCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        string normalizedInput = input.ToLower().Trim();
        
        foreach (var entry in _commandSynonyms)
        {
            if (entry.Key == normalizedInput)
                return entry.Key;
                
            if (entry.Value.Contains(normalizedInput))
                return entry.Key;
        }

        string result = CheckMultiWordMatches(normalizedInput);
        if (result != null)
            return result;

        result = CheckFuzzyMatches(normalizedInput);
        if (result != null)
            return result;

        return null;
    }
    
    private string CheckMultiWordMatches(string normalizedInput)
    {
        foreach (var entry in _commandSynonyms)
        {
            if (entry.Key.Contains(" ") && normalizedInput.Contains(entry.Key))
            {
                _gameText.DisplayMessage($"I understand that as '{entry.Key}'.");
                return entry.Key;
            }
            
            foreach (var synonym in entry.Value)
            {
                if (synonym.Contains(" ") && normalizedInput.Contains(synonym))
                {
                    _gameText.DisplayMessage($"I understand that as '{entry.Key}'.");
                    return entry.Key;
                }
            }
        }
        
        string firstWord = normalizedInput.Split(' ')[0];
        foreach (var entry in _commandSynonyms)
        {
            if (entry.Key == firstWord)
                return entry.Key;
                
            if (entry.Value.Contains(firstWord))
                return entry.Key;
        }
        
        return null;
    }
    
    private string CheckFuzzyMatches(string normalizedInput)
    {
        var bestMatches = new List<(string Command, int Distance)>();
        string firstWord = normalizedInput.Split(' ')[0];
        
        foreach (var entry in _commandSynonyms)
        {
            int distance = TextMatching.LevenshteinDistance(firstWord, entry.Key);
            if (distance <= _maxDistanceThreshold)
                bestMatches.Add((entry.Key, distance));
            
            foreach (var synonym in entry.Value)
            {
                string compareWord = synonym.Contains(" ") ? synonym.Split(' ')[0] : synonym;
                distance = TextMatching.LevenshteinDistance(firstWord, compareWord);
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