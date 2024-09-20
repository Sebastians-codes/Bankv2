using BankStorage;

Bank bank = new(new ConsoleUserInteractions(), new ConsoleUserInterface());

FakeAccountCreator.CreateFakeAccountsIfNoAccountsExist(10);

bank.Run();
