namespace CommonUtilities.Helpers;

public static class ConsoleHelper
{
    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine("\nPress any key to continue ...");
        Console.ReadKey();
    }

    public static string ReadLine(ReadLineModel readLineModel)
    {
        string input;
        while (true)
        {
            Console.WriteLine($"{readLineModel.question} (Enter X = Cancel)");
            input = Console.ReadLine() ?? string.Empty;

            if (!readLineModel.allowedEmpty && string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input! Please enter a non-empty value.");
                continue;
            }

            if (input.ToUpper().Equals("X")) return readLineModel.value;

            break;
        }

        return input.Trim();
    }
}