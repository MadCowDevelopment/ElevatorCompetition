using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorCompetition.Core
{
    public sealed class Simulation
    {
        private const int NumberOfFloors = 10;

        private const int TotalNumberOfTicks = 600;

        private List<ElevatorEnvironment> _environments;

        private int _tickCount;

        private readonly TimeSpan _timePerTick = TimeSpan.FromMilliseconds(10);

        public void Start(List<ElevatorControl> controls)
        {
            _environments = new List<ElevatorEnvironment>();
            foreach (var elevatorControl in controls)
            {
                _environments.Add(new ElevatorEnvironment(new Building(NumberOfFloors), elevatorControl));
                elevatorControl.Initialize(NumberOfFloors);
            }

            var lastTick = DateTime.Now;
            do
            {
                if (DateTime.Now - lastTick <= _timePerTick) continue;
                lastTick = DateTime.Now;
                _tickCount++;

                SpawnPassenger();
                foreach (var environment in _environments)
                {
                    environment.Update();
                }

                Render();
            } while (_tickCount < TotalNumberOfTicks);

            Console.WriteLine();
            Console.WriteLine();
            var offset = 0;
            foreach (var elevatorEnvironment in _environments)
            {
                var pos = Console.CursorTop;
                Console.SetCursorPosition(offset, pos);
                Console.Write("Score: {0}", elevatorEnvironment.Score);
                offset += 15;
            }
        }

        private void Render()
        {
            Console.Clear();

            var offset = 0;
            foreach (var elevatorEnvironment in _environments)
            {
                Print(elevatorEnvironment, offset);
                offset += 15;
            }
   
            Console.SetCursorPosition(0, NumberOfFloors + 1);
            Console.Write("Ticks: {0}", _tickCount);
        }

        private static void Print(ElevatorEnvironment elevatorEnvironment, int offset)
        {
            foreach (var floor in elevatorEnvironment.Building.GetFloors())
            {
                Console.SetCursorPosition(offset + 0, NumberOfFloors - floor.Number - 1);
                Console.Write("{0}: ", floor.Number);
                Console.SetCursorPosition(offset + 4, NumberOfFloors - floor.Number - 1);
                Console.Write("{0}", floor.NumberOfPassengers);
            }

            Console.SetCursorPosition(offset + 7, NumberOfFloors - elevatorEnvironment.ElevatorControl.CurrentFloor - 1);
            Console.Write(elevatorEnvironment.ElevatorControl.NumberOfPassengers);
        }

        private void SpawnPassenger()
        {
            if (_tickCount % 3 != 0) return;
            var floorNumber = Rng.Next(0, NumberOfFloors);
            int desiredFloor;
            do
            {
                desiredFloor = Rng.Next(0, NumberOfFloors);
            } while (desiredFloor == floorNumber);

            var direction = floorNumber < desiredFloor ? Direction.Up : Direction.Down;
            var patience = Rng.Next(10, 20);
            foreach (var elevatorEnvironment in _environments)
            {
                var passenger = new Passenger(direction, floorNumber, desiredFloor, patience);
                elevatorEnvironment.SpawnPassenger(passenger);
            }
        }
    }
}