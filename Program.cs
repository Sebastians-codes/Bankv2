using BankStorage.Bank;
using BankStorage.Interaction;
using BankStorage.Interface;

FakeAccountCreator.CreateFakeAccountsIfNoAccountsExist(10);

Bank bank = new(new ConsoleUserInteractions(), new ConsoleUserInterface());

bank.Run();