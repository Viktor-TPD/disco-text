using discotext.Models;
namespace discotext.Utils;

public class DialogueHandler
{
    private GameText _gameText;
        
    public DialogueHandler(GameText gameText)
    {
        _gameText = gameText;
    }
        
    public void HandleDialogue(Item item, Action onExit = null)
    {
        if (!item.HasDialogueChoices)
        {
            _gameText.DisplayMessage(item.Description);
            return;
        }
            
        int selectedIndex = 0;
        bool optionSelected = false;
        
        int totalOptions = item.DialogueOptions.Count + 1;
            
        while (!optionSelected)
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
                    optionSelected = true;
                    break;
            }
        }

        if (selectedIndex == item.DialogueOptions.Count)
        {
            Console.Clear();
            _gameText.DisplayMessage("You stop examining the " + item.Name + ".");
            onExit?.Invoke();
        }
        else
        {
            DialogueOption selectedOption = item.DialogueOptions[selectedIndex];
            Console.Clear();
            _gameText.DisplayMessage(selectedOption.Response);
            selectedOption.Effect?.Invoke();
        }
    }
}