

using BankStorage;

Account account = new(1111, "test", "ing", 1111);
Account account1 = new(2222, "test", "ing", 2222);


account.SendMoney(1111, 40);

account1.GetBalance();