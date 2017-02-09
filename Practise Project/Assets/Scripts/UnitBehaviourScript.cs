using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public enum UnitType { InfintryVehikle, ReconVehicle, Tank };
    public enum UnitStateType { Move, Fire, MoveAndFire, Waiting, SkippingStep }
    public enum ImpactType { ForestStaticImpact, StunningDynamicImpact, SmokescreenDynamicImpact };
    public enum TerrainType { Plain, Forest }


    public static class Controller
    {
        public static void MoveTo(Unit walker, Terrain destination)
        {

        }
    }

    public static class ObjectCreator
    {
        public static Unit CreateUnit(UnitType type, Object GraficUnitRef)
        {
            string StatPath = type.ToString("F") + ".stat.dat";
            string[] stats = System.IO.File.ReadAllLines(StatPath);
            Unit newUnit = new Unit(
                Convert.ToInt32(stats[1]),
                Convert.ToInt32(stats[2]),
                Convert.ToDouble(stats[3]),
                Convert.ToDouble(stats[4]),
                Convert.ToInt32(stats[5]),
                Convert.ToInt32(stats[6]),
                Convert.ToInt32(stats[7]),
                Convert.ToInt32(stats[8]));
            newUnit.type = type;
            newUnit.graficUnitRef = GraficUnitRef;
            return newUnit;
        }
        public static Impact CreateImpact(ImpactType type, bool isStatic)
        {
            if (isStatic)
            {
                string StatPath = type.ToString("F") + ".impact.dat";
                string[] stats = System.IO.File.ReadAllLines(StatPath);
                Impact newImpact = new Impact(
                    Convert.ToInt32(stats[1]),
                    Convert.ToInt32(stats[2]));
                newImpact.type = type;
                return newImpact;
            }
            else
            {
                switch (type)
                {
                    case ImpactType.StunningDynamicImpact:
                        {
                            return new StunningImpact(30);
                        }
                    case ImpactType.SmokescreenDynamicImpact:
                        {
                            return new SmokescreenImpact(45);
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
        }
    }

    public class Unit
    {
        //base varibles
        public UnitType type;
        public UnitStateType state;
        public Object graficUnitRef;
        public Terrain location;
        public string playerArmy;

        //depend varibles
        private int attack;
        private int health;
        private double accurancy;
        private double stealthness;
        //independ varibles
        private int ammo;
        //constants
        private int range;
        private int brusts;
        private int speed;

        public ImpactType impacts;

        public Unit(int attack, int health, double accurancy, double stealthness, int ammo, int range, int brusts, int speed)
        {
            this.attack = attack;
            this.health = health;
            this.accurancy = accurancy;
            this.stealthness = stealthness;
            this.ammo = ammo;
            this.range = range;
            this.brusts = brusts;
            this.speed = speed;
        }

        internal void SkipStep()
        {
            throw new NotImplementedException();
        }

        private Terrain FindMoveTarget()
        {
            return null;
        }
        public void Move()
        {
            this.state = UnitStateType.Move;
            Terrain destination = FindMoveTarget();
            if (destination != null)
                Controller.MoveTo(this, destination);
        }
    }
    public class Impact
    {
        public ImpactType type;
        protected double accurancy;
        protected double stealthness;

        public Impact(double accurancy, double stealthness)
        {
            this.accurancy = accurancy;
            this.stealthness = stealthness;
        }
        public virtual bool Operate(Unit owner) { return true; }
    }
    public class StaticImpact : Impact
    {
        public StaticImpact(double accurancy, double stealthness) : base(accurancy, stealthness) { }
    }
    public class StunningImpact : Impact
    {
        private int timeOfLife;
        public StunningImpact(int timeOfLife) : base(0, 1.1)
        {
            this.timeOfLife = timeOfLife;
        }
        public override bool Operate(Unit owner)
        {
            if (timeOfLife > 0)
            {
                owner.SkipStep();
                timeOfLife--;
                return true;
            }
            else return false;
        }
    }
    public class SmokescreenImpact : Impact
    {
        private int timeOfLife;
        public SmokescreenImpact(int timeOfLife) : base(0.2, 2)
        {
            this.timeOfLife = timeOfLife;
        }
        public override bool Operate(Unit owner)
        {
            if (timeOfLife > 0)
            {
                owner.Move();
                timeOfLife--;
                return true;
            }
            else return false;
        }
    }


    public class Terrain
    {
        public TerrainType type;
        public Object GraficUnitRef;

        public bool Busy;
        public List<Terrain> Neighbors;
    }
}

