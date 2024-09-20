using BankStorage.Bank;
using BankStorage.Interaction;
using BankStorage.Interface;

Bank bank = new(new ConsoleUserInteractions(), new ConsoleUserInterface());

FakeAccountCreator.CreateFakeAccountsIfNoAccountsExist(10);

bank.Run();
