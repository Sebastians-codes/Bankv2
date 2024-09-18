

using BankStorage;

Account account = new(1111, "test", "ing", 1111);
Account account1 = new(2222, "test", "ing", 2222);

account.MakeMovement(2000);

account.SendMoney(2222, 100);


Console.WriteLine("account0");
account.GetBalance();
Console.WriteLine("account1");
account1.GetBalance();