namespace ElevatorCompetition.Core
{
    internal class ElevatorEnvironment
    {
        public Building Building { get; private set; }
        public ElevatorControl ElevatorControl { get; private set; }

        public int Score { get; private set; }

        public ElevatorEnvironment(Building building, ElevatorControl elevatorControl)
        {
            Building = building;
            ElevatorControl = elevatorControl;
        }

        public void Update()
        {
            ElevatorControl.Update(Building);
            Building.Update();
        }

        public void SpawnPassenger(Passenger passenger)
        {
            Building.SpawnPassenger(passenger);
            passenger.GaveUp += passenger_GaveUp;
            passenger.Delivered += passenger_Delivered;
        }

        void passenger_GaveUp(Passenger sender)
        {
            sender.GaveUp -= passenger_GaveUp;
            Score -= sender.InitialPatience;
        }

        private void passenger_Delivered(Passenger sender, int patience)
        {
            sender.Delivered -= passenger_Delivered;
            Score += patience;
        }
    }
}