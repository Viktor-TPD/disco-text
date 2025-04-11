using discotext.Models;
using discotext.Core;
namespace discotext.Utils;

public class DialogueHandler
{
    private GameText _gameText;
    private Game _game;
    private bool _exitDialogueAfterEffect = false;
        
    public DialogueHandler(GameText gameText, Game game)
    {
        _gameText = gameText;
        _game = game;
    }
        
    public void HandleDialogue(Item item, Action onExit = null)
    {
        if (!item.HasDialogueChoices)
        {
            _gameText.DisplayMessage(item.Description);
            return;
        }
    
        int selectedIndex = 0;
        bool exitDialogue = false;
        int totalOptions = item.DialogueOptions.Count + 1;
    
        while (!exitDialogue)
        {
            _gameText.DisplayDialogueOptions(item, selectedIndex, true);
        
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + totalOptions) % totalOptions;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % totalOptions;
                    break;
                case ConsoleKey.Enter:
                    if (selectedIndex == item.DialogueOptions.Count)
                    {
                        exitDialogue = true;
                        Console.Clear();
                        _gameText.DisplayMessage("You stop examining the " + item.Name + ".");
                        
                        onExit?.Invoke();
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
                            onExit?.Invoke();
                            continue;
                        }
                        
                        if (_game != null)
                        {
                            var player = _game.GetPlayer();
                            if (player.Health <= 0 || player.Morale <= 0)
                            {
                                exitDialogue = true;
                                continue;
                            }
                        }
                        
                        _gameText.DisplayMessage("\nPress any key to continue...");
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                    break;
            }
        }
    }
    
    public void ExitDialogueAfterEffect()
    {
        _exitDialogueAfterEffect = true;
    }
}