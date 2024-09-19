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

    public void GetAccount(int customerNumber, int accountNumber) =>
        CurrentAccount = new Account(customerNumber, accountNumber);


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


        for (int i = 0; i < CurrentAccount.Movements.Count; i++)
        {
            decimal currentMovement = CurrentAccount.Movements[i];
            sum += currentMovement;
            strs[i] = $"{(currentMovement < 0 ? "Withdrew " : "Deposited")} {Math.Abs(currentMovement)}$      Balance:{sum}$";
        }
        strs[^1] = $"Your current balance is {CurrentAccount.Balance}$";

        strs = strs.Reverse().ToArray();

        return strs;
    }

    public string[] GetAccounts()
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
