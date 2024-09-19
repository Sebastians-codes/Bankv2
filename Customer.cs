namespace BankStorage;

public class Customer
{
    public int CustomerNumber { get; }
    public string? HolderFirstName { get; }
    public string? HolderLastName { get; }
    public int PinCode { get; }
    public Account CurrentAccount { get; private set; }
    private List<string> _accounts;
    private string? _customerPath;

    public Customer(int customerNumber, string holderFirstName, string holderLastName, int pinCode)
    {
        CustomerNumber = customerNumber;
        HolderFirstName = holderFirstName;
        HolderLastName = holderLastName;
        PinCode = pinCode;
        _customerPath = $"Accounts/{CustomerNumber}";

        InitializeDatabase();
    }

    public bool GetAccount(int customerNumber, int accountNumber)
    {
        var accounts = GetAccounts();

        foreach (var account in accounts)
        {
            if (account == accountNumber.ToString())
            {
                CurrentAccount = new Account(customerNumber, accountNumber);

                return true;
            }
        }

        return false;
    }

    public bool CreateNewAccount(int customerNumber, int accountNumber)
    {
        var accounts = GetAccounts();

        foreach (var account in accounts)
        {
            if (account == accountNumber.ToString())
            {
                return false;
            }
        }

        CurrentAccount = new Account(customerNumber, accountNumber);

        _accounts.Add($"{accountNumber}");

        return true;
    }

    public string[] GetMovementHistory()
    {
        var strs = new string[CurrentAccount.Movements.Count + 1];
        decimal sum = 0;

        strs[0] = $"Your current balance is {CurrentAccount.Balance}$";

        for (int i = 1; i < CurrentAccount.Movements.Count; i++)
        {
            decimal currentMovement = CurrentAccount.Movements[i];
            sum += currentMovement;
            strs[i] = $"{(currentMovement < 0 ? "Withdrew " : "Deposited")} {Math.Abs(currentMovement)}$      Balance:{sum}$";
        }

        strs = strs.Reverse().ToArray();

        return strs;
    }

    private string[] GetAccounts()
    {
        string[] accounts = new string[_accounts.Count];

        for (int i = 0; i < _accounts.Count; i++)
        {
            accounts[i] = $"Account {_accounts[i].Split(".")[0].Split("/")[^1]}";
        }

        return accounts;
    }

    private void InitializeDatabase()
    {
        if (!Directory.Exists(_customerPath))
        {
            Directory.CreateDirectory(_customerPath);
            return;
        }

        _accounts = Directory.GetFiles(_customerPath).ToList();
    }

    public string ToCsv() =>
        $"{CustomerNumber},{HolderFirstName},{HolderLastName},{PinCode}";

}
