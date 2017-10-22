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

namespace SpaceCommander
{
    public class GlobalController : MonoBehaviour
    {
        public List<SpaceShip> unitList; // список 
        public List<SpaceShip> selectedList; // спиков выделенных объектов
        public Army playerArmy;
        //selection
        public Texture AlliesSelectedGUIFrame;
        public Texture AlliesGUIFrame;
        public Texture EnemyGUIFrame;
        public Texture AlliesSelectedOutscreenPoint;
        public Texture AlliesOutscreenPoint;
        public Texture EnemyOutscreenPoint;
        public GameObject UnitaryShell;
        public AudioClip CannonShootSound;
        public GameObject ShellBlast;
        public GameObject RailgunShell;
        public GameObject LaserBeam;
        public GameObject PlasmaSphere;
        public GameObject Missile;
        public GameObject MissileTrap;
        public GameObject Torpedo;
        public GameObject MagMine;
        public GameObject MagnetoShell;
        public GameObject ExplosiveBlast;
        public GameObject EMIExplosionPrefab;
        public GameObject NukeBlast;
        public GameObject ShipDieBlast;
        public GameObject pathArrow;
        public GameObject greenBeam;
        public double[] RandomNormalPool;
        public double RandomNormalAverage;
        //public double[] RandomExponentPool;
        private float randomPoolBackCoount;
        //settings
        private INIHandler specINI;
        public INIHandler SpecINI { get { return specINI; } }
        private GameSettings settings;
        public GameSettings Settings { get { return settings; } set { settings = value; }}
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
        private void OnEnable()
        {
            LoadSettings();
            LoadTexts();
            LoadSpec();
            Mission = FindObjectOfType<Scenario>();
        }
        private void LoadSettings()
        {
            settings = new GameSettings();
            Settings.Load();
        }
        private void LoadSpec()
        {
            specINI = new INIHandler(Application.streamingAssetsPath + "\\spec.ini");
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
        private ForceShield shield;
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
                if (angel < weapons[slot][0].Dispersion * 5 || angel < 10)
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
            return weapons[slot][indexWeapons[slot]].IsReady;
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
    public abstract class Weapon : MonoBehaviour, IWeapon
    {
        public float DamageMultiplacator { set; get; }
        public float FirerateMultiplacator { set; get; }
        public float RangeMultiplacator { set; get; }
        public float DispersionMultiplicator { set; get; }
        public float RoundspeedMultiplacator { set; get; }
        public float APMultiplacator { set; get; }

        protected WeaponType type;
        protected GlobalController Global;
        protected Unit owner;

        protected Unit target;
        public Unit Target { set { target = value; } get { return target; } }
        protected float dispersion; //dafault 0;
        protected float shildBlinkTime; //default 0.01
        protected float roundSpeed; //default 1000;
        protected float firerate;

        protected bool PreAiming;
        protected float range;

        protected float backCount;

        public float Range { get { return range * (1 + RangeMultiplacator); } }
        public float RoundSpeed { get { return roundSpeed * (1 + RoundspeedMultiplacator); } }
        public float Dispersion { get { return dispersion * (1 + DispersionMultiplicator); } }
        public float ShildBlink { get { return shildBlinkTime; } }
        public float BackCounter { get { return backCount; } }

        public WeaponType Type { get { return type; } }

        public float Firerate { get { return firerate * (1 + FirerateMultiplacator); } }

        protected void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            owner = this.transform.GetComponentInParent<Unit>();
            roundSpeed = 150;
            shildBlinkTime = 0.01f;
            StatUp();
        }
        protected abstract void StatUp();
        // Update is called once per frame
        public virtual void Update()
        {
            if (PreAiming) Preaiming();
            if (backCount > 0)
                backCount -= Time.deltaTime;
            UpdateLocal();
        }
        protected virtual void UpdateLocal()
        { }
        private void Preaiming()
        {
            float angel = Vector3.Angle(this.transform.forward, this.gameObject.GetComponentInParent<Unit>().transform.forward);
            if (angel < 5 && target != null)
            {
                try
                {
                    float distance;
                    float approachTime;
                    Vector3 aimPoint = target.transform.position;
                    //Debug.Log(target.GetComponent<NavMeshAgent>().velocity);
                    Vector3 targetVelocity = target.Velocity - owner.Velocity;

                    distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до цели
                    approachTime = distance / RoundSpeed;
                    aimPoint = target.transform.position + targetVelocity * approachTime; //первое приближение

                    distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до точки первого приближения
                    approachTime = distance / RoundSpeed;
                    aimPoint = target.transform.position + targetVelocity * approachTime * 1.01f; //второе приближение

                    //distance = Vector3.Distance(this.gameObject.transform.position, aimPoint);
                    //approachTime = distance / averageRoundSpeed;
                    //aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime; //третье приближение

                    Quaternion targetRotation = Quaternion.LookRotation((aimPoint - this.transform.position).normalized, new Vector3(0, 1, 0)); //донаводка
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, Time.deltaTime * 5);
                }
                catch (MissingReferenceException)
                {
                    target = null;
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(Vector3.zero);//возврат
                this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, targetRotation, Time.deltaTime * 6);
            }
        }
        public abstract void Reset();
        public abstract bool IsReady { get; }
        public abstract float ShootCounter { get; }
        public abstract float MaxShootCounter { get; }
        public abstract bool Fire();
        protected abstract void Shoot(Transform target);
    }
    public abstract class MagWeapon : Weapon
    {
        public float ReloadMultiplacator { set; get; }
        public float AmmocampacityMultiplacator { set; get; }
        public float ShellmassMultiplacator { set; get; }

