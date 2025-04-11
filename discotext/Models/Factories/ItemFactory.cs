using discotext.Utils;
using discotext.Core;

namespace discotext.Models;
// @todo Refactor this file, it is way too large
public class ItemFactory
{
    private Player _player;
    private GameText _gameText;
    private Game _game;
    private DialogueActionHandler _exitDialogueHandler;
    private DialogueActionHandler _lookHandler;

    public ItemFactory(Player player, GameText gameText, Game game = null, 
        DialogueActionHandler exitDialogueHandler = null, 
        DialogueActionHandler lookHandler = null)
    {
        _player = player;
        _gameText = gameText;
        _game = game;
        _exitDialogueHandler = exitDialogueHandler;
        _lookHandler = lookHandler;
    }

    public Dictionary<string, Item> CreateAllItems()
    {
        var items = new Dictionary<string, Item>();
        
        items.Add("necktie", CreateNecktie());
        items.Add("mirror", CreateMirror());
        items.Add("sink", CreateSink());
        items.Add("window", CreateWindow());
        items.Add("ledger", CreateLedger());
        items.Add("ceiling fan", CreateCeilingFan());
        items.Add("second shoe", CreateSecondShoe());
        items.Add("shirt", CreateShirt());
        items.Add("pants", CreatePants());
        items.Add("door", CreateDoor());
        
        foreach (var item in items.Values)
        {
            item.ExitDialogueHandler = _exitDialogueHandler;
            item.LookHandler = _lookHandler;
        }
        
        return items;
    }

    private Item CreateDoor()
    {
        var door = new Item("door", "A solid wooden door with peeling green paint. It's the exit from this room.", false);
        door.InteractionResponses["examine"] = "The door is locked. You'll need a key to get out.";
        door.InteractionResponses["open"] = "You try the handle, but it's locked.";
        door.InteractionResponses["use"] = "The door is locked. You need a key to open it.";
        
        door.DialogueOptions.Add(new DialogueOption(
            "Try to open", 
            "You try the handle, but it's locked tight. You need a key.",
            null
        ));
        
        door.DialogueOptions.Add(new DialogueOption(
            "Knock on the door", 
            "You knock, but there's no answer. Just the echo of your knocking in an empty hallway.",
            null
        ));
        
        door.DialogueOptions.Add(new DialogueOption(
            "Use key", 
            "You insert the key into the lock. It turns with a satisfying click.",
            () => {
                bool hasKey = _player.Inventory.Any(i => i.Name == "room key");
                
                if (!hasKey)
                {
                    _gameText.DisplayMessage("You don't have a key.");
                    return;
                }
                
                bool wearingShirt = _player.Inventory.Any(i => i.Name == "shirt" && i.IsWorn);
                bool wearingPants = _player.Inventory.Any(i => i.Name == "pants" && i.IsWorn);
                bool wearingNecktie = _player.Inventory.Any(i => i.Name == "necktie" && i.IsWorn);
                bool wearingShoe = _player.Inventory.Any(i => i.Name == "second shoe" && i.IsWorn);
                bool isFullyDressed = wearingShirt && wearingPants && wearingNecktie && wearingShoe;
                
                if (!isFullyDressed)
                {
                    _gameText.DisplayMessage("You can't leave yet. You need to get dressed properly first.");
                    
                    if (!wearingShirt) _gameText.DisplayMessage("You need to find your shirt.");
                    if (!wearingPants) _gameText.DisplayMessage("You need to find your pants.");
                    if (!wearingNecktie) _gameText.DisplayMessage("You need to find your necktie.");
                    if (!wearingShoe) _gameText.DisplayMessage("You need to find your second shoe.");
                    
                    return;
                }
                
                _gameText.DisplayMessage("The door unlocks. The smell of the corridor hits you - cigarettes, cheap cleaning products, and mystery. It's time to go solve your case.");
                _gameText.DisplayOutro();
                if (door.ExitDialogueHandler != null)
                {
                    door.ExitDialogueHandler.Invoke();
                }
            
                if (_game != null)
                {
                    _game.EndGame();
                }
                Thread.Sleep(500);
            
                Environment.Exit(0);
            }
        ));
        
        return door;
    }

