using discotext.Utils;

namespace discotext;

public class Game
{
    private Player _player;
    public bool _isRunning;
    private Dictionary<string, Location> _locations;
    private CommandProcessor _commandProcessor;
    private GameText _gameText;

    public Game()
    {
        _locations = new Dictionary<string, Location>();
        _gameText = new GameText();
        InitializeGame();
        _player = new Player(_locations["CenterOfRoom"]);
        _commandProcessor = new CommandProcessor();
        _isRunning = false;
    }

    private void InitializeGame()
    {
        var CenterOfRoom = new Location("Center of Room",
            "You're in the middle of the room."
            );
        var Bathroom = new Location("Bathroom",
            "You're in the bathroom."
            );
        var Window = new Location("In Front of Window",
            "You're in front of a broken window."
        );
        var Door = new Location("In Front of Door",
            "You're in front of a heavy wooden door."
        );
        // **
        // Locations
        // **
        CenterOfRoom.Exits["Bathroom"] = Bathroom;
        CenterOfRoom.Exits["Window"] = Window;
        CenterOfRoom.Exits["Door"] = Door;
        
        Bathroom.Exits["back"] = CenterOfRoom;
        Window.Exits["back"] = CenterOfRoom;
        Door.Exits["back"] = CenterOfRoom;

        // **
        // Items
        // **
        var necktie = new Item("necktie",
            "A garish orange-and-blue tie. It seems to be gently swaying, even without a breeze.", 
            true);
        necktie.InteractionResponses["examine"] =
            "The tie seems to whisper to you. Something about taking it with you...";
        necktie.InteractionResponses["take"] =
            "The grotesque tie feels warm in your hands. You are reunited.";
        
        var shoe1 = new Item("shoe",
            "An obscenely green shoe adorned by scales. It seems to be missing its partner. It's very Disco.",
            true);
        shoe1.InteractionResponses["examine"] =
            "Each individual scale glimmers in the room's dim light.";
        shoe1.InteractionResponses["take"] =
            "Heavier than expected, you remove the shoe from the hat rack.";
        var shoe2 = new Item("the other shoe",
            "A projectile thrown in a fit of masculine energy. It seems to be missing its partner.",
            true);
        shoe2.InteractionResponses["examine"] =
            "It looks lonely out there. In the cold.";
        shoe2.InteractionResponses["take"] =
            "You carefully lean out the broken window and grab the shoe. \n " +
            "The cold sea breeze makes you nauseous.";
        
        var mirror = new Item("mirror", "A cracked bathroom mirror. Your reflection is... disturbing.", false);
        mirror.InteractionResponses["examine"] = "A haggard face stares back. Eyes bloodshot, stubble patchy. You barely recognize yourself.";

        var sink = new Item("sink",
            "A stained porcelain sink. The faucet drips continuously.",
            false);
        sink.InteractionResponses["examine"] = "Water slowly drips from the faucet. The basin has seen better days.";
        sink.InteractionResponses["use"] = "You splash cold water on your face. It doesn't help much with the hangover.";

        var window = new Item("window",
            "A window overlooking the balcony outside and the streets below. It sports an injury, a hole the approximate size of a fist. Sharp shards are scattered on the outside.",
            false);
        window.InteractionResponses["examine"] = "Through the broken glass, you can see the street. It's raining. The world seems blurry and far away. \n " + "You get a sinking feeling in your stomach as you spot a green snake-skin shoe on the balcony.";
        window.InteractionResponses["open"] = "The window is already open, in a sense. You decide not to mess with it.";

        var ledger = new Item("ledger", "A small notebook on the floor. Probably belongs to the hostel.", true);
        ledger.InteractionResponses["examine"] = "The ledger contains records of guests. You spot your name: 'Du Bois, H. - Room 1. Paid for 3 nights.'";
        ledger.InteractionResponses["take"] = "You pocket the ledger. Might be useful.";
        
        CenterOfRoom.Items.Add(necktie);
        CenterOfRoom.Items.Add(shoe1);
        CenterOfRoom.Items.Add(ledger);
        Bathroom.Items.Add(mirror);
        Bathroom.Items.Add(sink);
        Window.Items.Add(window);
        Window.Items.Add(shoe2);
        
    }

    public void Run()
    {
        _isRunning = true;
    }
}