using System;
using System.Data.SqlClient;
using System.Configuration;

namespace ATMManagementSystem
{
    class Program
    {
        // Connection string to connect to the SQL Server database
        static string connectionString = ConfigurationManager.ConnectionStrings["ATMDBConnection"].ConnectionString;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Welcome to the ATM Management System");
                Console.WriteLine("1. Existing User Login");
                Console.WriteLine("2. New User Registration");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        ExistingUserLogin(); // Handle existing user login
                        break;
                    case 2:
                        ConfirmNewAccountCreation(); // Confirm new account creation
                        break;
                    case 3:
                        Environment.Exit(0); // Exit the application
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // Method to confirm new account creation
        static void ConfirmNewAccountCreation()
        {
            Console.WriteLine("You have selected to create a new account.");
            Console.WriteLine("1. Proceed to create a new account");
            Console.WriteLine("2. Go back to the main menu");
            Console.WriteLine("3. Exit the application");
            Console.Write("Select an option: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    NewUserRegistration(); // Proceed to new account registration
                    break;
                case 2:
                    return; // Go back to the main menu
                case 3:
                    Environment.Exit(0); // Exit the application
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        // Method for existing user login
        static void ExistingUserLogin()
        {
            Console.Write("Enter Account Number: ");
            int accountNumber = int.Parse(Console.ReadLine());
            Console.Write("Enter PIN: ");
            string pin = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Query to fetch user details based on account number
                string query = "SELECT Name, AccountType, Balance, PIN FROM Users WHERE AccountNumber = @AccountNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    string accountType = reader["AccountType"].ToString();
                    decimal balance = Convert.ToDecimal(reader["Balance"]);
                    string storedPin = reader["PIN"].ToString();

                    // Verify the entered PIN with the stored PIN
                    if (pin == storedPin)
                    {
                        Console.WriteLine($"Welcome, {name} ({accountType})");
                        Console.WriteLine($"Current Balance: Ksh.{balance:N2}"); // Changed to Ksh.
                        PerformTransaction(accountNumber); // Proceed to transactions
                    }
                    else
                    {
                        Console.WriteLine("Invalid PIN. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Account not found. Please register as a new user.");
                }
            }
        }

        // Method for new user registration
        static void NewUserRegistration()
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Account Number: ");
            int accountNumber = int.Parse(Console.ReadLine());
            Console.Write("Enter Account Type (Savings/Current): ");
            string accountType = Console.ReadLine();
            Console.Write("Enter Initial Deposit: ");
            decimal balance = decimal.Parse(Console.ReadLine());
            Console.Write("Set a 4-digit PIN: ");
            string pin = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Query to check if the account number already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE AccountNumber = @AccountNumber";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@AccountNumber", accountNumber);
                int accountExists = (int)checkCommand.ExecuteScalar();

                if (accountExists > 0)
                {
                    Console.WriteLine("An account with this number already exists. Please log in instead.");
                    return;
                }

                // Query to insert new user details into the database
                string query = "INSERT INTO Users (AccountNumber, Name, AccountType, Balance, PIN) VALUES (@AccountNumber, @Name, @AccountType, @Balance, @PIN)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@AccountType", accountType);
                command.Parameters.AddWithValue("@Balance", balance);
                command.Parameters.AddWithValue("@PIN", pin);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Registration successful. You can now log in.");
                }
                else
                {
                    Console.WriteLine("Registration failed. Please try again.");
                }
            }
        }

        // Method to perform transactions (deposit, withdraw, check balance, change PIN)
        static void PerformTransaction(int accountNumber)
        {
            while (true)
            {
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Check Balance");
                Console.WriteLine("4. Change PIN");
                Console.WriteLine("5. Cancel Transaction");
                Console.Write("Select an option: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Deposit(accountNumber); // Deposit money
                        break;
                    case 2:
                        Withdraw(accountNumber); // Withdraw money
                        break;
                    case 3:
                        CheckBalance(accountNumber); // Check balance
                        break;
                    case 4:
                        ChangePin(accountNumber); // Change PIN
                        break;
                    case 5:
                        return; // Cancel transaction and return to main menu
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // Method to deposit money into the account
        static void Deposit(int accountNumber)
        {
            Console.Write("Enter amount to deposit: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Query to update the balance after deposit
                string query = "UPDATE Users SET Balance = Balance + @Amount WHERE AccountNumber = @AccountNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Deposit successful.");
                    CheckBalance(accountNumber); // Show updated balance
                }
                else
                {
                    Console.WriteLine("Deposit failed. Please try again.");
                }
            }
        }

        // Method to withdraw money from the account
        static void Withdraw(int accountNumber)
        {
            Console.Write("Enter amount to withdraw: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Query to update the balance after withdrawal (only if sufficient balance is available)
                string query = "UPDATE Users SET Balance = Balance - @Amount WHERE AccountNumber = @AccountNumber AND Balance >= @Amount";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Withdrawal successful.");
                    CheckBalance(accountNumber); // Show updated balance
                }
                else
                {
                    Console.WriteLine("Insufficient balance or withdrawal failed.");
                }
            }
        }

        // Method to check the current balance of the account
        static void CheckBalance(int accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Query to fetch the current balance
                string query = "SELECT Balance FROM Users WHERE AccountNumber = @AccountNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                decimal balance = (decimal)command.ExecuteScalar();
                Console.WriteLine($"Available Balance: Ksh.{balance:N2}"); // Changed to Ksh.
            }
        }

        // Method to change the PIN
        static void ChangePin(int accountNumber)
        {
            Console.Write("Enter your current PIN: ");
            string currentPin = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Query to verify the current PIN
                string query = "SELECT PIN FROM Users WHERE AccountNumber = @AccountNumber AND PIN = @CurrentPIN";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                command.Parameters.AddWithValue("@CurrentPIN", currentPin);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close(); // Close the reader before executing another query
                    Console.Write("Enter your new 4-digit PIN: ");
                    string newPin = Console.ReadLine();

                    // Query to update the PIN
                    string updateQuery = "UPDATE Users SET PIN = @NewPIN WHERE AccountNumber = @AccountNumber";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@NewPIN", newPin);
                    updateCommand.Parameters.AddWithValue("@AccountNumber", accountNumber);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("PIN changed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to change PIN. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect current PIN. Please try again.");
                }
            }
        }
    }
}