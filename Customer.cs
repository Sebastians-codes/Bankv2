namespace BankStorage;

public class Customer
{
    public int CustomerNumber { get; }
    public string? HolderFirstName { get; }
    public string? HolderLastName { get; }
    public int PinCode { get; }
    public Account CurrentAccount { get; private set; }
    private string[] _accounts;
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

    public void ChooseAccount()
    {
        for (int i = 0; i < _accounts.Length; i++)
        {
            Console.WriteLine(_accounts[i].Split(".")[0]);
        }

        int max = _accounts.Length;
        int choice;

        do
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (int.TryParse(key.KeyChar.ToString(), out choice) && choice > 0 && choice < max)
            {
                break;
            }

            Console.Clear();
            Console.WriteLine("Invalid input, try agian.");

        } while (true);

        CurrentAccount = new Account(CustomerNumber, choice);
    }

    private void InitializeDatabase()
    {
        if (!Directory.Exists(_customerPath))
        {
            Directory.CreateDirectory(_customerPath);
            return;
        }

        _accounts = Directory.GetFiles(_customerPath);
    }

    public string ToCsv() =>
        $"{CustomerNumber},{HolderFirstName},{HolderLastName},{PinCode}";

}
