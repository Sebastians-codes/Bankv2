namespace BankStorage;

public class Bank
{
    private readonly string _path = "Accounts/credentials.csv";
    private readonly UserInputs _inputs;
    private List<string[]> _accounts = [];

    public Bank(UserInputs inputs)
    {
        _inputs = inputs;
        InitializeDatabase();
    }

    public Customer LoginMenu()
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
                    Console.Clear();
                    do
                    {
                        Console.WriteLine("There was no account with that accountNumber.");
                        Console.WriteLine("Do you wanna try again or Open a new account?");
                        Console.WriteLine("Press Enter to create a new account or Esc too try again.");
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            Console.Clear();
                            return CreateNewCustomer();
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

            Console.Clear();
            do
            {
                int pincode = _inputs.GetPin("Enter Your pincode");

                if (pincode.ToString() == accountInfo[3])
                {
                    Console.Clear();
                    return new Customer(
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

    private Customer CreateNewCustomer()
    {
        do
        {
            int randomAccountNumber = Random.Shared.Next(1000, 10000);
            foreach (var account in _accounts)
            {
                if (account[0] == randomAccountNumber.ToString())
                {
                    continue;
                }
                break;
            }

            Console.Clear();
            Console.WriteLine($"Your account number is {randomAccountNumber} Write it down you will not see it again.");
            Console.WriteLine("Press Enter too continue or Esc to generate another account number.");
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Enter || key.Key == ConsoleKey.Escape)
            {
                continue;
            }

            Console.Clear();
            string? firstName = _inputs.GetName("Enter your firstname -> ", "Invalid input, try again.");
            string? lastName = _inputs.GetName("Enter your lastname -> ", "Invalid input, try again.");
            Console.Clear();

            int pinCode, secondAttempt;
            do
            {
                pinCode = _inputs.GetPin("Enter a pincode for your account -> ");
                secondAttempt = _inputs.GetPin("Enter the code again to confirm -> ");

                if (pinCode == secondAttempt)
                {
                    break;
                }
                Console.Clear();
                Console.WriteLine("codes did not match try again.");

            } while (true);

            Customer Customer = new(randomAccountNumber, firstName, lastName, pinCode);
            File.AppendAllLines(_path, [Customer.ToCsv()]);

            return Customer;

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
