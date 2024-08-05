using Azure;
using Azure.Data.Tables;

namespace TableStorageDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }

        public static void Menu()
        {
            string? connectionString = Environment.GetEnvironmentVariable("FirsTequilaConnectionString");
            string tableName = "Users";

            TableClient tableClient = new TableClient(connectionString, tableName);
            Pageable<TableEntity> users = tableClient.Query<TableEntity>();

            Console.WriteLine("Welcome to the Table Storage Demo!");
            Console.WriteLine("1. Users");
            Console.WriteLine("2. Add User");
            Console.WriteLine("3. Update User");
            Console.WriteLine("4. Delete User");
            Console.WriteLine("5. Exit");
            Console.Write("Enter an option: ");
            int option = int.Parse(Console.ReadLine() ?? "");

            switch (option)
            {
                case 1:
                    ViewUsers(users);
                    break;
                case 2:
                    AddUser(tableClient);
                    break;
                case 3:
                    UpdateUser(tableClient, users.First());
                    break;
                case 4:
                    DeleteUser(tableClient, users.First());
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }

        public static void ViewUsers(Pageable<TableEntity> users)
        {
            try
            {
                Console.WriteLine("");

                foreach (TableEntity user in users)
                {
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine($"Name: {user["Name"]}");
                    Console.WriteLine($"Email: {user["Email"]}");
                    Console.WriteLine($"Partition Key: {user.PartitionKey}");
                }
                Console.WriteLine("----------------------------------------");
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press any key to return to the menu");
            Console.ReadKey();
            Menu();
        }

        public static void AddUser(TableClient tableClient)
        {
            string? name;
            string? email;
            string? department = "";
            
            string fnet = "FirstNet";
            string fdigital = "First Digital";
            string ftech = "First Technology";

            tableClient.CreateIfNotExists();

            Console.WriteLine("\nEnter name: ");
            name = Console.ReadLine();

            Console.WriteLine("\nEnter email: ");
            email = Console.ReadLine();

            Console.WriteLine("\n1. " + fnet);
            Console.WriteLine("2. " + fdigital);
            Console.WriteLine("3. " + ftech);
            Console.WriteLine("Select department: ");
            int option = int.Parse(Console.ReadLine() ?? "");

            switch (option)
            {
                case 1:
                    department = fnet;
                    break;
                case 2:
                    department = fdigital;
                    break;
                case 3:
                    department = ftech;
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }

            if (department != "")
            {
                TableEntity user = new TableEntity(department, Guid.NewGuid().ToString())
                {
                    { "Name", name },
                    { "Email", email }
                };

                tableClient.AddEntity(user);
            }

            Console.WriteLine("\nPress any key to return to the menu");
            Console.ReadKey();
            Menu();
        }

        public static void UpdateUser(TableClient tableClient, TableEntity user)
        {
            string? name;

            Console.WriteLine("\nEnter new name: ");
            name = Console.ReadLine();

            user["Name"] = name;
            tableClient.UpdateEntity(user, ETag.All);

            Console.WriteLine("\nPress any key to return to the menu");
            Console.ReadKey();
            Menu();
        }

        public static void DeleteUser(TableClient tableClient, TableEntity user)
        {
            tableClient.DeleteEntity(user.PartitionKey, user.RowKey);

            Console.Write("\nDeleting most recent user");
            Thread.Sleep(1000);
            Console.Write(" .");
            Thread.Sleep(1000);
            Console.Write(" .");
            Thread.Sleep(1000);
            Console.Write(" .");
            Thread.Sleep(1000);
            Console.WriteLine("\nUser deleted!");

            Console.WriteLine("\nPress any key to return to the menu");
            Console.ReadKey();
            Menu();
        }
    }
}