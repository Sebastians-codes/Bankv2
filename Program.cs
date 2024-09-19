using BankStorage;

Bank bank = new(new UserInputs());

do
{
    if (!bank.LoginMenu())
    {
        break;
    }
    if (!bank.MainMenu())
    {
        break;
    }
} while (true);