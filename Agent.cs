using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace ConcurrentPrgrammingExam
{
    public enum SecurityLevel { Confidential, Secret, Top_Secret };
    public class Agent
    {
        public static Mutex queueMutex = new Mutex();
        public int name;
        public SecurityLevel secLevel;
        public Elevator elevator;
        public Floor[] accessableFloors;
        public Floor currentFloor = Floor.G;
        public Random rand = new Random();
        public int workDone = 0;
        public ManualResetEvent atHomeEvent = new ManualResetEvent(false);
        public bool GoHomeTimeFlag = false;
        public TextWriter writer;
        public bool AtHome
        {
            get { return atHomeEvent.WaitOne(0); }
        }
        public Agent(int name, SecurityLevel secLevel, Elevator elevator)
        {
            writer = elevator.writer;
            this.name = name;
            this.secLevel = secLevel;
            this.elevator = elevator;
            accessableFloors = secLevel switch
            {
                SecurityLevel.Confidential => new Floor[] { Floor.G },
                SecurityLevel.Secret => new Floor[] { Floor.G, Floor.S },
                SecurityLevel.Top_Secret => new Floor[] { Floor.G, Floor.S, Floor.T1, Floor.T2 },
                _ => throw new ArgumentException("This Security Level is currently not supported!")
            };
        }
        public void EnterElevator()
        {
          //  Thread.Sleep(500);
            elevator.Enter(this);
            writer.WriteLine($"Agent {name} entered the elevator!");
            ChooseDestination();
        }
        public void LeaveElevator()
        {
           // Thread.Sleep(500);
            elevator.Leave();
            writer.WriteLine($"Agent {name} left the elevator!");
            currentFloor = elevator.GetCurrentFloor();
            queueMutex.ReleaseMutex();
        }
        public void CallElevator()
        {
            queueMutex.WaitOne();
           // Thread.Sleep(500);
            writer.WriteLine($"Agent {name} is calling the elevator from {currentFloor}!");
            elevator.Call(currentFloor);
            EnterElevator();
        }

        public void ChooseDestination()
        {
            if (!GoHomeTimeFlag)
            {
                Thread.Sleep(500);
                do
                {
                    Floor destination = PressButton();
                    writer.WriteLine($"Agent {name} pressed button for floor {destination}!");
                    elevator.Call(destination);
                } while (elevator.door == Door.Closed);
                LeaveElevator();
            }
            else
            {
                Thread.Sleep(500);
                do
                {
                    writer.WriteLine($"Agent {name} pressed button for floor {Floor.G}!");
                    elevator.Call(Floor.G);
                } while (elevator.door == Door.Closed);
                LeaveElevator();
            }
        }

        public void GoHome()
        {
            writer.WriteLine($"It's time for Agent {name} to go home!");
            GoHomeTimeFlag = true;
            CallElevator();
            writer.WriteLine($"Agent {name} is leaving the building!");
            atHomeEvent.Set();
        }

        protected void Roam()
        {
            writer.WriteLine($"Agent {name} [{secLevel}] entered the building!");
            Thread.Sleep(500);
            while (workDone != 100)
            {
                CallElevator();
                DoSomeWork();
            }
            GoHome();
        }
        public void RoamThreadWorker()
        {
            var t = new Thread(Roam);
            t.Start();
        }

        public void DoSomeWork()
        {
            writer.WriteLine($"Agent {name} is doing some work at floor {currentFloor}! Work Done: {workDone}%\n");
           // Thread.Sleep(3000);
            var randNum = rand.Next(10, 30);
            if (workDone + randNum > 100)
                workDone = 100;
            else
                workDone += randNum;
        }
        public Floor PressButton()
        {
          //  Thread.Sleep(500);
            List<Floor> availableButtonsList = elevator.GetAvailableButtons().ToList();
            int randomNum = rand.Next(1000);
            int index = randomNum % 3;
            Floor chosenFloor = availableButtonsList[index];
            return chosenFloor;
        }

    }
}
