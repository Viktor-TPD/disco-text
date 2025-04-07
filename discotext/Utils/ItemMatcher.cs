namespace discotext.Utils;
public class ItemMatcher
{
    private readonly GameText _gameText;
    private readonly int _maxDistanceThreshold;

    public ItemMatcher(GameText gameText, int maxDistanceThreshold = 2)
    {
        _gameText = gameText;
        _maxDistanceThreshold = maxDistanceThreshold;
    }

    public Models.Item MatchItem(string itemName, IEnumerable<Models.Item> availableItems)
    {
        if (string.IsNullOrWhiteSpace(itemName) || !availableItems.Any())
        {
            return null;
        }
        string normalizedInput = itemName.ToLower().Trim();
            
        var exactMatch = availableItems.FirstOrDefault(i => 
            i.Name.ToLower() == normalizedInput);
            
        if (exactMatch != null)
        {
            return exactMatch;
        }
        var bestMatches = new List<(Models.Item Item, int Distance)>();
            
        foreach (var item in availableItems)
        {
            int distance = TextMatching.LevenshteinDistance(normalizedInput, item.Name.ToLower());
            if (distance <= _maxDistanceThreshold)
            {
                bestMatches.Add((item, distance));
            }
        }

        if (bestMatches.Count > 0)
        {
            var bestMatch = bestMatches.OrderBy(m => m.Distance).First();
            _gameText.DisplayMessage($"I understand you mean the {bestMatch.Item.Name}.");
            return bestMatch.Item;
        }

        return null;
    }
}