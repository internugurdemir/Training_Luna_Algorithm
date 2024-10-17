


using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public class Event
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public int Priority { get; set; }
    }

    public class Duration
    {
        public string From { get; set; }
        public string To { get; set; }
        public int DurationMinutes { get; set; }
    }

    static void Main()
    {
        List<Event> events = new List<Event>
        {
            new Event { Id = 1, StartTime = TimeSpan.Parse("09:00"), EndTime = TimeSpan.Parse("10:00"), Location = "A", Priority = 30 },
            new Event { Id = 2, StartTime = TimeSpan.Parse("09:30"), EndTime = TimeSpan.Parse("11:00"), Location = "B", Priority = 40 },
            new Event { Id = 3, StartTime = TimeSpan.Parse("11:15"), EndTime = TimeSpan.Parse("12:15"), Location = "A", Priority = 50 },
            new Event { Id = 4, StartTime = TimeSpan.Parse("12:30"), EndTime = TimeSpan.Parse("13:30"), Location = "C", Priority = 60 },
            new Event { Id = 5, StartTime = TimeSpan.Parse("13:00"), EndTime = TimeSpan.Parse("14:30"), Location = "B", Priority = 70 },
            new Event { Id = 6, StartTime = TimeSpan.Parse("14:45"), EndTime = TimeSpan.Parse("15:45"), Location = "D", Priority = 80 }
        };

        List<Duration> durations = new List<Duration>
        {
            new Duration { From = "A", To = "B", DurationMinutes = 20 },
            new Duration { From = "A", To = "C", DurationMinutes = 25 },
            new Duration { From = "A", To = "D", DurationMinutes = 15 },
            new Duration { From = "B", To = "C", DurationMinutes = 10 },
            new Duration { From = "B", To = "D", DurationMinutes = 20 },
            new Duration { From = "C", To = "D", DurationMinutes = 30 }
        };
        //List<Event> events = new List<Event>
        //{
        //    new Event { Id = 1, StartTime = TimeSpan.Parse("10:00"), EndTime = TimeSpan.Parse("12:00"), Location = "A", Priority = 50 },
        //    new Event { Id = 2, StartTime = TimeSpan.Parse("10:00"), EndTime = TimeSpan.Parse("11:00"), Location = "B", Priority = 30 },
        //    new Event { Id = 3, StartTime = TimeSpan.Parse("11:30"), EndTime = TimeSpan.Parse("12:30"), Location = "A", Priority = 40 },
        //    new Event { Id = 4, StartTime = TimeSpan.Parse("14:30"), EndTime = TimeSpan.Parse("16:00"), Location = "C", Priority = 70 },
        //    new Event { Id = 5, StartTime = TimeSpan.Parse("14:25"), EndTime = TimeSpan.Parse("15:30"), Location = "B", Priority = 60 },
        //    new Event { Id = 6, StartTime = TimeSpan.Parse("13:00"), EndTime = TimeSpan.Parse("14:00"), Location = "D", Priority = 80 }
        //};


        //List<Duration> durations = new List<Duration>
        //{
        //    new Duration { From = "A", To = "B", DurationMinutes = 15 },
        //    new Duration { From = "A", To = "C", DurationMinutes = 20 },
        //    new Duration { From = "A", To = "D", DurationMinutes = 10 },
        //    new Duration { From = "B", To = "C", DurationMinutes = 5 },
        //    new Duration { From = "B", To = "D", DurationMinutes = 25 },
        //    new Duration { From = "C", To = "D", DurationMinutes = 25 }
        //    };

        var result = GetMaxPriorityEvents(events, durations);
        Console.WriteLine("Selected Events:");
        foreach (var evt in result)
        {
            Console.WriteLine($"Event Id: {evt.Id}, Priority: {evt.Priority}");
        }
    }

    static List<Event> GetMaxPriorityEvents(List<Event> events, List<Duration> durations)
    {
        events = events.OrderBy(e => e.StartTime).ToList();
        List<Event> selectedEvents = new List<Event>();
        TimeSpan currentTime = TimeSpan.Zero;
        string currentLocation = "A"; // Assuming starting at location "A"
        int maxPriority = 0;

        void Backtrack(List<Event> currentEvents, int currentPriority, TimeSpan currentTime, string currentLocation, List<Event> remainingEvents)
        {
            if (currentPriority > maxPriority)
            {
                maxPriority = currentPriority;
                selectedEvents = new List<Event>(currentEvents);
            }

            foreach (var evt in remainingEvents)
            {
                int travelTime = GetTravelTime(currentLocation, evt.Location, durations);
                if (currentTime + TimeSpan.FromMinutes(travelTime) <= evt.StartTime)
                {
                    // Check if there exists another event with the same StartTime and higher Priority
                    if (events.Any(a => a.StartTime == evt.StartTime && a.Priority > evt.Priority && !currentEvents.Contains(a)))
                    {
                        continue; // Skip this event if a higher priority event at the same time exists
                    }

                    var nextEvents = new List<Event>(currentEvents) { evt };
                    var nextRemaining = remainingEvents.Where(e => e.Id != evt.Id).ToList();
                    Backtrack(nextEvents, currentPriority + evt.Priority, evt.EndTime, evt.Location, nextRemaining);
                }
            }
        }

        Backtrack(new List<Event>(), 0, currentTime, currentLocation, events);

        return selectedEvents;
    }

    static int GetTravelTime(string from, string to, List<Duration> durations)
    {
        if (from == to) return 0;
        var duration = durations.FirstOrDefault(d => (d.From == from && d.To == to) || (d.From == to && d.To == from));
        return duration?.DurationMinutes ?? int.MaxValue;
    }
    }