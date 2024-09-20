using BankStorage.Interaction;
using BankStorage.Interface;

namespace BankStorage.Bank;

public class Bank
{
    private readonly IUserInteractions _userInteractions;
    private readonly IUserInterface _userInterface;
    private readonly string _path = "Accounts/credentials.csv";
    private List<string[]> _accounts = [];
    private Customer _currentCustomer;

    public Bank(IUserInteractions inputs, IUserInterface userInterface)
    {
        _userInteractions = inputs;
        _userInterface = userInterface;
        InitializeDatabase();
    }

    public void Run()
    {
        do
        {
            if (!LoginMenu())
            {
                break;
            }
            if (!MainMenu())
            {
                break;
            }
        } while (true);
    }

    private bool MainMenu()
    {
        _userInterface.Clear();
        char key;
        SwapAccountMenu();

        do
        {
            PrintMenu();
            int input;

            do
            {
                _userInterface.Write("Your menu choice -> ");
                key = _userInteractions.ReadKey();

                if (int.TryParse(key.ToString(), out input) && input < 10 && input > 0)
                {
                    break;
                }

                _userInterface.Clear();
                _userInterface.WriteLine("Invalid input, try again.");
                PrintMenu();

            } while (true);

            _userInterface.Clear();
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
                    CustomerInfo();
                    break;
                case 7:
                    SwapAccountMenu();
                    break;
                case 8:
                    return true;
                case 9:
                    return false;
            }

            if (input == 7 || input == 8 || input == 9)
            {
                continue;
            }

            do
            {
                _userInterface.WriteLine("\nPress B to go back to main menu.");
                key = _userInteractions.ReadKey();

                if (char.ToLower(key) == 'b')
                {
                    break;
                }

                _userInterface.Clear();
                _userInterface.WriteLine("Invalid input.");
            } while (true);

            _userInterface.Clear();

        } while (true);
    }

    private bool LoginMenu()
    {
        do
        {
            int accountNumber, tries = 3;
            bool valid;
            string[] customerInfo;

            do
            {
                _userInterface.WriteLine("Press S to Start Login Q Too Quit");
                char key = _userInteractions.ReadKey();

                if (char.ToLower(key) == 's')
                {
                    break;
                }
                else if (char.ToLower(key) == 'q')
                {
                    return false;
                }

                _userInterface.Clear();
                _userInterface.WriteLine("Invalid input, try again.");

            } while (true);

            do
            {
                _userInterface.Clear();
                accountNumber = GetInt("Enter Your Customer Number -> ", "Not a valid Number try again.");
                (valid, customerInfo) = CustomerExists(accountNumber);
                if (!valid)
                {
                    _userInterface.Clear();
                    do
                    {
                        _userInterface.WriteLine("There was no account with that Customer Number.");
                        _userInterface.WriteLine("Do you wanna try again or Open a new account?");
                        _userInterface.WriteLine("Press N to create a new account or Q too try again.");
                        char key = _userInteractions.ReadKey();

                        if (char.ToLower(key) == 'n')
                        {
                            _userInterface.Clear();
                            _currentCustomer = CreateNewCustomer();

                            _accounts.Add(_currentCustomer.ToCsv().Split(","));
                            return true;
                        }
                        else if (char.ToLower(key) == 'q')
                        {
                            break;
                        }

                        _userInterface.Clear();
                        _userInterface.WriteLine("Invalid input try again.");

                    } while (true || !valid);
                }

                break;

            } while (true);

            if (!valid)
            {
                continue;
            }

            _userInterface.Clear();

            do
            {
                int pincode = GetPin("Enter Your pincode");

                if (pincode.ToString() == customerInfo[6])
                {
                    _userInterface.Clear();
                    _currentCustomer = new Customer(
                        int.Parse(customerInfo[0]),
                        customerInfo[1],
                        customerInfo[2],
                        customerInfo[3],
                        customerInfo[4],
                        customerInfo[5],
                        int.Parse(customerInfo[6]));
                    return true;
                }

                if (tries == 0)
                {
                    break;
                }

                _userInterface.Clear();
                _userInterface.WriteLine($"Invalid pincode. You have {--tries} left.");

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

        char key;
        do
        {
            int choice;

            do
            {
                foreach (var account in _currentCustomer.GetAccounts())
                {
                    _userInterface.WriteLine(account);
                }

                _userInterface.Write("Enter the Id of the account you want to log in too.\n" +
                "Or Press n too create a new account.");
                key = _userInteractions.ReadKey();

                if (int.TryParse(key.ToString(), out choice) &&
                    choice > 0 && choice < availableAccounts.Length + 1)
                {
                    break;
                }
                else if (key == 'n')
                {
                    if (availableAccounts.Length < 10)
                    {
                        _currentCustomer.CreateNewAccount(_currentCustomer.CustomerNumber, availableAccounts.Length + 1);
                        _userInterface.Clear();
                        return;
                    }
                }

            } while (true);

            _currentCustomer.GetAccount(_currentCustomer.CustomerNumber, choice);
            _userInterface.Clear();
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
                _userInterface.Clear();
                _userInterface.WriteLine("There is no account with that account number.");
                return;
            }

            accountToSend = GetInt(
                "Enter the account number you want to send money too -> ",
                "Invalid input, try again.", 0, 10000);

            if (!File.Exists($"Accounts/{customerToSend}/{accountToSend}.txt"))
            {
                _userInterface.Clear();
                _userInterface.WriteLine($"Customer {customerToSend} does not have an account with number {accountToSend}.");
                return;
            }
            else if (_currentCustomer.CustomerNumber == customerToSend &&
                accountToSend == _currentCustomer.CurrentAccount.AccountNumber)
            {
                _userInterface.Clear();
                _userInterface.Write("You can not send money to the same account your logged in too.");
                return;
            }

            movement = GetDecimal(
                     "Enter the amount you want too send -> ",
                     "Invalid input, try again.",
                     0m,
                     decimal.MaxValue);

            if (movement > _currentCustomer.CurrentAccount.Balance)
            {
                _userInterface.Clear();
                _userInterface.WriteLine($"You dont have enough money to send {movement}$.");
                _userInterface.WriteLine($"Your current balance is: {_currentCustomer.CurrentAccount.Balance}$.");
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
                _userInterface.WriteLine($"You withdrew {movement}$.");
                return;
            }
            _userInterface.Clear();
            _userInterface.WriteLine("You do not have enough money to withdraw that amount");

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
            _userInterface.WriteLine(movement);
        }
    }

    private void PrintBalance()
    {
        decimal balance = _currentCustomer.CurrentAccount.Balance;
        _userInterface.WriteLine($"Your Current Balance is {balance}$.");
    }

    private void CustomerInfo() =>
        _userInterface.WriteLine(_currentCustomer.GetCustomerInfo());

    private void PrintMenu()
    {
        _userInterface.WriteLine(@"1 -> ShowBalance
2 -> ShowHistory
3 -> Deposit
4 -> Withdraw
5 -> Transfer
6 -> Account Info
7 -> Swap Account
8 -> Logout
9 -> Quit");
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

            _userInterface.Clear();
            _userInterface.WriteLine($"Your account number is {randomAccountNumber} Write it down you will not see it again.");
            _userInterface.WriteLine("Press N too continue or R to generate another account number.");
            char key = _userInteractions.ReadKey();

            if (char.ToLower(key) != 'n' || char.ToLower(key) == 'r')
            {
                continue;
            }

            _userInterface.Clear();
            string? firstName = GetString("Enter your firstname -> ", "Invalid input, try again.");
            string? lastName = GetString("Enter your lastname -> ", "Invalid input, try again.");
            string? address = GetString("Enter your address -> ", "Invalid input, try again.");
            string? email = GetString("Enter your email -> ", "Invalid input, try again.");
            string? phoneNumber = GetInt(
                "Enter your phone number -> ",
                "Invalid Phone number, try again.",
                0699999999,
                0800000000).ToString();
            _userInterface.Clear();

            int pinCode, secondAttempt;
            do
            {
                pinCode = GetPin("Enter a pincode for your account");
                do
                {
                    _userInterface.Clear();
                    secondAttempt = GetPin("Enter the code again to confirm");

                    break;

                } while (true);

                if (pinCode == secondAttempt)
                {
                    break;
                }

                _userInterface.Clear();
                _userInterface.WriteLine("codes did not match try again.");

            } while (true);

            Customer newCustomer = new(
                randomAccountNumber,
                firstName,
                lastName,
                address,
                email,
                phoneNumber,
                pinCode);

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
            _userInterface.Write(message);
            if (int.TryParse(_userInteractions.ReadLine(),
                out int num) && num > min && num < max)
            {
                return num;
            }
            _userInterface.Clear();
            _userInterface.WriteLine(errorMesage);
        } while (true);
    }

    private decimal GetDecimal(string message, string errorMesage,
        decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
    {
        do
        {
            _userInterface.Write(message);

            if (int.TryParse(_userInteractions.ReadLine(),
                out int num) && num > min && num < max)
            {
                return num;
            }

            _userInterface.Clear();
            _userInterface.WriteLine(errorMesage);

        } while (true);
    }

    private string GetString(string message, string errorMesage)
    {
        do
        {
            _userInterface.Clear();
            _userInterface.Write(message);
            string? input = _userInteractions.ReadLine();

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
        char key;
        List<char> chars = [];

        do
        {
            _userInterface.WriteLine("dash \"-\" to erase.");
            _userInterface.Write($"{message}, between {minLength} and {maxLength} long. -> ");

            if (chars.Count() > 0)
            {
                foreach (char chr in chars)
                {
                    _userInterface.Write("*");
                }
            }

            key = _userInteractions.ReadKey();

            if (key == '-' && chars.Count > 0)
            {
                chars.RemoveAt(chars.LastIndexOf(chars.Last()));
                _userInterface.Clear();
                continue;
            }
            else if (!char.IsAsciiDigit(key) && chars.Count > minLength - 1 && key != '-')
            {
                break;
            }

            if (int.TryParse(key.ToString(), out int num) && num < 10 && num > 0 && chars.Count < maxLength)
            {
                chars.Add(key);
                _userInterface.Clear();
                continue;
            }
            else if (chars.Count == maxLength)
            {
                _userInterface.Clear();
                _userInterface.WriteLine("You have reached the maximum amount of numbers.");
            }
            else
            {
                _userInterface.Clear();
                _userInterface.WriteLine("Not a valid integer try again.");
            }

        } while (true);

        return int.Parse(string.Join("", chars));
    }
}