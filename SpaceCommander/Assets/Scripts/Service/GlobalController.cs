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

namespace SpaceCommander
{
    public class GlobalController : MonoBehaviour
    {
        public List<SpaceShip> unitList; // список 
        public List<SpaceShip> selectedList; // спиков выделенных объектов
        public Army playerArmy;

        public double[] RandomNormalPool;
        public double RandomNormalAverage;
        //public double[] RandomExponentPool;
        private float randomPoolBackCoount;
        //settings
        private GameSettings settings;
        public GameSettings Settings { get { return settings; } set { settings = value; }}
        public SoundStorage Sound { get; private set; }
        public PrefabStorage Prefab { get; private set; }
        public SpecINIHandler SpecINI { get; private set; }
        //texts
        private TextINIHandler localTexts;
        public string Texts(string key)
        {
            return localTexts.GetText("Text." + Settings.Localisation.ToString(), key);
        }
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
        private void OnEnable()
        {
            LoadSettings();
            LoadTexts();
            LoadSpec();
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
        private void Start()
        {

        }
        public void LoadTexts()
        {
            this.localTexts = new TextINIHandler(Application.streamingAssetsPath + "\\localisation_base.ini");
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
            {
                int alies = 0;
                int enemy = 0;
                foreach (Unit x in unitList)
                {
                    if (x.Team == this.playerArmy)
                        alies++;
                    else enemy++;
                }
                if (enemy == 0)
                    return 1;
                else if (alies == 0)
                    return -1;
                else return 0;
            }
        }
    }
    /**
     * Deprecated
     * **/
    public class NavmeshMovementController// : IDriver
    {
        private DriverStatus status;
        private SpaceShip walker;
        private Transform walkerTransform;
        private NavMeshAgent walkerAgent;
        private Queue<Vector3> path; //очередь путевых точек
        public Vector3 Velocity { get { return walkerAgent.velocity; } }
        public int PathPoints
        {
            get
            {
                if ((walkerAgent.pathEndPosition - walkerTransform.position).magnitude < 1)
                    return path.Count;
                else return path.Count + 1;
            }
        }
        public DriverStatus Status { get { return status; } }
        public Vector3 NextPoint { get { if (PathPoints > 1) return walkerAgent.pathEndPosition; else return Vector3.zero; } }
        //public float backCount; //время обновления пути.
        public NavmeshMovementController(GameObject walker)
        {
            this.walkerTransform = walker.transform;
            path = new Queue<Vector3>();
            walkerAgent = walker.GetComponent<NavMeshAgent>();
            walkerAgent.SetDestination(walker.transform.position);
            this.walker = walker.GetComponent<SpaceShip>();
            UpdateSpeed();
            //Debug.Log("Driver online");
        }
        public void Update()
        {
            UpdateSpeed();
            if ((walkerAgent.pathEndPosition - walkerTransform.position).magnitude < 10)
            {
                if (path.Count > 1)
                {
                    //Debug.Log("1");
                    //backCount = Vector3.Distance(walker.transform.position, path.Peek()) / (walker.GetComponent<NavMeshAgent>().speed*0.9f);
                    walkerAgent.SetDestination(path.Dequeue());
                }
                if (path.Count == 1 && (walkerAgent.pathEndPosition - walkerTransform.position).magnitude < 1)
                {
                    //backCount = Vector3.Distance(walker.transform.position, path.Peek()) / (walker.GetComponent<NavMeshAgent>().speed * 0.9f);
                    walkerAgent.SetDestination(path.Dequeue());
                }
            }
            //else backCount -= Time.deltaTime;
        }
        public void UpdateSpeed()
        {
            float distance = Vector3.Distance(walkerAgent.destination, walkerTransform.position);
            //Debug.Log(distance +" - "+ walker.gameObject.name);
            if (distance > 250)
                walkerAgent.speed = walker.Speed * 2.5f;
            else if (distance > 150)
                walkerAgent.speed = walker.Speed * 2f;
            else if (distance > 70)
                walkerAgent.speed = walker.Speed * 1.5f;
            else
                walkerAgent.speed = walker.Speed;
            walkerAgent.acceleration = walker.Speed * 1.6f;
            if (walker.CurrentTarget == null)
                walkerAgent.angularSpeed = walker.Speed * 3.3f;
            else
                walkerAgent.angularSpeed = walker.Speed * 0.05f;
        }
        public bool MoveTo(Vector3 destination)
        {
            ClearQueue();
            return MoveToQueue(destination);
        }
        public bool MoveToQueue(Vector3 destination)
        {
            if (path.Count < 10)
            {
                UpdateSpeed();
                path.Enqueue(destination);
                //backCount = Vector3.Distance(walker.transform.position, destination) / (walker.GetComponent<NavMeshAgent>().speed - 2);
                return true;
            }
            else return false;
        }
        public void ClearQueue()
        {
            walker.GetComponent<NavMeshAgent>().ResetPath();
            //backCount = 0;
            path.Clear();
        }

        public void BuildPathArrows()
        {
            throw new NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new NotImplementedException();
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