        protected float reloadingTime;
        protected int ammo;
        protected int ammoCampacity;
        protected int AmmoCampacity { get { return Mathf.RoundToInt(ammoCampacity * (1 + AmmocampacityMultiplacator)); } }
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //Global.SpecINI.Write(this.GetType().ToString(), "range", range.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "dispersion", dispersion.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "shildBlinkTime", shildBlinkTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "firerate", firerate.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "ammoCampacity", ammoCampacity.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "reloadingTime", reloadingTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "PreAiming", PreAiming.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "roundSpeed", roundSpeed.ToString());

            range = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "range"));
            dispersion = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "dispersion"));
            shildBlinkTime = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "shildBlinkTime"));
            firerate = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "firerate"));
            ammoCampacity = Convert.ToInt32(Global.SpecINI.ReadINI(this.GetType().ToString(), "ammoCampacity"));
            reloadingTime = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "reloadingTime"));
            PreAiming = Convert.ToBoolean(Global.SpecINI.ReadINI(this.GetType().ToString(), "PreAiming"));
            roundSpeed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "roundSpeed"));

            ammo = AmmoCampacity;
        }
        public override bool Fire()
        {
            if (IsReady)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                ammo--;
                backCount = 60f / Firerate;
                return true;
            }
            else return false;
        }
        public override void Update()
        {
            base.Update();
            if (ammo <= 0 && backCount <= 0)
            {
                ammo = AmmoCampacity;
                backCount = reloadingTime * (1 + ReloadMultiplacator);
            }
        }
        public override bool IsReady
        {
            get
            {
                return (ammo > 0 && backCount <= 0);
            }
        }
        public override float ShootCounter { get { return ammo; } }
        public override float MaxShootCounter { get { return reloadingTime * (1 + ReloadMultiplacator); } }
        public override void Reset()
        {
            ammo = AmmoCampacity;
            backCount = 0;
        }
    }
    public abstract class ShellWeapon : Weapon
    {
        public float ReloadMultiplacator { set; get; }
        public float AmmocampacityMultiplacator { set; get; }
        public float ShellmassMultiplacator { set; get; }

        protected float reloadingTime;
        protected float reloadBackCount;
        protected int ammo;
        protected int ammoCampacity;
        protected int AmmoCampacity { get { return Mathf.RoundToInt(ammoCampacity * (1 + AmmocampacityMultiplacator)); } }
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //Global.SpecINI.Write(this.GetType().ToString(), "range", range.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "dispersion", dispersion.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "shildBlinkTime", shildBlinkTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "firerate", firerate.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "ammoCampacity", ammoCampacity.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "reloadingTime", reloadingTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "PreAiming", PreAiming.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "roundSpeed", roundSpeed.ToString());

            range = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "range"));
            dispersion = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "dispersion"));
            shildBlinkTime = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "shildBlinkTime"));
            firerate = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "firerate"));
            ammoCampacity = Convert.ToInt32(Global.SpecINI.ReadINI(this.GetType().ToString(), "ammoCampacity"));
            reloadingTime = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "reloadingTime"));
            PreAiming = Convert.ToBoolean(Global.SpecINI.ReadINI(this.GetType().ToString(), "PreAiming"));
            roundSpeed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "roundSpeed"));

            ammo = AmmoCampacity;
        }
        public override bool Fire()
        {
            if (IsReady)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                ammo--;
                reloadBackCount = 60f / Firerate;
                return true;
            }
            else return false;
        }
        public override void Update()
        {
            base.Update();
            if (ammo < AmmoCampacity && backCount <= 0)
            {
                ammo++;
                backCount = reloadingTime * (1 + ReloadMultiplacator);
            }
            if (reloadBackCount > 0) reloadBackCount -= Time.deltaTime;
        }
        public override bool IsReady
        {
            get
            {
                return (ammo > 0 && reloadBackCount <= 0);
            }
        }
        public override float ShootCounter { get { return ammo; } }
        public override float MaxShootCounter { get { return reloadingTime * (1 + ReloadMultiplacator); } }
        public override void Reset()
        {
            ammo = AmmoCampacity;
            backCount = 0;
            reloadBackCount = 0;
        }
    }
    public abstract class EnergyWeapon : Weapon
    {
        protected float coolingMultiplacator;
        protected float maxheatMultiplacator;

        protected float heat;
        protected float maxHeat;
        protected bool overheat;
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;

            //Global.SpecINI.Write(this.GetType().ToString(), "range", range.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "dispersion", dispersion.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "shildBlinkTime", shildBlinkTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "firerate", firerate.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "maxHeat", maxHeat.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "PreAiming", PreAiming.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "roundSpeed", roundSpeed.ToString());

            range = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "range"));
            dispersion = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "dispersion"));
            shildBlinkTime = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "shildBlinkTime"));
            firerate = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "firerate"));
            maxHeat = Convert.ToInt32(Global.SpecINI.ReadINI(this.GetType().ToString(), "maxHeat"));
            PreAiming = Convert.ToBoolean(Global.SpecINI.ReadINI(this.GetType().ToString(), "PreAiming"));
            roundSpeed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "roundSpeed"));
        }
        public override bool Fire()
        {
            if (IsReady)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                backCount = 60f / Firerate;
                return true;
            }
            else return false;
        }
        public override void Update()
        {
            base.Update();
            if (heat > 0)
            {
                heat -= Time.deltaTime * 4 * (1 + coolingMultiplacator);
                if (heat > maxHeat)
                    overheat = true;
            }
            else overheat = false;
        }
        public override bool IsReady { get { return (!overheat && backCount <= 0); } }
        public override float ShootCounter { get { return heat; } }
        public override float MaxShootCounter { get { return maxHeat * (1 + maxheatMultiplacator); } }
        public override void Reset()
        {
            heat = 0;
            backCount = 0;
        }
    }
    public abstract class Round : MonoBehaviour
    {
        protected float speed;
        protected float damage;
        protected float armorPiersing;
        protected float ttl;
        protected bool canRicochet;

        public float Speed { get { return speed; } }
        public float Damage { get { return damage; } }
        public float ArmorPiersing { get { return armorPiersing; } }

        //public virtual void StatUp(ShellType type) { }
        //public virtual void StatUp(EnergyType type) { }
        // Use this for initialization
        //protected virtual void Start()
        //{
        //    //gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        //}

        // Update is called once per frame
        public virtual void Update()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else 
                Destroy();
        }
        protected virtual void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        if (!canRicochet)
                            Destroy();
                        break;
                    }
            }
        }
        public abstract void Destroy();
    }
    public abstract class SelfguidedMissile : MonoBehaviour
    {
        protected GlobalController Global;
        public Transform target;// цель для ракеты       
        //public GameObject Blast;// префаб взрыва   
        protected MissileType Type;
        protected float Speed;// скорость ракеты           
        public float DropImpulse;//импульс сброса          
        protected float TurnSpeed;// скорость поворота ракеты            
        protected float explosionTime;// длительность жизни
        public float AimCone;
        protected float lifeTime;//продолжительность жизни
        protected float detonateTimer;
        protected bool isArmed;

        protected virtual void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            Speed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "Speed"));
            DropImpulse = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "DropImpulse"));
            TurnSpeed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "TurnSpeed"));
            explosionTime = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "explosionTime"));
            AimCone = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "AimCone"));

            //Global.SpecINI.Write(this.GetType().ToString(), "Speed", Speed.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "DropImpulse", DropImpulse.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "TurnSpeed", TurnSpeed.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "explosionTime", explosionTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "AimCone", AimCone.ToString());
        }
        private void Update()
        {

            if (isArmed)
            {
                if (detonateTimer > 0)
                    detonateTimer -= Time.deltaTime;
                else Explode();
            }
            else
            {
                if (lifeTime > explosionTime)
                    Explode();
                else
                    lifeTime += Time.deltaTime;
            }
            if (lifeTime > 0.5)//задержка старта
            {
                if (target != null)//наведение
                {
                    Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                    // угол между направлением на цель и направлением ракеты порядок имеет значение.
                    Weapons.MissileTrap[] traps = FindObjectsOfType<Weapons.MissileTrap>();
                    if (traps.Length > 0)
                    {
                        foreach (Weapons.MissileTrap x in traps)
                        {
                            if (Vector3.Angle(x.transform.position - this.transform.position, this.transform.forward) < AimCone)
                            {
                                target = null;
                                break;
                            }
                        }
                    }
                }
                if (target != null && Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) > AimCone)
                    target = null;
                //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity.magnitude);

                //полет по прямой
                float multiplicator = Mathf.Pow((lifeTime * 0.05f), (1f / 4f));
                //Debug.Log(multiplicator);
                //Debug.Log(Convert.ToSingle(multiplicator));
                //gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed * Convert.ToSingle(multiplicator); 
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed * multiplicator, ForceMode.Acceleration);
            }
        }
        protected abstract void Explode();
        public void Arm()
        {
            if (!isArmed && lifeTime > 2.5)
            {
                isArmed = true;
                detonateTimer = 0.2f;
            }
        }
        // взрываем при коллизии
        public virtual void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        if (lifeTime > explosionTime / 20)
                            Arm();
                        break;
                    }
                default:
                    {
                        Arm();
                        break;
                    }
            }
        }
        private void OnTriggerStay(Collider collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Explosion":
                    {
                        if (lifeTime > explosionTime / 20)
                            Arm();
                        break;
                    }
            }
        }
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
    public abstract class Torpedo : MonoBehaviour
    {
        protected GlobalController Global;
        public Vector3 target;
        public Vector3 midPoint;
        public bool midPassed;
        public Army Team;
        public float Speed;// скорость ракеты      
        public float TurnSpeed;// скорость поворота ракеты            
        public float DropImpulse;//импульс сброса                  
        public float explosionRange; //расстояние детонации
        protected float lt;//продолжительность жизни
        protected virtual void Start()
        {
            Global = FindObjectOfType<GlobalController>();

            //Global.SpecINI.Write(this.GetType().ToString(), "Speed", Speed.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "DropImpulse", DropImpulse.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "TurnSpeed", TurnSpeed.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "explosionRange", explosionRange.ToString());

            Speed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "Speed"));
            DropImpulse = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "DropImpulse"));
            TurnSpeed = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "TurnSpeed"));
            explosionRange = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "explosionRange"));
        }
        public virtual void Update()
        {
            if (Vector3.Distance(this.transform.position, midPoint) < 1)
                midPassed = true;

            if (Vector3.Distance(this.transform.position, target) < explosionRange)
                Explode();
            else
                lt += Time.deltaTime;
            if (lt > 0.5)//задержка старта
            {
                Quaternion targetRotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);//контроль курса
                if (!midPassed && midPoint != Vector3.zero)
                    targetRotation = Quaternion.LookRotation(midPoint - this.transform.position, new Vector3(0, 1, 0));
                else if (target != Vector3.zero)
                    targetRotation = Quaternion.LookRotation(target - this.transform.position, new Vector3(0, 1, 0));

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                //полет по прямой
                float multiplicator = Mathf.Pow((lt * 0.5f), (1f / 8f)) * 0.7f;
                gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed * multiplicator;//AddForce(transform.forward * Speed * multiplicator, ForceMode.Acceleration);
            }
            else if (lt > 20)
                Explode();
        }
        public virtual void Explode()
        {
            GameObject blast = Instantiate(Global.ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.UnitaryTorpedo);
            Destroy(gameObject);
        }
        // взрываем при коллизии
        public virtual void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        if (lt > 1)
                            Explode();
                        break;
                    }
                case "Terrain":
                    {
                        if (lt > 1)
                            Explode();
                        break;
                    }
                case "Unit":
                    {
                        if (lt > 1.5)
                            Explode();
                        break;
                    }
            }
        }
        public virtual void SetTarget(Vector3 target)
        {
            this.target = target;
        }
        public virtual void SetMidpoint(Vector3 target)
        {
            this.midPoint = target;
        }
        public virtual void SetTeam(Army allies)
        {
            this.Team = allies;
        }
        public virtual bool Allies(Army army)
        {
            return (Team == army);
        }
    }
}
