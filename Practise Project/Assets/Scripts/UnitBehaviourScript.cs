using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public enum UnitType { InfintryVehikle, ReconVehicle, Tank };
    public enum ImpactType { ForestImpact};
    public enum TerrainType { Plain, Forest}

    public class UnitBehaviourScript : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class Unit
    {
        //base varibles
        public UnitType type;
        public Object GraficUnitRef;

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

        public Unit(UnitType type)
        {
            this.type = type;
            string StatPath = type.ToString("F") + ".stat.dat";
            string[] stats = System.IO.File.ReadAllLines(StatPath);
            this.attack = System.Convert.ToInt32(stats[1]);
            this.health = System.Convert.ToInt32(stats[2]);
            this.accurancy = System.Convert.ToDouble(stats[3]);
            this.stealthness = System.Convert.ToDouble(stats[4]);
            this.ammo = System.Convert.ToInt32(stats[5]);
            this.range = System.Convert.ToInt32(stats[6]);
            this.brusts = System.Convert.ToInt32(stats[7]);
            this.speed = System.Convert.ToInt32(stats[8]);
        }
    }
    public class Impact
    {
        public ImpactType type;
        private double accurancy;
        private double stealthness;

        public Impact(ImpactType type)
        {
            this.type = type;
            string StatPath = type.ToString("F") + ".impact.dat";
            string[] stats = System.IO.File.ReadAllLines(StatPath);
            this.accurancy = System.Convert.ToDouble(stats[1]);
            this.stealthness = System.Convert.ToDouble(stats[2]);
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

