using System.Collections.Generic;

namespace ElevatorCompetition.Core
{
    public class Floor
    {
        private readonly List<Passenger> _passengers;

        public Floor(int number)
        {
            _passengers = new List<Passenger>();
            Number = number;
        }

        public void AddPassenger(Passenger passenger)
        {
            _passengers.Add(passenger);
        }

        public int NumberOfPassengers
        {
            get { return _passengers.Count; }
        }

        public int Number { get; private set; }

        public List<Passenger> GetPassengers()
        {
            return new List<Passenger>(_passengers);
        }

        public void RemovePassenger(Passenger passenger)
        {
            _passengers.Remove(passenger);
        }
    }
}