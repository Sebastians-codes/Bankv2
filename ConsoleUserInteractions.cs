namespace BankStorage;

public class ConsoleUserInteractions : IUserInteractions
{
    public char ReadKey() =>
        Console.ReadKey(true).KeyChar;

    public string ReadLine() =>
        Console.ReadLine();
}
