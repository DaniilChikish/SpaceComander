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
        public int Health;
        public float Accurancy;
        public float Stealthness;
        public float Radiolink = 1.5f;
        //independ varibles

        //constants
        public int MaxHealth;
        public float RadarRange;
        public int Speed;
        //controllers
        //public List<ImpactType> impacts;
        private MovementController Driver; //make private after debug
        private ShootController Gunner;
        private GlobalController Global;
        public GameObject Primari;
        public GameObject Secondary;

        public List<GameObject> enemys;

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
            Gunner = new ShootController(Primari, Secondary);
            Global = FindObjectsOfType<GlobalController>()[0];
            Global.unitList.Add(gameObject);
        }
        void Update()
        {
            Driver.Update();
            Gunner.Update();
            ChoiseNextAction();
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

        //AI logick
        private void ChoiseNextAction()
        {
            enemys = Scan();
            if (enemys.Count > 0)
            {
                //Debug.Log("Enemy finded");
                Gunner.ShootHim(enemys[0]);
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
                    if ((Vector3.Distance(this.gameObject.transform.position, x.transform.position) < RadarRange))
                        if (Randomizer.Uniform(0, 100, 1)[0] < x.GetComponent<Unit>().Stealthness)
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
    }
}
