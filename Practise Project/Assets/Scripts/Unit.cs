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
        public Team alliesArmy;

        //depend varibles
        public int attack;
        public int health;
        public double accurancy;
        public double stealthness;
        //independ varibles
        public int ammo;
        //constants
        public int maxHealth;
        public double range;
        public int brusts;
        public int speed;
        //controllers
        public List<ImpactType> impacts;
        private MovementController Driver; //make private after debug
        private ShootController Gunner;
		private GlobalController Global;

   //     public Unit() { }
   //     public Unit(int attack, int health, double accurancy, double stealthness, int ammo, int range, int brusts, int speed)
   //     {
   //         this.attack = attack;
			//this.maxHealth = health;
   //         this.health = health;
   //         this.accurancy = accurancy;
   //         this.stealthness = stealthness;
   //         this.ammo = ammo;
   //         this.range = range;
   //         this.brusts = brusts;
   //         this.speed = speed;
   //     }

        public void SelectUnit(bool isSelect)
        {
            if (isSelect)
            {
                gameObject.GetComponentInChildren<Camera>().enabled = true;
                Global.selectedList.Add(gameObject);
            }
            else
            {
                gameObject.GetComponentInChildren<Camera>().enabled = false;
            }
        }

        //AI logick
        private void ChoiseNextAction()
        {
            List<GameObject> enemys = Scan();

            //...
        }


        private List<GameObject> Scan()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<Unit>().alliesArmy != alliesArmy)
                {
                    if ((TacticControler.Distance(this.gameObject, x) < range * Unit.VIEWCoof))
                        if (Randomizer.Uniform(0, 100, 1)[0] < x.GetComponent<Unit>().stealthness)
                            enemys.Add(x);
                }
            }
            return enemys;
        }

        internal void SkipStep()
        {

        }

        private Vector3 FindMoveTarget()
        {
            return new Vector3(0, 0, 0);
        }
        private void Move()
        {

            Driver.MoveTo(FindMoveTarget());

        }
        public void SendTo(Vector3 destination)
        {
            Driver.MoveTo(destination);
        }

        // Use this for initialization
        void Start()
        {
            Driver = new MovementController(this.gameObject);
            Gunner = new ShootController();
            Global = FindObjectsOfType<GlobalController>()[0];
            Global.unitList.Add(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            ChoiseNextAction();
        }
    }
}
