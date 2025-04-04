using discotext.Core;
using discotext.Models;

namespace discotext.Utils;
public class CommandProcessor
{
    private Game _game;
    private Player _player;
    private GameText _gameText;

    public CommandProcessor(Game game)
    {
        _game = game;
        _player = game.GetPlayer();
        _gameText = game.GetGameText();
    }

    public void ProcessCommand(string input)
    {
        var parts = input.Split(' ');
        var command = parts[0];

        switch (command)
        {
            case "look":
                Look();
                break;
            case "go":
            case "move":
                if (parts.Length > 1)
                    Move(parts[1]);
                else
                    _gameText.DisplayMessage("Go where?");
                break;
            case "examine":
            case "inspect":
                if (parts.Length > 1)
                    Examine(parts[1]);
                else
                    _gameText.DisplayMessage("Examine what?");
                break;
            case "take":
            case "grab":
                if (parts.Length > 1)
                    Take(parts[1]);
                else
                    _gameText.DisplayMessage("Take what?");
                break;
            case "inventory":
            case "items":
                ShowInventory();
                break;
            case "use":
                if (parts.Length > 1)
                    Use(parts[1]);
                else
                    _gameText.DisplayMessage("Use what?");
                break;
            case "status":
                _gameText.DisplayPlayerStatus(_player);
                break;
            case "help":
                ShowHelp();
                break;
            default:
                _gameText.DisplayMessage("I don't understand that command. Type 'help' for a list of commands.");
                break;
        }
    }

    private void Look()
    {
        _gameText.DisplayLocationDescription(_player.CurrentLocation);
    }

    private void Move(string direction)
    {
        if (_player.CurrentLocation.Exits.ContainsKey(direction))
        {
            _player.CurrentLocation = _player.CurrentLocation.Exits[direction];
            _gameText.DisplayLocationDescription(_player.CurrentLocation);
        }
        else
        {
            _gameText.DisplayMessage($"You can't go {direction} from here.");
        }
    }

    private void Examine(string itemName)
    {
        var inventoryItem = _player.Inventory.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
        if (inventoryItem != null)
        {
            if (inventoryItem.HasDialogueChoices)
            {
                HandleDialogueOptions(inventoryItem);
            }
            else if (inventoryItem.InteractionResponses.ContainsKey("examine"))
            {
                _gameText.DisplayMessage(inventoryItem.InteractionResponses["examine"]);
            }
            else
            {
                _gameText.DisplayMessage(inventoryItem.Description);
            }
            return;
        }

        var locationItem = _player.CurrentLocation.Items.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
        if (locationItem != null)
        {
            if (locationItem.HasDialogueChoices)
            {
                HandleDialogueOptions(locationItem);
            }
            else if (locationItem.InteractionResponses.ContainsKey("examine"))
            {
                _gameText.DisplayMessage(locationItem.InteractionResponses["examine"]);
            }
            else
            {
                _gameText.DisplayMessage(locationItem.Description);
            }
        }
        else
        {
            _gameText.DisplayMessage($"You don't see a {itemName} here.");
        }
    }
        
    private void HandleDialogueOptions(Item item)
    {
        int selectedIndex = 0;
        bool optionSelected = false;
            
        while (!optionSelected)
        {
            _gameText.DisplayDialogueOptions(item, selectedIndex);
                
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = Math.Min(item.DialogueOptions.Count - 1, selectedIndex + 1);
                    break;
                case ConsoleKey.Enter:
                    optionSelected = true;
                    break;
            }
        }
            
        DialogueOption selectedOption = item.DialogueOptions[selectedIndex];
        Console.Clear();
        _gameText.DisplayMessage(selectedOption.Response);
            
        selectedOption.Effect?.Invoke();
    }

    private void Take(string itemName)
    {
        var item = _player.CurrentLocation.Items.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
        if (item != null)
        {
            if (item.CanTake)
            {
                if (item.InteractionResponses.ContainsKey("take"))
                {
                    _gameText.DisplayMessage(item.InteractionResponses["take"]);
                }
                else
                {
                    _gameText.DisplayMessage($"You take the {item.Name}.");
                }
                _player.Inventory.Add(item);
                _player.CurrentLocation.Items.Remove(item);
            }
            else
            {
                _gameText.DisplayMessage($"You can't take the {item.Name}.");
            }
        }
        else
        {
            _gameText.DisplayMessage($"You don't see a {itemName} here.");
        }
    }

    private void ShowInventory()
    {
        if (_player.Inventory.Count == 0)
        {
            _gameText.DisplayMessage("You're not carrying anything.");
            return;
        }

        _gameText.DisplayMessage("You are carrying:");
        foreach (var item in _player.Inventory)
        {
            _gameText.DisplayMessage($"- {item.Name}: {item.Description}");
        }
    }

    private void Use(string itemName)
    {
        var inventoryItem = _player.Inventory.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
        if (inventoryItem != null)
        {
            if (inventoryItem.InteractionResponses.ContainsKey("use"))
            {
                _gameText.DisplayMessage(inventoryItem.InteractionResponses["use"]);
            }
            else
            {
                _gameText.DisplayMessage($"You're not sure how to use the {inventoryItem.Name}.");
            }
            return;
        }

        var locationItem = _player.CurrentLocation.Items.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
        if (locationItem != null)
        {
            if (locationItem.InteractionResponses.ContainsKey("use"))
            {
                _gameText.DisplayMessage(locationItem.InteractionResponses["use"]);
            }
            else
            {
                _gameText.DisplayMessage($"You're not sure how to use the {locationItem.Name}.");
            }
        }
        else
        {
            _gameText.DisplayMessage($"You don't see a {itemName} here.");
        }
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
    }
}