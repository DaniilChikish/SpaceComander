using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{

    public class Unit : MonoBehaviour
    {
        public const double VIEWCoof = 1.5;
        //base varibles
        public UnitType type;
        public UnitStateType state;
        public UnityEngine.Object graficUnitRef;
        public UnityEngine.Object location;
        public Team playerArmy;

        //depend varibles
        private int attack;
        private int health;
        private double accurancy;
        private double stealthness;
        //independ varibles
        private int ammo;
        //constants
        private double range;
        private int brusts;
        private int speed;
        //controllers
        public List<ImpactType> impacts;
        private MovementController Driver;
        private ShootController Gunner;

        public Unit() { }
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
        //AI logick
        private void ChoiseNextAction()
        {
            Dictionary<Unit, double> enemy = Scan();

            //...
        }


        private Dictionary<Unit, double> Scan()
        {
            //Unit[] units = (Unit)GetComponent("Units");
            Dictionary<Unit, double> enemy = new Dictionary<Unit, double>();

            //foreach (Unit x in units)
            //{
            //    if ((TacticControler.Distance(this, x) < range * Unit.VIEWCoof) && (TacticControler.Random(0, 100) < x.stealthness))
            //        enemy.Add(x, (TacticControler.Distance(this, x)));
            //}
            return enemy;
        }

        internal void SkipStep()
        {

        }

        private TerraCell FindMoveTarget()
        {
            return null;
        }
        public void Move()
        {
            //this.state = UnitStateType.Move;
            TerraCell destination = FindMoveTarget();
            if (destination != null)
            {
                Driver.MoveTo(this, destination);
            }
        }

        // Use this for initialization
        void Start()
        {
            Driver = new MovementController();
            Gunner = new ShootController();
        }

        // Update is called once per frame
        void Update()
        {
            ChoiseNextAction();
        }
    }
}
