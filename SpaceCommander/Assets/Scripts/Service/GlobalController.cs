using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//using SpaceCommander.Units;
using DeusUtility.Random;
using DeusUtility.UI;
using SpaceCommander.Service;
using SpaceCommander.Scenarios;

namespace SpaceCommander
{
    public class GlobalController : MonoBehaviour
    {
        private static GlobalController instance;
        public void OnEnable()
        {
            Initialise();
            instance = this;
        }
        public static GlobalController GetInstance()
        {
            if (instance == null)
            {
                GameObject newGlobal = new GameObject();
                instance = newGlobal.AddComponent<GlobalController>();
                instance.Initialise();
            }
            return instance;
        }
        //private void Start()
        //{
        //    Initialise();
        //}
        public void Initialise()
        {
            if (!handlersInitialised)
                InitialiseHandlers();
            InitialiseReference();            
        }
        public List<SpaceShip> unitList; // список 
        public List<SpaceShip> selectedList; // спиков выделенных объектов
        public Army playerArmy;

        public double[] RandomNormalPool;
        public double RandomNormalAverage;
        //public double[] RandomExponentPool;
        private float randomPoolBackCoount;
        //settings
        private static bool handlersInitialised;
        private static GameSettings settings;
        public GameSettings Settings { get { return settings; } set { settings = value; }}
        private static SpecINIHandler specIni;
        public SpecINIHandler SpecINI { get { return specIni; } private set { specIni = value; } }
        //texts
        private static TextINIHandler localTexts;
        public string Texts(string key)
        {
            return localTexts.GetText("Text." + Settings.Localisation.ToString(), key);
        }
        public SoundStorage Sound { get; private set; }
        public PrefabStorage Prefab { get; private set; }
        private Scenario Mission;
        public string MissionName
        {
            get
            {
                if (Mission != null)
                    return Mission.Name;
                else return SceneManager.GetActiveScene().name;
            }
        }
        public string MissionBrief
        {
            get
            {
                if (Mission != null)
                    return Mission.Brief;
                else return "Eliminate all enemies!";
            }
        }
        private ShipManualController manualController;
        public ShipManualController ManualController { get { return manualController; } }
        private void InitialiseHandlers()
        {
            LoadSettings();
            LoadTexts();
            LoadSpec();
            handlersInitialised = true;
        }
        private void InitialiseReference()
        {
            Sound = FindObjectOfType<SoundStorage>();
            Prefab = FindObjectOfType<PrefabStorage>();
            Mission = FindObjectOfType<Scenario>();
            manualController = FindObjectOfType<ShipManualController>();
        }
        private void LoadSettings()
        {
            settings = new GameSettings();
            Settings.Load();
        }
        private void LoadSpec()
        {
            SpecINI = new SpecINIHandler(Application.streamingAssetsPath + "\\spec.ini");
        }
        public void LoadTexts()
        {
            GlobalController.localTexts = new TextINIHandler(Application.streamingAssetsPath + "\\localisation_base.ini");
        }

