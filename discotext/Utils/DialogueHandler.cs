using discotext.Models;
namespace discotext.Utils;

public class DialogueHandler
{
    private GameText _gameText;
        
    public DialogueHandler(GameText gameText)
    {
        _gameText = gameText;
    }
        
    public void HandleDialogue(Item item)
    {
        if (!item.HasDialogueChoices)
        {
            _gameText.DisplayMessage(item.Description);
            return;
        }
            
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
                case ConsoleKey.Escape:
                    Console.Clear();
                    _gameText.DisplayMessage("You decide not to interact with it further.");
                    return;
            }
        }
            
        DialogueOption selectedOption = item.DialogueOptions[selectedIndex];
        Console.Clear();
        _gameText.DisplayMessage(selectedOption.Response);
            
        selectedOption.Effect?.Invoke();
    }
}