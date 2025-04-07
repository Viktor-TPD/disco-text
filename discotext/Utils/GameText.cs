using discotext.Models;
namespace discotext.Utils;

public class GameText
{
    public void DisplayIntro()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("======================================");
        Console.WriteLine("    Monday Morning. 07:14 AM          ");
        Console.WriteLine("======================================");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("You wake up with a splitting headache. The room spins around you.");
        Console.WriteLine("Your mouth tastes like something died in it. You have no memory of how you got here.");
        Console.WriteLine("This must be the mother of all hangovers...");
        Console.WriteLine();
    }
    public void DisplayHealthDeath()
    {
        Console.WriteLine("A taste of iron coats your mouth. Your chest hurts.");
        Console.WriteLine("The world turns dark.");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("======================================");
        Console.WriteLine("             GAME OVER                ");
        Console.WriteLine("     You died of a heart attack       ");
        Console.WriteLine("======================================");
        Console.ResetColor();
    }
    public void DisplayMoraleDeath()
    {
        Console.WriteLine("The world spins around you. The ground hits your knees as you fall.");
        Console.WriteLine("You never wanted to be a cop anyway.");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("======================================");
        Console.WriteLine("             GAME OVER                ");
        Console.WriteLine("      You never leave the room        ");
        Console.WriteLine("======================================");
        Console.ResetColor();
         
    }
    public void DisplayOutro()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("======================================");
        Console.WriteLine("             GAME OVER                ");
        Console.WriteLine("======================================");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Thanks for playing!");
        Console.WriteLine();
    }

    public void DisplayLocationDescription(Location location)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[{location.Name}]");
        Console.ResetColor();
        Console.WriteLine(location.Description);

        if (location.Exits.Count > 0)
        {
            Console.Write("You can go: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Join(", ", location.Exits.Keys));
            Console.ResetColor();
        }

        if (location.Items.Count > 0)
        {
            Console.WriteLine("You see:");
            foreach (var item in location.Items)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"- {item.Name}");
                Console.ResetColor();
                Console.WriteLine($": {item.Description}");
            }
        }

        Console.WriteLine();
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplayPlayerStatus(Player player)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("STATUS");
        Console.ResetColor();
        Console.WriteLine($"Health: {GetStatusBar(player.Health, 5)}");
        Console.WriteLine($"Morale: {GetStatusBar(player.Morale, 5)}");
        Console.WriteLine();
    }

    private string GetStatusBar(int value, int max)
    {
        string bar = "[";
        for (int i = 0; i < max; i++)
        {
            bar += i < value ? "■" : "x";
        }

        bar += "]";
        return bar;
    }

    public void DisplayDialogueOptions(Item item, int selectedIndex, bool showExitOption = false)
    {
        Console.Clear();
        Console.WriteLine(item.Description);
        Console.WriteLine();
        Console.WriteLine("Choose an option:");

        for (int i = 0; i < item.DialogueOptions.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
            }
            else
            {
                Console.ResetColor();
                Console.Write("  ");
            }

            Console.WriteLine(item.DialogueOptions[i].Text);
            Console.ResetColor();
        }

        if (showExitOption)
        {
            if (selectedIndex == item.DialogueOptions.Count)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
            }
            else
            {
                Console.ResetColor();
                Console.Write("  ");
            }

            Console.WriteLine("Stop examining");
            Console.ResetColor();
        }
    }
}