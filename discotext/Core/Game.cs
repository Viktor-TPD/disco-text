using discotext.Utils;

namespace discotext;

public class Game
{
    private Player _player;
    private bool _isRunning;
    private Dictionary<string, Location> _locations;
    private CommandProcessor _commandProcessor;
    private GameText _gameText;

    public Game()
    {
        _locations = new Dictionary<string, Location>();
        _gameText = new GameText();
        InitializeGame();
        _player = new Player(_locations["CenterOfRoom"]);
        _commandProcessor = new CommandProcessor(this);
        _isRunning = false;
    }

    private void InitializeGame()
    {
         // **
         // Locations
         // **   
        var centerOfRoom = new Location("CenterOfRoom",
            "You're in the middle of the room."
            );
        var bathroom = new Location("Bathroom",
            "You're in the bathroom."
            );
        var window = new Location("In Front of Window",
            "You're in front of a broken window."
        );
        var door = new Location("In Front of Door",
            "You're in front of a heavy wooden door."
        );

        centerOfRoom.Exits["Bathroom"] = bathroom;
        centerOfRoom.Exits["Window"] = window;
        centerOfRoom.Exits["Door"] = door;
        
        bathroom.Exits["back"] = centerOfRoom;
        window.Exits["back"] = centerOfRoom;
        door.Exits["back"] = centerOfRoom;
        
        _locations["CenterOfRoom"] = centerOfRoom;
        _locations["Bathroom"] = bathroom;
        _locations["ByTheDoor"] = door;
        _locations["ByTheWindow"] = window;

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

        var brokenWindow = new Item("window",
            "A window overlooking the balcony outside and the streets below. It sports an injury, a hole the approximate size of a fist. Sharp shards are scattered on the outside.",
            false);
        brokenWindow.InteractionResponses["examine"] = "Through the broken glass, you can see the street. It's raining. The world seems blurry and far away. \n " + "You get a sinking feeling in your stomach as you spot a green snake-skin shoe on the balcony.";
        brokenWindow.InteractionResponses["open"] = "The window is already open, in a sense. You decide not to mess with it.";

        var ledger = new Item("ledger", "A small notebook on the floor. Probably belongs to the hostel.", true);
        ledger.InteractionResponses["examine"] = "The ledger contains records of guests. You spot your name: 'Du Bois, H. - Room 1. Paid for 3 nights.'";
        ledger.InteractionResponses["take"] = "You pocket the ledger. Might be useful.";
        
        centerOfRoom.Items.Add(necktie);
        centerOfRoom.Items.Add(shoe1);
        centerOfRoom.Items.Add(ledger);
        bathroom.Items.Add(mirror);
        bathroom.Items.Add(sink);
        window.Items.Add(brokenWindow);
        window.Items.Add(shoe2);
        
    }

    public void Run()
    {
        _isRunning = true;
        _gameText.DisplayIntro();
        _gameText.DisplayLocationDescription(_player.CurrentLocation);
        
        while (_isRunning)
        {
            Console.Write("> ");
            string input = Console.ReadLine().ToLower();

            if (input == "exit" || input == "quit" || input == "goodbye")
            {
                _isRunning = false;
                continue;
            }
            
            _commandProcessor.ProcessCommand(input);
        }
        
        _gameText.DisplayOutro();
    }

    public Player GetPlayer()
    {
        return _player;
    }

    public GameText GetGameText()
    {
        return _gameText;
    }

    public void EndGame()
    {
        _isRunning = false;
    }
}