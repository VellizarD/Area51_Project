using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConcurrentPrgrammingExam
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"Simulation Started, current time: {DateTime.Now}\n");
            using (StreamWriter writer = new StreamWriter("C:\\Users\\Vili\\source\\repos\\ConcurrentPrgrammingExam\\ConcurrentPrgrammingExam\\Area51Elevator.txt"))
            {
                var syncWriter = StreamWriter.Synchronized(writer);
                var elevator = new Elevator(syncWriter);

                var agent1 = new Agent(1, SecurityLevel.Top_Secret, elevator);
                var agent2 = new Agent(2, SecurityLevel.Confidential, elevator);
                var agent3 = new Agent(3, SecurityLevel.Secret, elevator);

                List<Agent> agents = new List<Agent>
                {
                    agent1,
                    agent2,
                    agent3
                };

                syncWriter.WriteLine($"Simulation Started, current time: {DateTime.Now}");

                agents.ForEach(x => x.RoamThreadWorker());

                while (agents.Any(x => !x.AtHome))
                { }

                syncWriter.WriteLine($"Simulation Ended, current time: {DateTime.Now}");
            }
            Console.WriteLine($"Simulation Ended, current time: {DateTime.Now}");
            Console.Read();
        }
    }
}
