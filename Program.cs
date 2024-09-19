using BankStorage;

Bank bank = new(new UserInputs());

Account account = bank.LoginMenu();

account.MakeMovement(2000, true);

account.ShowMovementHistory();