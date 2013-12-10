using System.Collections.Generic;
using System.Linq;

namespace ElevatorCompetition.Core
{
    public class Building
    {
        private readonly List<Floor> _floors = new List<Floor>();

        public Building(int numberOfFloors)
        {
            _floors = new List<Floor>();
            for (var i = 0; i < numberOfFloors; i++)
            {
                _floors.Add(new Floor(i));
            }
        }

        public void SpawnPassenger(Passenger passenger)
        {
            passenger.PickedUp += passenger_PickedUp;
            passenger.GaveUp += passenger_GaveUp;
            _floors[passenger.InitialFloor].AddPassenger(passenger);
        }

        private void passenger_GaveUp(Passenger passenger)
        {
            UnscubscribeAndRemovePassenger(passenger);
        }

        private void passenger_PickedUp(Passenger passenger)
        {
            UnscubscribeAndRemovePassenger(passenger);
        }

        private void UnscubscribeAndRemovePassenger(Passenger passenger)
        {
            passenger.PickedUp -= passenger_PickedUp;
            passenger.GaveUp -= passenger_GaveUp;
            _floors[passenger.InitialFloor].RemovePassenger(passenger);
        }

        public List<Floor> GetFloors()
        {
            return new List<Floor>(_floors);
        }

        public List<Passenger> GetPassengersOnFloor(int floorNumber)
        {
            var floor = _floors.FirstOrDefault(p => p.Number == floorNumber);
            if (floor == null)
            {
                return new List<Passenger>();
            }

            return floor.GetPassengers();
        }

        public List<Passenger> GetAllPassengers()
        {
            return new List<Passenger>(_floors.SelectMany(p => p.GetPassengers()));
        }

        public void Update()
        {
            GetAllPassengers().ForEach(p => p.Update());
        }
    }
}