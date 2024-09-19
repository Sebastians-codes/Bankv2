namespace BankStorage;

public class Account
{
    public int AccountNumber { get; }
    public decimal Balance { get; private set; }
    public List<decimal> Movements { get; private set; } = [];
    private string _path;

    public Account(int CustomerNumber, int accountNumber)
    {
        AccountNumber = accountNumber;
        _path = $"Accounts/{CustomerNumber}/{AccountNumber}.txt";

        InitializeBalance();
    }

    public void GetBalance() =>
        Console.WriteLine($"Your account balance is {Balance}$");

    public void Deposit(decimal movement)
    {
        Balance += movement;
        Movements.Add(movement);
        File.AppendAllText(_path, $",{movement}");
    }

    public void Withdraw(decimal movement)
    {
        Balance -= movement;
        Movements.Add(decimal.Parse($"-{movement}"));
        File.AppendAllText(_path, $",-{movement}");
    }

    public void SendMoney(int customerNumber, int accountNumber, decimal movement)
    {
        string transferPath = $"Accounts/{customerNumber}/{accountNumber}.txt";

        Balance -= movement;
        Movements.Add(decimal.Parse($"-{movement}"));

        File.AppendAllText(transferPath, $",{movement}");

        File.AppendAllText(_path, $",-{movement}");
    }

    public string ToCsv() =>
        $"{AccountNumber}";

    private void InitializeBalance()
    {

        if (!File.Exists(_path))
        {
            File.AppendAllText(_path, $"{Balance}");
            return;
        }

        string[] movements = File.ReadAllText(_path).Split(",");

        if (movements.Length < 1)
        {
            return;
        }

        Movements.Clear();

        for (int i = 0; i < movements.Length; i++)
        {
            Movements.Add(decimal.Parse(movements[i]));
        }

        Balance = Movements.Sum();
    }
}
