namespace BankStorage;

public class Account
{
    public int AccountNumber { get; }
    public string? HolderFirstName { get; }
    public string? HolderLastName { get; }
    public int PinCode { get; }
    private string _path;
    private decimal _balance;
    private List<decimal> _movements = [];

    public Account(int accountNumber, string holderFirstName, string holderLastName, int pinCode)
    {
        AccountNumber = accountNumber;
        HolderFirstName = holderFirstName;
        HolderLastName = holderLastName;
        PinCode = pinCode;
        _path = $"AccountBalances/{AccountNumber}.txt";

        InitializeBalance();
    }

    public void GetBalance()
    {
        InitializeBalance();
        Console.WriteLine($"Your account balance is {_balance}$");
    }

    public void ShowMovementHistory()
    {
        var strs = new string[_movements.Count];
        decimal sum = 0;

        for (int i = 0; i < _movements.Count; i++)
        {
            sum += _movements[i];
            strs[i] = $"{(_movements[i] < 0 ? "Withdrew " : "Deposited")} {Math.Abs(_movements[i])}$      Balance:{sum}$";
        }

        strs = strs.Reverse().ToArray();

        GetBalance();

        foreach (var str in strs)
        {
            Console.WriteLine(str);
        }
    }

    public void MakeMovement(decimal movement, bool deposit)
    {
        if (_balance < movement && !deposit)
        {
            Console.WriteLine("Not enough money too withdrawl that amount");
            return;
        }

        if (!deposit)
        {
            Console.WriteLine($"You withdrew {movement}$.");
            _balance -= movement;
        }
        else
        {
            Console.WriteLine($"You deposited {movement}$.");
            _balance += movement;
        }

        _movements.Add((deposit ? movement : decimal.Parse($"-{movement}")));

        File.AppendAllText(_path, (deposit ? $",{movement}" : $",-{movement}"));
    }

    public void SendMoney(int accountNumber, decimal movement)
    {
        if (accountNumber == AccountNumber)
        {
            Console.WriteLine("You can not send money to yourself...");
            return;
        }

        string transferPath = $"AccountBalances/{accountNumber}.txt";
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
