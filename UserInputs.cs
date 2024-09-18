namespace BankStorageg;

public class UserInputs
{
    public int GetInt(string message, string errorMesage,
        int min = int.MinValue, int max = int.MaxValue)
    {
        do
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadKey(true).ToString(),
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

            if (int.TryParse(Console.ReadKey(true).ToString(),
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
}