    private void AddTakeOffOption(Item item, string itemName)
    {
        item.DialogueOptions.Add(new DialogueOption(
            $"Take {(itemName == "pants" ? "them" : "it")} off",
            $"You take off the {itemName}.",
            () => {
                item.IsWorn = false;
                
                if (itemName == "pants")
                {
                    item.DialogueOptions.Add(new DialogueOption(
                        "Put them on",
                        "You struggle into the tight pants. They still fit, somehow, despite everything.",
                        () => {
                            if (!_player.Inventory.Contains(item))
                            {
                                if (_player.CurrentLocation.Items.Contains(item))
                                {
                                    _player.CurrentLocation.Items.Remove(item);
                                }
                                _player.Inventory.Add(item);
                            }
                            
                            item.IsWorn = true;
                            item.DialogueOptions.RemoveAll(option => option.Text == "Put them on");
                            
                            bool hasKey = _player.Inventory.Any(i => i.Name == "room key");
                            if (!hasKey)
                            {
                                item.DialogueOptions.Add(new DialogueOption(
                                    "Check the pockets", 
                                    "You find a few coins, a business card for a detective agency, and a ROOM KEY.",
                                    () => {
                                        var key = new Item("room key", "A small brass key for the hostel room door.", true, ItemType.Key);
                                        key.InteractionResponses["examine"] = "A simple brass key with the number '1' etched on it. It should unlock the door to leave this room.";
                                        key.InteractionResponses["use"] = "You unlock the door with the key.";
                                        _player.Inventory.Add(key);
                                        _gameText.DisplayMessage("You pocket the room key. Now you can leave this place.");
                                        
                                        item.DialogueOptions.RemoveAll(option => option.Text == "Check the pockets");
                                    }
                                ));
                            }
                            
                            AddTakeOffOption(item, itemName);
                            _gameText.DisplayMessage("The familiar embrace of your disco pants makes you feel more like yourself.");
                        }
                    ));
                }
                else if (itemName == "necktie")
                {
                    item.DialogueOptions.Add(new DialogueOption(
                        "Put it on",
                        "You put on the necktie. It feels uncomfortably tight, like it's trying to strangle you. But also... right.",
                        () => {
                            if (!_player.Inventory.Contains(item))
                            {
                                if (_player.CurrentLocation.Items.Contains(item))
                                {
                                    _player.CurrentLocation.Items.Remove(item);
                                }
                                _player.Inventory.Add(item);
                            }
                            
                            item.IsWorn = true;
                            item.DialogueOptions.RemoveAll(option => option.Text == "Put it on");
                            AddTakeOffOption(item, itemName);
                            _gameText.DisplayMessage("You're now wearing the necktie.");
                        }
                    ));
                }
                else if (itemName == "shirt")
                {
                    item.DialogueOptions.Add(new DialogueOption(
                        "Put it on",
                        "You put on the shirt, buttoning it haphazardly. Some buttons are missing, and it hangs open in places.",
                        () => {
                            if (!_player.Inventory.Contains(item))
                            {
                                if (_player.CurrentLocation.Items.Contains(item))
                                {
                                    _player.CurrentLocation.Items.Remove(item);
                                }
                                _player.Inventory.Add(item);
                            }
                            
                            item.IsWorn = true;
                            item.DialogueOptions.RemoveAll(option => option.Text == "Put it on");
                            AddTakeOffOption(item, itemName);
                            _gameText.DisplayMessage("Even a dirty shirt is better than no shirt.");
                        }
                    ));
                }
                else if (itemName == "second shoe")
                {
                    item.DialogueOptions.Add(new DialogueOption(
                        "Put it on",
                        "You slip the shoe onto your foot. At least now you're fully shod, even if the rest of you is a mess.",
                        () => {
                            if (!_player.Inventory.Contains(item))
                            {
                                if (_player.CurrentLocation.Items.Contains(item))
                                {
                                    _player.CurrentLocation.Items.Remove(item);
                                }
                                _player.Inventory.Add(item);
                            }
                            
                            item.IsWorn = true;
                            item.DialogueOptions.RemoveAll(option => option.Text == "Put it on");
                            AddTakeOffOption(item, itemName);
                            _gameText.DisplayMessage("Having both shoes on makes you feel marginally more put-together.");
                        }
                    ));
                }
                
                item.DialogueOptions.RemoveAll(option => option.Text == $"Take {(itemName == "pants" ? "them" : "it")} off");
            }
        ));
    }

