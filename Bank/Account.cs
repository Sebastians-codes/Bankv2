namespace BankStorage.Bank;

internal class Account
{
    internal int AccountNumber { get; }
    internal decimal Balance { get; private set; }
    internal List<decimal> Movements { get; private set; } = [];
    private string _path;

    internal Account(int CustomerNumber, int accountNumber)
    {
        AccountNumber = accountNumber;
        _path = $"Accounts/{CustomerNumber}/{AccountNumber}.txt";

        InitializeBalance();
    }

    internal void Deposit(decimal movement)
    {
        Balance += movement;
        Movements.Add(movement);
        File.AppendAllText(_path, $",{movement}");
    }

    internal void Withdraw(decimal movement)
    {
        Balance -= movement;
        Movements.Add(decimal.Parse($"-{movement}"));
        File.AppendAllText(_path, $",-{movement}");
    }

    internal void SendMoney(int customerNumber, int accountNumber, decimal movement)
    {
        string transferPath = $"Accounts/{customerNumber}/{accountNumber}.txt";

        Balance -= movement;
        Movements.Add(decimal.Parse($"-{movement}"));

        File.AppendAllText(transferPath, $",{movement}");

        File.AppendAllText(_path, $",-{movement}");
    }

    internal string ToCsv() =>
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
