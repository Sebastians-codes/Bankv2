namespace BankStorage;

public class Account
{
    public int AccountNumber { get; init; }
    public string? HolderFirstName { get; init; }
    public string? HolderLastName { get; init; }
    public int PinCode { get; init; }
    private string _path;
    private decimal _balance;
    private List<decimal> _movements = [];

    public Account(int accountNumber, string holderFirstName, string holderLastName, int pinCode)
    {
        AccountNumber = accountNumber;
        HolderFirstName = holderFirstName;
        HolderLastName = holderLastName;
        PinCode = pinCode;
        _path = $"AccountBalances/{AccountNumber}.csv";

        InitializeBalance();
    }

    public void GetBalance()
    {
        InitializeBalance();
        Console.WriteLine(_balance);
    }

    public void MakeMovement(decimal movement)
    {
        if (_balance < movement)
        {
            Console.WriteLine("Not enough money too withdrawl that amount");
            return;
        }

        _balance += movement;

        _movements.Add(movement);

        File.AppendAllText(_path, $",{movement}");
    }

    public void SendMoney(int accountNumber, decimal movement)
    {
        if (accountNumber == AccountNumber)
        {
            Console.WriteLine("You can not send money to yourself...");
            return;
        }

        string transferPath = $"AccountBalances/{accountNumber}.csv";
        if (movement > _balance)
        {
            Console.WriteLine("You do not have enough money.");
            return;
        }

        if (!File.Exists(transferPath))
        {
            Console.WriteLine($"There is no account with the account number {accountNumber}.");
            return;
        }

        _balance -= movement;
        _movements.Add(movement);

        File.AppendAllText(transferPath, $",{movement}");

        File.AppendAllText(_path, $",-{movement}");
    }

    private void InitializeBalance()
    {
        if (!File.Exists(_path))
        {
            File.AppendAllText(_path, $"{_balance}");
            return;
        }

        string[] movements = File.ReadAllText(_path).Split(",");

        if (movements.Length < 1)
        {
            return;
        }

        _movements.Clear();

        for (int i = 0; i < movements.Length; i++)
        {
            _movements.Add(decimal.Parse(movements[i]));
        }

        _balance = _movements.Sum();
    }
}
