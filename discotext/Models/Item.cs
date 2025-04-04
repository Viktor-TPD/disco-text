using discotext.Utils;

namespace discotext.Models
{
    public enum ItemType
    {
        Normal,   // Standard items
        Key,      // Items that unlock things
        Special,  // Items with unique properties
        Note      // Written information
    }
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanTake { get; set; }
        public ItemType Type { get; set; }
        public Dictionary<string, string> InteractionResponses { get; set; }
        public List<DialogueOption> DialogueOptions { get; set; }
        public bool HasDialogueChoices => DialogueOptions != null && DialogueOptions.Count > 0;

        public Item(string name, string description, bool canTake = true, ItemType type = ItemType.Normal)
        {
            Name = name;
            Description = description;
            CanTake = canTake;
            Type = type;
            InteractionResponses = new Dictionary<string, string>();
            DialogueOptions = new List<DialogueOption>();
        }
    }
    
    public class DialogueOption
    {
        public string Text { get; set; }
        public string Response { get; set; }
        public Action Effect { get; set; }
        
        public DialogueOption(string text, string response, Action effect = null)
        {
            Text = text;
            Response = response;
            Effect = effect;
        }
    }

    public class ItemFactory
    {
        private Player _player;
        private GameText _gameText;

        public ItemFactory(Player player, GameText gameText)
        {
            _player = player;
            _gameText = gameText;
        }

        public Dictionary<string, Item> CreateAllItems()
        {
            var items = new Dictionary<string, Item>();
            
            items.Add("necktie", CreateNecktie());
            items.Add("mirror", CreateMirror());
            items.Add("sink", CreateSink());
            items.Add("window", CreateWindow());
            items.Add("ledger", CreateLedger());
            items.Add("ceiling_fan", CreateCeilingFan());
            
            return items;
        }

        private Item CreateNecktie()
        {
            var necktie = new Item("necktie", "A garish orange-and-blue tie. It seems to be gently swaying, even without a breeze.", true);
            necktie.InteractionResponses["examine"] = "The tie seems to whisper to you. Something about taking it with you...";
            necktie.InteractionResponses["take"] = "You grab the tie. It feels warm, almost alive in your hands.";
            
            necktie.DialogueOptions.Add(new DialogueOption(
                "Put it on", 
                "You put on the necktie. It feels uncomfortably tight, like it's trying to strangle you. But also... right.",
                null
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
            sink.InteractionResponses["use"] = "You splash cold water on your face. It doesn't help much with the hangover.";
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
            var ceilingFan = new Item("ceiling-fan", "An old ceiling fan spinning slowly. It creaks with each rotation.", false);
            ceilingFan.DialogueOptions.Add(new DialogueOption(
                "Try to jump and grab it", 
                "You leap upward, but the fan is too high. Your headache worsens with the sudden movement.",
                () => { 
                    _player.Health--; 
                    _gameText.DisplayMessage("Your headache intensifies. That wasn't a good idea."); 
                }
            ));
            ceilingFan.DialogueOptions.Add(new DialogueOption(
                "Watch the hypnotic spinning", 
                "You stare at the fan, entranced by its rhythmic movement. A strange thought comes to you: 'What kind of cop are you?'",
                null
            ));
            ceilingFan.DialogueOptions.Add(new DialogueOption(
                "Look for the switch to turn it off", 
                "You spot a pull-chain, but it's broken. The fan seems destined to keep spinning, much like your thoughts.",
                null
            ));
            return ceilingFan;
        }
    }
}