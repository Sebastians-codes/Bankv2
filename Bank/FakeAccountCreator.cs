using Faker;

namespace BankStorage.Bank;

public static class FakeAccountCreator
{
    private static List<int> _usedCustomerNumbers = [];
    private static List<int> _usedPhoneNumbers = [];

    public static void CreateFakeAccountsIfNoAccountsExist(int amount)
    {
        if (AccountsExist())
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            int customerNumber = RandomUniqueCustomerNumber();
            string? firstName = Name.First();
            string? lastName = Name.Last();
            string? address = Address.StreetAddress();
            string? email = Internet.Email();
            string? phoneNumber = Phone.Number();
            int pinCode = Random.Shared.Next(1000, 10000);

            CreateFakeAccount(
                customerNumber,
                firstName,
                lastName,
                address,
                email,
                phoneNumber,
                pinCode
            );
        }
        Console.WriteLine($"Created {amount} fake accounts.");
    }

    private static void CreateFakeAccount(
        int customerNumber,
        string firstName,
        string lastName,
        string address,
        string email,
        string phoneNumber,
        int pinCode)
    {
        var cust = new Customer(
            customerNumber,
            firstName,
            lastName,
            address,
            email,
            phoneNumber,
            pinCode);

        File.AppendAllLines("Accounts/credentials.csv", [cust.ToCsv()]);
    }

    private static int RandomUniqueCustomerNumber()
    {
        do
        {
            int randomCustomerNumber = Random.Shared.Next(1000, 100000000);

            if (!_usedCustomerNumbers.Contains(randomCustomerNumber))
            {
                _usedCustomerNumbers.Add(randomCustomerNumber);
                return randomCustomerNumber;
            }

        } while (true);
    }

    private static bool AccountsExist()
    {
        if (!Directory.Exists("Accounts"))
        {
            return false;
        }

        return true;
    }
}
