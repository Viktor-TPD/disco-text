namespace discotext.Models;
public class LocationFactory
{
    private Dictionary<string, Item> _allItems;

    public LocationFactory(Dictionary<string, Item> allItems)
    {
        _allItems = allItems;
    }

    public Dictionary<string, Location> CreateLocations()
    {
        var centerOfRoom = new Location("Center of Room", 
            "You're in the middle of a trashed hostel room. The morning light casts long shadows across the chaos. Your head is pounding. A ceiling fan spins lazily overhead.");
        
        var bathroom = new Location("Bathroom", 
            "A cramped, dingy bathroom. The mirror is cracked, and there's water on the floor. The sink drips steadily.");
        
        var nearDoor = new Location("By the Door", 
            "You're standing by the door to the hallway. It's solid wood with peeling green paint. It appears to be locked.");
        
        var nearWindow = new Location("By the Window", 
            "You're by the window overlooking a street. Light filters through unwashed glass. You can see your reflection faintly - you look terrible.");

        centerOfRoom.Exits["bathroom"] = bathroom;
        centerOfRoom.Exits["door"] = nearDoor;
        centerOfRoom.Exits["window"] = nearWindow;

        bathroom.Exits["back"] = centerOfRoom;
        nearDoor.Exits["back"] = centerOfRoom;
        nearWindow.Exits["back"] = centerOfRoom;

        nearDoor.Items.Add(_allItems["door"]);
        bathroom.Items.Add(_allItems["mirror"]);
        bathroom.Items.Add(_allItems["sink"]);
        nearWindow.Items.Add(_allItems["window"]);
        centerOfRoom.Items.Add(_allItems["ledger"]);
        centerOfRoom.Items.Add(_allItems["ceiling fan"]);
        
        nearWindow.Items.Add(_allItems["second shoe"]);
        bathroom.Items.Add(_allItems["shirt"]);
        centerOfRoom.Items.Add(_allItems["pants"]);

        var locations = new Dictionary<string, Location>
        {
            { "CenterOfRoom", centerOfRoom },
            { "Bathroom", bathroom },
            { "ByTheDoor", nearDoor },
            { "ByTheWindow", nearWindow }
        };

        return locations;
    }
}