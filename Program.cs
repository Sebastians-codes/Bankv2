using BankStorage;

Bank bank = new(new ConsoleUserInputs(), new ConsoleUserInterface());

FakeAccountCreator.CreateFakeAccountsIfNoAccountsExist(10);

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
