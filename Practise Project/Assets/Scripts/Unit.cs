using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{

    public class Unit : MonoBehaviour
    {
        
        
        //base varibles
        public UnitClass type;
        public UnitStateType state;
        public Team alliesArmy;
        public bool isSelected;
        public string UnitName;

        //selection
        public float NameFrameOffset;
        public Texture Selection;
        public float SelectionFrameWidth;
        public float SelectionFrameHeight;
        public float SelectionFrameOffset;

        //depend varibles
        public float Health;
        public GameObject DieBlast;
        public float Accurancy;
        public float Stealthness;
        public float Radiolink = 1.5f;
        //independ varibles

        //constants
        public float MaxHealth;
        public float RadarRange;
        public int Speed;
        //controllers
        //public List<ImpactType> impacts;
        private MovementController Driver; //make private after debug
        private ShootController Gunner;
        private GlobalController Global;
        private float synchAction;

        public List<GameObject> enemys;
        public GameObject CurrentTarget;

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
        void Start()
        {
            UnitName = type.ToString();
            Driver = new MovementController(this.gameObject);
            Gunner = new ShootController(this.gameObject);
            Global = FindObjectsOfType<GlobalController>()[0];
            Global.unitList.Add(gameObject);
        }
        void Update()
        {
            if (Health < 0)
                Die();
            Driver.Update();
            Gunner.Update();

            if (synchAction > 0)
                synchAction -= Time.deltaTime;
            else
            {
                synchAction = 0.25f;
                ChoiseNextAction();
            }
        }
        private void FixedUpdate()
        {

        }
        void OnGUI()
        {
            if (isSelected)
            {
                Vector3 crd = Camera.main.WorldToScreenPoint(transform.position);
                crd.y = Screen.height - crd.y;

                GUIStyle style = new GUIStyle();
                style.fontSize = 12;
                //style.font = GuiProcessor.getI.rusfont;
                style.normal.textColor = Color.cyan;
                style.alignment = TextAnchor.MiddleCenter;
                //style.fontStyle = FontStyle.Italic;

                GUI.DrawTexture (new Rect (crd.x - SelectionFrameWidth/2, crd.y - SelectionFrameOffset, SelectionFrameWidth, SelectionFrameHeight), Selection);
                GUI.Label(new Rect(crd.x - 120, crd.y - NameFrameOffset, 240, 18), UnitName, style);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        this.Health = this.Health - collision.gameObject.GetComponent<Shell>().Damage;
                        break;
                    }
                case "Explosion":
                    {
                        float multiplicator = Mathf.Pow(((-Vector3.Distance(this.gameObject.transform.position, collision.gameObject.transform.position) + collision.gameObject.GetComponent<Explosion>().MaxRadius) * 0.01f), (1 / 3));
                        this.Health = this.Health - collision.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                        break;
                    }
            }
        }

        public void SelectUnit(bool isSelect)
        {
            if (isSelect)
            {
                gameObject.GetComponentInChildren<Camera>().enabled = true;
                Global.selectedList.Add(gameObject);
                isSelected = true;
            }
            else
            {
                gameObject.GetComponentInChildren<Camera>().enabled = false;
                isSelected = false;
            }
        }
        public void Die()
        {
            Instantiate(DieBlast, gameObject.transform.position, gameObject.transform.rotation);
            Global.selectedList.Remove(this.gameObject);
            Global.unitList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        //AI logick
        private void ChoiseNextAction()
        {
            enemys = Scan();
            if (enemys.Count > 0)
            {
                float distance = GetNearest();
                Gunner.SetAim(CurrentTarget);
                if (distance < Gunner.GetRangePrimary())
                    Gunner.ShootHimPrimary(CurrentTarget);
                else
                    Gunner.ShootHimSecondary(CurrentTarget);
            }
            else
            {
                enemys = RequestScout();
                if (enemys.Count > 0)
                {
                    float distance = GetNearest();
                    Gunner.SetAim(CurrentTarget);
                    if (distance < Gunner.GetRangePrimary())
                        Gunner.ShootHimPrimary(CurrentTarget);
                    else
                        Gunner.ShootHimSecondary(CurrentTarget);
                }
            }
            //...
        }
        private List<GameObject> Scan()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<Unit>().alliesArmy != alliesArmy)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange)
                    {
                        double multiplicator = Math.Pow(((-distance + RadarRange) * 0.02), (1 / 5));
                        if (Randomizer.Uniform(0, 100, 1)[0] < x.GetComponent<Unit>().Stealthness * multiplicator)
                            enemys.Add(x);
                    }
                }
            }
            return enemys;
        }
        private List<GameObject> RequestScout()
        {
            List<GameObject> enemys = new List<GameObject>();
            List<GameObject> allies = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<Unit>().alliesArmy == alliesArmy)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange*Radiolink)
                    {
                        enemys.AddRange(x.GetComponent<Unit>().GetScout());
                    }
                }
            }
            return enemys;
        }
        public List<GameObject> GetScout()
        {
            return Scan();
        }
        private float GetNearest()
        {
            float minDistance = RadarRange;
            int minIndex = 0;
            float distance;
            for (int i = 0; i < enemys.Count; i++)
            {
                distance = Vector3.Distance(this.transform.position, enemys[i].transform.position);
                if (distance<minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }        
            }
            CurrentTarget = enemys[minIndex];
            return minDistance;
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
    }
}
