using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public enum UnitClass { Scout, Recon, ECM, Figther, Bomber, Command, LR_Corvette, Guard_Corvette, Support_Corvette, Turret};
    public enum UnitStateType { MoveAI, UnderControl, Waiting, Idle};
    public enum TargetStateType { BehindABarrier,  InPrimaryRange, InSecondaryRange, Captured, NotFinded}; 
    //public enum ImpactType { ForestStaticImpact };
    public enum Army { Green, Red, Blue };
    public interface IUnit
    {
        UnitClass Type { get; }
        Army Team { get; }
        Transform ObjectTransform { get; }
        void MakeDamage(float damage);
        void Die();
    }
    public delegate int SortEnemys(IUnit x, IUnit y);
    public interface IDriver
    {
        void Update();
        void UpdateSpeed();
        bool MoveTo(Vector3 destination);
        bool MoveToQueue(Vector3 destination);
        void ClearQueue();
        int PathPoints { get; }
    }
    public interface IGunner
    {
        void Update();
        bool SetAim(GameObject target);
        bool ShootHim(GameObject target, int slot);
        bool ResetAim();
        float GetRange(int slot);
    }
    public enum WeaponType { Cannon, Laser, Plazma, Missile, Torpedo}
    public interface IWeapon
    {
        float Range { get; }
        float RoundSpeed { get; }
        int Ammo { get; }
        float Cooldown { get; }
        float Dispersion { get; }
        float CoolingTime { get; }
        float ShildBlink { get; }
        void StatUp();
        bool Fire(GameObject target);
    }
    public enum ShellType { Solid, SolidAP, Subcaliber, HightExplosive, Camorous, CamorousAP, Uranium, Сumulative, Railgun}
    public enum ShellLineType { ArmorPenetration, ShildOwerheat, QuickShell, Explosive, Universal}
    public interface IShell
    {
        float Speed { get; }
        float Damage { get; }
        float ArmorPiersing { get; }
        void StatUp(ShellType type);
    }
    public enum EnergyType { RedRay, GreenRay, BlueRay, Plazma }
    public interface IEnergy
    {
        float Speed { get; }
        float Damage { get; }
        float ArmorPiersing { get; }
        void StatUp(EnergyType type);
        float GetEnergy();
    }
    public enum MissileType { Selfguided, Unguided }
    public enum TorpedoType { Unitary, Nuke, Sprute }
    public enum BlastType { UnitaryTorpedo, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette, Shell, ExplosiveShell }
    public class GlobalController : MonoBehaviour
    {
        public List<GameObject> unitList; // список 
        public List<GameObject> selectedList; // спиков выделенных объектов
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
        public GameObject SelfGuidedMissile;
        public GameObject UnguidedMissile;
        public GameObject UnitaryTorpedo;
        public GameObject NukeTorpedo;
        public GameObject SpruteTorpedo;
        public GameObject ExplosiveBlast;
        public GameObject NukeBlast;
        public GameObject ShipDieBlast;
		public double[] RandomNormalPool;
		public double RandomNormalMin;
		public double[] RandomExponentPool;
		private float randomPoolBackCoount;


        public void Update()
		{
            if (randomPoolBackCoount < 0)
            {
                RandomNormalPool = Randomizer.Normal(1, 1, 128, 0, 128);
                RandomNormalMin = RandomNormalPool.Min();
                RandomExponentPool = Randomizer.Exponential(7, 128, 0, 128);
            }
            else randomPoolBackCoount -= Time.deltaTime;
		}
    }
    public abstract class SpaceShip : MonoBehaviour, IUnit
    {
        //base varibles
        protected UnitClass type;
        public UnitClass Type { get { return type; } }
        public UnitStateType aiStatus;
        public TargetStateType targetStatus;
        public Army team;
        public bool isSelected;
        public string UnitName;

        //depend varibles
        protected float radiolink;

        //constants
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speed; //set in child
        public float Health {
            set { armor.hitpoints = value; }
            get { return armor.hitpoints; }
        }
        public float MaxHealth { get { return armor.maxHitpoints; } }
        public float RadarRange { get { return radarRange; } }
        public float Speed { get { return speed; } }
        public Army Team { get { return team; } }
        public Transform ObjectTransform { get { return this.gameObject.transform; } }
        //modules
        public bool movementAiEnabled; // default true
        public bool combatAIEnabled;  // default true
        public bool selfDefenceModuleEnabled;  // default true
        public bool roleModuleEnabled; // default true
        protected float stealthness; //set in child
        public float Stealthness { get { return stealthness; } }
        public float inhibition;
        protected bool detected;
        public float cooldownDetected;
        //controllers
        protected IDriver Driver;
        protected IGunner Gunner;
        protected GlobalController Global;
        protected Armor armor;
        protected ForceShield shield;
        public ForceShield GetShieldRef { get { return shield; } }
        protected float synchAction;
        public float orderBackCount; //Make private after debug;
        public List<GameObject> enemys;
        public GameObject CurrentTarget;
        protected List<GameObject> capByTarget;
        public string ManeuverName; //debug only
        protected SortEnemys sortDelegate;
        protected void Start()//_______________________Start
        {
            movementAiEnabled = true;
            combatAIEnabled = true;
            selfDefenceModuleEnabled = true;
            roleModuleEnabled = true;
            sortDelegate = SortEnemysBase;
            StatsUp();//

            //Health = MaxHealth;
            cooldownDetected = 0;
            radarPover = 1;
            //waitingBackCount = 0.2f;
            aiStatus = UnitStateType.Idle;
            UnitName = type.ToString();
            Global = FindObjectOfType<GlobalController>();
            //armor = GetComponentInChildren<Armor>(); 
            armor = this.gameObject.GetComponent<Armor>(); 

            shield = this.gameObject.GetComponent<ForceShield>();
            //shield = GetComponentInChildren<ForceShield>();
            //
            Driver = new MovementController(this.gameObject);
            Gunner = new ShootController(this.gameObject);
            capByTarget = new List<GameObject>();
            Global.unitList.Add(gameObject);
            this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = false;

            if (team == Global.playerArmy)
                this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = true;
        }
        protected abstract void StatsUp();
        protected void Update()//______________________Update
        {
            Driver.Update();
            Gunner.Update();
            //cooldowns
            DecrementBaseCounters();
            DecrementCounters();
            //action
            if (synchAction <= 0)
            {
                synchAction = 0.05f;
                if (movementAiEnabled)
                {
                    if (combatAIEnabled)
                    {
                        CombatFunction();
                        if (targetStatus == TargetStateType.NotFinded)
                        {
                            if (orderBackCount < 0)
                            {
                                aiStatus = UnitStateType.Waiting;
                                orderBackCount = 1f;
                            }
                            else
                            {
                                aiStatus = UnitStateType.Idle;
                            }
                        }
                        else if (aiStatus == UnitStateType.Idle && aiStatus != UnitStateType.UnderControl)
                        {
                            //waitingBackCount = 0;
                            Driver.ClearQueue();
                            CombatManeuverFunction();
                            aiStatus = UnitStateType.MoveAI;
                        }
                        if (roleModuleEnabled)
                            RoleFunction();
                        if (selfDefenceModuleEnabled)
                            SelfDefenceFunction();
                    }
                    if (Driver.PathPoints == 0)
                        switch (aiStatus)
                        {
                            case UnitStateType.MoveAI:
                                {
                                    CombatManeuverFunction();
                                    break;
                                }
                            case UnitStateType.UnderControl:
                                {
                                    if (!isSelected)
                                    {
                                        aiStatus = UnitStateType.Waiting;
                                        orderBackCount = 1f;
                                    }
                                    break;
                                }
                            case UnitStateType.Waiting:
                                {

                                    break;
                                }
                            case UnitStateType.Idle:
                                {
                                    IdleManeuverFunction();
                                    break;
                                }
                        }
                }
                else
                {
                    if (combatAIEnabled)
                    {
                        CombatFunction();
                        if (roleModuleEnabled)
                            RoleFunction();
                        if (selfDefenceModuleEnabled)
                            SelfDefenceFunction();
                    }
                }
            }
        }
        protected void DecrementBaseCounters()
        {
            synchAction -= Time.deltaTime;
            if (orderBackCount > 0)
                orderBackCount -= Time.deltaTime;
            //waitingBackCount = Driver.backCount;//синхронизация счетчиков
            if (this.team != Global.playerArmy)
                cooldownDetected -= Time.deltaTime;
            else if (isSelected)
            {
                cooldownDetected = 0.1f;
                this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = true;
            }
            else
            {
                this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = false;
            }
            if (cooldownDetected < 0)
            {
                this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = false;
                //this.gameObject.transform.FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
                this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
            }
            if (inhibition > 0)
                inhibition -= Time.deltaTime;
        }
        protected abstract void DecrementCounters();
        protected void OnGUI()
        {
            Vector3 crd = Camera.main.WorldToScreenPoint(transform.position);
            crd.y = Screen.height - crd.y;
            if (team == Global.playerArmy)
            {
                if (isSelected)
                {
                    //Debug.Log("draw selected");
                    GUIStyle style = new GUIStyle();
                    style.fontSize = 12;
                    //style.font = GuiProcessor.getI.rusfont;
                    style.normal.textColor = Color.cyan;
                    style.alignment = TextAnchor.MiddleCenter;
                    //style.fontStyle = FontStyle.Italic;

                    GUI.DrawTexture(new Rect(crd.x - Global.GUIFrameWidth / 2, crd.y - Global.GUIFrameOffset, Global.GUIFrameWidth, Global.GUIFrameHeight), Global.AlliesSelectedGUIFrame);
                    GUI.Label(new Rect(crd.x - 120, crd.y - Global.NameFrameOffset, 240, 18), UnitName, style);
                }
                else
                {
                    //Debug.Log("draw allies");
                    GUI.DrawTexture(new Rect(crd.x - Global.GUIFrameWidth / 2, crd.y - Global.GUIFrameOffset, Global.GUIFrameWidth, Global.GUIFrameHeight), Global.AlliesGUIFrame);
                }
            }
            else if (cooldownDetected > 0)
            {
                //Debug.Log("draw enemy");
                GUIStyle style = new GUIStyle();
                style.fontSize = 12;
                //style.font = GuiProcessor.getI.rusfont;
                style.normal.textColor = Color.red;
                style.alignment = TextAnchor.MiddleCenter;
                //style.fontStyle = FontStyle.Italic;

                GUI.DrawTexture(new Rect(crd.x - Global.GUIFrameWidth / 2, crd.y - Global.GUIFrameOffset, Global.GUIFrameWidth, Global.GUIFrameHeight), Global.EnemyGUIFrame);
                //GUI.Label(new Rect(crd.x - 120, crd.y - Global.NameFrameOffset, 240, 18), UnitName, style);
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
        public virtual void MakeDamage(float damage)
        {
            this.Health = this.Health - damage;
        }
        public void Die()//____________________________Die
        {
            Explosion();
            Global.selectedList.Remove(this.gameObject);
            Global.unitList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        protected virtual void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        //AI logick
        protected bool CombatFunction()
        {
            targetStatus = TargetStateType.NotFinded;
            enemys = Scan();//поиск в зоне действия радара
            if (CurrentTarget == null)//текущей цели нет
            {
                if (enemys.Count > 0)
                {

                    bool output;
                    if (RadarWarningResiever() > 0)
                        output = OpenFire(RetaliatoryCapture());//ответный захват
                    else
                        output = OpenFire(GetNearest());//огонь по ближайшей
                    CooperateFire();//целеуказание союзникам
                    return output;
                }
                else
                {
                    enemys = RequestScout();//запрос разведданных
                    if (enemys.Count > 0)
                    {
                        if (enemys[0] == null) enemys.Clear();
                        else
                        return OpenFire(GetNearest());
                    }
                    //переходим в ожидение
                        return false;
                }
            }
            else
            {
                if (enemys.Count > 0 && sortDelegate(CurrentTarget.GetComponent<IUnit>(), enemys[0].GetComponent<IUnit>()) == 1)
                    CurrentTarget = enemys[0];
                float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
                if (distance < RadarRange)
                {
                    bool output = OpenFire(CurrentTarget);
                    CooperateFire();
                    return output;
                }
                else
                {
                    if (!TargetScouting())
                    {
                        CurrentTarget = null;
                        Gunner.ResetAim();
                        return false;//переходим в ожидение
                    }
                    else return OpenFire(CurrentTarget);
                }
            }
        }
        protected virtual bool CombatManeuverFunction()
        {
                switch (targetStatus)
                {
                    case TargetStateType.Captured:
                        {
                            return ToSecondaryDistance();
                        }
                    case TargetStateType.InPrimaryRange:
                        {
                            return IncreaseDistance();
                        }
                    case TargetStateType.InSecondaryRange:
                        {
                            return ToPrimaryDistance();
                        }
                    case TargetStateType.BehindABarrier:
                        {
                            return Rush();
                        }
                    default:
                        return false;
                }
        }
        protected virtual bool IdleManeuverFunction()
        {
            return PatroolPoint();
        }
        protected bool PatroolPoint()
        {
            ManeuverName = "Patrool";
            //waitingBackCount = 30f;
            Vector3 target1 = new Vector3(0, 0, 40); //Vector3 target1 = this.transform.forward * 40f;
            target1 += this.transform.position + new Vector3(0, 0.5f, 0);
            //            Debug.Log(target1);
            Driver.MoveToQueue(target1);

            float random = Convert.ToSingle(Randomizer.Uniform(-10, 10, 1)[0]);
            Vector3 target2;
            if (random > 0)
                target2 = new Vector3(40, 0, 0); //target2 = this.transform.right * 40f;
            else
                target2 = new Vector3(-40, 0, 0); //target2 = this.transform.right * -40f;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target4;
            if (random < 0)
                target4 = new Vector3(40, 0, 0); //target4 = this.transform.right * 40f;
            else
                target4 = new Vector3(-40, 0, 0); //target4 = this.transform.right * -40f;
            target4 += this.transform.position + new Vector3(0, 0.5f, 0);
            //          Debug.Log(target4);
            Driver.MoveToQueue(target4);

            Vector3 target3 = new Vector3(0, 0, -40);//Vector3 target3 = -this.transform.forward * 40f;
            target3 += this.transform.position + new Vector3(0, 0.5f, 0);
            //        Debug.Log(target3);
            Driver.MoveToQueue(target3);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool ToPrimaryDistance()
        {
            ManeuverName = "ToPrimaryDistance";
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRange(0) * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool ToSecondaryDistance()
        {
            ManeuverName = "ToSecondaryDistance";
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRange(1) * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position;
            return Driver.MoveToQueue(target);
        }
        protected bool ShortenDistance()
        {
            ManeuverName = "ShortenDistance";
            Vector3 target = this.transform.forward * 30f;
            target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool IncreaseDistance()
        {
            ManeuverName = "IncreaseDistance";
            Vector3 target = this.transform.forward * -30f;
            target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool Evasion()
        {
            ManeuverName = "Evasion";
            //waitingBackCount = 1f;
            float random = Convert.ToSingle(Randomizer.Uniform(-10, 10, 1)[0]);
            Vector3 target;
            if (random > 0)
                target = this.transform.right * (random + 20f);
            else
                target = this.transform.right * (random - 20f);
            target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool Rush()
        {
            ManeuverName = "Ruch";
            //waitingBackCount = 5f;
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRange(0) * 0.4f;
            return Driver.MoveToQueue(target);
        }

        protected abstract bool RoleFunction();
        protected abstract bool SelfDefenceFunction();
        protected bool SelfDefenceFunctionBase()
        {
            if (aiStatus == UnitStateType.Waiting && orderBackCount < 0 && ShortRangeRadar() > 3)
                Evasion();
            return true;
        }
        protected virtual int ShortRangeRadar()
        {
            int output = 0;
            List<GameObject> rounds = new List<GameObject>();
            rounds.AddRange(GameObject.FindGameObjectsWithTag("Torpedo"));
            foreach (GameObject x in rounds)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.5 && !x.GetComponent<Torpedo>().Allies(team))
                    return 10;
            }
            rounds.Clear();
            rounds.AddRange(GameObject.FindGameObjectsWithTag("Missile"));
            foreach (GameObject x in rounds)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.5)
                    output++;
                if (output > 9)
                    return output;
            }
            return output;
        }
        protected virtual int RadarWarningResiever()
        {
            capByTarget.Clear();
            foreach (GameObject x in enemys)
            {
                if (x.GetComponent<SpaceShip>().CurrentTarget == this)
                    capByTarget.Add(x);
            }
            capByTarget.Sort(delegate (GameObject x, GameObject y) { return sortDelegate(x.GetComponent<IUnit>(), y.GetComponent<IUnit>()); });
            return capByTarget.Count;
        }
        protected virtual GameObject RetaliatoryCapture()
        {
            return capByTarget[0];
        }
        protected bool OpenFire(GameObject target)
        {
            CurrentTarget = target;
            bool shot = false;
            float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
            RaycastHit hit;
            Physics.Raycast(this.transform.position, CurrentTarget.transform.position - this.transform.position, out hit);
            if (hit.transform==CurrentTarget.transform)
            {
                targetStatus = TargetStateType.Captured;//наведение
                Gunner.SetAim(CurrentTarget);
                if (inhibition <= 0)//если не действует подавление оружия
                {
                    if (distance < Gunner.GetRange(1) && distance > 50)
                    {
                        targetStatus = TargetStateType.InSecondaryRange;
                        shot = Gunner.ShootHim(CurrentTarget, 1);
                    }
                    if (distance < Gunner.GetRange(0))//выбор оружия
                    {
                        targetStatus = TargetStateType.InPrimaryRange;
                        shot = Gunner.ShootHim(CurrentTarget, 0);
                    }
                }
            }
            return shot;
        }
        protected List<GameObject> Scan() //___________Scan
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                if (distance < RadarRange)
                {
                    if (!x.GetComponent<SpaceShip>().Allies(team))
                    {
                        float multiplicator = Mathf.Pow(((-distance + RadarRange) * 0.02f), (1f / 5f)) * ((2f / (distance + 0.1f)) + 1);
                        if (Randomizer.Uniform(0, 100, 1)[0] < x.GetComponent<SpaceShip>().Stealthness * radarPover * multiplicator * 100)
                            enemys.Add(x);
                    }
                }
            }
            enemys.Sort(delegate (GameObject x, GameObject y) { return sortDelegate(x.GetComponent<IUnit>(), y.GetComponent<IUnit>()); });
            return enemys;
        }
        private int SortEnemysBase(IUnit x, IUnit y)
        {
                if (Vector3.Distance(this.transform.position, x.ObjectTransform.position) > Vector3.Distance(this.transform.position, y.ObjectTransform.position))
                    return 1;
                else return -1;
        }
        public virtual bool Allies(Army army)
        {
            if (army == Global.playerArmy)
            {
                cooldownDetected = 1;
                this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = true;
            }
            return (team == army);
        }
        protected List<GameObject> RequestScout()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<SpaceShip>().team == team)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange * radiolink)
                    {
                        enemys.AddRange(x.GetComponent<SpaceShip>().GetScout(this.transform.position));
                    }
                }
            }
            return enemys;
        }
        public GameObject[] GetScout(Vector3 sender)
        {
            if (Vector3.Distance(this.gameObject.transform.position, sender) < RadarRange * radiolink)
                return enemys.ToArray();
            else return new GameObject[0];
        }
        protected bool TargetScouting()
        {
            List<GameObject> scoutingenemys = RequestScout();
            return scoutingenemys.Exists(x => CurrentTarget);
        }
        protected GameObject GetNearest()
        {
            return enemys[0];
        }
        protected void CooperateFire()
        {
            foreach (GameObject x in Global.unitList)
            {
                if ((x.GetComponent<IUnit>().Team == team) && (x.GetComponent<IUnit>().Type == type))
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange * radiolink)
                    {
                        x.GetComponent<SpaceShip>().GetFireSupport(CurrentTarget);
                    }
                }
            }
        }
        public void GetFireSupport(GameObject Target)
        {
            CurrentTarget = Target;
        }
        public virtual void SendTo(Vector3 destination)
        {
            orderBackCount = Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveTo(destination);
        }
        public virtual void SendToQueue(Vector3 destination)
        {
            orderBackCount += Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveToQueue(destination);
        }
    }
    public class MovementController : IDriver
    {
        private GameObject walker;
        //private Vector3 moveDestination;
        private Queue<Vector3> path; //очередь путевых точек
        public int PathPoints {
            get
            {
                if ((walker.GetComponent<NavMeshAgent>().pathEndPosition - walker.transform.position).magnitude < 1)
                    return path.Count;
                else return path.Count + 1;
            }
        }
        //public float backCount; //время обновления пути.
        public MovementController(GameObject walker)
        {
            this.walker = walker;
            UpdateSpeed();
            path = new Queue<Vector3>();
            walker.GetComponent<NavMeshAgent>().SetDestination(walker.transform.position);
            //Debug.Log("Driver online");
        }
        public void Update()
        {
            if ((walker.GetComponent<NavMeshAgent>().pathEndPosition - walker.transform.position).magnitude < 10)
            {
                UpdateSpeed();
                if (path.Count > 1)
                {
                    //Debug.Log("1");
                    //backCount = Vector3.Distance(walker.transform.position, path.Peek()) / (walker.GetComponent<NavMeshAgent>().speed*0.9f);
                    walker.GetComponent<NavMeshAgent>().SetDestination(path.Dequeue());
                }
                if (path.Count == 1&& (walker.GetComponent<NavMeshAgent>().pathEndPosition - walker.transform.position).magnitude < 1)
                {
                    //backCount = Vector3.Distance(walker.transform.position, path.Peek()) / (walker.GetComponent<NavMeshAgent>().speed * 0.9f);
                    walker.GetComponent<NavMeshAgent>().SetDestination(path.Dequeue());
                }
            }
            //else backCount -= Time.deltaTime;
        }
        public void UpdateSpeed()
        {
            walker.GetComponent<NavMeshAgent>().speed = walker.GetComponent<SpaceShip>().Speed;
            walker.GetComponent<NavMeshAgent>().acceleration = walker.GetComponent<SpaceShip>().Speed * 1.6f;
            if (walker.GetComponent<SpaceShip>().CurrentTarget == null)
                walker.GetComponent<NavMeshAgent>().angularSpeed = walker.GetComponent<SpaceShip>().Speed * 3.3f;
            else
                walker.GetComponent<NavMeshAgent>().angularSpeed = walker.GetComponent<SpaceShip>().Speed * 0.05f;
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
        public GameObject body;
        private float turnSpeed;
        private GameObject Target;
        //private Vector3 oldTargetPosition;
        //private Vector3 aimPoint; //точка сведения
        private ForceShield shield;
        private float[] synchWeapons;
        private int[] indexWeapons;
        //private float averageRoundSpeed;
        //private float averageRange;

        public ShootController(GameObject body)
        {
            this.body = body;
            turnSpeed = body.GetComponent<SpaceShip>().Speed * 0.5f;
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

            shield = body.GetComponent<SpaceShip>().GetShieldRef;
            //Debug.Log("Gunner online");
        }
        public bool ShootHim(GameObject target, int slot)
        {
            if (synchWeapons[slot] <= 0)
            {
                float angel = Vector3.Angle(target.transform.position - body.transform.position, body.transform.forward);
                if (angel < weapons[slot][0].Dispersion * 5 || angel < 1)
                {
                    if (indexWeapons[slot] >= weapons[slot].Length)
                        indexWeapons[slot] = 0;
                    if (weapons[slot][indexWeapons[slot]].Cooldown <= 0)
                    {
                        shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                        synchWeapons[slot] = this.weapons[slot][0].CoolingTime / this.weapons[slot].Length;
                        bool output = weapons[slot][indexWeapons[slot]].Fire(target);
                        indexWeapons[slot]++;
                        return output;
                    }
                    else indexWeapons[slot]++;
                }
            }
            return false;
        }
        public void Update()
        {
            for (int slot = 0; slot< weapons.Length; slot++ )
            if (synchWeapons[slot] > 0)
                synchWeapons[slot] -= Time.deltaTime;

            if (Target != null)
            {
                //Debug.Log("target - " + Target.transform.position);
                //Debug.Log("aim - " + aimPoint);
                //наведение на цель
                Quaternion targetRotation = Quaternion.LookRotation((Target.transform.position - body.transform.position).normalized * turnSpeed, new Vector3(0, 1, 0));
                body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, Time.deltaTime * body.GetComponent<SpaceShip>().Speed * 0.2f);
            }
        }
        public bool SetAim(GameObject target)
        {
            if (Target == null)
            {
                Target = target;
                //Debug.Log("set target - " + Target.transform.position);
                //Debug.Log("set aim - " + oldTargetPosition);
                return true;
            }
            else return false;
        }
        public bool ResetAim()
        {
            Target = null;
            return true;
        }
        public float GetRange(int slot)
        {
            return weapons[slot][0].Range;
        }
    }
    public abstract class Weapon : MonoBehaviour, IWeapon
    {
        protected GlobalController Global;
        protected float range;
        public int ammo;
        protected float coolingTime;
        public float cooldown;
        public float dispersion; //dafault 0;
        protected float shildBlinkTime; //default 0.01
        protected float averageRoundSpeed; //default 1000;

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
        }

        public bool Fire(GameObject target)
        {
            float distance;
            float approachTime;
            Vector3 aimPoint = target.transform.position;
            //Debug.Log(target.GetComponent<NavMeshAgent>().velocity);

            distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до цели
            approachTime = distance / averageRoundSpeed;
            aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime; //первое приближение

            distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до точки первого приближения
            approachTime = distance / averageRoundSpeed;
            aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime * 1.1f; //второе приближение

            //distance = Vector3.Distance(this.gameObject.transform.position, aimPoint);
            //approachTime = distance / averageRoundSpeed;
            //aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime; //третье приближение

            Quaternion targetRotation = Quaternion.LookRotation((aimPoint - this.transform.position).normalized, new Vector3(0, 1, 0)); //донаводка
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 10);
            if ((ammo > 0) && (cooldown <= 0))
            {
                //Debug.Log("Fire");
                Shoot(target.transform);
                cooldown = coolingTime;
                ammo--;
            }
            return false;
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
        public void Update()
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
        public void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lt = 0;
        }
        public virtual void Update()
        {
            if (target != Vector3.zero)//контроль курса
            {
                Quaternion targetRotation = Quaternion.LookRotation(target - this.transform.position, new Vector3(0, 1, 0));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
            }
            //полет по прямой
            float multiplicator = Mathf.Pow((lt * 0.5f), (1f / 8f)) * 0.7f;
            gameObject.GetComponent<Rigidbody>().velocity = transform.forward * Speed * multiplicator;//AddForce(transform.forward * Speed * multiplicator, ForceMode.Acceleration);
            if (Vector3.Distance(this.transform.position, target) < explosionRange)
                Explode();
            else
                lt += Time.deltaTime;
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
