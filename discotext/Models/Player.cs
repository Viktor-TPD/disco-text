namespace discotext;

public class Player
{
    public Location CurrentLocation { get; set; }
    public List<Item> Inventory { get; set; }
    public int Health { get; set; }
    public int Morale { get; set; }

    public Player(Location startingLocation)
    {
        CurrentLocation = startingLocation;
        Inventory = new List<Item>();
        Health = 2;
        Morale = 2;
    }
}