    private Item CreateNecktie()
    {
        var necktie = new Item("necktie", "A garish orange-and-blue tie. It seems to be gently swaying, even without a breeze.", true, ItemType.Clothing);
        necktie.InteractionResponses["examine"] = "The tie seems to whisper to you. Something about taking it with you...";
        necktie.InteractionResponses["take"] = "You grab the tie. It feels warm, almost alive in your hands.";
        
        necktie.DialogueOptions.Add(new DialogueOption(
            "Put it on", 
            "You put on the necktie. It feels uncomfortably tight, like it's trying to strangle you. But also... right.",
            () => { 
                necktie.IsWorn = true;
                if (!_player.Inventory.Contains(necktie))
                {
                    if (_player.CurrentLocation.Items.Contains(necktie))
                    {
                        _player.CurrentLocation.Items.Remove(necktie);
                    }
                    _player.Inventory.Add(necktie);
                }
                necktie.DialogueOptions.RemoveAll(option => option.Text == "Put it on");
                AddTakeOffOption(necktie, "necktie");
                _gameText.DisplayMessage("You're now wearing the necktie.");
            }
        ));
        necktie.DialogueOptions.Add(new DialogueOption(
            "Listen to it", 
            "You hold the tie closer to your ear. It whispers something about 'the pale' and 'finding your gun'.",
            null
        ));
        necktie.DialogueOptions.Add(new DialogueOption(
            "Examine the pattern", 
            "The pattern is a chaotic swirl of orange and blue. Staring at it too long makes you feel dizzy.",
            () => { 
                _player.Morale++; 
                _gameText.DisplayMessage("Something about this tie resonates with your inner chaos. Your morale increases slightly."); 
            }
        ));
        
        return necktie;
    }

    private Item CreateMirror()
    {
        var mirror = new Item("mirror", "A cracked bathroom mirror. Your reflection is... disturbing.", false);
        mirror.InteractionResponses["examine"] = "A haggard face stares back. Eyes bloodshot, stubble patchy. You barely recognize yourself.";
        
        mirror.DialogueOptions.Add(new DialogueOption(
            "Try to remember who you are", 
            "Flashes of memory - a police badge, shouting, the taste of alcohol. Nothing concrete.",
            null
        ));
        mirror.DialogueOptions.Add(new DialogueOption(
            "Attempt to tidy yourself up", 
            "You smooth down your hair and wipe your face, but it doesn't help much. You're a mess.",
            null
        ));
        mirror.DialogueOptions.Add(new DialogueOption(
            "Punch the reflection", 
            "Your fist connects with the mirror, further cracking it. Pain shoots through your hand.",
            () => { 
                _player.Health--; 
                _gameText.DisplayMessage("Your hand is bleeding. That was stupid."); 
            }
        ));
        
        return mirror;
    }

    private Item CreateSink()
    {
        var sink = new Item("sink", "A stained porcelain sink. The faucet drips continuously.", false);
        sink.InteractionResponses["examine"] = "Water slowly drips from the faucet. The basin has seen better days.";
        sink.InteractionResponses["use"] =
            "You splash cold water on your face. It doesn't help much with the hangover.";
        return sink;
    }
    private Item CreateWindow()
    {
        var window = new Item("window", "A grimy window overlooking the street below.", false);
        window.InteractionResponses["examine"] = "Through the dirty glass, you can see the street. It's raining. The world seems blurry and far away.";
        window.InteractionResponses["open"] = "You try to open the window, but it's stuck. Probably for the best.";
        return window;
    }

    private Item CreateLedger()
    {
        var ledger = new Item("ledger", "A small notebook on the floor. Probably belongs to the hostel.", true);
        ledger.InteractionResponses["examine"] = "The ledger contains records of guests. You spot your name: 'Du Bois, H. - Room 1. Paid for 3 nights.'";
        ledger.InteractionResponses["take"] = "You pocket the ledger. Might be useful.";
        return ledger;
    }
    