        public void Update()
        {
            if (randomPoolBackCoount < 0)
            {
                RandomNormalPool = Randomizer.Normal(1, 1, 128, 0, 128);
                RandomNormalAverage = RandomNormalPool.Average();
                //RandomExponentPool = Randomizer.Exponential(7, 128, 0, 128);
                randomPoolBackCoount = 10;
            }
            else randomPoolBackCoount -= Time.deltaTime;
        }
        public int CheckVictory()
        {
            if (Mission != null)
                return Mission.CheckVictory();
            else
                return Scenario.DefaultOrder();
        }
    }
    public class ShootController : IGunner
    {
        private IWeapon[][] weapons;
        public SpaceShip owner;
        private Unit target;
        private float targetLockdownCount;
        //private Vector3 oldTargetPosition;
        //private Vector3 aimPoint; //точка сведения
        private IShield shield;
        private float[] synchWeapons;
        private int[] indexWeapons;
        //private float averageRoundSpeed;
        //private float averageRange;
        public Unit Target { get { return target; } }
        public IWeapon[][] Weapon { get { return weapons; } }
        public ShootController(SpaceShip body)
        {
            this.owner = body;
            List<IWeapon[]> buffer = new List<IWeapon[]>();
            for (int i = 0; i < body.transform.childCount; i++)
            {
                IWeapon[] buffer2 = body.transform.GetChild(i).GetComponentsInChildren<IWeapon>();
                if (buffer2.Length > 0)
                {
                    buffer.Add(buffer2);
                }
            }

            weapons = buffer.ToArray();
            synchWeapons = new float[weapons.Length];
            indexWeapons = new int[weapons.Length];

            shield = body.GetShieldRef;
            //Debug.Log("Gunner online");
        }
        public bool ShootHim(int slot)
        {
            if (synchWeapons[slot] <= 0)
            {
                float angel;
                if (target != null) angel = Vector3.Angle(target.transform.position - owner.transform.position, owner.transform.forward);
                else angel = 0;
                if (angel < weapons[slot][0].Dispersion * 5 || angel < weapons[slot][0].AimAngle * 1.1f)
                {
                    if (indexWeapons[slot] >= weapons[slot].Length)
                        indexWeapons[slot] = 0;
                    if (weapons[slot][indexWeapons[slot]].IsReady)
                    {
                        shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                        synchWeapons[slot] = (60f / this.weapons[slot][0].Firerate) / this.weapons[slot].Length;
                        bool output = weapons[slot][indexWeapons[slot]].Fire();
                        indexWeapons[slot]++;
                        return output;
                    }
                    else indexWeapons[slot]++;
                }
            }
            return false;
        }
        public bool Volley(int slot)//relative cooldown indexWeapon, ignore angel;
        {
            if (indexWeapons[slot] >= weapons[slot].Length)
                indexWeapons[slot] = 0;
            if (synchWeapons[slot] <= 0 && weapons[slot][indexWeapons[slot]].IsReady)
            {
                int i = 0;
                shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                synchWeapons[slot] = 60f / this.weapons[slot][0].Firerate;
                indexWeapons[slot]++;
                for (i = 0; i < weapons[slot].Length; i++)
                {
                    weapons[slot][i].Fire();
                }
                return true;
            }
            else return false;
        }
        public void Update()
        {
            for (int slot = 0; slot < weapons.Length; slot++)
                if (synchWeapons[slot] > 0)
                    synchWeapons[slot] -= Time.deltaTime;
            if (targetLockdownCount > 0)
                targetLockdownCount -= Time.deltaTime;
        }
        public bool SeeTarget()
        {
            if (target != null)
            {
                RaycastHit hit;
                Physics.Raycast(owner.transform.position, target.transform.position - owner.transform.position, out hit, 100000, (1 << 0 | 1 << 8)); //1 - default layer, 9 - terrain layer -1
                return (hit.transform == target.transform);
            }
            else return false;
        }
        public bool AimOnTarget()
        {
            if (target != null)
                return (Vector3.Angle(owner.transform.forward, target.transform.position - owner.transform.position) < 5f);
            else return false;
        }
        public bool TargetInRange(int slot)
        {
            return (target != null && Vector3.Distance(target.transform.position, owner.transform.position) < weapons[slot][0].Range);
        }
        public bool CanShoot(int slot)
        {
            if (indexWeapons[slot] >= weapons[slot].Length)
                indexWeapons[slot] = 0;
            return (weapons[slot][indexWeapons[slot]].IsReady || weapons[slot][indexWeapons[slot]].BackCounter <= 2);
        }
        public bool SetAim(Unit target, bool immediately, float lockdown)
        {
            if (this.target == null || targetLockdownCount < 0 || immediately)
            {
                this.target = target;
                targetLockdownCount = lockdown;
                for (int j = 0; j < weapons.Length; j++)
                    for (int i = 0; i < weapons[j].Length; i++)
                    {
                        weapons[j][i].Target = target;
                    }
                //Debug.Log("set target - " + Target.transform.position);
                //Debug.Log("set aim - " + oldTargetPosition);
                return true;
            }
            else return false;
        }
        public bool ResetAim()
        {
            this.target = null;
            for (int j = 0; j < weapons.Length; j++)
                for (int i = 0; i < weapons[j].Length; i++)
                {
                    weapons[j][i].Target = null;
                }
            return true;
        }
        public void ReloadWeapons()
        {
            for (int i = 0; i < weapons.Length; i++)
                for (int j = 0; j < weapons[i].Length; j++)
                {
                    weapons[i][j].Reset();
                }
        }
        public float GetRange(int slot)
        {
            return weapons[slot][0].Range;
        }
    }
}
