namespace SpaceCommander.Test
{
    public class GunMachine : AI.SpaceShip
    {
        protected override void StatsUp()
        {
            type = Mechanics.UnitClass.LR_Corvette;
            radarRange = 2500; //set in child
            radarPover = 10;
            speedThrust = 10; //set in child
            speedShift = 10;
            speedRotation = 30;
            stealthness = 1f; //set in child
            radiolink = 5f;
            movementAiEnabled = false;
            selfDefenceModuleEnabled = false;
        }
        protected override void DecrementLocalCounters()
        {
        }
    }
}
