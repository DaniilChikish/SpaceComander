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
        //public float GUIFrameWidth;
        //public float GUIFrameHeight;
        //public float GUIFrameOffset;
        public GameObject UnitaryShell;
        public AudioClip CannonShootSound;
        public GameObject ShellBlast;
        public GameObject RailgunShell;
        public GameObject LaserBeam;
        public GameObject PlasmaSphere;
        public GameObject Missile;
        public GameObject MissileTrap;
        public GameObject Torpedo;
        public GameObject ExplosiveBlast;
        public GameObject NukeBlast;
        public GameObject ShipDieBlast;
        public double[] RandomNormalPool;
        public double RandomNormalAverage;
        //public double[] RandomExponentPool;
        private float randomPoolBackCoount;
        //settings
        private SerializeSettings settings;
        public bool SettingsSaved;
        public Languages Localisation { get { return settings.localisation; } set { SettingsSaved = false; settings.localisation = value; } }
        public float SoundLevel { get { return settings.soundLevel; } set { SettingsSaved = false; settings.soundLevel = value; } }
        public float MusicLevel { get { return settings.musicLevel; } set { SettingsSaved = false; settings.musicLevel = value; } }
        public bool StaticProportion
        {
            get { return settings.staticProportion; }
            set { SettingsSaved = false; settings.staticProportion = value; }
        }
        //texts
        private Dictionary<string, string> localTexts;
        public string Texts(string key)
        {
            if (localTexts.ContainsKey(key))
                return localTexts[key];
            else return key;
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
            //Debug.Log("Global - OnEnable");
            LoadTexts();
            //text_0 = Texts["string2_key"];
            //Debug.Log("GlobalController started");
            Mission = FindObjectOfType<Scenario>();
        }
        public void SaveSettings()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeSettings));
            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream(Application.streamingAssetsPath + "\\settings.dat", FileMode.Create))
            {
                formatter.Serialize(fs, settings);
            }
            SettingsSaved = true;
        }
        public void SetDefault()
        {
            settings = new SerializeSettings();
            settings.localisation = Languages.English;
            settings.staticProportion = true;
            settings.musicLevel = 100;
            settings.soundLevel = 100;
            SaveSettings();
        }
        public void LoadSettings()
        {
            string path = Application.streamingAssetsPath + "\\settings.dat";
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeSettings));
            // десериализация
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                settings = (SerializeSettings)formatter.Deserialize(fs);
            }
            catch (FileNotFoundException)
            {
                SetDefault();
            }
            SettingsSaved = true;
        }

        private void Start()
        {

        }
        private void SaveText()
        {
            SerializeData<string, string> serialeze = new SerializeData<string, string>();
            //Texts.lang = Languages.English;
            serialeze.Data = new Dictionary<string, string>();
            serialeze.Data.Add("Pause", "Pause_value");
            serialeze.Data.Add("string2_key", "string2_value");
            serialeze.Data.Add("string3_key", "string3_value");
            serialeze.OnBeforeSerialize();
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeData<string, string>));
            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream(Application.streamingAssetsPath + "\\temp.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, serialeze);
                //Debug.Log("Объект сериализован");
            }
        }
        public void LoadTexts()
        {
            string path = Application.streamingAssetsPath + "\\local";
            switch (Localisation)
            {
                case Languages.English:
                    {
                        path += "\\eng\\base_eng.xml";
                        break;
                    }
                case Languages.Russian:
                    {
                        path += "\\rus\\base_rus.xml";
                        break;
                    }
                case Languages.temp:
                    {
                        path += "\\temp.xml";
                        break;
                    }
            }

            //SaveText();//debug only
            //SerializeData<string, string> textsSer = new SerializeData<string, string>();
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeData<string, string>));
            // десериализация
            Debug.Log("open - " + path);
            FileStream fs = new FileStream(path, FileMode.Open);
            SerializeData<string, string> serialeze = (SerializeData<string, string>)formatter.Deserialize(fs);
            serialeze.OnAfterDeserialize();
            Debug.Log(serialeze.ToString());
            localTexts = new Dictionary<string, string>();
            localTexts = serialeze.Data;
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
    public class MovementController : IDriver
    {
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
        public Vector3 NextPoint { get { if (PathPoints > 1) return walkerAgent.pathEndPosition; else return Vector3.zero; } }
        //public float backCount; //время обновления пути.
        public MovementController(GameObject walker)
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
    }
    public class ShootController : IGunner
    {
        private IWeapon[][] weapons;
        public SpaceShip owner;
        private float turnSpeed;
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
            turnSpeed = body.Speed * 0.5f;
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
            if (!owner.ManualControl && target != null)
            {
                targetLockdownCount -= Time.deltaTime;
                //Debug.Log("target - " + Target.transform.position);
                //Debug.Log("aim - " + aimPoint);
                //наведение на цель
                if ((target.transform.position - owner.transform.position).normalized != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation((target.transform.position - owner.transform.position).normalized * turnSpeed, new Vector3(0, 1, 0));
                    owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime * turnSpeed * 0.2f);
                }
            }
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
                for (int j = 0; i < weapons[i].Length; j++)
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
        protected WeaponType type;
        protected GlobalController Global;
        protected Unit target;
        public Unit Target { set { target = value; } get { return target; } }
        protected float dispersion; //dafault 0;
        protected float shildBlinkTime; //default 0.01
        protected float averageRoundSpeed; //default 1000;
        protected int firerate;

        protected bool PreAiming;
        protected float range;

        protected float backCount;

        public float Range { get { return range; } }
        public float RoundSpeed { get { return averageRoundSpeed; } }
        public float Dispersion { get { return dispersion; } }
        public float ShildBlink { get { return shildBlinkTime; } }
        public float BackCounter { get { return backCount; } }

        public WeaponType Type { get { return type; } }

        public int Firerate { get { return firerate; } }

        protected void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            averageRoundSpeed = 150;
            shildBlinkTime = 0.01f;
            StatUp();
        }
        public abstract void StatUp();
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

                    distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до цели
                    approachTime = distance / averageRoundSpeed;
                    Vector3 targetVelocity = target.Velocity;
                    targetVelocity.y = 0; //исключаем вертикальную компоненту
                    aimPoint = target.transform.position + targetVelocity * approachTime; //первое приближение

                    distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до точки первого приближения
                    approachTime = distance / averageRoundSpeed;
                    targetVelocity = target.Velocity;
                    targetVelocity.y = 0;
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
    public abstract class RoundWeapon : Weapon
    {
        protected float reloadingTime;
        protected int ammo;
        protected int ammoCampacity;
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
                ammo = ammoCampacity;
                backCount = reloadingTime;
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
        public override float MaxShootCounter { get { return reloadingTime; } }
        public override void Reset()
        {
            ammo = ammoCampacity;
            backCount = 0;
        }
    }
    public abstract class EnergyWeapon : Weapon
    {
        protected float heat;
        protected float maxHeat;
        protected bool overheat;
        public override bool Fire()
        {
            if (IsReady)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                heat++;
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
                heat -= Time.deltaTime;
                if (heat > maxHeat)
                    overheat = true;
            }
            else overheat = false;
        }
        public override bool IsReady { get { return (!overheat && backCount <= 0); } }
        public override float ShootCounter { get { return heat; } }
        public override float MaxShootCounter { get { return maxHeat; } }
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
                        Destroy();
                        break;
                    }
            }
        }
        protected abstract void Destroy();
    }
    public abstract class SelfguidedMissile : MonoBehaviour
    {
        public Transform target;// цель для ракеты       
        //public GameObject Blast;// префаб взрыва   
        protected MissileType type;
        protected float Speed;// скорость ракеты           
        public float DropImpulse;//импульс сброса          
        protected float TurnSpeed;// скорость поворота ракеты            
        protected float explosionTime;// длительность жизни
        public float AimCone;
        protected float lt;//продолжительность жизни
        protected float detonateTimer;
        protected bool isArmed;

        public abstract void Start();
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
                if (lt > explosionTime)
                    Explode();
                else
                    lt += Time.deltaTime;
            }
            if (lt > 0.5)//задержка старта
            {
                if (target != null)//наведение
                {
                    Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
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
                float multiplicator = Mathf.Pow((lt * 0.05f), (1f / 4f));
                //Debug.Log(multiplicator);
                //Debug.Log(Convert.ToSingle(multiplicator));
                //gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed * Convert.ToSingle(multiplicator); 
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed * multiplicator, ForceMode.Acceleration);
            }
        }
        protected abstract void Explode();
        public void Arm()
        {
            if (!isArmed && lt > 2.5)
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
                        if (lt > explosionTime / 20)
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
                        if (lt > explosionTime / 20)
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
        protected abstract void Start();
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
