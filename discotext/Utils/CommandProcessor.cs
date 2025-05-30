using System.Reflection.Metadata.Ecma335;
using discotext.Core;
using discotext.Models;

namespace discotext.Utils;
public class CommandProcessor
{
    private Game _game;
    private Player _player;
    private GameText _gameText;
    private CommandMatcher _commandMatcher;
    private ItemMatcher _itemMatcher;
    private bool _exitDialogueAfterEffect = false;
    
    public CommandProcessor(Game game)
    {
        _game = game;
        _player = game.GetPlayer();
        _gameText = game.GetGameText();
        _commandMatcher = new CommandMatcher(_gameText);
        _itemMatcher = new ItemMatcher(_gameText);
    }
    
    public void ExitDialogueAfterEffect()
    {
        _exitDialogueAfterEffect = true;
    }

    public void ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            _gameText.DisplayMessage("Please enter a command.");
            return;
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            _gameText.DisplayMessage("Please enter a command.");
            return;
        }

        string matchedCommand = _commandMatcher.MatchCommand(parts[0]);
        
        if (matchedCommand == null)
        {
            _gameText.DisplayMessage($"I don't understand '{parts[0]}'. Type 'help' for a list of commands.");
            return;
        }
        switch (matchedCommand)
        {
            case "look":
                Look();
                break;
            case "go":
            case "move":
                if (parts.Length > 1)
                    Move(string.Join(" ", parts.Skip(1)));
                else
                    _gameText.DisplayMessage("Go where?");
                break;
            case "examine":
            case "inspect":
                if (parts.Length > 1)
                {
                    string itemName = string.Join(" ", parts.Skip(1));
                    Examine(itemName);
                }
                else
                    _gameText.DisplayMessage("Examine what?");

                break;
            case "take":
            case "grab":
                if (parts.Length > 1)
                {
                    string itemName = string.Join(" ", parts.Skip(1));
                    Take(itemName);
                }

                else
                    _gameText.DisplayMessage("Take what?");
                break;
            case "inventory":
            case "items":
                ShowInventory();
                break;
            case "use":
                if (parts.Length > 1)
                {
                    string itemName = string.Join(" ", parts.Skip(1));
                    Use(itemName);
                }
                else
                    _gameText.DisplayMessage("Use what?");
                break;
            case "status":
                _gameText.DisplayPlayerStatus(_player);
                break;
            case "help":
                ShowHelp();
                break;
            
                // "Joke Commands" - no game mechanic attached, just to satisfy player curiosity
                case "die":
                    Die();
                    break;
                case "dance":
                    Dance();
                    break;
                case "scream":
                    Scream();
                    break;
            default:
                _gameText.DisplayMessage("I don't understand that command. Type 'help' for a list of commands.");
                break;
        }
    }

    public void Look()
    {
        _gameText.DisplayLocationDescription(_player.CurrentLocation);
    }

    private void Move(string direction)
    {
        if (_player.CurrentLocation.Exits.ContainsKey(direction))
        {
            _player.CurrentLocation = _player.CurrentLocation.Exits[direction];
            _gameText.DisplayLocationDescription(_player.CurrentLocation);
            return;
        }
    
        var availableExits = _player.CurrentLocation.Exits.Keys.ToList();
    
        var bestMatches = new List<(string Exit, int Distance)>();
        foreach (var exit in availableExits)
        {
            int distance = TextMatching.LevenshteinDistance(direction.ToLower(), exit.ToLower());
            if (distance <= 2)
                bestMatches.Add((exit, distance));
        }
    
        if (bestMatches.Count > 0)
        {
            var bestMatch = bestMatches.OrderBy(m => m.Distance).First();
            _gameText.DisplayMessage($"I understand you want to go {bestMatch.Exit}.");
            _player.CurrentLocation = _player.CurrentLocation.Exits[bestMatch.Exit];
            _gameText.DisplayLocationDescription(_player.CurrentLocation);
            return;
        }
    
        _gameText.DisplayMessage($"You can't go {direction} from here.");
    }
    private void Examine(string itemName)
    {
        var availableItems = _player.Inventory
            .Concat(_player.CurrentLocation.Items)
            .ToList();

        var matchedItem = _itemMatcher.MatchItem(itemName, availableItems);

        if (matchedItem == null)
        {
            _gameText.DisplayMessage($"You don't see a {itemName} here.");
            return;
        }

        var inventoryItem = _player.Inventory.Contains(matchedItem);

        if (matchedItem.Name == "second shoe" && inventoryItem && _player.CurrentLocation.Name == "By the Window")
        {
            bool hasThrowOption = matchedItem.DialogueOptions.Any(option => option.Text == "Throw it back out the window");
            
            if (!hasThrowOption)
            {
                matchedItem.DialogueOptions.Add(new DialogueOption(
                    "Throw it back out the window",
                    "You toss the shoe back out the window in frustration. Now you have to go get it again. Brilliant move.",
                    () =>
                    {
                        _player.Morale--;
                        _gameText.DisplayMessage("You immediately regret your impulsive decision. Your morale decreases.");
                        _player.Inventory.Remove(matchedItem);
                        _player.CurrentLocation.Items.Add(matchedItem);

                        _gameText.DisplayMessage("\nPress any key to continue...");
                        Console.ReadKey(true);
                        Console.Clear();
                        
                        ExitDialogueAfterEffect();
                        Look();
                    }
                ));
            }
        }
        else if (matchedItem.Name == "second shoe" && inventoryItem && _player.CurrentLocation.Name != "By the Window")
        {
            matchedItem.DialogueOptions.RemoveAll(option => option.Text == "Throw it back out the window");
        }

        if (matchedItem.HasDialogueChoices)
        {
            if (inventoryItem)
            {
                HandleDialogueOptions(matchedItem);
            }
            else
            {
                _game.GetDialogueHandler().HandleDialogue(matchedItem, () => Look());
            }
        }
        else if (matchedItem.InteractionResponses.ContainsKey("examine"))
        {
            _gameText.DisplayMessage(matchedItem.InteractionResponses["examine"]);
        }
        else
        {
            _gameText.DisplayMessage(matchedItem.Description);
        }
    }
    private void HandleDialogueOptions(Item item)
{
    int selectedIndex = 0;
    bool exitDialogue = false;
    
    while (!exitDialogue)
    {
        int totalOptions = item.DialogueOptions.Count + 1;
        
        _gameText.DisplayInventoryDialogueOptions(item, selectedIndex);
        
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                selectedIndex = (selectedIndex - 1 + totalOptions) % totalOptions;
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                selectedIndex = (selectedIndex + 1) % totalOptions;
                break;
            case ConsoleKey.Enter:
                if (selectedIndex == item.DialogueOptions.Count)
                {
                    exitDialogue = true;
                    Console.Clear();
                    
                    Look();
                }
                else
                {
                    DialogueOption selectedOption = item.DialogueOptions[selectedIndex];
                    Console.Clear();
                    _gameText.DisplayMessage(selectedOption.Response);
                    
                    _exitDialogueAfterEffect = false;
                    
                    selectedOption.Effect?.Invoke();
                    
                    if (_exitDialogueAfterEffect)
                    {
                        exitDialogue = true;
                        continue;
                    }
                    
                    if (_player.Health <= 0)
                    {
                        exitDialogue = true;
                        return;
                    }
                    if (_player.Morale <= 0)
                    {
                        
                        exitDialogue = true;
                        return; 
                    }
                    
                    _gameText.DisplayMessage("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                }
                break;
        }
    }
}
    private void Take(string itemName)
    {
        var locationItems = _player.CurrentLocation.Items;
    
        var matchedItem = _itemMatcher.MatchItem(itemName, locationItems);
    
        if (matchedItem == null)
        {
            _gameText.DisplayMessage($"You don't see a {itemName} here.");
            return;
        }
    
        if (matchedItem.CanTake)
        {
            if (matchedItem.InteractionResponses.ContainsKey("take"))
            {
                _gameText.DisplayMessage(matchedItem.InteractionResponses["take"]);
            }
            else
            {
                _gameText.DisplayMessage($"You take the {matchedItem.Name}.");
            }
        
            _player.Inventory.Add(matchedItem);
            _player.CurrentLocation.Items.Remove(matchedItem);
        }
        else
        {
            _gameText.DisplayMessage($"You can't take the {matchedItem.Name}.");
        }
    }

    private void ShowInventory()
    {
        if (_player.Inventory.Count == 0)
        {
            _gameText.DisplayMessage("You're not carrying anything.");
            return;
        }
        
        var wornItems = _player.Inventory.Where(item => item.IsWorn).ToList();
        if (wornItems.Any())
        {
            _gameText.DisplayMessage("You are wearing:");
            foreach (var item in wornItems)
            {
                _gameText.DisplayMessage($"- {item.Name}");
            }
            _gameText.DisplayMessage("");
        }

        var carriedItems = _player.Inventory.Where(item => !item.IsWorn).ToList();
        if (carriedItems.Any())
        {
            _gameText.DisplayMessage("You are carrying:");
            foreach (var item in carriedItems)
            {
                _gameText.DisplayMessage($"- {item.Name}: {item.Description}");
            }
        } 
    }

    private void Use(string itemName)
    {
        var availableItems = _player.Inventory
            .Concat(_player.CurrentLocation.Items)
            .ToList();
    
        var matchedItem = _itemMatcher.MatchItem(itemName, availableItems);
    
        if (matchedItem == null)
        {
            _gameText.DisplayMessage($"You don't see a {itemName} here.");
            return;
        }
    
        bool inInventory = _player.Inventory.Contains(matchedItem);
    
        if (matchedItem.InteractionResponses.ContainsKey("use"))
        {
            _gameText.DisplayMessage(matchedItem.InteractionResponses["use"]);
        }
        else if (matchedItem.HasDialogueChoices)
        {
            if (inInventory)
            {
                HandleDialogueOptions(matchedItem);
            }
            else
            {
                _game.GetDialogueHandler().HandleDialogue(matchedItem, () => Look());
            }
        }
        else
        {
            _gameText.DisplayMessage($"You're not sure how to use the {matchedItem.Name}.");
        }
    }

    private void Die()
    {
        _gameText.DisplayMessage("It's not worth it.");
        _player.Health = 0;
    }

    private void Dance()
    {
        _gameText.DisplayMessage("You do a little jig.");
    }

    private void Scream()
    {
        _gameText.DisplayMessage("You gather air into your lungs and release a primal scream. It feels good.");
    }

    private void ShowHelp()
    {
        _gameText.DisplayMessage("Available commands:");
        _gameText.DisplayMessage("- look: Look around your current location");
        _gameText.DisplayMessage("- go [direction]: Move in a direction (e.g., 'go bathroom')");
        _gameText.DisplayMessage("- examine [item]: Look at an item more closely");
        _gameText.DisplayMessage("- take [item]: Pick up an item");
        _gameText.DisplayMessage("- inventory: See what you're carrying");
        _gameText.DisplayMessage("- use [item]: Use or interact with an item");
        _gameText.DisplayMessage("- status: Check your health and morale");
        _gameText.DisplayMessage("- help: Show this help message");
        _gameText.DisplayMessage("- quit: End the game");
        _gameText.DisplayMessage("- This game uses fuzzy matching for your inputs. \n " +
                                 "There are some synonyms available for all commands.");
    }
}