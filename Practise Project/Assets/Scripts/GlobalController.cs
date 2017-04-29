using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace PracticeProject
{
    public enum UnitClass { Scout, Recon, ECM, Figther, Bomber, Command, LR_Corvette, Guard_Corvette, Support_Corvette, Turret};
    public enum UnitStateType { MoveAI, UnderControl, Waiting};
    public enum SquadStatus { Free, InSquad, SquadMaster }
    public enum TacticSituation { SectorСlear, Attack, Defense, Retreat, ExitTheBattle }
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
    public delegate int SortSpaceShip(IUnit x, IUnit y);
    public interface IDriver
    {
        void Update();
        void UpdateSpeed();
        bool MoveTo(Vector3 destination);
        bool MoveToQueue(Vector3 destination);
        void ClearQueue();
        int PathPoints { get; }
        Vector3 NextPoint{ get; }
    }
    public interface IGunner
    {
        void Update();
        bool SetAim(Transform target);
        bool ShootHim(SpaceShip target, int slot);
        bool Volley(SpaceShip[] targets, int slot);
        bool ResetAim();
        void ReloadWeapons();
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
        void Reset();
        void InstantCool();
        bool Fire(GameObject target);
    }
    public enum ShellType { Solid, SolidAP, Subcaliber, HightExplosive, Camorous, CamorousAP, Uranium, Сumulative, Railgun, SolidBig, CamorousBig, SubcaliberBig}
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
    public enum MissileType { Hunter, Bombardier, Metheor }
    public enum TorpedoType { Unitary, Nuke, Sprute }
    public enum BlastType { UnitaryTorpedo, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette, Shell, ExplosiveShell }
    public interface IImpact
    {
        string ImpactName { get; }
        void ActImpact();
        void CompleteImpact();
        string ToString();
    }
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
        private void Start()
        {
            //Debug.Log("GlobalController started");
            Mission = FindObjectOfType<Scenario>();
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
    public abstract class SpaceShip : MonoBehaviour, IUnit
    {
        //base varibles
        protected UnitClass type;
        public UnitClass Type { get { return type; } }
        protected UnitStateType aiStatus;
        protected TargetStateType targetStatus;
        protected TacticSituation situation;
        public Army team;
        public bool isSelected;
        public string UnitName;
        public List<IImpact> Impacts;
        public Vector3 Anchor;

        //depend varibles
        protected float radiolink;

        //constants
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speed; //set in child
        public float Health { set { armor.hitpoints = value; } get { return armor.hitpoints; } }
        public float MaxHealth { get { return armor.maxHitpoints; } }
        public float ShieldRecharging { set { shield.recharging = value; } get { return shield.recharging; } }
        public float RadarRange { set { radarRange = value; } get { return radarRange; } }
        public float Speed { set { speed = value; } get { return speed; } }
        public Army Team { get { return team; } }
        public Transform ObjectTransform { get { return this.gameObject.transform; } }
        //modules
        public bool movementAiEnabled; // default true
        public bool combatAIEnabled;  // default true
        public bool selfDefenceModuleEnabled;  // default true
        public bool roleModuleEnabled; // default true
        protected float stealthness; //set in child
        public float Stealthness { get { return stealthness; } }
        protected bool detected;
        public float cooldownDetected;
        public bool NeedReloading;
        //controllers
        protected IDriver Driver;
        protected IGunner Gunner;
        protected GlobalController Global;
        protected Armor armor;
        protected ForceShield shield;
        public ForceShield GetShieldRef { get { return shield; } }
        protected float synchAction;
        public float orderBackCount; //Make private after debug;
        protected List<SpaceShip> enemys = new List<SpaceShip>();
        protected List<SpaceShip> allies = new List<SpaceShip>();
        public SpaceShip CurrentTarget;
        protected List<SpaceShip> capByTarget;
        public string ManeuverName; //debug only
        protected SortSpaceShip EnemySortDelegate;
        protected SortSpaceShip AlliesSortDelegate;
        protected SquadStatus unitSquadStatus;
        protected SpaceShip[] Squad;

        //base interface
        protected void Start()//_______________________Start
        {
            Debug.Log("Unit " + this.gameObject.name + " started");
            movementAiEnabled = true;
            combatAIEnabled = true;
            selfDefenceModuleEnabled = true;
            roleModuleEnabled = true;
            EnemySortDelegate = SortEnemysBase;
            radarPover = 1;
            StatsUp();//

            //Health = MaxHealth;
            cooldownDetected = 0;
            //waitingBackCount = 0.2f;
            aiStatus = UnitStateType.Waiting;
            UnitName = type.ToString();
            Global = FindObjectOfType<GlobalController>();
            Global.unitList.Add(this);
            Anchor = this.transform.position;
            //
            armor = this.gameObject.GetComponent<Armor>();
            shield = this.gameObject.GetComponent<ForceShield>();
            //
            Driver = new MovementController(this.gameObject);
            Gunner = new ShootController(this);
            NeedReloading = false;
            capByTarget = new List<SpaceShip>();
            Squad = new SpaceShip[3];
            //List<SpaceShip> enemys = new List<SpaceShip>();
            //List<SpaceShip> allies = new List<SpaceShip>();
            Impacts = new List<IImpact>();

            this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = false;

            if (team == Global.playerArmy)
                this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = true;
        }
        public void ResetStats()
        {
            Start();
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
                    SituationAnalysys();
                    if (situation == TacticSituation.ExitTheBattle)
                    {
                        Driver.ClearQueue();
                        aiStatus = UnitStateType.MoveAI;
                        GoToBase();
                    }
                    if (combatAIEnabled)
                    {
                        CombatFunction();
                        if (targetStatus == TargetStateType.NotFinded)
                        {
                            if (orderBackCount <= 0)
                            {
                                aiStatus = UnitStateType.Waiting;
                                orderBackCount = 1f;
                            }
                            else if (aiStatus == UnitStateType.Waiting)
                            {
                                //aiStatus = UnitStateType.Idle;
                                situation = TacticSituation.SectorСlear;
                            }
                        }
                        else if (situation == TacticSituation.SectorСlear && aiStatus != UnitStateType.UnderControl)
                        {
                            //waitingBackCount = 0;
                            Driver.ClearQueue();
                            aiStatus = UnitStateType.MoveAI;
                            CombatManeuverFunction();
                            //RegroupingManeuver();
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
                                    aiStatus = UnitStateType.MoveAI;
                                    if (unitSquadStatus == SquadStatus.InSquad && Squad[0] != null)
                                        SyncSquadManeuver();
                                    else
                                        CombatManeuverFunction();
                                    //RegroupingManeuver();
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
                                    if (situation == TacticSituation.SectorСlear)
                                        IdleManeuverFunction();
                                    else if (orderBackCount<0)
                                    {
                                        aiStatus = UnitStateType.MoveAI;
                                        if (unitSquadStatus == SquadStatus.InSquad && Squad[0] != null)
                                            SyncSquadManeuver();
                                        else
                                            CombatManeuverFunction();
                                    }
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
            if (Impacts.Count > 0)
            {
                for (int i = 0; i < Impacts.Count; i++)
                    Impacts[i].ActImpact();
            }
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
                Global.selectedList.Add(this);
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
            shield.Owerheat();
            Explosion();
            Global.selectedList.Remove(this);
            Global.unitList.Remove(this);
            Destroy(this.gameObject);
        }
        protected virtual void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        public void ReloadWeapons()
        {
            if (NeedReloading)
            {
                Gunner.ReloadWeapons();
                NeedReloading = false;
            }
        }

        //AI logick
        protected bool CombatManeuverFunction()
        {
            switch (situation)
            {
                case TacticSituation.Attack:
                    {
                        return AttackManeuver();
                    }
                case TacticSituation.Defense:
                    {
                        return DefenseManeuver();
                    }
                case TacticSituation.Retreat:
                    {
                        return RetreatManeuver();
                    }
                default:
                    return false;
            }
        }
        protected virtual bool AttackManeuver()
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
        protected virtual bool DefenseManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        return PatroolPoint();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return Evasion(CurrentTarget.transform.right);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        return PatroolLinePerpendicularly();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return PatroolLinePerpendicularly();
                    }
                default:
                    return false;
            }
        }
        //protected virtual bool RegroupingManeuver()
        //{
        //    if (this.unitSquadStatus == SquadStatus.SquadMaster && Driver.NextPoint != Vector3.zero)
        //    {
        //        if (Squad[1] != null)
        //        {
        //            Squad[1].SendToQueue(Driver.NextPoint + -this.transform.forward * RadarRange * 0.1f + this.transform.right * RadarRange * 0.2f);
        //        }
        //        if (Squad[2] != null)
        //        {
        //            Squad[2].SendToQueue(Driver.NextPoint + -this.transform.forward * RadarRange * 0.1f + -this.transform.right * RadarRange * 0.2f);
        //        }
        //        return true;
        //    }
        //    else return false;
        //}
        protected virtual bool RetreatManeuver()
        {
            SpaceShip nearest = FindAllies(UnitClass.Support_Corvette);
            if (nearest != null)
                return Driver.MoveToQueue(this.transform.position + new Vector3(0, 0.5f, 0) + -nearest.transform.forward * 30f);
            else
            {
                nearest = FindAllies(UnitClass.Guard_Corvette);
                if (nearest != null)
                    return Driver.MoveToQueue(this.transform.position + new Vector3(0, 0.5f, 0) + -nearest.transform.forward * 30f);
                else return BackToAncour();
            }
        }
        protected virtual bool IdleManeuverFunction()
        {
            if (Vector3.Distance(this.transform.position, Anchor) > 300)
                return BackToAncour();
            return PatroolPoint();
        }
        protected bool CombatFunction()
        {
            targetStatus = TargetStateType.NotFinded;
            Scan();//поиск в зоне действия радара
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
                if (enemys.Count > 0 && EnemySortDelegate(CurrentTarget.GetComponent<IUnit>(), enemys[0].GetComponent<IUnit>()) == 1)
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
        protected void SituationAnalysys()
        {
            switch (situation)
            {
                case TacticSituation.Attack:
                    {
                        if (allies.Count == 0)
                            situation = TacticSituation.Retreat;
                        else if (enemys.Count > allies.Count)
                            situation = TacticSituation.Defense;
                        if (enemys.Count == 0 && CurrentTarget == null)
                            situation = TacticSituation.SectorСlear;
                        break;
                    }
                case TacticSituation.Defense:
                    {
                        if (allies.Count == 0)
                            situation = TacticSituation.Retreat;
                        else if (enemys.Count < allies.Count)
                            situation = TacticSituation.Attack;
                        if (enemys.Count == 0 && CurrentTarget == null)
                            situation = TacticSituation.SectorСlear;
                        break;
                    }
                case TacticSituation.Retreat:
                    {
                        if (enemys.Count < allies.Count)
                            situation = TacticSituation.Attack;
                        else if (enemys.Count > allies.Count)
                            situation = TacticSituation.Defense;
                        if (enemys.Count == 0 && CurrentTarget == null)
                            situation = TacticSituation.SectorСlear;
                        break;
                    }
                case TacticSituation.ExitTheBattle:
                    {
                        if (unitSquadStatus == SquadStatus.SquadMaster)
                        {
                            if (Squad[1] != null)
                                Squad[1].unitSquadStatus = SquadStatus.Free;
                            if (Squad[2] != null)
                                Squad[2].unitSquadStatus = SquadStatus.Free;
                        }
                        else if (unitSquadStatus == SquadStatus.InSquad)
                        {
                            unitSquadStatus = SquadStatus.Free;
                        }
                        break;
                    }
                case TacticSituation.SectorСlear:
                    {
                        if (enemys.Count > 0)
                            situation = TacticSituation.Attack;
                        break;
                    }
            }
            if (unitSquadStatus == SquadStatus.InSquad)
            {
                if (Squad[0] == null)
                    unitSquadStatus = SquadStatus.Free;
                else situation = Squad[0].situation;
            }
            if (unitSquadStatus == SquadStatus.Free)
                FormSquad();
        }

        //sevice function
        //base maneuvers
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
        protected bool PatroolLineForward()
        {
            ManeuverName = "Patrool";
            Vector3 target2;
            target2 = this.transform.forward * 60f;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool PatroolLineForward(float length)
        {
            ManeuverName = "Patrool";
            Vector3 target2;
            target2 = this.transform.forward * length;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool PatroolLinePerpendicularly()
        {
            ManeuverName = "Patrool";

            float random = Convert.ToSingle(Randomizer.Uniform(-10, 10, 1)[0]);
            Vector3 target2;
            if (random > 0)
                /*target2 = new Vector3(60, 0, 0);*/ target2 = this.transform.right * 60f;
            else
                /*target2 = new Vector3(-60, 0, 0);*/ target2 = this.transform.right * -60f;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target4;
            if (random < 0)
                /*target4 = new Vector3(60, 0, 0);*/ target4 = this.transform.right * 60f;
            else
                /*target4 = new Vector3(-60, 0, 0);*/ target4 = this.transform.right * -60f;
            target4 += this.transform.position + new Vector3(0, 0.5f, 0);
            //          Debug.Log(target4);
            Driver.MoveToQueue(target4);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool PatroolLinePerpendicularly(float length)
        {
            ManeuverName = "Patrool";

            float random = Convert.ToSingle(Randomizer.Uniform(-10, 10, 1)[0]);
            Vector3 target2;
            if (random > 0)
                /*target2 = new Vector3(60, 0, 0);*/
                target2 = this.transform.right * length;
            else
                /*target2 = new Vector3(-60, 0, 0);*/
                target2 = this.transform.right * -length;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target4;
            if (random < 0)
                /*target4 = new Vector3(60, 0, 0);*/
                target4 = this.transform.right * length;
            else
                /*target4 = new Vector3(-60, 0, 0);*/
                target4 = this.transform.right * -length;
            target4 += this.transform.position + new Vector3(0, 0.5f, 0);
            //          Debug.Log(target4);
            Driver.MoveToQueue(target4);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool PatroolLineParallel()
        {
            ManeuverName = "Patrool";
            Vector3 target2;
            target2 = this.transform.forward * 60f;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target4;
            target4 = this.transform.forward * -60f;
            target4 += this.transform.position + new Vector3(0, 0.5f, 0);
            //          Debug.Log(target4);
            Driver.MoveToQueue(target4);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool PatroolLineParallel(float length)
        {
            ManeuverName = "Patrool";
            Vector3 target2;
            target2 = this.transform.forward * length;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target4;
            target4 = this.transform.forward * -length;
            target4 += this.transform.position + new Vector3(0, 0.5f, 0);
            //          Debug.Log(target4);
            Driver.MoveToQueue(target4);

            Vector3 target5;
            target5 = this.transform.position;

            return Driver.MoveToQueue(target5);
        }
        protected bool ToPrimaryDistance()
        {
            ManeuverName = "ToPrimaryDistance";
            Vector3 target = CurrentTarget.transform.position + (this.transform.position - CurrentTarget.transform.position).normalized * Gunner.GetRange(0) * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool ToSecondaryDistance()
        {
            ManeuverName = "ToSecondaryDistance";
            Vector3 target = CurrentTarget.transform.position + (this.transform.position - CurrentTarget.transform.position).normalized * Gunner.GetRange(1) * 0.9f + new Vector3(0, 0.5f, 0);
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
        protected bool Evasion(Vector3 hazardRight)
        {
            ManeuverName = "Evasion";
            //waitingBackCount = 1f;
            float random = Convert.ToSingle(Randomizer.Uniform(-10, 10, 1)[0]);
            Vector3 target;
            if (random > 0)
                target = hazardRight * (random + 20f);
            else
                target = hazardRight * (random - 20f);
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
        protected bool BackToAncour()
        {
            ManeuverName = "BackToAncour";
            return Driver.MoveToQueue(Anchor);
        }
        protected bool GoToBase()
        {
            Base[] bases = FindObjectsOfType<Base>();
            if (bases.Length > 0)
            {
                foreach (Base x in bases)
                    if (x.team == this.Team)
                        return Driver.MoveToQueue(x.GetInQueue(this));
                return BackToAncour();
            }
            else return BackToAncour();
        }

        //sensors
        protected void Scan() //___________Scan
        {
            enemys.Clear();
            allies.Clear();
            foreach (SpaceShip x in Global.unitList)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                if (distance < RadarRange)
                {
                    SpaceShip unknown = x.GetComponent<SpaceShip>();
                    if (!unknown.Allies(team))
                    {
                        float multiplicator = Mathf.Pow(((-distance + RadarRange) * 0.02f), (1f / 5f)) * ((2f / (distance + 0.1f)) + 1);
                        if (Randomizer.Uniform(0, 100, 1)[0] < unknown.Stealthness * radarPover * multiplicator * 100)
                            enemys.Add(x);
                    }
                    else
                    {
                        if (unknown != this)
                            if ((distance < this.RadarRange * this.radiolink) && (distance < unknown.RadarRange * unknown.radiolink))
                                allies.Add(x);
                    }
                }
            }
            enemys.Sort(delegate (SpaceShip x, SpaceShip y) { return EnemySortDelegate(x.GetComponent<IUnit>(), y.GetComponent<IUnit>()); });
            allies.Sort(delegate (SpaceShip x, SpaceShip y) { return AlliesSortDelegate(x.GetComponent<IUnit>(), y.GetComponent<IUnit>()); });
        }
        protected virtual GameObject ShortRangeRadar()
        {
            List<GameObject> hazard = new List<GameObject>();
            hazard.AddRange(GameObject.FindGameObjectsWithTag("Torpedo"));
            foreach (GameObject x in hazard)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.5 && !x.GetComponent<Torpedo>().Allies(team))
                    return x;
            }
            hazard.Clear();
            hazard.AddRange(GameObject.FindGameObjectsWithTag("Missile"));
            foreach (GameObject x in hazard)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.3)
                {
                    float angel = Vector3.Angle(this.transform.position-x.transform.position, x.transform.forward);
                    if (angel < 10)
                        return x;
                }
            }
            return null;
        }
        protected virtual int RadarWarningResiever()
        {
            capByTarget.Clear();
            foreach (SpaceShip x in enemys)
            {
                if (x.GetComponent<SpaceShip>().CurrentTarget == this)
                    capByTarget.Add(x);
            }
            capByTarget.Sort(delegate (SpaceShip x, SpaceShip y) { return EnemySortDelegate(x.GetComponent<IUnit>(), y.GetComponent<IUnit>()); });
            return capByTarget.Count;
        }
        private int SortEnemysBase(IUnit x, IUnit y)
        {
                if (Vector3.Distance(this.transform.position, x.ObjectTransform.position) > Vector3.Distance(this.transform.position, y.ObjectTransform.position))
                    return 1;
                else return -1;
        }
        public void ArmorCriticalAlarm()
        { situation = TacticSituation.ExitTheBattle; }
        public void ShieldCriticalAlarm()
        { situation = TacticSituation.Retreat; }

        //combat
        protected SpaceShip GetNearest()
        {
            return enemys[0];
        }
        protected virtual SpaceShip RetaliatoryCapture()
        {
            return capByTarget[0];
        }
        protected bool OpenFire(SpaceShip target)
        {
            CurrentTarget = target;
            bool shot = false;
            float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
            RaycastHit hit;
            Physics.Raycast(this.transform.position, CurrentTarget.transform.position - this.transform.position, out hit);
            if (hit.transform == CurrentTarget.transform)
            {
                targetStatus = TargetStateType.Captured;//наведение
                Gunner.SetAim(CurrentTarget.transform);
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
            return shot;
        } //_________OpenFire
        protected abstract bool RoleFunction();
        protected abstract bool SelfDefenceFunction();
        protected bool SelfDefenceFunctionBase()
        {
            GameObject hazard = ShortRangeRadar();
            if (hazard != null)
            {
                if (situation == TacticSituation.SectorСlear)
                {
                    Driver.ClearQueue();
                    aiStatus = UnitStateType.MoveAI;
                    situation = TacticSituation.Defense;
                    Evasion(hazard.transform.right);
                }
                else if (Driver.PathPoints == 0)
                {
                    aiStatus = UnitStateType.MoveAI;
                    Evasion(hazard.transform.right);
                }
                return true;
            }
            else return false;
        }

        //group interaction
        public virtual bool Allies(Army army)
        {
            if (army == Global.playerArmy)
            {
                cooldownDetected = 1;
                this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = true;
            }
            return (team == army);
        }
        protected List<SpaceShip> RequestScout()
        {
            List<SpaceShip> enemys = new List<SpaceShip>();
            foreach (SpaceShip x in allies)
            {
                enemys.AddRange(x.GetScout());
            }
            return enemys;
        }
        public SpaceShip[] GetScout()
        {
            return enemys.ToArray();
        }
        //public bool Ping(Vector3 sender)
        //{
        //    if (Vector3.Distance(this.gameObject.transform.position, sender) < RadarRange * radiolink)
        //        return true;
        //    else return false;
        //}
        protected bool TargetScouting()
        {
            List<SpaceShip> scoutingenemys = RequestScout();
            return scoutingenemys.Exists(x => CurrentTarget);
        }
        protected void CooperateFire()
        {
            foreach (SpaceShip x in allies)
            {
                if (x.Type == type)
                {
                    x.GetFireSupport(CurrentTarget);
                }
            }
        }
        public void GetFireSupport(SpaceShip Target)
        {
            CurrentTarget = Target;
        }
        //
        protected void FormSquad()
        {
            int inSquadCount = 0;
            Squad[0] = this;
            foreach (SpaceShip x in allies)
            {
                if (x.Type == this.Type)
                {
                    if (x.unitSquadStatus == SquadStatus.Free)
                    {
                        this.unitSquadStatus = SquadStatus.SquadMaster;
                        x.unitSquadStatus = SquadStatus.InSquad;
                        inSquadCount++;
                        this.Squad[inSquadCount] = x;
                        x.Squad[0] = this.Squad[0];
                        x.Squad[1] = this.Squad[1];
                        x.Squad[2] = this.Squad[2];
                    }
                    else if (x.unitSquadStatus == SquadStatus.SquadMaster)
                        if (x.ComeInSquadRequest(this))
                            return;
                    if (inSquadCount >= 2)
                        return;
                }
            }
        }
        protected bool ComeInSquadRequest(SpaceShip sender)
        {
            if (Squad[2] == null)
            {
                if (sender.unitSquadStatus == SquadStatus.Free)
                {
                    sender.unitSquadStatus = SquadStatus.InSquad;
                    Squad[2] = sender;
                    Squad[1].Squad[2] = sender;
                    return true;
                }
            }
            return false;
        }
        protected bool SyncSquadManeuver()
        {
            //Debug.Log("squad maneuver");
            Vector3 destination = Squad[0].SyncManeuverRequest(this);
            if (destination != Vector3.zero)
                return Driver.MoveToQueue(destination);
            else return CombatManeuverFunction();
        }
        public Vector3 SyncManeuverRequest(SpaceShip sender)
        {
            if (Driver.NextPoint != Vector3.zero)
            {
                if (sender = Squad[1])
                    return Driver.NextPoint + -this.transform.forward * RadarRange * 0.1f + this.transform.right * RadarRange * 0.2f;
                else if (sender = Squad[2])
                    return Driver.NextPoint + -this.transform.forward * RadarRange * 0.1f + -this.transform.right * RadarRange * 0.2f;
                else return Vector3.zero;
            }
            else return Vector3.zero;
        }
        //
        protected SpaceShip FindAllies(UnitClass ofType)
        {
            foreach (SpaceShip x in allies)
            {
                if (x.Type == ofType)
                    return x;
            }
            return null;
        }
        //
        protected int EMCSortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.Scout: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.ECM: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Recon: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default:
                    {
                        xPriority = 0;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.Scout: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.ECM: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Recon: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default:
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -100 && (xDictance - yDistance) < 100)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
        protected int ScoutSortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.Recon: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Scout: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.ECM: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default: //более крупные цели не интересны
                    {
                        xPriority = 0;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.Recon: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Scout: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.ECM: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default: //более крупные цели не интересны
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -100 && (xDictance - yDistance) < 100)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
        protected int ReconSortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.ECM: //не интерсен
                    {
                        xPriority = 0;
                        break;
                    }
                case UnitClass.Recon: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Scout: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default: //более крупные цели
                    {
                        xPriority = 10;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.ECM: //не интересен
                    {
                        yPriority = 0;
                        break;
                    }
                case UnitClass.Recon: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Scout: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default://более крупные цели
                    {
                        yPriority = 10;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -100 && (xDictance - yDistance) < 100)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
        protected int BomberSortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.Support_Corvette: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Guard_Corvette: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.LR_Corvette: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Bomber: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Figther: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default:
                    {
                        xPriority = 0;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.Support_Corvette: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Guard_Corvette: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.LR_Corvette: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Bomber: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Figther: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default:
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -300 && (xDictance - yDistance) < 300)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
        protected int LRCorvetteSortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.Support_Corvette: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Guard_Corvette: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.LR_Corvette: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Bomber: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default:
                    {
                        xPriority = 0;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.Support_Corvette: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Guard_Corvette: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.LR_Corvette: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Bomber: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default:
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -300 && (xDictance - yDistance) < 300)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
        protected int SupportCorvetteSortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.Support_Corvette: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Bomber: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                case UnitClass.Guard_Corvette: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                case UnitClass.LR_Corvette: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default:
                    {
                        xPriority = 0;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.Support_Corvette: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Bomber: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                case UnitClass.Guard_Corvette: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                case UnitClass.LR_Corvette: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default:
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -300 && (xDictance - yDistance) < 300)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
        //remote control
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
        private SpaceShip walker;
        private Transform walkerTransform;
        private NavMeshAgent walkerAgent;
        //private Vector3 moveDestination;
        private Queue<Vector3> path; //очередь путевых точек
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
            if ((walkerAgent.pathEndPosition - walkerTransform.position).magnitude < 10)
            {
                UpdateSpeed();
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
                if (angel < weapons[slot][0].Dispersion * 5 || angel < 1)
                {
                    if (indexWeapons[slot] >= weapons[slot].Length)
                        indexWeapons[slot] = 0;
                    if (weapons[slot][indexWeapons[slot]].Cooldown <= 0)
                    {
                        shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                        synchWeapons[slot] = this.weapons[slot][0].CoolingTime / this.weapons[slot].Length;
                        bool output = weapons[slot][indexWeapons[slot]].Fire(target.gameObject);
                        indexWeapons[slot]++;
                        return output;
                    }
                    else indexWeapons[slot]++;
                }
            }
            return false;
        }
        public bool Volley(SpaceShip[] targets, int slot)//relative cooldown indexWeapon, ignore angel;
        {
            if (indexWeapons[slot] >= weapons[slot].Length)
                indexWeapons[slot] = 0;
            if (synchWeapons[slot] <= 0 && weapons[slot][indexWeapons[slot]].Cooldown <= 0)
            {
                int i = 0, j = 0;
                shield.Blink(weapons[slot][indexWeapons[slot]].ShildBlink);
                synchWeapons[slot] = this.weapons[slot][0].CoolingTime / this.weapons[slot].Length;
                indexWeapons[slot]++;
                for (i = 0; i < weapons[slot].Length; i++)
                {
                    weapons[slot][i].InstantCool();
                    weapons[slot][i].Fire(targets[j].gameObject);
                    j++;
                    if (j >= targets.Length) j = 0;
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
        public bool SetAim(Transform target)
        {
            if (targetTransform == null)
            {
                targetTransform = target;
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
        protected GlobalController Global;
        protected float range;
        //protected int maxAmmo;
        protected int ammo;
        protected float coolingTime;
        protected float cooldown;
        protected float dispersion; //dafault 0;
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
        public void Reset()
        {
            Start();
            cooldown = coolingTime * 10;
        }
        public void InstantCool()
        {
            cooldown = 0;
        }
        public bool Fire(GameObject target)
        {
            float distance;
            float approachTime;
            Vector3 aimPoint = target.transform.position;
            //Debug.Log(target.GetComponent<NavMeshAgent>().velocity);

            distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до цели
            approachTime = distance / averageRoundSpeed;
            Vector3 targetVelocity = target.GetComponent<NavMeshAgent>().velocity;
            targetVelocity.y = 0; //исключаем вертикальную компоненту
            aimPoint = target.transform.position + targetVelocity * approachTime; //первое приближение

            distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до точки первого приближения
            approachTime = distance / averageRoundSpeed;
            targetVelocity = target.GetComponent<NavMeshAgent>().velocity;
            targetVelocity.y = 0;
            aimPoint = target.transform.position + targetVelocity * approachTime * 1.1f; //второе приближение

            //distance = Vector3.Distance(this.gameObject.transform.position, aimPoint);
            //approachTime = distance / averageRoundSpeed;
            //aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime; //третье приближение

            Quaternion targetRotation = Quaternion.LookRotation((aimPoint - this.transform.position).normalized, new Vector3(0, 1, 0)); //донаводка
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 10);
            if ((ammo > 0) && (cooldown <= 0))
            {
                //Debug.Log("Fire");
                this.GetComponentInChildren<ParticleSystem>().Play();
                Shoot(target.transform);
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
        protected float DropImpulse;//импульс сброса          
        protected float TurnSpeed;// скорость поворота ракеты            
        protected float explosionTime;// длительность жизни
        protected float AimCone;
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
            if (lt > 1.5)
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
                case "Shell":
                    {
                        Arm();
                        break;
                    }
                case "Unit":
                    {
                        if (lt > explosionTime / 20)
                            Explode();
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