    private Item CreateCeilingFan()
    {
        var ceilingFan = new Item("ceiling fan", "An old ceiling fan spinning slowly. It creaks with each rotation.\nA necktie dangles from one of the blades.", false);
        ceilingFan.IsOn = true;
        ceilingFan.HasTie = true;

        Action<bool> updateFanState = null;
        updateFanState = (bool isOn) => {
            ceilingFan.IsOn = isOn;
            
            ceilingFan.DialogueOptions.Clear();
            
            if (isOn)
            {
                ceilingFan.Description = ceilingFan.HasTie
                    ? "An old ceiling fan spinning slowly. It creaks with each rotation.\nA garish orange-and-blue necktie dangles from one of the blades."
                    : "An old ceiling fan spinning slowly. It creaks with each rotation.";
                
                if (ceilingFan.HasTie)
                {
                    ceilingFan.DialogueOptions.Add(new DialogueOption(
                        "Try to grab the necktie", 
                        "You leap upward, trying to grab the necktie, but the spinning blades smack your hand hard. Your headache worsens with the sudden movement and pain.",
                        () => { 
                            _player.Health--; 
                            _gameText.DisplayMessage("Your headache intensifies and your hand stings. That wasn't a good idea."); 
                        }
                    ));
                }
                
                ceilingFan.DialogueOptions.Add(new DialogueOption(
                    "Watch the hypnotic spinning", 
                    "You stare at the fan, entranced by its rhythmic movement. A strange thought comes to you: 'What kind of cop are you?'",
                    null
                ));
                
                ceilingFan.DialogueOptions.Add(new DialogueOption(
                    "Turn it off", 
                    "You reach up and pull the chain. After a few stuttering rotations, the fan comes to a stop.",
                    () => { 
                        updateFanState(false);
                    }
                ));
            }
            else
            {
                ceilingFan.Description = ceilingFan.HasTie
                    ? "An old ceiling fan, now motionless. A pull chain dangles below it.\nA garish orange-and-blue necktie hangs from one of the blades, now within reach."
                    : "An old ceiling fan, now motionless. A pull chain dangles below it.";
                
                if (ceilingFan.HasTie)
                {
                    ceilingFan.DialogueOptions.Add(new DialogueOption(
                        "Grab the necktie", 
                        "With the fan stopped, you easily grab the necktie from the blade. It feels warm, almost alive in your hands.",
                        () => { 
                            var necktie = CreateNecktie();
                            _player.Inventory.Add(necktie);
                            _gameText.DisplayMessage("You take the necktie. It feels warm, almost alive in your hands.");
        
                            ceilingFan.HasTie = false;
        
                            updateFanState(false);
                        }
                    ));
                }
                
                ceilingFan.DialogueOptions.Add(new DialogueOption(
                    "Inspect the motionless fan", 
                    "Without the whirring blades, you can see the fan is old and dusty." + 
                    (ceilingFan.HasTie ? " The necktie seems oddly out of place here." : ""),
                    null
                ));
                
                ceilingFan.DialogueOptions.Add(new DialogueOption(
                    "Turn it back on", 
                    "You pull the chain again. The fan reluctantly starts spinning with a creaking sound.",
                    () => {
                        updateFanState(true);
                    }
                ));
            }
        };

        updateFanState(true);

        return ceilingFan;
    }
    private Item CreateSecondShoe()
    {
        var secondShoe = new Item("second shoe", "A worn-out shoe. It seems to match the one still on your foot.", true, ItemType.Clothing);
        secondShoe.InteractionResponses["examine"] = "This is definitely your other shoe. How did it end up outside the window?";
        secondShoe.InteractionResponses["take"] = "You retrieve your shoe from the window ledge. It's damp from the rain.";
        
        secondShoe.DialogueOptions.Add(new DialogueOption(
            "Put it on", 
            "You slip the shoe onto your foot. At least now you're fully shod, even if the rest of you is a mess.",
            () => { 
                if (!_player.Inventory.Contains(secondShoe))
                {
                    if (_player.CurrentLocation.Items.Contains(secondShoe))
                    {
                        _player.CurrentLocation.Items.Remove(secondShoe);
                    }
                    _player.Inventory.Add(secondShoe);
                }
                
                secondShoe.IsWorn = true;
                secondShoe.DialogueOptions.RemoveAll(option => option.Text == "Put it on");
                AddTakeOffOption(secondShoe, "second shoe");
                _gameText.DisplayMessage("Having both shoes on makes you feel marginally more put-together. It may be the only good thing about you right now."); 
            }
        ));
        secondShoe.DialogueOptions.Add(new DialogueOption(
            "Try to remember how it got there", 
            "Fragments of memory - climbing? Falling? Running? Nothing makes sense.",
            null
        ));

        return secondShoe;
    }
    private Item CreateShirt()
    {
        var shirt = new Item("shirt", "Your once-white dress shirt, now stained with unknown substances.", true, ItemType.Clothing);
        shirt.InteractionResponses["examine"] = "The shirt is wrinkled and stained with what looks like alcohol, sweat, and... is that blood?";
        shirt.InteractionResponses["take"] = "You pick up your shirt. It doesn't smell great.";
        
        shirt.DialogueOptions.Add(new DialogueOption(
            "Put it on", 
            "You put on the shirt, buttoning it haphazardly. Some buttons are missing, and it hangs open in places.",
            () => { 
                shirt.IsWorn = true;
                if (!_player.Inventory.Contains(shirt))
                {
                    if (_player.CurrentLocation.Items.Contains(shirt))
                    {
                        _player.CurrentLocation.Items.Remove(shirt);
                    }
                    _player.Inventory.Add(shirt);
                } 
                shirt.DialogueOptions.RemoveAll(option => option.Text == "Put it on");
                AddTakeOffOption(shirt, "shirt");
                _gameText.DisplayMessage("Even a dirty shirt is better than no shirt."); 
            }
        ));
        shirt.DialogueOptions.Add(new DialogueOption(
            "Check the pockets", 
            "You find a crumpled receipt in the pocket. It's from a bar called 'The Whirling-In-Rags'. Your tab was astronomical.",
            null
        ));
        shirt.DialogueOptions.Add(new DialogueOption(
            "Try to clean it", 
            "You attempt to rinse some of the stains out in the sink. The water turns a disturbing brownish-red, but the shirt doesn't look much better.",
            null
        ));
        
        return shirt;
    }

