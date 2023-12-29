using SSD_Assignment___Banking_Application;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Diagnostics.Eventing.Reader;

namespace Banking_Application
{
    public class Program
    {
        //public  Bank_Account ConvertBase64(Bank_Account ba)
        //{

        //}

        public static void Main(string[] args)
        {
            
            Data_Access_Layer dal = Data_Access_Layer.getInstance();
            EventLogger eventLogger = new EventLogger("Application", "Banking-App");
            Encryption_Handler encryption_handler = new Encryption_Handler();

            //eventLogger.ReadAllEvents();
            //eventLogger.WriteEvent("This is a test event.", EventLogEntryType.Information);
            //EventRecord lastEvent = eventLogger.ReadLastEvent();
            //if (lastEvent != null)
            //{
            //    Console.WriteLine($"Last Event ID: {lastEvent.Id}");
            //    Console.WriteLine($"Last Event Level: {lastEvent.LevelDisplayName}");
            //    Console.WriteLine($"Last Event Message: {lastEvent.FormatDescription()}");
            //} 

            dal.loadBankAccountNumbers();

            bool running = true;

            do
            {

                Console.WriteLine("");
                Console.WriteLine("***Banking Application Menu***");
                Console.WriteLine("1. Add Bank Account");
                Console.WriteLine("2. Close Bank Account");
                Console.WriteLine("3. View Account Information");
                Console.WriteLine("4. Make Lodgement");
                Console.WriteLine("5. Make Withdrawal"); 
                Console.WriteLine("6. Exit");  
                Console.WriteLine("CHOOSE OPTION:");
                String option = Console.ReadLine();


               // encryption_handler.serializeObject();
                switch(option)
                {
                    case "1":
                        String accountType = "";
                        int loopCount = 0;
                        
                        do
                        {

                           if(loopCount > 0)
                                Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");

                            Console.WriteLine("");
                            Console.WriteLine("***Account Types***:");
                            Console.WriteLine("1. Current Account.");
                            Console.WriteLine("2. Savings Account.");
                            Console.WriteLine("CHOOSE OPTION:");
                            accountType = Console.ReadLine();

                            loopCount++;

                        } while (!(accountType.Equals("1") || accountType.Equals("2")));

                        String name = "";
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                                Console.WriteLine("INVALID NAME ENTERED - PLEASE TRY AGAIN");

                            Console.WriteLine("Enter Name: ");
                            name = Console.ReadLine();

                            loopCount++;

                        } while (name.Equals(""));

                        String addressLine1 = "";
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                                Console.WriteLine("INVALID ÀDDRESS LINE 1 ENTERED - PLEASE TRY AGAIN");

                            Console.WriteLine("Enter Address Line 1: ");
                            addressLine1 = Console.ReadLine();

                            loopCount++;

                        } while (addressLine1.Equals(""));

                        Console.WriteLine("Enter Address Line 2: ");
                        String addressLine2 = Console.ReadLine();
                        
                        Console.WriteLine("Enter Address Line 3: ");
                        String addressLine3 = Console.ReadLine();

                        String town = "";
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                                Console.WriteLine("INVALID TOWN ENTERED - PLEASE TRY AGAIN");

                            Console.WriteLine("Enter Town: ");
                            town = Console.ReadLine();

                            loopCount++;

                        } while (town.Equals(""));

                        double balance = -1;
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                                Console.WriteLine("INVALID OPENING BALANCE ENTERED - PLEASE TRY AGAIN");

                            Console.WriteLine("Enter Opening Balance: ");
                            String balanceString = Console.ReadLine();

                            try
                            {
                                balance = Convert.ToDouble(balanceString);
                            }

                            catch 
                            {
                                loopCount++;
                            }

                        } while (balance < 0);

                        Bank_Account ba;
                        string accNo;
                        Hash h;

                        if (Convert.ToInt32(accountType) == Account_Type.Current_Account)
                        {
                            double overdraftAmount = -1;
                            loopCount = 0;

                            do
                            {

                                if (loopCount > 0)
                                    Console.WriteLine("INVALID OVERDRAFT AMOUNT ENTERED - PLEASE TRY AGAIN");

                                Console.WriteLine("Enter Overdraft Amount: ");
                                String overdraftAmountString = Console.ReadLine();

                                try
                                {
                                    overdraftAmount = Convert.ToDouble(overdraftAmountString);
                                }

                                catch
                                {
                                    loopCount++;
                                }

                            } while (overdraftAmount < 0);
                            byte[] iv = encryption_handler.CreateIV();
                            string accountNo = System.Guid.NewGuid().ToString();

                            Current_Account ca = new Current_Account(accountNo, name, addressLine1, addressLine2, addressLine3, town, balance, overdraftAmount, iv);

                            Current_Account ea = encryption_handler.EncrypCurrentAccount(ca); // encrypt bank account
                            Console.WriteLine("\nEncrypted Bank Account");
                            Console.WriteLine("---------------------------");
                            Console.WriteLine(ea.ToString()); //print
                            Console.WriteLine("---------------------------\n");

                         

                            accNo = dal.addBankAccount(ea); // add encrypted bank account

                            string hash = encryption_handler.serializeObject(ea);
                            h = new(ea.accountNo, hash);

                            Console.WriteLine("\n---------------------------");
                            Console.WriteLine("New Account Number Is: " + ca.accountNo); // print account number
                            Console.WriteLine("Encryted: " + ea.accountNo);
                            Console.WriteLine("test prop: " + encryption_handler.EncryptForAccountSearch(ca.accountNo));
                            Console.WriteLine("---------------------------\n");

                            Console.WriteLine("Decrypted Bank Account");
                            Console.WriteLine("---------------------------");
                            Bank_Account da = encryption_handler.DecrypCurrentAccount(ea); // decrypted account
                            Console.WriteLine(da.ToString());
                            Console.WriteLine("---------------------------\n");
                            dal.addHash(h);
                        }

