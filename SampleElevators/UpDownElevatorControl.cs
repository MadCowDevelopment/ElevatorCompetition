using System.IO;
using System.Linq;
using ElevatorCompetition.Core;

namespace SampleElevators
{
    public class UpDownElevatorControl : ElevatorControl
    {
        private bool _movesUp;

        protected override void OnUpdate(Building building)
        {
            var passengersThatWantToGetOffHere = Passengers.Where(p => p.DesiredFloor == CurrentFloor).ToList();
            if (passengersThatWantToGetOffHere.Any())
            {
                DeliverPassenger(passengersThatWantToGetOffHere.First());
                return;
            }

            var passengerThatWantToGetInHere = building.GetPassengersOnFloor(CurrentFloor);
            if (passengerThatWantToGetInHere.Any() && Passengers.Count < Capacity)
            {
                PickupPassenger(passengerThatWantToGetInHere.First());
                return;
            }

            if (_movesUp)
            {
                if (CurrentFloor + 1 <= MaxFloor)
                {
                    MoveUp();
                }
                else
                {
                    _movesUp = !_movesUp;
                }
            }
            else
            {
                if (CurrentFloor - 1 >= 0)
                {
                    MoveDown();
                }
                else
                {
                    _movesUp = !_movesUp;
                }
            }
        }
    }
}
