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

    public bool ChooseAccount()
    {
        InitializeDatabase();


        if (_accounts.Length == 0)
        {
            do
            {
                Console.WriteLine("You do not have any accounts do you want to create one? y/n");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (char.ToLowerInvariant(key.KeyChar) == 'y')
                {
                    CurrentAccount = new Account(CustomerNumber, 1);
                    Console.Clear();
                    return true;
                }
                if (char.ToLowerInvariant(key.KeyChar) == 'n')
                {
                    Console.Clear();
                    return false;
                }
                Console.Clear();
                Console.WriteLine("Invalid input, try again.");

            } while (true);
        }

        Console.WriteLine("select the account you want too use.");
        Console.WriteLine("to make new one enter a number that you currently do not have. max is 10 accounts.");
        int max = _accounts.Length + 1;
        int choice;

        do
        {
            PrintAccounts();
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (int.TryParse(key.KeyChar.ToString(), out choice) && choice > 0 && choice < 10)
            {
                break;
            }

            Console.Clear();
            Console.WriteLine("Invalid input, try agian.");

        } while (true);
        Console.Clear();
        CurrentAccount = new Account(CustomerNumber, choice);
        return true;
    }

    public string[] ShowMovementHistory()
    {
        var strs = new string[CurrentAccount.Movements.Count];
        decimal sum = 0;

        for (int i = 0; i < CurrentAccount.Movements.Count; i++)
        {
            decimal currentMovement = CurrentAccount.Movements[i];
            sum += currentMovement;
            strs[i] = $"{(currentMovement < 0 ? "Withdrew " : "Deposited")} {Math.Abs(currentMovement)}$      Balance:{sum}$";
        }

        strs = strs.Reverse().ToArray();

        GetBalance();

        return strs;
    }

    private void PrintAccounts()
    {
        for (int i = 0; i < _accounts.Length; i++)
        {
            Console.WriteLine($"Account {_accounts[i].Split(".")[0].Split("/")[^1]}");
        }
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
