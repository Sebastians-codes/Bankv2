namespace BankStorage;

public class Bank
{
    private readonly string _path = "Accounts/credentials.csv";
    private List<string[]> _accounts = [];
    private Customer _currentCustomer;

    public Bank()
    {
        InitializeDatabase();
    }

    public bool MainMenu()
    {
        Console.Clear();
        ConsoleKeyInfo key;
        SwapAccountMenu();

        do
        {
            PrintMenu();
            int input;

            do
            {
                Console.Write("Your menu choice -> ");
                key = Console.ReadKey(true);

                if (int.TryParse(key.KeyChar.ToString(), out input) && input < 9 && input > 0)
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
                    PrintBalance();
                    break;
                case 2:
                    PrintMovementHistory();
                    break;
                case 3:
                    MakeDeposit();
                    break;
                case 4:
                    MakeWithdrawl();
                    break;
                case 5:
                    SendMoney();
                    break;
                case 6:
                    SwapAccountMenu();
                    break;
                case 7:
                    return true;
                case 8:
                    return false;
            }

            if (input == 6 || input == 7 || input == 8)
            {
                continue;
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
            string[] customerInfo;

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
                accountNumber = GetInt("Enter Your Customer Number -> ", "Not a valid Number try again.");
                (valid, customerInfo) = CustomerExists(accountNumber);
                if (!valid)
                {
                    Console.Clear();
                    do
                    {
                        Console.WriteLine("There was no account with that Customer Number.");
                        Console.WriteLine("Do you wanna try again or Open a new account?");
                        Console.WriteLine("Press Enter to create a new account or Esc too try again.");
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            Console.Clear();
                            _currentCustomer = CreateNewCustomer();

                            _accounts.Add(_currentCustomer.ToCsv().Split(","));
                            File.AppendAllLines(_path, [_currentCustomer.ToCsv()]);

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
                int pincode = GetPin("Enter Your pincode");

                if (pincode.ToString() == customerInfo[3])
                {
                    Console.Clear();
                    _currentCustomer = new Customer(
                        int.Parse(customerInfo[0]),
                        customerInfo[1],
                        customerInfo[2],
                        int.Parse(customerInfo[3]));
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

    private void SwapAccountMenu()
    {
        var availableAccounts = _currentCustomer.GetAccounts();

        if (availableAccounts.Length == 0)
        {
            _currentCustomer.CreateNewAccount(_currentCustomer.CustomerNumber, 1);
            return;
        }

        ConsoleKeyInfo key;
        do
        {
            int choice;

            do
            {
                foreach (var account in _currentCustomer.GetAccounts())
                {
                    Console.WriteLine(account);
                }

                Console.Write("Enter the Id of the account you want to log in too.\n" +
                "Or Press n too create a new account.");
                key = Console.ReadKey(true);

                if (int.TryParse(key.KeyChar.ToString(), out choice) &&
                    choice > 0 && choice < availableAccounts.Length + 1)
                {
                    break;
                }
                else if (key.KeyChar == 'n')
                {
                    if (availableAccounts.Length < 10)
                    {
                        _currentCustomer.CreateNewAccount(_currentCustomer.CustomerNumber, availableAccounts.Length + 1);
                        Console.Clear();
                        return;
                    }
                }

            } while (true);

            _currentCustomer.GetAccount(_currentCustomer.CustomerNumber, choice);
            Console.Clear();
            return;

        } while (true);
    }

    private void SendMoney()
    {
        int customerToSend, accountToSend;
        decimal movement;
        do
        {
            customerToSend = GetInt(
                            "Enter the costumer number you want to send money too -> ",
                            "Invalid input, try again.", 1000, 10000);

            (bool valid, _) = CustomerExists(customerToSend);

            if (!valid)
            {
                Console.Clear();
                Console.WriteLine("There is no account with that account number.");
                return;
            }

            accountToSend = GetInt(
                "Enter the account number you want to send money too -> ",
                "Invalid input, try again.", 0, 10000);

            if (!File.Exists($"Accounts/{customerToSend}/{accountToSend}.txt"))
            {
                Console.Clear();
                Console.WriteLine($"Customer {customerToSend} does not have an account with number {accountToSend}.");
                return;
            }
            else if (_currentCustomer.CustomerNumber == customerToSend &&
                accountToSend == _currentCustomer.CurrentAccount.AccountNumber)
            {
                Console.Clear();
                Console.Write("You can not send money to the same account your logged in too.");
                return;
            }

            movement = GetDecimal(
                     "Enter the amount you want too send -> ",
                     "Invalid input, try again.",
                     0m,
                     decimal.MaxValue);

            if (movement > _currentCustomer.CurrentAccount.Balance)
            {
                Console.Clear();
                Console.WriteLine($"You dont have enough money to send {movement}$.");
                Console.WriteLine($"Your current balance is: {_currentCustomer.CurrentAccount.Balance}$.");
                continue;
            }

            break;
        } while (true);

        _currentCustomer.CurrentAccount.SendMoney(customerToSend, accountToSend, movement);
    }

    private void MakeWithdrawl()
    {
        do
        {
            var movement = GetDecimal(
                            "Enter the amount you want too withdraw -> ",
                            "Invalid input, try again.",
                            0m,
                            decimal.MaxValue);

            if (movement < _currentCustomer.CurrentAccount.Balance)
            {
                _currentCustomer.CurrentAccount.Withdraw(movement);
                Console.WriteLine($"You withdrew {movement}$.");
                return;
            }
            Console.Clear();
            Console.WriteLine("You do not have enough money to withdraw that amount");

        } while (true);
    }

    private void MakeDeposit()
    {
        decimal movement = GetDecimal(
                        "Enter the amount you want too deposit -> ",
                        "Invalid input, try again.",
                        0m,
                        decimal.MaxValue);

        _currentCustomer.CurrentAccount.Deposit(movement);
    }

    private void PrintMovementHistory()
    {
        string[] movements = _currentCustomer.GetMovementHistory();
        foreach (string movement in movements)
        {
            Console.WriteLine(movement);
        }
    }

    private void PrintBalance()
    {
        decimal balance = _currentCustomer.CurrentAccount.Balance;
        Console.WriteLine($"Your Current Balance is {balance}$.");
    }

    private void PrintMenu()
    {
        Console.WriteLine(@"1 -> ShowBalance
2 -> ShowHistory
3 -> Deposit
4 -> Withdraw
5 -> Transfer
6 -> Swap Account
7 -> Logout
8 -> Quit");
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
            string? firstName = GetName("Enter your firstname -> ", "Invalid input, try again.");
            string? lastName = GetName("Enter your lastname -> ", "Invalid input, try again.");
            Console.Clear();

            int pinCode, secondAttempt;
            do
            {
                pinCode = GetPin("Enter a pincode for your account");
                do
                {
                    Console.Clear();
                    secondAttempt = GetPin("Enter the code again to confirm");

                    break;

                } while (true);

                if (pinCode == secondAttempt)
                {
                    break;
                }

                Console.Clear();
                Console.WriteLine("codes did not match try again.");

            } while (true);

            Customer newCustomer = new(randomAccountNumber, firstName, lastName, pinCode);
            File.AppendAllLines(_path, [newCustomer.ToCsv()]);

            return newCustomer;

        } while (true);
    }

    private (bool valid, string[] accountInfo) CustomerExists(int customerNumber)
    {
        foreach (var account in _accounts)
        {
            if (customerNumber.ToString() == account[0])
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

    private int GetInt(string message, string errorMesage,
        int min = int.MinValue, int max = int.MaxValue)
    {
        do
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(),
                out int num) && num > min && num < max)
            {
                return num;
            }
            Console.Clear();
            Console.WriteLine(errorMesage);
        } while (true);
    }

    private decimal GetDecimal(string message, string errorMesage,
        decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
    {
        do
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(),
                out int num) && num > min && num < max)
            {
                return num;
            }

            Console.Clear();
            Console.WriteLine(errorMesage);

        } while (true);
    }

    private string GetName(string message, string errorMesage)
    {
        do
        {
            Console.Clear();
            Console.Write(message);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && input.Trim().Length > 1)
            {
                string name = "";

                foreach (var str in input)
                {
                    if (str != ' ')
                    {
                        name += str;
                    }
                }

                return $"{char.ToUpperInvariant(name[0])}{name.ToLowerInvariant()[1..]}";
            }

        } while (true);
    }

    private int GetPin(string message, int minLength = 4, int maxLength = 8)
    {
        ConsoleKeyInfo key;
        List<char> chars = [];

        do
        {
            Console.Write($"{message}, between {minLength} and {maxLength} long. -> ");

            if (chars.Count() > 0)
            {
                foreach (char chr in chars)
                {
                    Console.Write("*");
                }
            }

            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter && chars.Count > minLength - 1)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && chars.Count > 0)
            {
                chars.RemoveAt(chars.LastIndexOf(chars.Last()));
                Console.Clear();
                continue;
            }

            if (int.TryParse(key.KeyChar.ToString(), out int num) && num < 10 && num > 0 && chars.Count < maxLength)
            {
                chars.Add(key.KeyChar);
                Console.Clear();
                continue;
            }
            else if (chars.Count == maxLength)
            {
                Console.Clear();
                Console.WriteLine("You have reached the maximum amount of numbers.");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Not a valid integer try again.");
            }

        } while (true);

        return int.Parse(string.Join("", chars));
    }
}