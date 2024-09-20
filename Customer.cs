namespace BankStorage;

public class Customer
{
    public int CustomerNumber { get; }
    public string? HolderFirstName { get; }
    public string? HolderLastName { get; }
    public string? Address { get; }
    public string? Email { get; }
    public string? PhoneNumber { get; }
    public int PinCode { get; }
    public Account CurrentAccount { get; private set; }
    private List<string> _accounts;
    private string? _customerPath;

    public Customer(
        int customerNumber,
        string holderFirstName,
        string holderLastName,
        string address,
        string email,
        string phoneNumber,
        int pinCode)
    {
        CustomerNumber = customerNumber;
        HolderFirstName = holderFirstName;
        HolderLastName = holderLastName;
        Address = address;
        Email = email;
        PhoneNumber = phoneNumber;
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

    public string GetCustomerInfo() =>
        @$"Name: {HolderFirstName} {HolderLastName}
Address: {Address}

Contact Info
Email: {Email}
PhoneNumber {PhoneNumber}";

    public string ToCsv() =>
        $"{CustomerNumber},{HolderFirstName},{HolderLastName},{Address},{Email},{PhoneNumber},{PinCode}";

    private void InitializeDatabase()
    {
        if (!Directory.Exists(_customerPath))
        {
            Directory.CreateDirectory(_customerPath);
            _accounts = [];
            return;
        }

        _accounts = Directory.GetFiles(_customerPath).ToList();
    }


}
