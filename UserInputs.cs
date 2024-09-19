namespace BankStorage;

public class UserInputs
{
    public int GetInt(string message, string errorMesage,
        int min = int.MinValue, int max = int.MaxValue)
    {
        do
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(),
                out int num) && num > min && num < max)
            {
                return num;
            }
            Console.Clear();
            Console.WriteLine(errorMesage);
        } while (true);
    }

    public decimal GetDecimal(string message, string errorMesage,
        decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
    {
        do
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(),
                out int num) && num > min && num < max)
            {
                return num;
            }

            Console.Clear();
            Console.WriteLine(errorMesage);

        } while (true);
    }

    public string GetName(string message, string errorMesage)
    {
        do
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && input.Trim().Length > 1)
            {
                string name = "";

                foreach (var str in input)
                {
                    if (str != ' ')
                    {
                        name += str;
                    }
                }

                return $"{char.ToUpperInvariant(name[0])}{name.ToLowerInvariant()[1..]}";
            }

        } while (true);
    }

    public int GetPin(string message, int minLength = 4, int maxLength = 8)
    {
        ConsoleKeyInfo key;
        List<char> chars = [];

        do
        {
            if (chars.Count() == maxLength)
            {
                Console.Clear();
                Console.WriteLine($"You have reached the maximum length of {maxLength}.");
                Console.WriteLine("Press Enter too Continue.");
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                continue;
            }

            Console.Write($"{message}, between {minLength} and {maxLength} long. -> ");

            if (chars.Count() > 0)
            {
                foreach (char chr in chars)
                {
                    Console.Write("*");
                }
            }

            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter && chars.Count > minLength - 1)
            {
                break;
            }

            if (int.TryParse(key.KeyChar.ToString(), out int num) && num < 10 && num > 0)
            {
                chars.Add(key.KeyChar);
                Console.Clear();
                continue;
            }

            Console.Clear();
            Console.WriteLine("Not a valid integer try again.");

        } while (true);

        return int.Parse(string.Join("", chars));
    }
}
