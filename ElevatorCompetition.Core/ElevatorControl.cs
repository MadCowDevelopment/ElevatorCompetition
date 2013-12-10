using System.Collections.Generic;

namespace ElevatorCompetition.Core
{
    public abstract class ElevatorControl
    {
        public int CurrentFloor { get; private set; }
        protected int MaxFloor { get; private set; }
        protected int Capacity
        {
            get { return 10; }
        }

        public int NumberOfPassengers { get { return Passengers.Count; }}

        protected readonly List<Passenger> Passengers = new List<Passenger>();
        private bool _turnTaken;

        protected ElevatorControl()
        {
            CurrentFloor = 0;
        }

        public void Initialize(int numberOfFloors)
        {
            MaxFloor = numberOfFloors - 1;
        }

        public void Update(Building building)
        {
            _turnTaken = false;
            OnUpdate(building);
        }

        protected abstract void OnUpdate(Building building);


        protected void MoveUp()
        {
            if (_turnTaken) return;
            _turnTaken = true;

            if (CurrentFloor < MaxFloor)
            {
                CurrentFloor++;
            }
        }

        protected void MoveDown()
        {
            if (_turnTaken) return;
            _turnTaken = true;

            if (CurrentFloor > 0)
            {
                CurrentFloor--;
            }
        }

        protected void PickupPassenger(Passenger passenger)
        {
            if (_turnTaken) return;
            _turnTaken = true;

            if (Passengers.Count >= Capacity)
            {
                return;
            }

            if (passenger.Pickup())
            {
                Passengers.Add(passenger);
            }
        }

        protected void DeliverPassenger(Passenger passenger)
        {
            if (_turnTaken) return;
            _turnTaken = true;

            if (passenger.Deliver(CurrentFloor))
            {
                Passengers.Remove(passenger);
            }
        }
    }
}