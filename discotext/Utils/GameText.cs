namespace discotext.Utils;

public class GameText
{
    public void DisplayIntro()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("======================================");
        Console.WriteLine("         DISCO ELYSIUM ROOM           ");
        Console.WriteLine("======================================");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("You wake up with a splitting headache. The room spins around you.");
        Console.WriteLine("Your mouth tastes like something died in it. You have no memory of how you got here.");
        Console.WriteLine("This must be the mother of all hangovers...");
        Console.WriteLine();
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

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplayLocationDescription(Location location)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[{location.Name}]");
        Console.ResetColor();
        Console.WriteLine(location.Description);

        // Show available exits
        if (location.Exits.Count > 0)
        {
            Console.Write("You can go: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Join(", ", location.Exits.Keys));
            Console.ResetColor();
        }

        // Show visible items
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
}