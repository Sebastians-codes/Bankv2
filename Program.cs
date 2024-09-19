using BankStorage;

Bank bank = new();

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