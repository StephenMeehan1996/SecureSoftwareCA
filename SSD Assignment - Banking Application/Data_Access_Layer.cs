using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using SSD_Assignment___Banking_Application;

namespace Banking_Application
{
    public class Data_Access_Layer
    {

    
        List<string> accountNumbers = new List<string>(); // List to store account numbers

        public static String databaseName = "Banking Database.db";
        private static Data_Access_Layer instance = new Data_Access_Layer();
        Encryption_Handler encryption_handler = new Encryption_Handler();
        EventLogger eventLogger = new EventLogger("Application", "Banking-App");
        Bank_Account ba;
    

        private Data_Access_Layer()//Singleton Design Pattern (For Concurrency Control) - Use getInstance() Method Instead.
        {
            
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
                        interestRate REAL,
                        IV TEXT
                    );

                    CREATE TABLE IF NOT EXISTS hashTBL(
                        hashTblID INT PRIMARY KEY IDENTITY,
                        accountNo TEXT NOT NULL,
                        hashValue TEXT NOT NULL
                    )";

                command.ExecuteNonQuery();
                
            }
        }

        public string HandleBankAccountInsert(Bank_Account ba)
        {
            if (ba.GetType() == typeof(Current_Account))
            {
                Bank_Account ca = encryption_handler.EncrypCurrentAccount((Current_Account)ba); // encrypt bank account
              
                string hash = encryption_handler.serializeObject(ca);
                Hash h = new(ca.accountNo, hash);

                addBankAccount(ca); 

                addHash(h);

                return (string.Format($"\n--------------------------------\nBank Account Succesfully Added\nYour Account number is: {ba.accountNo} \n----------------------------\n"));
            }
            else
            {
                Bank_Account sa = encryption_handler.EncryptSavingsAccount((Savings_Account)ba); // encrypt bank account
            
                string hash = encryption_handler.serializeObject(sa);
                Hash h = new(sa.accountNo, hash);

                addBankAccount(sa); 
                addHash(h);

                return (string.Format($"\n--------------------------------\nBank Account Succesfully Added\nYour Account number is: {ba.accountNo} \n----------------------------\n"));
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
                            ca.iv = Convert.FromBase64String(dr.GetString(10));
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
                            sa.iv = Convert.FromBase64String(dr.GetString(10));
                            foundAccount = sa;
                        }
                    }
                }
            }

            if (foundAccount != null)
            {
                //decrypt from DB
                Bank_Account da;

                if (foundAccount.GetType() == typeof(Current_Account))
                {
                    da = encryption_handler.DecrypCurrentAccount((Current_Account)foundAccount);

                }
                else
                {
                    da = encryption_handler.DecryptSavingsAccount((Savings_Account)foundAccount);
                }

                return da; // Return the found account or null if not found
            }

            return null;
        }

        public Bank_Account EncryptForHashing(Bank_Account ba)
        {
            if(ba == null)
            {
                return null;
            }

            Bank_Account da;

            if (ba.GetType() == typeof(Current_Account))
            {
                da = encryption_handler.EncrypCurrentAccount((Current_Account)ba);

            }
            else
            {
                da = encryption_handler.EncryptSavingsAccount((Savings_Account)ba);
            }

            return da; // Return the found account or null if not found

        }

        public String addBankAccount(Bank_Account ba)
        {
            DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time
            string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

            if (ba.GetType() == typeof(Current_Account))
                ba = (Current_Account)ba;
            else
                ba = (Savings_Account)ba;

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
                        @interestRate,
                        @IV
                      )
                        ";

                command.Parameters.AddWithValue("@accountNo", ba.accountNo);
                command.Parameters.AddWithValue("@name", ba.name);
                command.Parameters.AddWithValue("@address_line_1", ba.address_line_1);
                command.Parameters.AddWithValue("@address_line_2", ba.address_line_2);
                command.Parameters.AddWithValue("@address_line_3", ba.address_line_3);
                command.Parameters.AddWithValue("@town", ba.town);
                command.Parameters.AddWithValue("@balance", ba.balance);
                command.Parameters.AddWithValue("@IV", Convert.ToBase64String(ba.iv));

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
            eventLogger.WriteEvent($"Account Number: {ba.accountNo}\nAction: Created New Account\nTime: {formattedTimestamp}", EventLogEntryType.Information);
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
            eventLogger.WriteEvent($"Account Number: {h.accountNo}\nAction: Created Hash for new account\nTime: {formattedTimestamp}", EventLogEntryType.Information);
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

            eventLogger.WriteEvent($"Account Number: {h.accountNo}\nAction: Updated hash\nTime: {formattedTimestamp}", EventLogEntryType.Information);

            return h.hashValue;
        }

        public bool CompareHashValue(Bank_Account ba)
        {
            Bank_Account ea = EncryptForHashing(ba);
            string currrentHash = encryption_handler.serializeObject(ea);

            if (RetrieveHashValue(ea.accountNo) != currrentHash)
            {
                Console.WriteLine("\n------------------------");
                Console.WriteLine("Integrity of data has been breached");
                Console.WriteLine("------------------------\n");

                DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time
                string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

                eventLogger.WriteEvent($"Account Number: {ba.accountNo}\nAction: Hash Mismatch\nTime: {formattedTimestamp}", EventLogEntryType.Error);

                return false;

            }

            return true;

        }

        public string RetrieveHashValue(string accountNo)
        {
            string hashValue = null;
       

            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                  @"
                    SELECT hashValue 
                    FROM hashTBL 
                    WHERE accountNo = @accountNo
                  ";

                command.Parameters.AddWithValue("@accountNo", accountNo);

                // Execute the query to fetch the hash value
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Check if a record is found
                    {
                        hashValue = reader["hashValue"].ToString(); // Retrieve the hash value
                    }
                }
            }

            // Log retrieval action to the event log if the hash value was retrieved
            if (hashValue != null)
            {
                DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time
                string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

                eventLogger.WriteEvent($"Account Number: {accountNo}\nAction: Retrieved hash\nTime: {formattedTimestamp}", EventLogEntryType.Information);
            }

            return hashValue;
        }



        public Bank_Account findBankAccountByAccNo(String accNo) 
        {
            accountNumbers = new List<string>();
            loadBankAccountNumbers();

            string encryptedNum = encryption_handler.EncryptForAccountSearch(accNo);
           
            foreach (string num in accountNumbers)
            {
                //Console.WriteLine(num);

                if (num.Equals(encryptedNum))
                {   
                    return loadBankAccount(num);
                }

            }
            return null; 
        }

        public bool closeBankAccount(String accNo) 
        {
            DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time
            string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

            if (accNo == null)
                return false;
            else
            {
                //accounts.Remove(toRemove);

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Bank_Accounts WHERE accountNo = @accountNo";

                    command.Parameters.AddWithValue("@accountNo", accNo);

                    command.ExecuteNonQuery();
                }

                eventLogger.WriteEvent($"Account Number: {accNo}\nAction: Account Closed\nTime: {formattedTimestamp}", EventLogEntryType.Information);

                return true;
            }

        }

        public bool lodge(String accNo, double amountToLodge)
        {
            if (accNo == null)
                return false;

            else
            {
                Bank_Account ba = findBankAccountByAccNo(accNo);
                
                double newBalance = ba.balance += amountToLodge;
                string encryptedNum = encryption_handler.EncryptForAccountSearch(ba.accountNo);
                DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time
                string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = @balance WHERE accountNo = @accountNo";

                    command.Parameters.AddWithValue("@balance",  newBalance);
                    command.Parameters.AddWithValue("@accountNo", encryptedNum);

                    command.ExecuteNonQuery();

                    Console.WriteLine("\n------------------------");
                    Console.WriteLine($"Lodge Successfull - New Balance: {newBalance}");
                    Console.WriteLine("------------------------\n");

                    Bank_Account ea = EncryptForHashing(ba);
             

                    string hash = encryption_handler.serializeObject(ea);
                    Hash h = new(ea.accountNo, hash);
                    updateHash(h);

                    eventLogger.WriteEvent($"Account Number: {ba.accountNo}\nAction: Lodgement of {amountToLodge}\nTime: {formattedTimestamp}", EventLogEntryType.Information);
                   

                    ea = null;
                    hash = null;
                    h = null;

                }

                ba = null;
                encryptedNum= null;
                GC.Collect();
        
                return true;
            }

        }

        public bool withdraw(String accNo, double amountToWithdraw)
        {
            Bank_Account ba = findBankAccountByAccNo(accNo);
            double newBalance = ba.balance -= amountToWithdraw;
            double maxWidthdraw;
            DateTime currentDateTime = DateTime.Now; // or DateTime.UtcNow for UTC time
            string formattedTimestamp = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");

            if (ba.GetType() == typeof(Current_Account))
            {
                Current_Account ca = (Current_Account)ba;
                maxWidthdraw= ca.balance + ca.overdraftAmount;
                Console.WriteLine("here " + maxWidthdraw);
                ca = null; // For Garbage
            }
            else
            {
                maxWidthdraw = ba.balance;
            }

            if (amountToWithdraw <= maxWidthdraw)
              {
                string encryptedNum = encryption_handler.EncryptForAccountSearch(ba.accountNo);

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = @balance WHERE accountNo = @accountNo";

                    command.Parameters.AddWithValue("@balance", newBalance);
                    command.Parameters.AddWithValue("@accountNo", encryptedNum);

                    command.ExecuteNonQuery();

                    Console.WriteLine("\n------------------------");
                    Console.WriteLine($"Widthdraw Successfull - New Balance: {newBalance}");
                    Console.WriteLine("------------------------\n");


                    Bank_Account ea = EncryptForHashing(ba);


                    string hash = encryption_handler.serializeObject(ea);
                    Hash h = new(ea.accountNo, hash);
                    updateHash(h);

                    eventLogger.WriteEvent($"Account Number: {ba.accountNo}\nAction: Widthdrawel of {amountToWithdraw}\nTime: {formattedTimestamp}", EventLogEntryType.Information);

                    ea = null;
                    hash = null;
                    h = null;
                }

                encryptedNum = null;
                ba = null;
                GC.Collect();
     
                return true;

            }
            return false;
        }

    }
}
