using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application
{
    public class EventLogger
    {
        private string logName;
        private string sourceName;

        public EventLogger(string logName, string sourceName)
        {
            this.logName = logName;
            this.sourceName = sourceName;

            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
                Console.WriteLine($"Event source '{sourceName}' created.");
            }
        }

        public EventRecord ReadLastEvent()
        {
            EventLogQuery query = new EventLogQuery(logName, PathType.LogName,
                $"*[System[Provider[@Name='{sourceName}']]]")
            {
                ReverseDirection = true
            };

            using (EventLogReader logReader = new EventLogReader(query))
            {
                return logReader.ReadEvent();
            }
        }

        public void ReadAllEvents()
        {
            EventLogQuery query = new EventLogQuery(logName, PathType.LogName,
                $"*[System[Provider[@Name='{sourceName}']]]")
            {
                ReverseDirection = true
            };

            using (EventLogReader logReader = new EventLogReader(query))
            {
                for (EventRecord eventInstance = logReader.ReadEvent(); eventInstance != null; eventInstance = logReader.ReadEvent())
                {
                    Console.WriteLine($"Event ID: {eventInstance.Id}");
                    Console.WriteLine($"Level: {eventInstance.LevelDisplayName}");
                    Console.WriteLine($"Message: {eventInstance.FormatDescription()}");
                    Console.WriteLine();
                }
            }
        }

        public void WriteEvent(string message, EventLogEntryType eventType)
        {
            using (EventLog eventLog = new EventLog(logName))
            {
                eventLog.Source = sourceName;
                eventLog.WriteEntry(message, eventType);
            }
        }

        //public void DeleteLastEvent()
        //{
        //    EventRecord lastEvent = ReadLastEvent();

        //    if (lastEvent != null)
        //    {
        //        lastEvent.DeleteEvent();
        //        Console.WriteLine("Last event deleted.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("No events found to delete.");
        //    }
        //}
    }
}
