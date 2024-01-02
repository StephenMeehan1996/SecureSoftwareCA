using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;


namespace SSD_Assignment___Banking_Application
{
    internal class ResourceCal
    { 
      
        public double GetRemainingCPU()
        {
             //not sure if this this the best way to do this. 
            // source: https://stackoverflow.com/questions/9777661/returning-cpu-usage-in-wmi-using-c-sharp

           using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Name"].ToString() == "_Total")
                    {
                        //return free CPU
                        double usage = Convert.ToDouble(obj["PercentProcessorTime"]);
                        return 100 - usage;
                    }
                }
            }
            return 0;
        }

        public double GetRemainingRAM()
        {
            double usedMemoryPercentage = 0;

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong totalMemory = Convert.ToUInt64(obj["TotalVisibleMemorySize"]); // Total physical memory in KB
                    ulong freeMemory = Convert.ToUInt64(obj["FreePhysicalMemory"]); // Free physical memory in KB

                    usedMemoryPercentage = 100 - ((freeMemory * 100.0) / totalMemory);

                    
                  
                   // Console.WriteLine("Used RAM Percentage: " + usedMemoryPercentage.ToString("0.00") + "%");
                }
            }
            //retun free RAM
            return 100 - usedMemoryPercentage;
        }
    }
}
