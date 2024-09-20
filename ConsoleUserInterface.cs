namespace BankStorage;

public class ConsoleUserInterface : IUserInterface
{
    public void Write(string message) =>
        Console.Write(message);

    public void WriteLine(string message) =>
        Console.WriteLine(message);

    public void Clear() =>
        Console.Clear();
}
