namespace BankStorage;

public class Bank
{
    private readonly string _path = "accounts.csv";
    private readonly UserInputs _inputs;
    private List<string[]> _accounts = [];

    private Account _currentAccount;

    public Bank(UserInputs inputs)
    {
        _inputs = inputs;
        InitializeDatabase();
    }

    public bool MainMenu()
    {
        Console.Clear();
        ConsoleKeyInfo key;
        decimal movement;

        do
        {
            PrintMenu();
            int input;

            do
            {
                Console.Write("Your menu choice -> ");
                key = Console.ReadKey(true);

                if (int.TryParse(key.KeyChar.ToString(), out input) && input < 8 && input > 0)
                {
                    break;
                }

                Console.Clear();
                Console.WriteLine("Invalid input, try again.");
                PrintMenu();

            } while (true);

            Console.Clear();
            switch (input)
            {
                case 1:
                    _currentAccount.GetBalance();
                    break;
                case 2:
                    _currentAccount.ShowMovementHistory();
                    break;
                case 3:
                    movement = _inputs.GetDecimal(
                        "Enter the amount you want too deposit -> ",
                        "Invalid input, try again.",
                        0m,
                        decimal.MaxValue);
                    _currentAccount.MakeMovement(movement, true);
                    break;
                case 4:
                    movement = _inputs.GetDecimal(
                        "Enter the amount you want too withdraw -> ",
                        "Invalid input, try again.",
                        0m,
                        decimal.MaxValue);
                    _currentAccount.MakeMovement(movement, false);
                    break;
                case 5:
                    movement = _inputs.GetDecimal(
                            "Enter the amount you want too send -> ",
                            "Invalid input, try again.",
                            0m,
                            decimal.MaxValue);
                    int accountToSend = _inputs.GetInt(
                        "Enter the account number you want to send money too -> ",
                        "Invalid input, try again.", 1000, 10000);
                    _currentAccount.SendMoney(accountToSend, movement);
                    break;
                case 6:
                    return true;
                case 7:
                    return false;
            }

            do
            {
                Console.WriteLine("\nPress Enter to go back to main menu.");
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                Console.Clear();
                Console.WriteLine("Invalid input.");
            } while (true);

            Console.Clear();

        } while (true);
    }

    public bool LoginMenu()
    {
        do
        {
            int accountNumber, tries = 3;
            bool valid;
            string[] accountInfo;

            do
            {
                Console.WriteLine("Press Enter to Start Login Esc Too Quit");
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return false;
                }

                Console.Clear();
                Console.WriteLine("Invalid input, try again.");

            } while (true);

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
                            _currentAccount = CreateNewAccount();
                            return true;
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
                    _currentAccount = new Account(
                        int.Parse(accountInfo[0]),
                        accountInfo[1],
                        accountInfo[2],
                        int.Parse(accountInfo[3]));
                    return true;
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

    private void PrintMenu()
    {
        Console.WriteLine(@"1 -> ShowBalance
2 -> ShowHistory
3 -> Deposit
4 -> Withdraw
5 -> Transfer
6 -> Logout
7 -> Quit");
    }

    private Account CreateNewAccount()
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
                Console.Clear();
                secondAttempt = _inputs.GetPin("Enter the code again to confirm -> ");

                if (pinCode == secondAttempt)
                {
                    break;
                }
                Console.Clear();
                Console.WriteLine("codes did not match try again.");

            } while (true);

            Account newAccount = new(randomAccountNumber, firstName, lastName, pinCode);
            File.AppendAllLines(_path, [newAccount.ToCsv()]);

            return newAccount;

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
