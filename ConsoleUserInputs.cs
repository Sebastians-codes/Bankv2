namespace BankStorage;

public class ConsoleUserInputs : IUserInputs
{
    public char ReadKey() =>
        Console.ReadKey(true).KeyChar;

    public string ReadLine() =>
        Console.ReadLine();
}
