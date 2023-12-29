using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using SSD_Assignment___Banking_Application;

namespace Banking_Application
{
    public class Data_Access_Layer
    {

        private List<Bank_Account> accounts;
        List<string> accountNumbers = new List<string>(); // List to store account numbers

        public static String databaseName = "Banking Database.db";
        private static Data_Access_Layer instance = new Data_Access_Layer();
        Encryption_Handler encryption_handler = new Encryption_Handler();

        private Data_Access_Layer()//Singleton Design Pattern (For Concurrency Control) - Use getInstance() Method Instead.
        {
            accounts = new List<Bank_Account>();
        }

        public static Data_Access_Layer getInstance()
        {
            return instance;
        }

        private SqliteConnection getDatabaseConnection()
        {

            String databaseConnectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = Data_Access_Layer.databaseName,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            return new SqliteConnection(databaseConnectionString);

        }

        private void initialiseDatabase()
        {
            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
              @"
                    CREATE TABLE IF NOT EXISTS Bank_Accounts(    
                        accountNo TEXT PRIMARY KEY,
                        name TEXT NOT NULL,
                        address_line_1 TEXT,
                        address_line_2 TEXT,
                        address_line_3 TEXT,
                        town TEXT NOT NULL,
                        balance REAL NOT NULL,
                        accountType INTEGER NOT NULL,
                        overdraftAmount REAL,
                        interestRate REAL
                    );

                    CREATE TABLE IF NOT EXISTS Hash_Table(
                        hashTblID INT PRIMARY KEY IDENTITY,
                        accountNo TEXT NOT NULL,
                        hashValue TEXT NOT NULL
                    )";

                command.ExecuteNonQuery();
                
            }
        }

