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

namespace SpaceCommander
{
    public class GlobalController : MonoBehaviour
    {
        public List<SpaceShip> unitList; // список 
        public List<SpaceShip> selectedList; // спиков выделенных объектов
        public Army playerArmy;
        //selection
        public float NameFrameOffset;
        public Texture AlliesSelectedGUIFrame;
        public Texture AlliesGUIFrame;
        public Texture EnemyGUIFrame;
        public float GUIFrameWidth;
        public float GUIFrameHeight;
        public float GUIFrameOffset;
        public GameObject UnitaryShell;
        public AudioClip CannonShootSound;
        public GameObject ShellBlast;
        public GameObject RailgunShell;
        public GameObject LaserBeam;
        public GameObject PlasmaSphere;
        public GameObject Missile;
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
        public Vector2 Resolution {
            get { return new Vector2(settings.screenExpWidth, settings.screenExpHeight); }
            set { SettingsSaved = false; settings.screenExpWidth = value.x; settings.screenExpHeight = value.y; }
        }
        //texts
        public Dictionary<string,string> Texts;
        private Scenario Mission;
        public string MissionName { get
            {
                if (Mission != null)
                    return Mission.Name;
                else return SceneManager.GetActiveScene().name;
            } }
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
            XmlSerializer formatter = new XmlSerializer(typeof(SerializeData<string,string>));
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
            Texts = new Dictionary<string, string>();
            Texts = serialeze.Data;
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
                foreach (IUnit x in unitList)
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
        public int PathPoints {
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
                if (path.Count == 1&& (walkerAgent.pathEndPosition - walkerTransform.position).magnitude < 1)
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
        private Transform targetTransform;
        //private Vector3 oldTargetPosition;
        //private Vector3 aimPoint; //точка сведения
        private ForceShield shield;
        private float[] synchWeapons;
        private int[] indexWeapons;
        //private float averageRoundSpeed;
        //private float averageRange;

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
        public bool ShootHim(SpaceShip target, int slot)
        {
            if (synchWeapons[slot] <= 0)
            {
                float angel = Vector3.Angle(target.transform.position - owner.transform.position, owner.transform.forward);
                if (angel < weapons[slot][0].Dispersion * 5 || angel < 10)
                {
                    if (indexWeapons[slot] >= weapons[slot].Length)
                        indexWeapons[slot] = 0;
                    if (weapons[slot][indexWeapons[slot]].Cooldown <= 0)
                    {
                        shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                        synchWeapons[slot] = this.weapons[slot][0].CoolingTime / this.weapons[slot].Length;
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
            if (synchWeapons[slot] <= 0 && weapons[slot][indexWeapons[slot]].Cooldown <= 0)
            {
                int i = 0;
                shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                synchWeapons[slot] = this.weapons[slot][0].CoolingTime / this.weapons[slot].Length;
                indexWeapons[slot]++;
                for (i = 0; i < weapons[slot].Length; i++)
                {
                    weapons[slot][i].InstantCool();
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
            if (owner.NeedReloading == false)
                CheckWeapons();
            if (targetTransform != null)
            {
                //Debug.Log("target - " + Target.transform.position);
                //Debug.Log("aim - " + aimPoint);
                //наведение на цель
                Quaternion targetRotation = Quaternion.LookRotation((targetTransform.position - owner.transform.position).normalized * turnSpeed, new Vector3(0, 1, 0));
                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime * turnSpeed * 0.2f);
            }
        }
        public bool SetAim(IUnit target)
        {
            if (targetTransform == null)
            {
                targetTransform = target.ObjectTransform;
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
            targetTransform = null;
            return true;
        }
        public void CheckWeapons()
        {
            //Debug.Log("_" + weapons.Length);
            for (int i = 0; i < weapons.Length; i++)
            {
                //Debug.Log(i +"-" + weapons.Length);
                for (int j = 0; j < weapons[i].Length; j++)
                {
                    if (weapons[i][j].Ammo <= 0)
                    {
                        owner.NeedReloading = true;
                        return;
                    }
                }
            }
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
        protected IUnit target;
        public IUnit Target { set { target = value; } get { return target; } }
        protected GlobalController Global;
        protected float range;
        //protected int maxAmmo;
        protected int ammo;
        protected float coolingTime;
        protected float cooldown;
        protected float dispersion; //dafault 0;
        protected float shildBlinkTime; //default 0.01
        protected float averageRoundSpeed; //default 1000;
        protected bool PreAiming;
        public float Range { get { return range; } }
        public float RoundSpeed { get { return averageRoundSpeed; } }
        public int Ammo { get { return ammo; } }
        public float Cooldown { get { return cooldown; } }
        public float Dispersion { get { return dispersion; } }
        public float CoolingTime { get { return coolingTime; } }
        public float ShildBlink { get { return shildBlinkTime; } }

        protected void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            averageRoundSpeed = 150;
            shildBlinkTime = 0.01f;
            StatUp();
        }
        public abstract void StatUp();
        // Update is called once per frame
        public void Update()
        {
            if (cooldown > 0)
                cooldown -= Time.deltaTime;
            if (PreAiming)
            {
                float angel = Vector3.Angle(this.transform.forward, this.gameObject.GetComponentInParent<Transform>().forward);
                if (angel < 5)
                {
                    try
                    {
                        if (target != null && target.ObjectTransform != null)
                        {
                            float distance;
                            float approachTime;
                            Vector3 aimPoint = target.ObjectTransform.position;
                            //Debug.Log(target.GetComponent<NavMeshAgent>().velocity);

                            distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до цели
                            approachTime = distance / averageRoundSpeed;
                            Vector3 targetVelocity = target.Velocity;
                            targetVelocity.y = 0; //исключаем вертикальную компоненту
                            aimPoint = target.ObjectTransform.position + targetVelocity * approachTime; //первое приближение

                            distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до точки первого приближения
                            approachTime = distance / averageRoundSpeed;
                            targetVelocity = target.Velocity;
                            targetVelocity.y = 0;
                            aimPoint = target.ObjectTransform.position + targetVelocity * approachTime * 1.01f; //второе приближение

                            //distance = Vector3.Distance(this.gameObject.transform.position, aimPoint);
                            //approachTime = distance / averageRoundSpeed;
                            //aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime; //третье приближение

                            Quaternion targetRotation = Quaternion.LookRotation((aimPoint - this.transform.position).normalized, new Vector3(0, 1, 0)); //донаводка
                            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 10);
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        target = null;
                    }
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(this.gameObject.GetComponentInParent<Transform>().forward, new Vector3(0, 1, 0)); //возврат
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 7);
                }
            }
        }
        public void Reset()
        {
            Start();
            cooldown = coolingTime * 10;
        }
        public void InstantCool()
        {
            cooldown = 0;
        }
        public bool Fire()
        {
            if ((ammo > 0) && (cooldown <= 0))
            {
                //Debug.Log("Fire");
                this.GetComponentInChildren<ParticleSystem>().Play();
                Shoot(target.ObjectTransform);
                return true;
                //cooldown = coolingTime; !set in children
                //ammo--; !set in children
            }
            else return false;
        }
        protected abstract void Shoot(Transform target);
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
                Explode();
        }
        protected virtual void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        Explode();
                        break;
                    }
            }
        }
        protected abstract void Explode();
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
        public void Update()
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
                    if (Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) > AimCone)
                        target = null;
                }
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
            if (!isArmed && lt > 1.5)
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
        public Army Team;
        public float Speed;// скорость ракеты      
        public float TurnSpeed;// скорость поворота ракеты            
        public float DropImpulse;//импульс сброса                  
        public float explosionRange; //расстояние детонации
        protected float lt;//продолжительность жизни
        protected abstract void Start();
        public virtual void Update()
        {
            if (Vector3.Distance(this.transform.position, target) < explosionRange)
                Explode();
            else
                lt += Time.deltaTime;
            if (lt > 0.5)//задержка старта
            {
                if (target != Vector3.zero)//контроль курса
                {
                    Quaternion targetRotation = Quaternion.LookRotation(target - this.transform.position, new Vector3(0, 1, 0));
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                }
                //полет по прямой
                float multiplicator = Mathf.Pow((lt * 0.5f), (1f / 8f)) * 0.7f;
                gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed * multiplicator;//AddForce(transform.forward * Speed * multiplicator, ForceMode.Acceleration);
            }
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
                case "Unit":
                    {
                        if (Vector3.Distance(this.transform.position, target) < explosionRange * 0.1)
                            Explode();
                        break;
                    }
            }
        }
        public virtual void SetTarget(Vector3 target)
        {
            this.target = target;
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
    public static class Randomizer
    {
        public static double[] Uniform(int minValue, int maxValue, int Count)
        {
            double[] Out = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                Out[i] = Uniform(minValue, maxValue);
            }
            return Out;
        }
        public static double[] Exponential(double Rate, int Count, int minValue, int maxValue)
        {
            double[] stairWidth = new double[257];
            double[] stairHeight = new double[256];
            const double x1 = 7.69711747013104972;
            const double A = 3.9496598225815571993e-3; /// area under rectangle

            setupExpTables(ref stairWidth, ref stairHeight, x1, A);

            double[] Out = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                Out[i] = Exponential(Rate, stairWidth, stairHeight, x1, minValue, maxValue);
            }

            return Out;
        }
        public static double[] Normal(double Mu, double Sigma, int Count, int minValue, int maxValue)
        {
            double[] stairWidth = new double[257];
            double[] stairHeight = new double[256];
            const double x1 = 3.6541528853610088;
            const double A = 4.92867323399e-3; /// area under rectangle

            setupNormalTables(ref stairWidth, ref stairHeight, x1, A);

            double[] Out = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                Out[i] = Mu + NormalZiggurat(stairWidth, stairHeight, x1, minValue, maxValue) * Sigma;
            }

            return Out;
        }

        private static double Uniform(double A, double B)
        {
            return A + RandomDouble() * (B - A);// maxValue;
        }
        private static double MagiсUniform(double A, double B)
        {
            return A + RandomMagiсInt() * (B - A) / 256;
        }

        private static void setupExpTables(ref double[] stairWidth, ref double[] stairHeight, double x1, double A)
        {
            // coordinates of the implicit rectangle in base layer
            stairHeight[0] = Math.Exp(-x1);
            stairWidth[0] = A / stairHeight[0];
            // implicit value for the top layer
            stairWidth[256] = 0;
            for (int i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                stairWidth[i] = -Math.Log(stairHeight[i - 1]);
                stairHeight[i] = stairHeight[i - 1] + A / stairWidth[i];
            }
        }
        private static void setupNormalTables(ref double[] stairWidth, ref double[] stairHeight, double x1, double A)
        {
            // coordinates of the implicit rectangle in base layer
            stairHeight[0] = Math.Exp(-.5 * x1 * x1);
            stairWidth[0] = A / stairHeight[0];
            // implicit value for the top layer
            stairWidth[256] = 0;
            for (int i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                stairWidth[i] = Math.Sqrt(-2 * Math.Log(stairHeight[i - 1]));
                stairHeight[i] = stairHeight[i - 1] + A / stairWidth[i];
            }
        }

        private static double ExpZiggurat(double[] stairWidth, double[] stairHeight, double x1, int minValue, int maxValue)
        {
            int iter = 0;
            do
            {
                int stairId = RandomInt() & 255;
                double x = Uniform(0, stairWidth[stairId]); // get horizontal coordinate
                if (x < stairWidth[stairId + 1]) /// if we are under the upper stair - accept
                    return x;
                if (stairId == 0) // if we catch the tail
                    return x1 + ExpZiggurat(stairWidth, stairHeight, x1, minValue, maxValue);
                if (Uniform(stairHeight[stairId - 1], stairHeight[stairId]) < Math.Exp(-x)) // if we are under the curve - accept
                    return x;
                // rejection - go back
            } while (++iter <= 1e9); // one billion should be enough to be sure there is a bug
            return double.NaN; // fail due to some error
        }
        private static double NormalZiggurat(double[] stairWidth, double[] stairHeight, double x1, int minValue, int maxValue)
        {
            int iter = 0;
            do
            {
                int B = RandomMagiсInt();
                int stairId = B & 255;
                double x = MagiсUniform(0, stairWidth[stairId]); // get horizontal coordinate
                if (x < stairWidth[stairId + 1])
                    return ((int)B > 0) ? x : -x;
                if (stairId == 0) // handle the base layer
                {
                    double z = -1;
                    double y;
                    if (z > 0) // we don't have to generate another exponential variable as we already have one
                    {
                        x = Exponential(x1, stairWidth, stairHeight, x1, minValue, maxValue);
                        z -= 0.5 * x * x;
                    }
                    if (z <= 0) // if previous generation wasn't successful
                    {
                        do
                        {
                            x = Exponential(x1, stairWidth, stairHeight, x1, minValue, maxValue);
                            y = Exponential(1, stairWidth, stairHeight, x1, minValue, maxValue);
                            z = y - 0.5 * x * x; // we storage this value as after acceptance it becomes exponentially distributed
                        } while (z <= 0);
                    }
                    x += x1;
                    return ((int)B > 0) ? x : -x;
                }
                // handle the wedges of other stairs
                if (MagiсUniform(stairHeight[stairId - 1], stairHeight[stairId]) < Math.Exp(-.5 * x * x))
                    return ((int)B > 0) ? x : -x;
            } while (++iter <= 1e9); /// one billion should be enough
            return double.NaN; /// fail due to some error
        }
        private static double Exponential(double Rate, double[] stairWidth, double[] stairHeight, double x1, int minValue, int maxValue)
        {
            return ExpZiggurat(stairWidth, stairHeight, x1, minValue, maxValue) / Rate;
        }
        private static double RandomDouble()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int BUF_LENGTH = 1;
            byte[] buf = new byte[BUF_LENGTH];
            rand.GetBytes(buf);
            double Out = 0;
            foreach (byte x in buf)
                Out += x / 256.0;
            return Out;
        }
        private static int RandomInt()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int BUF_LENGTH = 1;
            byte[] buf = new byte[BUF_LENGTH];
            rand.GetBytes(buf);
            int Out = 0;
            foreach (byte x in buf)
                Out += x;
            return Out / BUF_LENGTH;
        }
        private static int RandomMagiсInt()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int BUF_LENGTH = 256;
            byte[] buf = new byte[BUF_LENGTH];
            rand.GetBytes(buf);
            int Out = 0;
            foreach (byte x in buf)
                Out += x;
            return Out / BUF_LENGTH;
        }
    }
}
