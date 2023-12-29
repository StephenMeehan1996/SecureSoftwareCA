using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class Bank_Account
{

    public String accountNo;
    public String name;
    public String address_line_1;
    public String address_line_2;
    public String address_line_3;
    public String town;
    public double balance;
    public byte[] iv;


    public Bank_Account()
    {

    }

    public Bank_Account(String accountNo,String name, String address_line_1, String address_line_2, String address_line_3, String town, double balance, byte[] iv)
    {
        this.accountNo = accountNo;
        this.name = name;
        this.address_line_1 = address_line_1;
        this.address_line_2 = address_line_2;
        this.address_line_3 = address_line_3;
        this.town = town;
        this.balance = balance;
        this.iv = iv;
    }

        public void lodge(double amountIn)
        {

            balance += amountIn;

        }

        public abstract bool withdraw(double amountToWithdraw);

        public abstract double getAvailableFunds();

        public override String ToString() {

        return string.Format($"Account No: {accountNo}\nName: {name}\nAddress Line 1: {address_line_1}\nAddress Line 2: {address_line_2}\nAddress Line 3: {address_line_3}\nTown: {town}\nBalance: {balance}\nIV: {string.Join(", ", iv)}");

        }

    }

