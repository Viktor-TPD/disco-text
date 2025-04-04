using discotext.Models;
public class Location
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, Location> Exits { get; set; }
    public List<Item> Items { get; set; }
    public List<string> PossibleInteractions { get; set; }

    public Location(string name, string description)
    {
        Name = name;
        Description = description;
        Exits = new Dictionary<string, Location>();
        Items = new List<Item>();
        PossibleInteractions = new List<string>();
    }
}
