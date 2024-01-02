using SSD_Assignment___Banking_Application;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;

namespace Banking_Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            Data_Access_Layer dal = Data_Access_Layer.getInstance();
            EventLogger eventLogger = new EventLogger("Application", "Banking-App");
            Encryption_Handler encryption_handler = new Encryption_Handler();
            InputValidator validator = new InputValidator();
            ResourceCal rcal = new ResourceCal();
            Bank_Account ba;
            string accNo;
            bool running = true;

            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n------------------------");
                Console.WriteLine("Banking App");
                Console.WriteLine("------------------------\n");


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
                Console.ForegroundColor = ConsoleColor.White;

                switch (option)
                {
                    case "1":
                        String accountType = "";
                        int loopCount = 0;
                        
                        do
                        {

                            if (loopCount > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Option Selected - Please Choose Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

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
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Name Entered - Please Try Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            Console.WriteLine("Enter Name: ");
                            name = Console.ReadLine();

                            loopCount++;

                        } while (string.IsNullOrEmpty(name) || name.Length >= 35 || !validator.IsValidString(name));

                        String addressLine1 = "";
                        loopCount = 0;

                        do
                        {
                            if (loopCount > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Address Line 1 Entered - Please Try Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            Console.WriteLine("Enter Address Line 1: ");
                            addressLine1 = Console.ReadLine();

                            loopCount++;

                        } while (string.IsNullOrEmpty(addressLine1) || addressLine1.Length >= 30 || !validator.IsValidString(addressLine1));

                        String addressLine2 = "";
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Address Line 2 Entered - Please Try Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            Console.WriteLine("Enter Address Line 2: ");
                            addressLine2 = Console.ReadLine();

                            loopCount++;

                        } while (string.IsNullOrEmpty(addressLine2) || addressLine2.Length >= 30 || !validator.IsValidString(addressLine2));

                        String addressLine3 = "";
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Address Line 3 Entered - Please Try Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            Console.WriteLine("Enter Address Line 3: ");
                            addressLine3 = Console.ReadLine();

                            loopCount++;

                        } while (string.IsNullOrEmpty(addressLine3) || addressLine3.Length >= 30 || !validator.IsValidString(addressLine3));

                        String town = "";
                        loopCount = 0;

                        do
                        {

                            if (loopCount > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Town Entered - Please Try Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            Console.WriteLine("Enter Town: ");
                            town = Console.ReadLine();

                            loopCount++;

                        } while (string.IsNullOrEmpty(town) || town.Length >= 25 || !validator.IsValidString(town));

                        double balance;
                        loopCount = 0;

                        do
                        {
                            if (loopCount > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Invalid Balanced Entered - Please Try Again");
                                Console.WriteLine("------------------------\n");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            Console.WriteLine("Enter Opening Balance: ");
                            string balanceString = Console.ReadLine();

                            if (validator.IsValidDouble(balanceString, out balance))
                            {
                                loopCount = 0; 
                            }
                            else
                            {
                                loopCount++;
                            }

                        } while (balance <= 0);



                        if (Convert.ToInt32(accountType) == Account_Type.Current_Account)
                        {
                            double overdraftAmount;
                            loopCount = 0;

                            do
                            {
                                if (loopCount > 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n------------------------");
                                    Console.WriteLine("Invalid OverDraft Amount Entered - Please Try Again");
                                    Console.WriteLine("------------------------\n");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                                Console.WriteLine("Enter Overdraft Amount: ");
                                String overdraftAmountString = Console.ReadLine();

                                if (validator.IsValidDouble(overdraftAmountString, out overdraftAmount))
                                {
                                    loopCount = 0;
                                }
                                else
                                {
                                    loopCount++;
                                }

                            } while (overdraftAmount <= 0);

                            byte[] iv = encryption_handler.CreateIV();
                            string accountNo = System.Guid.NewGuid().ToString();

                            ba = new Current_Account(accountNo, name, addressLine1, addressLine2, addressLine3, town, balance, overdraftAmount, iv);
                        }

                        else
                        {
                            double interestRate;
                            loopCount = 0;

                            do
                            {
                                if (loopCount > 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n------------------------");
                                    Console.WriteLine("Invalid Interest Rate Entered - Please Try Again");
                                    Console.WriteLine("------------------------\n");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            

                                Console.WriteLine("Enter Interest Rate: ");
                                String interestRateString = Console.ReadLine();

                                if (validator.IsValidDouble(interestRateString, out interestRate))
                                {
                                    loopCount = 0;
                                }
                                else
                                {
                                    loopCount++;
                                }

                            } while (interestRate <= 0);

                            byte[] iv = encryption_handler.CreateIV(); // create account random IV
                            string accountNo = System.Guid.NewGuid().ToString(); // Create account num
                            ba = new Savings_Account(accountNo,name, addressLine1, addressLine2, addressLine3, town, balance, interestRate, iv);
                            iv = null;
                            accNo= null; 
                        }

                        Console.WriteLine(dal.HandleBankAccountInsert(ba));
                        ba = null;
                        GC.Collect();

                        break;

                    case "2":
                        Console.WriteLine("Enter Account Number: ");
                        accNo = Console.ReadLine();
                        if (!string.IsNullOrEmpty(accNo) && accNo.Length == 36)
                        {

                            ba = dal.findBankAccountByAccNo(accNo);

                        if (ba is null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n------------------------");
                            Console.WriteLine("Account Does Not Exist");
                            Console.WriteLine("------------------------\n");
                            break;

                        }
                        
                        Console.WriteLine(ba.ToString());
                        String ans = "";
                        
                        do
                            {
                                Console.WriteLine("Proceed With Delection (Y/N)?");
                                ans = Console.ReadLine();

                                switch (ans)
                                {
                                    case "Y":
                                    case "y":
                                        dal.closeBankAccount(encryption_handler.EncryptForAccountSearch(ba.accountNo));
                                        break;
                                    case "N":
                                    case "n":
                                        break;
                                    default:
                                        Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");
                                        break;
                                }
                            } while (!(ans.Equals("Y") || ans.Equals("y") || ans.Equals("N") || ans.Equals("n")));
                        

                            accNo = null;
                            ba = null;
                            GC.Collect();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n------------------------");
                            Console.WriteLine("Account Number Not Valid");
                            Console.WriteLine("------------------------\n");
                        }

                        break;
                    case "3":

                        for (int attempt = 0; attempt < 3; attempt++) // will attempt three times before quiting
                        {
                            if (rcal.GetRemainingCPU() < 3 || rcal.GetRemainingRAM() < 2) // checks resource's remaining
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Resource Availability Error, Trying again... ");
                                Console.WriteLine("------------------------\n");

                            }

                            else
                            {
                                Console.WriteLine("Enter Account Number: ");
                                accNo = Console.ReadLine();
                                ba = null;

                                //Example of Utlilizing relection
                                if (!string.IsNullOrEmpty(accNo) && accNo.Length == 36)
                                {
                                    Type dataAccessType = typeof(Data_Access_Layer);

                                    MethodInfo method = dataAccessType.GetMethod("findBankAccountByAccNo");

                                    if (method != null)
                                    {
                                        var access = Activator.CreateInstance(dataAccessType);

                                        object result = method.Invoke(access, new object[] { accNo });

                                        if (result != null && result is Bank_Account)
                                        {
                                            ba = (Bank_Account)result; //cast result to bank account type
                                        }
                                    }

                                    if (ba is null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("\n------------------------");
                                        Console.WriteLine("Account Does Not Exist");
                                        Console.WriteLine("------------------------\n");

                                        break;
                                    }

                                    if (dal.CompareHashValue(ba))
                                    {
                                        Console.WriteLine("\n" + ba.ToString());

                                        accNo = null;
                                        ba = null;
                                        GC.Collect();
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n------------------------");
                                    Console.WriteLine("Account Number Not Valid");
                                    Console.WriteLine("------------------------\n");
                                }

                              break;
                            }
                        }
                        break;

                    case "4": //Lodge

                        for (int attempt = 0; attempt < 3; attempt++) // will attempt three times before quiting
                        {
                            if (rcal.GetRemainingCPU() < 3 || rcal.GetRemainingRAM() < 2) // checks resource's remaining
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Resource Availability Error, Trying again... ");
                                Console.WriteLine("------------------------\n");

                            }
                            else
                            {
                                Console.WriteLine("Enter Account Number: ");
                                accNo = Console.ReadLine();

                                if (!string.IsNullOrEmpty(accNo) && accNo.Length == 36)
                                {
                                    ba = dal.findBankAccountByAccNo(accNo);

                                    if (ba is null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("\n------------------------");
                                        Console.WriteLine("Account Does Not Exist");
                                        Console.WriteLine("------------------------\n");

                                        break;
                                    }

                                    if (dal.CompareHashValue(ba))
                                    { // confirm has is the same //

                                        double amountToLodge;
                                        loopCount = 0;

                                        do
                                        {
                                            if (loopCount > 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("\n------------------------");
                                                Console.WriteLine("Invalid Amount Entered - Please Try Again");
                                                Console.WriteLine("------------------------\n");
                                                Console.ForegroundColor = ConsoleColor.White;
                                            }

                                            Console.WriteLine("Enter Amount To Lodge: ");
                                            String amountToLodgeString = Console.ReadLine();

                                            if (validator.IsValidDouble(amountToLodgeString, out amountToLodge))
                                            {
                                                loopCount = 0;
                                            }
                                            else
                                            {
                                                loopCount++;
                                            }

                                        } while (amountToLodge <= 0);

                                        dal.lodge(accNo, amountToLodge);

                                        accNo = null;
                                        ba = null;
                                        GC.Collect();
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n------------------------");
                                    Console.WriteLine("Account Number Not Valid");
                                    Console.WriteLine("------------------------\n");

                                }

                               break;
                            }
                        }
                        break;
                    case "5": //Withdraw

                        for (int attempt = 0; attempt < 3; attempt++) // will attempt three times before quiting
                        {
                            if (rcal.GetRemainingCPU() < 3 || rcal.GetRemainingRAM() < 2) // checks resource's remaining
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n------------------------");
                                Console.WriteLine("Resource Availability Error, Trying again... ");
                                Console.WriteLine("------------------------\n");

                            }
                            else
                            {
                                Console.WriteLine("Enter Account Number: ");
                                accNo = Console.ReadLine();

                                if (!string.IsNullOrEmpty(accNo) && accNo.Length == 36)
                                {
                                    ba = dal.findBankAccountByAccNo(accNo);

                                    if (ba is null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("\n------------------------");
                                        Console.WriteLine("Account Does Not Exist");
                                        Console.WriteLine("------------------------\n");

                                        break;
                                    }

                                    if (dal.CompareHashValue(ba))
                                    {
                                        double amountToWithdraw;
                                        loopCount = 0;

                                        do
                                        {

                                            if (loopCount > 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("\n------------------------");
                                                Console.WriteLine("Invalid Amount Entered - Please Try Again");
                                                Console.WriteLine("------------------------\n");
                                                Console.ForegroundColor = ConsoleColor.White;
                                            }

                                            Console.WriteLine($"Enter Amount To Withdraw (€{ba.balance})");

                                            String amountToWithdrawString = Console.ReadLine();

                                            if (validator.IsValidDouble(amountToWithdrawString, out amountToWithdraw))
                                            {
                                                loopCount = 0;
                                            }
                                            else
                                            {
                                                loopCount++;
                                            }

                                        } while (amountToWithdraw < 0);

                                        bool withdrawalOK = dal.withdraw(accNo, amountToWithdraw);

                                        if (withdrawalOK == false)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("\n------------------------");
                                            Console.WriteLine("Insufficient Funds Available.");
                                            Console.WriteLine("------------------------\n");
                                        }
                                    }

                                    accNo = null;
                                    ba = null;
                                    GC.Collect();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n------------------------");
                                    Console.WriteLine("Account Number Not Valid");
                                    Console.WriteLine("------------------------\n");
                                }

                              break;
                            }
                        }  
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n------------------------");
                        Console.WriteLine("Invalid Option Selected - Please Try Again");
                        Console.WriteLine("------------------------\n");
                        break;
                }
                
                
            } while (running != false);

        }

    }
}