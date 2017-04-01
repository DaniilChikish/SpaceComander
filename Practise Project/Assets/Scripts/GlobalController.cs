using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public enum UnitClass { Scout, Recon, ECM, Figther, Bomber, Command, LR_Corvette, Guard_Corvette, Support_Corvette };
    public enum UnitStateType { MoveAI, UnderControl, Waiting, Idle};
    public enum TargetStateType { BehindABarrier,  InPrimaryRange, InSecondaryRange, Captured, NotFinded}; 
    //public enum ImpactType { ForestStaticImpact };
    //public enum TerrainType { Plain, Forest };
    public enum Army { Green, Red, Blue };
    public enum WeaponType { Cannon, Laser, Plazma, Missile, Torpedo }
    public enum BlastType { UnitaryTorpedo, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette }
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
        public GameObject CannonUnitaryShell;
        public AudioClip CannonShootSound;
        public GameObject CannonShellBlast;
        public GameObject LaserBeam;
        public GameObject PlasmaSphere;
        public GameObject SelfGuidedMissile;
        public GameObject UnguidedMissile;
        public GameObject MissileBlast;
        public GameObject UnitaryTorpedo;
        public GameObject SpruteTorpedo;
        public GameObject UnitaryTorpedoBlast;
        public GameObject SmallShipDieBlast;
        public GameObject MediumShipDieBlast;
        public GameObject CorvetteDieBlast;
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
    public abstract class Unit : MonoBehaviour
    {
        //base varibles
        protected UnitClass type;
        public UnitClass Type { get { return type; } }
        public UnitStateType aiStatus;
        public TargetStateType targetStatus;
        public Army Team;
        public bool isSelected;
        public string UnitName;

        //depend varibles
        public float Health;
        protected float radiolink;

        //constants
        protected float maxHealth; //set in child
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speed; //set in child
        public float MaxHealth { get { return maxHealth; } }
        public float RadarRange { get { return radarRange; } }
        public float Speed { get { return speed; } }
        public Army Army { get { return Team; } }
        //modules
        public bool movementAiEnabled; // default true
        public bool battleAIEnabled;  // default true
        public bool selfDefenceModuleEnabled;  // default true
        public bool roleModuleEnabled; // default true
        protected float stealthness; //set in child
        public float Stealthness { get { return stealthness; } }
        public float inhibition;
        protected bool detected;
        protected float cooldownDetected;
        //controllers
        protected MovementController Driver;
        protected ShootController Gunner;
        protected GlobalController Global;
        protected float synchAction;
        public float orderBackCount; //Make private after debug;
        public List<GameObject> enemys;
        public GameObject CurrentTarget;
        protected List<GameObject> capByTarget;
        public string ManeuverName; //debug only

        protected void Start()//_______________________Start
        {
            movementAiEnabled = true;
            battleAIEnabled = true;
            selfDefenceModuleEnabled = true;
            roleModuleEnabled = true;

            StatsUp();//

            Health = MaxHealth;
            cooldownDetected = 0;
            radarPover = 1;
            //waitingBackCount = 0.2f;
            aiStatus = UnitStateType.Idle;
            UnitName = type.ToString();
            Driver = new MovementController(this.gameObject);
            Gunner = new ShootController(this.gameObject);
            capByTarget = new List<GameObject>();
            Global = FindObjectOfType<GlobalController>();
            Global.unitList.Add(gameObject);
            this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = false;

            if (Team == Global.playerArmy)
                this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = true;
        }
        protected abstract void StatsUp();
        protected void Update()//______________________Update
        {
            if (Health < 0)
                Die();
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
                    if (battleAIEnabled)
                    {
                        BattleFunction();
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
                            BattleManeuverFunction();
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
                                    BattleManeuverFunction();
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
                    if (battleAIEnabled)
                    {
                        BattleFunction();
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
            if (this.Team != Global.playerArmy)
                cooldownDetected -= Time.deltaTime;
            else if (isSelected)
            {
                cooldownDetected = 0.1f;
                this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = true;
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
            if (Team == Global.playerArmy)
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

        protected void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        float multoplicator = 1;
                        if (!gameObject.GetComponent<ForceShield>().shildOwerheat) multoplicator = multoplicator * 0.01f;
                        this.Health -= collision.gameObject.GetComponent<Round>().Damage * multoplicator * 1.2f;
                        break;
                    }
                case "Energy":
                    {
                        float multoplicator = 1;
                        if (!gameObject.GetComponent<ForceShield>().shildOwerheat) multoplicator = multoplicator * 0.05f;
                        this.Health -= collision.gameObject.GetComponent<Round>().Damage * multoplicator * 0.2f;
                        break;
                    }
            }
        }
        protected void OnTriggerStay(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "Explosion":
                    {
                        float multiplicator = Mathf.Pow(((-Vector3.Distance(this.gameObject.transform.position, other.gameObject.transform.position) + other.gameObject.GetComponent<Explosion>().MaxRadius) * 0.01f), (1 / 3));
                        if (!gameObject.GetComponent<ForceShield>().shildOwerheat) multiplicator = multiplicator * 0.15f;
                        this.Health = this.Health - other.gameObject.GetComponent<Explosion>().Damage * multiplicator;
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
        public void Die()//____________________________Die
        {
            Instantiate(Global.SmallShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            Global.selectedList.Remove(this.gameObject);
            Global.unitList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        //AI logick
        protected bool BattleFunction()
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
                        return OpenFire(GetNearest());
                    else//переходим в ожидение
                        return false;
                }
            }
            else
            {
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
        protected virtual bool BattleManeuverFunction()
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
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRangePrimary() * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool ToSecondaryDistance()
        {
            ManeuverName = "ToPrimaryDistance";
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRangeSecondary() * 0.9f + new Vector3(0, 0.5f, 0);
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
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRangePrimary() * 0.4f;
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
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.5 && !x.GetComponent<Torpedo>().Allies(Team))
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
                if (x.GetComponent<Unit>().CurrentTarget == this)
                    capByTarget.Add(x);
            }
            capByTarget.Sort(delegate (GameObject x, GameObject y)
            {
                if (Vector3.Distance(this.transform.position, x.transform.position) > Vector3.Distance(this.transform.position, y.transform.position))
                    return 1;
                else return -1;
            });
            return capByTarget.Count;
        }
        protected virtual GameObject RetaliatoryCapture()
        {
            return capByTarget[0];
        }
        protected bool OpenFire(GameObject target)
        {
            CurrentTarget = target;
            float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
            RaycastHit hit;
            Physics.Raycast(this.transform.position, CurrentTarget.transform.position - this.transform.position, out hit);
            if (hit.transform==CurrentTarget.transform)
            {
                targetStatus = TargetStateType.Captured;//наведение
                Gunner.SetTarget(CurrentTarget);
                if (inhibition <= 0)//если не действует подавление оружия
                {
                    if (distance < Gunner.GetRangePrimary())//выбор оружия
                    {
                        targetStatus = TargetStateType.InPrimaryRange;
                        return Gunner.ShootHimPrimary(CurrentTarget);
                    }
                    else if (distance < Gunner.GetRangeSecondary())
                    {
                        targetStatus = TargetStateType.InSecondaryRange;
                        return Gunner.ShootHimSecondary(CurrentTarget);
                    }
                }
            }
            return false;
        }
        protected List<GameObject> Scan() //___________Scan
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                if (distance < RadarRange)
                {
                    if (!x.GetComponent<Unit>().Allies(Team))
                    {
                        float multiplicator = Mathf.Pow(((-distance + RadarRange) * 0.02f), (1f / 5f)) * ((2f / (distance + 0.1f)) + radarPover);
                        if (Randomizer.Uniform(0, 100, 1)[0] < x.GetComponent<Unit>().Stealthness * multiplicator * 100)
                            enemys.Add(x);
                    }
                }
            }
            enemys.Sort(delegate (GameObject x, GameObject y)
            {
                if (Vector3.Distance(this.transform.position, x.transform.position) > Vector3.Distance(this.transform.position, y.transform.position))
                    return 1;
                else return -1;
            });
            return enemys;
        }
        public virtual bool Allies(Army army)
        {
            if (army != Global.playerArmy)
            {
                cooldownDetected = 1;
                this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = true;
            }
            return (Team == army);
        }
        protected List<GameObject> RequestScout()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<Unit>().Team == Team)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange * radiolink)
                    {
                        enemys.AddRange(x.GetComponent<Unit>().GetScout(this.transform.position));
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
                if (x.GetComponent<Unit>().Team == Team)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange * radiolink)
                    {
                        x.GetComponent<Unit>().GetFireSupport(CurrentTarget);
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
    public class MovementController
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
            walker.GetComponent<NavMeshAgent>().speed = walker.GetComponent<Unit>().Speed;
            walker.GetComponent<NavMeshAgent>().acceleration = walker.GetComponent<Unit>().Speed * 1.6f;
            if (walker.GetComponent<Unit>().CurrentTarget == null)
                walker.GetComponent<NavMeshAgent>().angularSpeed = walker.GetComponent<Unit>().Speed * 3.3f;
            else
                walker.GetComponent<NavMeshAgent>().angularSpeed = walker.GetComponent<Unit>().Speed * 0.05f;
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
    public class ShootController
    {
        private Weapon[] primary;
        private Weapon[] secondary;
        public GameObject body;
        private GameObject Target;
        private Vector3 oldTargetPosition;
        private Vector3 aimPoint;
        private ForceShield shield;
        //private float backCount;
        private float synchPrimary;
        private int indexPrimary;
        private float synchSecondary;
        private int indexSecondary;
        public ShootController(GameObject body)
        {
            this.body = body;
            this.primary = body.transform.FindChild("Primary").GetComponentsInChildren<Weapon>();
            synchPrimary = this.primary[0].CoolingTime / this.primary.Length;
            indexPrimary = 0;

            this.secondary = body.transform.FindChild("Secondary").GetComponentsInChildren<Weapon>();
            synchSecondary = this.secondary[0].CoolingTime / this.secondary.Length;
            indexSecondary = 0;

            //Target = null;

            shield = body.GetComponent<ForceShield>();
            //Debug.Log("Gunner online");
        }
        public bool ShootHimPrimary(GameObject target)
        {
            if (synchPrimary < 0)
            {
                float angel = Vector3.Angle(aimPoint - body.transform.position, body.transform.forward);
                if (angel < primary[0].dispersion * 5 || angel < 1)
                {
                    if (indexPrimary >= primary.Length)
                        indexPrimary = 0;
                    if (primary[indexPrimary].Cooldown <= 0)
                    {
                        shield.Blink(primary[indexPrimary].ShildBlink);
                        bool output = primary[indexPrimary].Fire(target.transform);
                        indexPrimary++;
                        return output;
                    }
                    else indexPrimary++;
                }
            }
            return false;
        }
        public bool ShootHimSecondary(GameObject target)
        {
            if (synchSecondary < 0)
            {
                float angel = Vector3.Angle(aimPoint - body.transform.position, body.transform.forward);
                if (angel < secondary[0].dispersion * 5 || angel < 1)
                {
                    if (indexSecondary >= secondary.Length)
                        indexSecondary = 0;
                    if (secondary[indexSecondary].Cooldown <= 0)
                    {
                        shield.Blink(secondary[indexSecondary].ShildBlink);
                        bool output = secondary[indexSecondary].Fire(target.transform);
                        indexSecondary++;
                        return output;
                    }
                    else indexSecondary++;
                }
            }
            return false;
        }
        public void Update()
        {
            if (synchPrimary > 0)
                synchPrimary -= Time.deltaTime;
            else
                synchPrimary = this.primary[0].CoolingTime / this.primary.Length;
            if (synchSecondary > 0)
                synchSecondary -= Time.deltaTime;
            else
                synchSecondary = this.secondary[0].CoolingTime / this.secondary.Length;

            if (Target != null)
            {
                //просчет новой точки упреждения
                float time = 2;
                aimPoint = oldTargetPosition + ((Target.transform.position - oldTargetPosition) * time); //корректировка на движение цели

                float distance = Vector3.Distance(body.transform.position, aimPoint);        
                float approachTime;
                if (distance < primary[0].Range)
                    approachTime = distance / primary[0].RoundSpeed;
                else
                    approachTime = distance / secondary[0].RoundSpeed;
                aimPoint = aimPoint + ((Target.transform.position - oldTargetPosition) * (approachTime * 1.5f)); //корректировка на движение снаряда

                //Debug.Log("target - " + Target.transform.position);
                //Debug.Log("aim - " + aimPoint);
                //наведение на точку упреждения
                Quaternion targetRotation = Quaternion.LookRotation(aimPoint - body.transform.position, new Vector3(0, 1, 0));
                body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, Time.deltaTime * body.GetComponent<Unit>().Speed * 0.2f);
                oldTargetPosition = Target.transform.position;
            }
            else oldTargetPosition = Vector3.zero;
        }
        public bool SetTarget(GameObject target)
        {
            if (Target == null)
            {
                Target = target;
                oldTargetPosition = target.transform.position;
                //Debug.Log("set target - " + Target.transform.position);
                //Debug.Log("set aim - " + oldTargetPosition);
                return true;
            }
            else return false;
        }
        //public bool SetAIM(Vector3 aimPoint)
        //{

        //}
        public bool ResetAim()
        {
            Target = null;
            return true;
        }
        public float GetRangePrimary()
        {
            return primary[0].Range;
        }
        public float GetRangeSecondary()
        {
            return secondary[0].Range;
        }
    }
    public abstract class Weapon : MonoBehaviour
    {
        protected GlobalController Global;
        protected float range;
        public int ammo;
        protected float coolingTime;
        public float cooldown;
        public float dispersion;
        protected float shildBlinkTime;
        protected float avarageRounSpeed; //default 1000;
        public float ShildBlink { get { return shildBlinkTime; } }
        //public WeaponType Type;


        public float Range { get { return range; } }
        public float RoundSpeed { get { return avarageRounSpeed; } }
        public int Ammo { get { return ammo; } }
        public float Cooldown { get { return cooldown; } }
        public float CoolingTime { get { return coolingTime; } }

        protected void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            avarageRounSpeed = 500;
            StatUp();

        }
        protected abstract void StatUp();
        // Update is called once per frame
        public void Update()
        {
            if (cooldown > 0)
                cooldown -= Time.deltaTime;
        }

        public bool Fire(Transform target)
        {
            if ((ammo > 0) && (cooldown <= 0))
            {
                //Debug.Log("Fire");
                Shoot(target);
                cooldown = coolingTime;
                ammo--;
            }
            return false;
        }
        protected abstract void Shoot(Transform target);
    }
    public abstract class Round : MonoBehaviour
    {
        public float speed;
        public float damage;
        public float ttl;
        public float Speed { get { return speed; } }
        public float Damage { get { return damage; } }

        // Use this for initialization
        protected virtual void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }

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
        //public virtual float GetEnergy()
        //{
        //    float output = damage;
        //    damage = 0;
        //    return output;
        //}
        protected virtual void Explode()
        {
            Destroy(this.gameObject);
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
        public void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lt = 0;
        }
        public virtual void Update()
        {
            if (target != null)//контроль курса
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
            Instantiate(Global.UnitaryTorpedoBlast, this.transform.position, this.transform.rotation);
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
