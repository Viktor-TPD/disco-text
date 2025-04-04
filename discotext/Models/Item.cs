namespace discotext;

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool CanTake { get; set; }
    public Dictionary<string, string> InteractionResponses { get; set; }

    public Item(string name, string description, bool canTake = true)
    {
        Name = name;
        Description = description;
        CanTake = canTake;
        InteractionResponses = new Dictionary<string, string>();
    }
}