    private Item CreatePants()
    {
        var pants = new Item("pants", "Your disco pants. They're flashy, inappropriate for a police officer, and somehow still awesome.", true, ItemType.Clothing);
        pants.InteractionResponses["examine"] = "These are some serious disco pants - flared, shimmery, and totally inappropriate for police work. But they're *you*.\n" +
                                                "There's a jingle in the pocket.";
        pants.InteractionResponses["take"] = "You grab your pants. They feel like an old friend.";
        
        pants.DialogueOptions.Add(new DialogueOption(
            "Put them on", 
            "You struggle into the tight pants. They still fit, somehow, despite everything.",
            () => { 
                pants.IsWorn = true;
                if (!_player.Inventory.Contains(pants))
                {
                    if (_player.CurrentLocation.Items.Contains(pants))
                    {
                        _player.CurrentLocation.Items.Remove(pants);
                    }
                    _player.Inventory.Add(pants);
                }
                pants.DialogueOptions.RemoveAll(option => option.Text == "Put them on");
                
                bool checkPocketsExists = pants.DialogueOptions.Any(option => option.Text == "Check the pockets");

                if (!checkPocketsExists)
                {
                    pants.DialogueOptions.Add(new DialogueOption(
                        "Check the pockets", 
                        "You find a few coins, a business card for a detective agency, and a ROOM KEY.",
                        () => {
                            var key = new Item("room key", "A small brass key for the hostel room door.", true, ItemType.Key);
                            key.InteractionResponses["examine"] = "A simple brass key with the number '1' etched on it. It should unlock the door to leave this room.";
                            key.InteractionResponses["use"] = "You unlock the door with the key.";
                            _player.Inventory.Add(key);
                            _gameText.DisplayMessage("You pocket the room key. Now you can leave this place.");
            
                            pants.DialogueOptions.RemoveAll(option => option.Text == "Check the pockets");
                        }
                    ));
                }
                
                AddTakeOffOption(pants, "pants");
                _gameText.DisplayMessage("The familiar embrace of your disco pants makes you feel more like yourself."); 
            }
        ));
        
        pants.DialogueOptions.Add(new DialogueOption(
            "Admire the fabric", 
            "The material catches the light, shifting from purple to blue. These pants cost you a month's salary, but they were worth every penny.",
            () => { 
                _player.Morale++; 
                _gameText.DisplayMessage("Just looking at these pants reminds you of better times. Your morale increases slightly."); 
            }
        ));
        
        return pants;
    }
}