using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConcurrentPrgrammingExam
{
    public enum Floor { G = 1, S, T1, T2 };
    public enum Door { Open, Closed };
    public class Elevator
    {
        public List<Floor> floors = new List<Floor>() { Floor.G, Floor.S, Floor.T1, Floor.T2 };
        public Floor currentFloor = Floor.G;
        public Agent agentInElevator = null;
        public Door door = Door.Closed;
        public TextWriter writer;
        public Elevator(TextWriter writer)
        {
            this.writer = writer;
        }
        public bool SecutrityCheck(Agent agent, Floor destination)
        {
            return agent.accessableFloors.Contains(destination);
        }
        public void Enter(Agent agent)
        {
            SetAgentInElevator(agent);
        }
        public void Leave()
        {
            SetAgentInElevator(null);
        }
        public bool IsAgentInElevator()
        {
            return agentInElevator != null;
        }
        public void SetAgentInElevator(Agent agent)
        {
            agentInElevator = agent;
        }
        public Floor GetCurrentFloor()
        {
            return currentFloor;
        }
        public void Call(Floor floorCalled)
        {
            
            if (currentFloor != floorCalled)
            {
                door = Door.Closed;
              if(!IsAgentInElevator()) writer.WriteLine($"Elevator was called at {floorCalled}");
                MoveThroughFloors(floorCalled);
            }
            else
            {
                writer.WriteLine($"Elevator is already at {floorCalled}");
                OpenDoors();
            }
        }
        public void MoveThroughFloors(Floor destination)
        {
            for(int i = 0; i < Math.Abs(destination - currentFloor); i++)
            {
            //    Thread.Sleep(1000);
                writer.Write("\t*");
            }
            currentFloor = destination;
            writer.WriteLine($"\nElevator arrived at {destination}");
            if (IsAgentInElevator())
            {
                OpenDoors(agentInElevator, destination);
            }
            else
            {
                OpenDoors();
            }
        }
        public void OpenDoors()
        {
            writer.WriteLine("Elevator opened!");
            door = Door.Open;
        }

        public void OpenDoors(Agent agent, Floor destination)
        {
            if (SecutrityCheck(agent, destination))
            {
                door = Door.Open;
                writer.WriteLine("Elevator opened!");
            }
            else
            {
                writer.WriteLine("		  ----------------------------------------------");
                writer.WriteLine("\t\t  Δ=> Agent has unsufficient Security Level! <=Δ");
                writer.WriteLine("\t\tPlease push a button according to your Security Level!");
                writer.WriteLine("		------------------------------------------------------");
            }
        }
        public IEnumerable<Floor> GetAvailableButtons()
        {
           foreach(var el in floors)
            {
                if (!el.Equals(currentFloor))
                    yield return el;
            }
        }
    }
}
