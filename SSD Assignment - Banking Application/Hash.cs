using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSD_Assignment___Banking_Application
{
    [Serializable]
    public class Hash
    {
        public String accountNo;
        public String hashValue;

        public Hash()
        {

        }

        public Hash(String accountNo, String hashValue)
        {
            this.accountNo = accountNo;
            this.hashValue = hashValue;
          
        }

        public override String ToString()
        {

            return string.Format($"Account No: {accountNo}\nHash: {hashValue}\n ");

        }
    }
}
