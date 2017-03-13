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

    public static class TacticControler
    {
        internal static int Distance(Unit unitX, Unit unitY)
        {
            throw new NotImplementedException();
        }

        public static double Random(int min, int max)
        {
            System.Security.Cryptography.RNGCryptoServiceProvider random = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] arr = new byte[1];
            random.GetBytes(arr);
            return min + ((arr[0] / 256.0) * (max - min));
        }
    }

    public class MovementController
    {
        public int backCount;
        public bool MoveTo(Unit walker, TerraCell destination)
        {
		walker.state = UnitStateType.Move;
            return false;
        }
		void Update()
        {
		backCount--;
        }
    }
    public class ShootController
    {
        public int backCount;
        public bool ShootHim(Unit shooter, Unit target)
        {
            return false;
        }
		void Update()
        {
		backCount--;
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
