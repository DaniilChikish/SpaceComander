using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public enum UnitType { InfintryVehikle, ReconVehicle, Tank };
    public enum UnitStateType { Move, Fire, MoveAndFire, Waiting, SkippingStep };
    public enum ImpactType { ForestStaticImpact};
    public enum TerrainType { Plain, Forest };
    public enum Team { Green, Red, Blue};


    public class MovementController
    {
        public int backCount;
        public bool MoveTo(Unit walker, TerraCell destination)
        {
            return false;
        }
    }
    public class ShootController
    {
        public int backCount;
        public bool ShootHim(Unit shooter, Unit target)
        {
            return false;
        }
    }

    public static class ObjectCreator
    {
        public static Unit RestoreUnit(UnitType type, UnityEngine.Object GraficUnitRef)
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
    }
}

