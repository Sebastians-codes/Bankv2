namespace BankStorage;

public class Account
{
    public int AccountNumber { get; }
    private string _path;
    private decimal _balance;
    private List<decimal> _movements = [];
    private int _customerNumber;

    public Account(int CustomerNumber, int accountNumber)
    {
        AccountNumber = accountNumber;
        _customerNumber = CustomerNumber;
        _path = $"Accounts/{CustomerNumber}/{AccountNumber}.txt";

        InitializeBalance();
    }

    public void GetBalance() =>
        Console.WriteLine($"Your account balance is {_balance}$");

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

    public void SendMoney(int customerNumber, int accountNumber, decimal movement)
    {
        if (accountNumber == AccountNumber && customerNumber == _customerNumber)
        {
            Console.WriteLine("You can not send money to yourself...");
            return;
        }

        string transferPath = $"Accounts/{customerNumber}/{accountNumber}.txt";
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
        _movements.Add(decimal.Parse($"-{movement}"));

        File.AppendAllText(transferPath, $",{movement}");

        File.AppendAllText(_path, $",-{movement}");
    }

    public string ToCsv() =>
        $"{AccountNumber}";

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
