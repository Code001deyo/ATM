# ATM Management System

A C# console application that simulates ATM operations including user authentication, account management, and financial transactions.

## Features

- User registration and login
- Account balance checking
- Cash deposit and withdrawal
- PIN changing functionality
- SQL Server database integration

## Prerequisites

- .NET Framework 4.7.2
- SQL Server (LocalDB or Express)
- Visual Studio (recommended)

## Installation

1. Clone this repository:
```bash
git clone https://github.com/[your-username]/ATM-Management-System.git
```

2. Open the solution in Visual Studio

3. Set up the database:
- Create a database named "ATMDB"
- Run the following SQL script to create the Users table:
```sql
CREATE TABLE Users (
    AccountNumber INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    AccountType NVARCHAR(50) NOT NULL,
    Balance DECIMAL(18,2) NOT NULL,
    PIN NVARCHAR(4) NOT NULL
);
```

4. Update the connection string in App.config:
```xml
<connectionStrings>
    <add name="ATMDBConnection" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ATMDB;Integrated Security=True" 
         providerName="System.Data.SqlClient"/>
</connectionStrings>
```

## Usage

1. Run the application
2. Main menu options:
   - Existing User Login
   - New User Registration
   - Exit

3. After login, available transactions:
   - Deposit
   - Withdraw
   - Check Balance
   - Change PIN
   - Cancel Transaction

## Screenshots

![Main Menu](screenshots/main-menu.png)
![Transaction Menu](screenshots/transaction-menu.png)

## License

MIT License
