using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using ElevatorCompetition;
using ElevatorCompetition.Core;

namespace SampleElevators
{
    public class CrowdFirstElevatorControl : ElevatorControl
    {
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

            if (Passengers.Any())
            {
                if (Passengers.Count(p => p.DesiredFloor > CurrentFloor) >
                    Passengers.Count(p => p.DesiredFloor < CurrentFloor))
                {
                    MoveUp();
                }
                else
                {
                    MoveDown();
                }
            }
            else
            {
                if (building.GetAllPassengers().Count(p => p.InitialFloor > CurrentFloor) >
                    building.GetAllPassengers().Count(p => p.InitialFloor < CurrentFloor))
                {
                    MoveUp();
                }
                else
                {
                    MoveDown();
                }
            }
        }
    }
}