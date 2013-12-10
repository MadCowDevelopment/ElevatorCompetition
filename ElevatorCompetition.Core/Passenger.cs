using System;

namespace ElevatorCompetition.Core
{
    public class Passenger
    {
        private bool _isWaiting = true;
        public Direction Direction { get; private set; }
        public int InitialFloor { get; private set; }
        public int DesiredFloor { get; private set; }

        public int Patience { get; private set; }
        public int InitialPatience { get; private set; }

        public Passenger(Direction direction, int initialFloor, int desiredFloor, int patience)
        {
            Direction = direction;
            InitialFloor = initialFloor;
            DesiredFloor = desiredFloor;
            InitialPatience = patience;
            Patience = InitialPatience;
        }

        public event Action<Passenger, int> Delivered;
        public event Action<Passenger> PickedUp;
        public event Action<Passenger> GaveUp;

        private void RaiseGaveUp()
        {
            var handler = GaveUp;
            if (handler != null) handler(this);
        }

        private void RaisePickedUp()
        {
            var handler = PickedUp;
            if (handler != null) handler(this);
        }

        private void RaiseDelivered()
        {
            var handler = Delivered;
            if (handler != null) handler(this, Patience);
        }

        public bool Deliver(int floor)
        {
            if (floor == DesiredFloor)
            {
                RaiseDelivered();
                return true;
            }

            return false;
        }

        public void Update()
        {
            Patience -= 1;
            if (Patience == 0  && !_isWaiting)
            {
                RaiseGaveUp();
            }
        }

        public bool Pickup()
        {
            _isWaiting = false;
            RaisePickedUp();
            return true;
        }
    }

    public static class Rng
    {
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        public static int Next(int min, int max)
        {
            return Random.Next(min, max);
        }
    }
}