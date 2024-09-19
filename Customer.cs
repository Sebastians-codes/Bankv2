using BankStorage;

namespace BankStorageg;

public class Customer
{
    public int CustomerNumber { get; }
    public string? HolderFirstName { get; }
    public string? HolderLastName { get; }
    public int PinCode { get; }
    public Account CurrentAccount { get; private set; }
    private string[] _accounts;
    private string? _path;

    public Customer(int customerNumber, string holderFirstName, string holderLastName, int pinCode)
    {
        CustomerNumber = customerNumber;
        HolderFirstName = holderFirstName;
        HolderLastName = holderLastName;
        PinCode = pinCode;
        _path = $"Accounts/{CustomerNumber}";

        InitializeDatabase();
    }

    public Account ChooseAccount()
    {
        for (int i = 0; i < _accounts.Length; i++)
        {
            Console.WriteLine(_accounts[i].Split(".")[0]);
        }
    }

    private void InitializeDatabase()
    {
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
            return;
        }

        _accounts = Directory.GetFiles(_path);
    }
}