        public void loadBankAccounts()
        {
            if (!File.Exists(Data_Access_Layer.databaseName))
                initialiseDatabase();
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Bank_Accounts";
                    SqliteDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {

                        int accountType = dr.GetInt16(7);

                        if (accountType == Account_Type.Current_Account)
                        {
                            Current_Account ca = new Current_Account();
                            ca.accountNo = dr.GetString(0);
                            ca.name = dr.GetString(1);
                            ca.address_line_1 = dr.GetString(2);
                            ca.address_line_2 = dr.GetString(3);
                            ca.address_line_3 = dr.GetString(4);
                            ca.town = dr.GetString(5);
                            ca.balance = dr.GetDouble(6);
                            ca.overdraftAmount = dr.GetDouble(8);
                            accounts.Add(ca);
                        }
                        else
                        {
                            Savings_Account sa = new Savings_Account();
                            sa.accountNo = dr.GetString(0);
                            sa.name = dr.GetString(1);
                            sa.address_line_1 = dr.GetString(2);
                            sa.address_line_2 = dr.GetString(3);
                            sa.address_line_3 = dr.GetString(4);
                            sa.town = dr.GetString(5);
                            sa.balance = dr.GetDouble(6);
                            sa.interestRate = dr.GetDouble(9);
                            accounts.Add(sa);
                        }


                    }

                }

            }
        }

        public void loadBankAccountNumbers()
        {
            if (!File.Exists(Data_Access_Layer.databaseName))
                initialiseDatabase();
            else
            {
                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT accountNo FROM Bank_Accounts"; // Select only the accountNo column
                    SqliteDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        string accountNumber = dr["accountNo"].ToString(); // Retrieve account number
                        accountNumbers.Add(accountNumber); // Add account number to the list
                    }
                }
            }

            foreach (var item in accountNumbers)
            {
                Console.WriteLine(item);

            }
        }

        public Bank_Account loadBankAccount(string accountNumberToFind)
        {
            Bank_Account foundAccount = null; // Variable to store the found account

            if (!File.Exists(Data_Access_Layer.databaseName))
                initialiseDatabase();
            else
            {
                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Bank_Accounts WHERE accountNo = @AccountNumber";
                    command.Parameters.AddWithValue("@AccountNumber", accountNumberToFind);
                    SqliteDataReader dr = command.ExecuteReader();

                    if (dr.Read())
                    {
                        int accountType = dr.GetInt16(7);

                        if (accountType == Account_Type.Current_Account)
                        {
                            Current_Account ca = new Current_Account();
                            ca.accountNo = dr.GetString(0);
                            ca.name = dr.GetString(1);
                            ca.address_line_1 = dr.GetString(2);
                            ca.address_line_2 = dr.GetString(3);
                            ca.address_line_3 = dr.GetString(4);
                            ca.town = dr.GetString(5);
                            ca.balance = dr.GetDouble(6);
                            ca.overdraftAmount = dr.GetDouble(8);
                            foundAccount = ca;
                        }
                        else
                        {
                            Savings_Account sa = new Savings_Account();
                            sa.accountNo = dr.GetString(0);
                            sa.name = dr.GetString(1);
                            sa.address_line_1 = dr.GetString(2);
                            sa.address_line_2 = dr.GetString(3);
                            sa.address_line_3 = dr.GetString(4);
                            sa.town = dr.GetString(5);
                            sa.balance = dr.GetDouble(6);
                            sa.interestRate = dr.GetDouble(9);
                            foundAccount = sa;
                        }
                    }
                }
            }

            return foundAccount; // Return the found account or null if not found
        }

        public String addBankAccount(Bank_Account ba)
        {


            if (ba.GetType() == typeof(Current_Account))
                ba = (Current_Account)ba;
            else
                ba = (Savings_Account)ba;

            accounts.Add(ba);

           

            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
            INSERT INTO Bank_Accounts VALUES (
                @accountNo, 
                @name, 
                @address_line_1, 
                @address_line_2, 
                @address_line_3, 
                @town, 
                @balance, 
                @accountType, 
                @overdraftAmount, 
                @interestRate
            )
        ";

                command.Parameters.AddWithValue("@accountNo", ba.accountNo);
                command.Parameters.AddWithValue("@name", ba.name);
                command.Parameters.AddWithValue("@address_line_1", ba.address_line_1);
                command.Parameters.AddWithValue("@address_line_2", ba.address_line_2);
                command.Parameters.AddWithValue("@address_line_3", ba.address_line_3);
                command.Parameters.AddWithValue("@town", ba.town);
                command.Parameters.AddWithValue("@balance", ba.balance);

                if (ba.GetType() == typeof(Current_Account))
                {
                    Current_Account ca = (Current_Account)ba;
                    command.Parameters.AddWithValue("@accountType", 1);
                    command.Parameters.AddWithValue("@overdraftAmount", ca.overdraftAmount);
                    command.Parameters.AddWithValue("@interestRate", DBNull.Value);
                }
                else
                {
                    Savings_Account sa = (Savings_Account)ba;
                    command.Parameters.AddWithValue("@accountType", 2);
                    command.Parameters.AddWithValue("@overdraftAmount", DBNull.Value);
                    command.Parameters.AddWithValue("@interestRate", sa.interestRate);
                }

                command.ExecuteNonQuery();
            }

            return ba.accountNo;

        }


        public String addHash(Hash h)
        {
            DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time

            // Format the date and time as a string
            string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                  command.CommandText =
                    @"
                      INSERT INTO hashTBL VALUES (
                      @accountNo, 
                      @hashValue,
                      @timeStamp
                    )
                    ";

                command.Parameters.AddWithValue("@accountNo", h.accountNo);
                command.Parameters.AddWithValue("@hashValue", h.hashValue);
                command.Parameters.AddWithValue("@timeStamp", formattedTimestamp);

                command.ExecuteNonQuery();
            }

            return h.hashValue;

        }

        public String updateHash(Hash h)
        {
            DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time

            // Format the date and time as a string
            string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                  command.CommandText =
                   @"
                    UPDATE hashTBL 
                    SET hashValue = @hashValue, timeStamp = @timeStamp 
                    WHERE accountNo = @accountNo
                   ";

                command.Parameters.AddWithValue("@hashValue", h.hashValue);
                command.Parameters.AddWithValue("@timeStamp", formattedTimestamp);
                command.Parameters.AddWithValue("@accountNo", h.accountNo);
           
                command.ExecuteNonQuery();
            }

            return h.hashValue;

        }



        public Bank_Account findBankAccountByAccNo(String accNo) 
        {
            loadBankAccountNumbers();

            string encryptedNum = encryption_handler.EncryptForAccountSearch(accNo);
            Console.WriteLine("\nEncryptedNumber: " + encryptedNum + "\n");


            foreach (string num in accountNumbers)
            {
                //Console.WriteLine(num);

                if (num.Equals(encryptedNum))
                {   
                    Console.WriteLine($"{num}");
                    return loadBankAccount(num);
                }

            }

            return null; 
        }

        public bool closeBankAccount(String accNo) 
        {

            Bank_Account toRemove = null;
            
            foreach (Bank_Account ba in accounts)
            {

                if (ba.accountNo.Equals(accNo))
                {
                    toRemove = ba;
                    break;
                }

            }

            if (toRemove == null)
                return false;
            else
            {
                accounts.Remove(toRemove);

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Bank_Accounts WHERE accountNo = @accountNo";

                    command.Parameters.AddWithValue("@accountNo", toRemove.accountNo);

                    command.ExecuteNonQuery();
                }

                return true;
            }

        }

        public bool lodge(String accNo, double amountToLodge)
        {

            Bank_Account toLodgeTo = null;

            foreach (Bank_Account ba in accounts)
            {

                if (ba.accountNo.Equals(accNo))
                {
                    ba.lodge(amountToLodge);
                    toLodgeTo = ba;
                    break;
                }

            }

            if (toLodgeTo == null)
                return false;
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = @balance WHERE accountNo = @accountNo";

                    command.Parameters.AddWithValue("@balance", toLodgeTo.balance);
                    command.Parameters.AddWithValue("@accountNo", toLodgeTo.accountNo);

                    command.ExecuteNonQuery();
                }

                return true;
            }

        }

        public bool withdraw(String accNo, double amountToWithdraw)
        {

            Bank_Account toWithdrawFrom = null;
            bool result = false;

            foreach (Bank_Account ba in accounts)
            {

                if (ba.accountNo.Equals(accNo))
                {
                    result = ba.withdraw(amountToWithdraw);
                    toWithdrawFrom = ba;
                    break;
                }

            }

            if (toWithdrawFrom == null || result == false)
                return false;
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = @balance WHERE accountNo = @accountNo";

                    command.Parameters.AddWithValue("@balance", toWithdrawFrom.balance);
                    command.Parameters.AddWithValue("@accountNo", toWithdrawFrom.accountNo);

                    command.ExecuteNonQuery();
                }

                return true;
            }

        }

    }
}
