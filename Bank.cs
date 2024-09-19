namespace BankStorage;

public class Bank
{
    private readonly string _path = "accounts.csv";
    private readonly UserInputs _inputs;
    private List<string[]> _accounts = [];

    public Bank(UserInputs inputs)
    {
        _inputs = inputs;
        InitializeDatabase();
    }

    public Account LoginMenu()
    {
        do
        {
            int accountNumber, tries = 3;
            bool valid;
            string[] accountInfo;

            do
            {
                Console.Clear();
                accountNumber = _inputs.GetInt("Enter Your Account Number -> ", "Not a valid Number try again.");
                (valid, accountInfo) = AccountExists(accountNumber);
                if (!valid)
                {
                    do
                    {
                        Console.WriteLine("There was no account with that accountNumber.");
                        Console.WriteLine("Do you wanna try again or Open a new account?");
                        Console.WriteLine("Press Enter to create a new account or Esc too try again.");
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            Console.Clear();
                            return CreateNewAccount();
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            break;
                        }
                        Console.Clear();
                        Console.WriteLine("Invalid input try again.");

                    } while (true || !valid);
                }
                break;
            } while (true);

            if (!valid)
            {
                continue;
            }

            do
            {
                int pincode = _inputs.GetPin("Enter Your pincode -> ");

                if (pincode.ToString() == accountInfo[3])
                {
                    return new Account(
                        int.Parse(accountInfo[0]),
                        accountInfo[1],
                        accountInfo[2],
                        int.Parse(accountInfo[3]));
                }

                if (tries == 0)
                {
                    break;
                }

                Console.Clear();
                Console.WriteLine($"Invalid pincode. You have {--tries} left.");

            } while (true);

        } while (true);
    }

    private Account CreateNewAccount()
    {
        do
        {
            int randomAccountNumber = Random.Shared.Next(0000, 100000000);
            foreach (var account in _accounts)
            {
                if (account[0] == randomAccountNumber.ToString())
                {
                    continue;
                }
                break;
            }
        } while (true);
    }

    private (bool valid, string[] accountInfo) AccountExists(int accountNumber)
    {
        foreach (var account in _accounts)
        {
            if (accountNumber.ToString() == account[0])
            {
                return (true, account);
            }
        }
        return (false, []);
    }

    private void InitializeDatabase()
    {
        if (!File.Exists(_path))
        {
            File.Create(_path);
            return;
        }

        string[] contents = File.ReadAllLines(_path);

        if (contents.Length == 0)
        {
            return;
        }

        foreach (var account in contents)
        {
            _accounts.Add(account.Split(","));
        }
    }
}