                        else
                        {

                            double interestRate = -1;
                            loopCount = 0;

                            do
                            {

                                if (loopCount > 0)
                                    Console.WriteLine("INVALID INTEREST RATE ENTERED - PLEASE TRY AGAIN");

                                Console.WriteLine("Enter Interest Rate: ");
                                String interestRateString = Console.ReadLine();

                                try
                                {
                                    interestRate = Convert.ToDouble(interestRateString);
                                }

                                catch
                                {
                                    loopCount++;
                                }

                            } while (interestRate < 0);
                            byte[] iv = encryption_handler.CreateIV(); // create account random IV
                            string accountNo = System.Guid.NewGuid().ToString(); // Create account num
                            Savings_Account sa = new Savings_Account(accountNo,name, addressLine1, addressLine2, addressLine3, town, balance, interestRate, iv);

                            Savings_Account ea = encryption_handler.EncryptSavingsAccount(sa); // encrypt bank account
                            Console.WriteLine("\nEncrypted Bank Account");
                            Console.WriteLine("---------------------------");
                            Console.WriteLine(ea.ToString()); //print
                            Console.WriteLine("---------------------------\n");


                            accNo = dal.addBankAccount(ea); // add encrypted bank account
                            Console.WriteLine("New Account Number Is: " + sa.accountNo); // print account number


                            string hash = encryption_handler.serializeObject(ea);
                            h = new(ea.accountNo, hash);

                            Console.WriteLine("Decrypted Bank Account");
                            Console.WriteLine("---------------------------");
                            Bank_Account da = encryption_handler.DecryptSavingsAccount(ea); // decrypted account
                            Console.WriteLine(da.ToString());
                            Console.WriteLine("---------------------------\n");

                          
                            dal.addHash(h);
                          
                        }

                        break;

                    case "2":
                        Console.WriteLine("Enter Account Number: ");
                        accNo = Console.ReadLine();

                        ba = dal.findBankAccountByAccNo(accNo);

                        if (ba is null)
                        {
                            Console.WriteLine("Account Does Not Exist");
                        }
                        else
                        {
                            Console.WriteLine(ba.ToString());

                            String ans = "";

                            do
                            {

                                Console.WriteLine("Proceed With Delection (Y/N)?"); 
                                ans = Console.ReadLine();

                                switch (ans)
                                {
                                    case "Y":
                                    case "y": dal.closeBankAccount(accNo);
                                        break;
                                    case "N":
                                    case "n":
                                        break;
                                    default:
                                        Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");
                                        break;
                                }
                            } while (!(ans.Equals("Y") || ans.Equals("y") || ans.Equals("N") || ans.Equals("n")));
                        }

                        break;
                    case "3":
                        Console.WriteLine("Enter Account Number: ");
                        accNo = Console.ReadLine();
                 

                        ba = dal.findBankAccountByAccNo(accNo);

                        if(ba is null) 
                        {
                            Console.WriteLine("Account Does Not Exist");
                        }
                        else
                        {
                            Console.WriteLine(ba.ToString());
                        }

                        break;
                    case "4": //Lodge
                        Console.WriteLine("Enter Account Number: ");
                        accNo = Console.ReadLine();

                        ba = dal.findBankAccountByAccNo(accNo);

                        if (ba is null)
                        {
                            Console.WriteLine("Account Does Not Exist");
                        }
                        else
                        {
                            double amountToLodge = -1;
                            loopCount = 0;

                            do
                            {

                                if (loopCount > 0)
                                    Console.WriteLine("INVALID AMOUNT ENTERED - PLEASE TRY AGAIN");

                                Console.WriteLine("Enter Amount To Lodge: ");
                                String amountToLodgeString = Console.ReadLine();

                                try
                                {
                                    amountToLodge = Convert.ToDouble(amountToLodgeString);
                                }

                                catch
                                {
                                    loopCount++;
                                }

                            } while (amountToLodge < 0);

                            dal.lodge(accNo, amountToLodge);
                        }
                        break;
                    case "5": //Withdraw
                        Console.WriteLine("Enter Account Number: ");
                        accNo = Console.ReadLine();

                        ba = dal.findBankAccountByAccNo(accNo);

                        if (ba is null)
                        {
                            Console.WriteLine("Account Does Not Exist");
                        }
                        else
                        {
                            double amountToWithdraw = -1;
                            loopCount = 0;

                            do
                            {

                                if (loopCount > 0)
                                    Console.WriteLine("INVALID AMOUNT ENTERED - PLEASE TRY AGAIN");

                                Console.WriteLine("Enter Amount To Withdraw (€" + ba.getAvailableFunds() + " Available): ");
                                String amountToWithdrawString = Console.ReadLine();

                                try
                                {
                                    amountToWithdraw = Convert.ToDouble(amountToWithdrawString);
                                }

                                catch
                                {
                                    loopCount++;
                                }

                            } while (amountToWithdraw < 0);

                            bool withdrawalOK = dal.withdraw(accNo, amountToWithdraw);

                            if(withdrawalOK == false)
                            {

                                Console.WriteLine("Insufficient Funds Available.");
                            }
                        }
                        break;
                    case "6":
                        running = false;
                        break;
                    default:    
                        Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");
                        break;
                }
                
                
            } while (running != false);

        }

    }
}