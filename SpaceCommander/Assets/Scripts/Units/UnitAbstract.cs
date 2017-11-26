using System;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.UI;
using DeusUtility;
using SpaceCommander.General;
using SpaceCommander.UI;
using SpaceCommander.Mechanics;
namespace SpaceCommander.Mechanics
{
    public abstract class Unit : MonoBehaviour
    {
        protected UnitClass type;
        [SerializeField]
        private Army team;
        public Dictionary<Army, bool> Relationship { get; protected set;}
        protected string unitName;
        public float cooldownDetected;
        #region Public properties
        public UnitClass Type { get { return type; } }
        public string UnitName { get { return unitName; } }
        public virtual Army Team { get { return team; } protected set { team = value; } }

        #endregion
        #region Abstract properties
        public abstract Unit CurrentTarget { get; }
        public abstract float Speed { get; }
        public float SpeedMultiplicator { set; get; }
        public abstract float Acceleration { get; }
        public float AccelerationMultiplicator { set; get; }
        public abstract float RotationSpeed { get; }
        public float RotationSpeedMultiplicator { set; get; }
        public abstract float ShiftSpeed { get; }
        public float ShiftSpeedMultiplicator { set; get; }
        public abstract float Hull { set; get; }
        public abstract float MaxHull { get; }
        public abstract void ArmorCriticalAlarm();
        public float MaxHullMultiplacator { set; get; }
        public abstract float ShellResist { get; }
        public abstract float EnergyResist { get; }
        public abstract float BlastResist { get; }
        public float ResistMultiplacator { set; get; }
        public abstract float ShieldForce { set; get; }
        public abstract float ShieldRecharging { get; }
        public float ShieldRechargingMultiplacator { set; get; }
        public abstract float ShieldCampacity { get; }
        public float ShieldCampacityMultiplacator { set; get; }
        public abstract bool ShieldOwerheat { get; }
        public abstract void ShieldCriticalAlarm();
        public abstract float RadarRange { get; }
        public float RadarRangeMultiplacator { set; get; }
        public abstract float Stealthness { get; }
        public float StealthnessMultiplacator { set; get; }
        public abstract Vector3 Velocity { get; }
        #endregion
        #region Abstract functions
        public abstract void MakeImpact(IImpact impact);
        public abstract bool HaveImpact(string impactName);
        public abstract void RemoveImpact(IImpact impact);
        public abstract void MakeDamage(float damage);
        public abstract void Die();
        #endregion
        public int SortEnemysBase(Unit x, Unit y)
        {
            if (Vector3.Distance(this.transform.position, x.transform.position) > Vector3.Distance(this.transform.position, y.transform.position))
                return 1;
            else return -1;
        }
        public abstract void ResetTarget();
        public RelationshipType CheckRelationship(Army army)
        {
            //if (army == GlobalController.Instance.playerArmy)
            //{
            //    cooldownDetected = 1;
            //    this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = true;
            //}
            if (Relationship.ContainsKey(army))
                if (Relationship[army])
                    return RelationshipType.Allies;
                else return RelationshipType.Enemys;
            else return RelationshipType.Neutral;
        }
        //protected Dictionary<IUseDriver, Vector3> followers;
        //private const float followingDistance = 50;
        //public Dictionary<IUseDriver, Vector3> Followers { get { return followers; } }
        //public void AddFollower(IUseDriver follower, Vector3 point)
        //{
        //    followers.Add(follower, point);
        //}
        //public virtual Vector3 AddFollower(IUseDriver follower)
        //{
            //Vector3[] defaultFollowingPoints =
            //{
            //    (Vector3.right * followingDistance + -Vector3.forward * followingDistance/2f),
            //    (-Vector3.right * followingDistance + -Vector3.forward * followingDistance/2f),
            //    (Vector3.up * followingDistance + -Vector3.forward * followingDistance/2+ Vector3.right * followingDistance * Mathf.Sqrt(3f)/2f),
            //    (Vector3.up * followingDistance + -Vector3.forward * followingDistance/2+ -Vector3.right * followingDistance * Mathf.Sqrt(3f)/2f),
            //    (-Vector3.up * followingDistance + -Vector3.forward * followingDistance/2+ Vector3.right * followingDistance * Mathf.Sqrt(3f)/2f),
            //    (-Vector3.up * followingDistance + -Vector3.forward * followingDistance/2+ -Vector3.right * followingDistance * Mathf.Sqrt(3f)/2f),
            //};
            //foreach (Vector3 p in defaultFollowingPoints)
            //    if (!followers.ContainsValue(p))
            //    {
            //        followers.Add(follower, p);
            //        return p;
            //    }
            //foreach (IUseDriver u in followers.Keys)
            //    if (u.Followers.Keys.Count < defaultFollowingPoints.Length)
            //       return u.AddFollower(follower);
        //    return Vector3.zero;
        //}
        //public Vector3 GetFollowPoint(Unit follower)
        //{
        //    return followers[follower];
        //}
        //public void RemoveFollower(Unit follower)
        //{
        //    followers.Remove(follower);
        //}
        public abstract void GetFireSupport(Unit Target);
    }
    public delegate int SortUnit(Unit x, Unit y);
}
namespace SpaceCommander.AI
{
    public abstract class SpaceShip : Unit, IEngine, ISpaceShipObservable
    {
        #region Class-constants
        public const float AIUpdateRate = 20f; //per second
        #endregion
        #region Base varibles
        protected UnitStateType aiStatus;
        protected TargetStateType targetStatus;
        protected TacticSituation situation;
        public bool isSelected;
        public Vector3 Anchor;
        #endregion
        #region GUI
        public HUDBase hud;
        private ShipManualController manualController;
        public Texture enemyIcon;
        public Texture aliesIcon;
        public Texture selectedIcon;
        private Renderer enemyMap;
        private Renderer aliesMap;
        private Renderer selectedMap;
        #endregion
        #region Depend varibles
        protected float radiolink;
        #endregion
        #region Object-constants
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speedThrust; //set in child
        protected float acceleration;
        protected float speedRotation;
        protected float speedShift;
        #endregion
        #region Override properties
        public override float Hull { set { armor.Hitpoints = value; } get { return armor.Hitpoints; } }
        public override float MaxHull { get { return armor.MaxHitpoints * (1 + MaxHullMultiplacator); } }
        //public override Army Team { get { return base.Team; } }
        public override Vector3 Velocity
        {
            get
            {
                if (ManualControl) return this.gameObject.GetComponent<Rigidbody>().velocity;
                else return Driver.Velocity;
            }
        }
        private GameObject[] reams; //jetream particles
        private AudioSource engineSound;
        public virtual Vector3 ScaleJetream
        {
            get
            {
                if (reams.Length > 0)
                    return reams[0].transform.localScale;
                else return Vector3.zero;
            }
            set
            {
                for (int i = 0; i < reams.Length; i++)
                    reams[i].transform.localScale = value;
                engineSound.volume = Global.Settings.SoundLevel * (0.4f + value.x * 0.4f);
                engineSound.pitch = 0.75f + value.x * 0.5f;
            }
        }
        public override float Acceleration { get { return acceleration * (1 + AccelerationMultiplicator); } }
        public override float Speed { get { return speedThrust * (1 + SpeedMultiplicator); } }
        public override float RotationSpeed { get { return speedRotation * (1 + RotationSpeedMultiplicator); } }
        public override float ShiftSpeed { get { return speedShift * (1 + ShiftSpeedMultiplicator); } }
        public override float RadarRange { get { return radarRange * (1 + RadarRangeMultiplacator); } }
        public override float ShieldForce { get { return shield.Force; } set { shield.Force = value; } }
        public override float ShieldRecharging { get { return shield.Recharging * (1 + ShieldRechargingMultiplacator); } }
        public override float ShieldCampacity { get { return shield.MaxCampacity * (1 + ShieldCampacityMultiplacator); } }
        public override bool ShieldOwerheat { get { return shield.IsOverheat; } }
        public override Unit CurrentTarget { get { return Gunner.Target; } }
        public override float ShellResist { get { return armor.ShellResist * (1 + ResistMultiplacator); } }
        public override float EnergyResist { get { return armor.EnergyResist * (1 + ResistMultiplacator); } }
        public override float BlastResist { get { return armor.BlastResist * (1 + ResistMultiplacator); } }
        public override float Stealthness { get { return stealthness * (1 + StealthnessMultiplacator); } }
        #endregion
        #region Own properties
        public IShield GetShieldRef { get { return shield; } }
        public SpellModule[] Module { get { return module; } }
        #endregion
        #region Interface
        public IWeapon[] PrimaryWeapon { get { return Gunner.Weapon[0]; } }
        public IWeapon[] SecondaryWeapon { get { return Gunner.Weapon[1]; } }
        public Transform GetTransform()
        {
            return this.gameObject.transform;
        }
        #endregion
        #region Modules
        public bool movementAiEnabled; // default true
        public bool combatAIEnabled;  // default true
        public bool selfDefenceModuleEnabled;  // default true
        protected float stealthness; //set in child
        protected bool detected;
        #endregion
        #region Controllers
        public bool ManualControl { set; get; }
        public IDriver Driver;
        protected IGunner gunner;
        public IGunner Gunner { get { return gunner; } }
        protected GlobalController Global;
        protected IArmor armor;
        protected IShield shield;
        private List<IImpact> impacts;
        public SpellModule[] module;
        protected float synchAction;
        public float movementAIDelay; //Make private after debug;
        protected List<Unit> enemys = new List<Unit>();
        protected List<Unit> allies = new List<Unit>();
        protected List<Unit> capByTarget;
        public string GunnerTarget; //debug only
        protected SortUnit EnemySortDelegate;
        protected SortUnit AlliesSortDelegate;
        protected SquadStatus unitSquadStatus;
        protected SpaceShip[] Squad;
        public string[] ImpactList;
        #endregion
        #region Base interface
        protected void Start()//_______________________Start
        {
            Global = GlobalController.Instance;
            movementAiEnabled = true;
            combatAIEnabled = true;
            selfDefenceModuleEnabled = true;
            EnemySortDelegate = SortEnemysBase;
            radarPover = 1;
            cooldownDetected = 0;
            aiStatus = UnitStateType.Waiting;
            unitName = type.ToString();
            hud = FindObjectOfType<HUDBase>();
            Global.unitList.Add(this);
            Anchor = this.transform.position;
            //
            armor = this.gameObject.GetComponent<IArmor>();
            shield = this.gameObject.GetComponent<IShield>();
            reams = GameObjectUtility.GetChildObjectByName(this.transform, "Jetream").ToArray();
            engineSound = this.gameObject.GetComponent<AudioSource>();
            //
            StatsUp();
            //
            Relationship = Global.GetRalationship(Team);
            gunner = new SpaceShipShootController(this);
            Driver = new SpaceMovementController(this.gameObject);
            capByTarget = new List<Unit>();
            Squad = new SpaceShip[3];
            //List<SpaceShip> enemys = new List<SpaceShip>();
            //List<SpaceShip> allies = new List<SpaceShip>();
            impacts = new List<IImpact>();

            manualController = FindObjectOfType<ShipManualController>();

            aliesMap = this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>();
            aliesMap.enabled = false;
            enemyMap = this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>();
            enemyMap.enabled = false;
            selectedMap = this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>();
            selectedMap.enabled = false;

            if (Team == Global.playerArmy)
                this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = true;

            Debug.Log("Unit " + this.gameObject.name + " started");
        }
        public void ResetStats()
        {
            Start();
        }
        protected virtual void StatsUp()
        {
            acceleration = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "acceleration"));
            speedThrust = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedThrust"));
            speedRotation = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedRotation"));
            speedShift = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedShift"));
            radarRange = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radarRange"));
            radarPover = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radarPover"));
            stealthness = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "stealthness"));
            radiolink = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radiolink"));

            float maxHitpoints = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "maxHitpoints"));
            float shellResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "shellResist"));
            float energyResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "energyResist"));
            float blastResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "blastResist"));
            armor.StatUp(maxHitpoints, maxHitpoints, shellResist, energyResist, blastResist);

            shield.MaxCampacity = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "maxCampacity"));
            shield.Force = shield.MaxCampacity;
            shield.Recharging = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "recharging"));
        }
        protected void Update()//______________________Update
        {
            if (CurrentTarget != null)
                GunnerTarget = CurrentTarget.ToString();
            else GunnerTarget = "NULL";
            Driver.Update();
            Gunner.Update();
            //cooldowns
            DecrementBaseCounters();
            DecrementLocalCounters();
            //action
            if (ManualControl)
            {
                Scan();
                if (CurrentTarget != null) CooperateFire();
            }
            else if (synchAction <= 0)
            {
                synchAction = 1f / AIUpdateRate;
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
                            //if (Driver.Status == DriverStatus.Movement && movementAIDelay <= 0)
                            //{
                            //    aiStatus = UnitStateType.Waiting;
                            //    movementAIDelay = 1f;
                            //}
                            //else
                            if (aiStatus == UnitStateType.Waiting)
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
                        if (selfDefenceModuleEnabled)
                            SelfDefenceFunction();
                    }
                    if (Driver.Status == DriverStatus.Waiting && movementAIDelay <= 0)
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
                                        movementAIDelay = 1f;
                                    }
                                    break;
                                }
                            case UnitStateType.Waiting:
                                {
                                    if (situation == TacticSituation.SectorСlear)
                                        IdleManeuverFunction();
                                    else if (movementAIDelay <= 0)
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
                        CombatFunction();
                    if (selfDefenceModuleEnabled)
                        SelfDefenceFunction();
                }
            }
        }
        protected void FixedUpdate()
        {
            if (!ManualControl)
                Driver.FixedUpdate();
        }
        protected void DecrementBaseCounters()
        {
            synchAction -= Time.deltaTime;
            if (Driver.Status == DriverStatus.Waiting && movementAIDelay > 0)
                movementAIDelay -= Time.deltaTime;
            //waitingBackCount = Driver.backCount;//синхронизация счетчиков
            if (this.Team != Global.playerArmy)
                cooldownDetected -= Time.deltaTime;
            else if (isSelected)
            {
                cooldownDetected = 0.1f;
                selectedMap.enabled = true;
            }
            else
            {
                selectedMap.enabled = false;
            }
            if (cooldownDetected < 0)
            {
                selectedMap.enabled = false;
                //this.gameObject.transform.FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
                enemyMap.enabled = false;
            }
            else
                enemyMap.enabled = true;
            if (module != null && module.Length > 0)
            {
                for (int i = 0; i < module.Length; i++)
                    module[i].Update();
            }
            if (impacts.Count > 0)
            {
                ImpactListUp();
                for (int i = 0; i < impacts.Count; i++)
                    impacts[i].ActImpact();
            }
        }
        protected void ImpactListUp()
        {
            {
                ImpactList = new string[impacts.Count];
                for (int i = 0; i < impacts.Count; i++)
                {
                    ImpactList[i] = impacts[i].Name;
                }
            }
        }
        protected abstract void DecrementLocalCounters();
        protected void OnGUI()
        {
            if (hud != null && !ManualControl)
            {
                //GUI.skin = hud.Skin;
                //if (Global.StaticProportion && hud.scale != 1)
                //    GUI.matrix = Matrix4x4.Scale(Vector3.one * hud.scale);

                float scaleLocal = (hud.scale / 1.5f) * Global.Settings.IconsScale;

                float distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);

                float border = 40;
                bool outOfBorder = false;
                Vector3 crd;
                if (Global.ManualController.enabled) crd = UIUtil.WorldToScreenCircle(this.transform.position, border, out outOfBorder);
                else crd = UIUtil.WorldToScreenFrame(this.transform.position, border, out outOfBorder);

                Vector2 frameSize;
                if (!outOfBorder) frameSize = new Vector2(Global.Prefab.AlliesGUIFrame.width, Global.Prefab.AlliesGUIFrame.height);// * hud.scale;
                else frameSize = new Vector2(Global.Prefab.AlliesOutscreenPoint.width, Global.Prefab.AlliesOutscreenPoint.height);
                Vector2 iconSize = new Vector2(aliesIcon.width, aliesIcon.height);// * hud.scale;
                float frameX;
                float frameY;
                float iconX;
                float iconY;

                if (true) //perspective
                    distance = Mathf.Clamp(distance, 400, 2000);
                float distFactor = 1000 / distance;
                if (!outOfBorder)
                    frameSize = frameSize * distFactor * scaleLocal;
                else
                    frameSize = Vector2.zero;
                frameY = crd.y - frameSize.y / 2f - (12 * distFactor * scaleLocal);
                iconSize = iconSize * scaleLocal;
                frameX = crd.x - frameSize.x / 2f;
                iconX = crd.x - iconSize.x / 2f;
                if (!outOfBorder)
                    iconY = frameY - iconSize.y * 1.2f;
                else
                    iconY = crd.y - iconSize.y / 2;//frameY - (iconSize.y - ((border + 20))) * 1.2f;
                GUIStyle style = new GUIStyle();
                style.fontSize = Mathf.RoundToInt(24 * scaleLocal);
                //style.font = GuiProcessor.getI.rusfont;
                style.normal.textColor = Color.red;
                style.alignment = TextAnchor.MiddleCenter;
                //style.fontStyle = FontStyle.Italic;
                Texture frameToDraw = null;
                Texture iconToDraw = null;
                bool drawStatBars = false;
                if (Team == Global.playerArmy)
                {
                    if (isSelected)
                    {
                        style.normal.textColor = Color.cyan;
                        if (Global.Settings.SelectedUI.ShowUnitFrame)
                        {
                            if (!outOfBorder)
                                frameToDraw = Global.Prefab.AlliesSelectedGUIFrame;
                        }
                        if (Global.Settings.SelectedUI.ShowUnitIcon)
                            iconToDraw = selectedIcon;
                        if (Global.Settings.SelectedUI.ShowUnitName)
                            GUI.Label(new Rect(crd.x - 120, crd.y - (frameSize.y / 2) * 1.1f, 240, 18), UnitName, style);
                        drawStatBars = Global.Settings.SelectedUI.ShowUnitStatus;
                    }
                    else
                    {
                        style.normal.textColor = Color.green;
                        if (Global.Settings.AliesUI.ShowUnitFrame)
                        {
                            if (!outOfBorder)
                                frameToDraw = Global.Prefab.AlliesGUIFrame;
                        }
                        if (Global.Settings.AliesUI.ShowUnitIcon)
                            iconToDraw = aliesIcon;
                        if (Global.Settings.AliesUI.ShowUnitName)
                            GUI.Label(new Rect(crd.x - 120, crd.y - (frameSize.y / 2) * 1.1f, 240, 18), UnitName, style);
                        drawStatBars = Global.Settings.AliesUI.ShowUnitStatus;
                    }
                }
                else if (cooldownDetected > 0)
                {
                    style.normal.textColor = Color.red;
                    if (Global.Settings.EnemyUI.ShowUnitFrame)
                    {
                        if (!outOfBorder)
                            frameToDraw = Global.Prefab.EnemyGUIFrame;
                    }
                    if (Global.Settings.EnemyUI.ShowUnitFrame)
                        iconToDraw = enemyIcon;
                    if (Global.Settings.EnemyUI.ShowUnitName)
                        GUI.Label(new Rect(crd.x - 100, crd.y - (frameSize.y / 2) * 1.1f, 200, 18), UnitName, style);
                    drawStatBars = Global.Settings.EnemyUI.ShowUnitStatus;
                }

                if (frameToDraw != null)
                    GUI.DrawTexture(new Rect(new Vector2(frameX, frameY), frameSize), frameToDraw);
                if (iconToDraw != null)
                    GUI.DrawTexture(new Rect(new Vector2(iconX, iconY), iconSize), iconToDraw);
                if (drawStatBars && !outOfBorder)
                {
                    Vector2 backSize = new Vector2(200, 32) * distFactor * scaleLocal;
                    Vector2 lineSize = new Vector2(200 - 24, 4) * distFactor * scaleLocal;
                    Vector2 backPos = new Vector2(crd.x - backSize.x / 2, crd.y - (frameSize.y / 2) * 1f);
                    Vector2 linePos1 = new Vector2(backPos.x + (12 * scaleLocal * distFactor), backPos.y + (12 * scaleLocal * distFactor));
                    Vector2 linePos2 = new Vector2(backPos.x + (12 * scaleLocal * distFactor), backPos.y + (16 * scaleLocal * distFactor));
                    GUI.DrawTexture(new Rect(backPos, backSize), hud.UnitStatBack);
                    GUI.DrawTexture(UIUtil.TransformBar(new Rect(linePos1, lineSize), (this.ShieldForce / this.ShieldCampacity)), hud.UnitShieldLine);
                    GUI.DrawTexture(UIUtil.TransformBar(new Rect(linePos2, lineSize), (this.Hull / this.MaxHull)), hud.UnitArmorLine);
                }
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
        public override void MakeDamage(float damage)
        {
            this.Hull = this.Hull - damage;
        }
        public override void MakeImpact(IImpact impact)
        {
            impacts.Add(impact);
        }
        public override bool HaveImpact(string impactName)
        {
            if (this.impacts.Exists(x => x.Name == impactName)) return true;
            else return false;
        }
        public override void RemoveImpact(IImpact impact)
        {
            impacts.Remove(impact);
        }
        public override void Die()//____________________________Die
        {
            shield.Owerheat();
            Explosion();
            Destroy(this.gameObject);
        }
        private void OnDestroy()
        {
            Global.selectedList.Remove(this);
            Global.unitList.Remove(this);            
        }
        protected virtual void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        #endregion

        #region AI logick
        protected bool CombatManeuverFunction()
        {
            switch (situation)
            {
                case TacticSituation.Attack:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Enemy, SpellFunction.Attack, SpellFunction.Buff });
                        return AttackManeuver();
                    }
                case TacticSituation.Defense:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Defence, SpellFunction.Self, SpellFunction.Buff });
                        return DefenseManeuver();
                    }
                case TacticSituation.Retreat:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Emergency, SpellFunction.Self, SpellFunction.Buff });
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
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.IncreaseDistance, Gunner.Target.transform);
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
                        return Driver.ExecetePointManeuver(PointManeuverType.PatroolPyramid, this.transform.position, this.transform.forward * 250);
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, CurrentTarget.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        return Driver.ExecetePointManeuver(PointManeuverType.PatroolLine, this.transform.position, this.transform.right * 250);
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return Driver.ExecetePointManeuver(PointManeuverType.PatroolPyramid, this.transform.position, this.transform.forward * 250);
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
            Unit nearest = FindAllies(UnitClass.Support_Corvette);
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
            if (Vector3.Distance(this.transform.position, Anchor) > 5000)
                return BackToAncour();
            return Driver.ExecetePointManeuver(PointManeuverType.PatroolDiamond, this.transform.position, this.transform.forward * 50);
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
                        output = OpenFire(RetaliatoryCapture(), 8);//ответный захват
                    else
                        output = OpenFire(GetNearest(), 4);//огонь по ближайшей
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
                            return OpenFire(GetNearest(), 2);
                    }
                    //переходим в ожидение
                    return false;
                }
            }
            else
            {
                if (enemys.Count > 0 && EnemySortDelegate(CurrentTarget, enemys[0]) == 1)
                    Gunner.SetAim(enemys[0], false, 4);
                float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
                if (distance < RadarRange)
                {
                    bool output = OpenFire(CurrentTarget, 1);
                    CooperateFire();
                    return output;
                }
                else
                {
                    if (!TargetScouting())
                    {
                        Gunner.ResetAim();
                        return false;//переходим в ожидение
                    }
                    else return OpenFire(CurrentTarget, 1);
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
                        else if (enemys.Count > allies.Count + 1)
                            situation = TacticSituation.Defense;
                        if (enemys.Count == 0 && CurrentTarget == null)
                            situation = TacticSituation.SectorСlear;
                        break;
                    }
                case TacticSituation.Defense:
                    {
                        if (allies.Count == 0)
                            situation = TacticSituation.Retreat;
                        else if (enemys.Count <= allies.Count + 1)
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
            if (aiStatus == UnitStateType.UnderControl)
                situation = TacticSituation.Defense;
        }

        protected void UseModule(SpellFunction[] functions)
        {
            if (module != null && module.Length > 0)
                foreach (SpellModule m in module)
                    if (m.FunctionsIs(functions))
                        m.EnableIfReady();
        }
        #endregion
        #region Sevice functions
        #region Base maneuvers

        protected bool ToPrimaryDistance()
        {
            Vector3 target = CurrentTarget.transform.position + (this.transform.position - CurrentTarget.transform.position).normalized * Gunner.GetRange(0) * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool ToSecondaryDistance()
        {
            Vector3 target = CurrentTarget.transform.position + (this.transform.position - CurrentTarget.transform.position).normalized * Gunner.GetRange(1) * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position;
            return Driver.MoveToQueue(target);
        }

        protected bool Rush()
        {
            //waitingBackCount = 5f;
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRange(0) * 0.4f;
            return Driver.MoveToQueue(target);
        }
        protected bool BackToAncour()
        {
            return Driver.MoveToQueue(Anchor);
        }
        protected bool GoToBase()
        {
            return BackToAncour();
        }
        #endregion
        #region Sensors
        protected void Scan() //___________Scan
        {
            enemys.RemoveAll(x => x == null);
            allies.RemoveAll(x => x == null);
            foreach (Unit unknown in Global.unitList)
                if (unknown != this)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, unknown.transform.position);
                    if (distance < RadarRange * radiolink)
                    {
                        float multiplicator = Mathf.Pow(((-distance + RadarRange) * 0.02f), (1f / 5f)) * ((2f / (distance + 0.1f)) + 1);
                        if (radarPover * multiplicator > unknown.Stealthness)
                        {
                            unknown.cooldownDetected = 1;
                            switch (CheckRelationship(unknown.Team))
                            {
                                case RelationshipType.Enemys:
                                    {
                                        if (!enemys.Contains(unknown))
                                            enemys.Add(unknown);
                                        break;
                                    }
                                case RelationshipType.Allies:
                                    {
                                        if (!allies.Contains(unknown))// if ((distance < this.RadarRange * this.radiolink) && (distance < unknown.RadarRange * unknown.radiolink))
                                            allies.Add(unknown);
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        if (enemys.Contains(unknown))
                            enemys.Remove(unknown);
                        if (allies.Contains(unknown))
                            allies.Remove(unknown);
                    }
                }
            enemys.Sort(delegate (Unit x, Unit y) { return EnemySortDelegate(x, y); });
            allies.Sort(delegate (Unit x, Unit y) { return AlliesSortDelegate(x, y); });
        }
        protected virtual GameObject AntiTorpedoRadar()
        {
            List<GameObject> hazard = new List<GameObject>();
            hazard.AddRange(GameObject.FindGameObjectsWithTag("Torpedo"));
            foreach (GameObject x in hazard)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.5 && !x.GetComponent<Missile>().Allies(Team))
                    return x;
            }
            return null;
        }
        protected virtual GameObject AntiMissileRadar()
        {
            List<GameObject> hazard = new List<GameObject>();
            hazard.AddRange(GameObject.FindGameObjectsWithTag("Missile"));
            foreach (GameObject x in hazard)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.3 && !x.GetComponent<Missile>().Allies(Team))
                        return x;
            }
            return null;
        }
        protected virtual int RadarWarningResiever()
        {
            capByTarget.Clear();
            foreach (Unit x in enemys)
            {
                if (x.CurrentTarget != null && x.CurrentTarget.transform == this.transform)
                    capByTarget.Add(x);
            }
            capByTarget.Sort(delegate (Unit x, Unit y) { return EnemySortDelegate(x, y); });
            return capByTarget.Count;
        }
        public override void ArmorCriticalAlarm()
        {
            UseModule(new SpellFunction[] { SpellFunction.Emergency, SpellFunction.Hull });
            situation = TacticSituation.ExitTheBattle;
        }
        public override void ShieldCriticalAlarm()
        {
            UseModule(new SpellFunction[] { SpellFunction.Emergency, SpellFunction.Shield });
            situation = TacticSituation.Retreat;
        }
        #endregion
        #region Combat
        protected Unit GetNearest()
        {
            return enemys[0];
        }
        protected virtual Unit RetaliatoryCapture()
        {
            return capByTarget[0];
        }
        protected bool OpenFire(Unit target, float lockdown)
        {
            Gunner.SetAim(target, false, lockdown);
            if (Gunner.SeeTarget())
            {
                targetStatus = TargetStateType.Captured;//наведение
                if (Gunner.TargetInRange(0))//выбор оружия
                {
                    targetStatus = TargetStateType.InPrimaryRange;
                    if (Gunner.ShootHim(0))
                        return true;
                }
                if (Gunner.TargetInRange(1))
                {
                    targetStatus = TargetStateType.InSecondaryRange;
                    return Gunner.ShootHim(1);
                }
            }
            else targetStatus = TargetStateType.BehindABarrier;
            return false;
        } //_________OpenFire
        public override void ResetTarget()
        {
            Gunner.ResetAim();
        }
        protected virtual void SelfDefenceFunction()
        {
            MissileProtection();
            TorpedoProtection();
            UseModule(new SpellFunction[] { SpellFunction.Defence, SpellFunction.Self });
        }
        protected bool MissileProtection()
        {
            GameObject hazard = AntiMissileRadar();
            if (hazard != null)
            {
                if (module != null && module.Length > 0)
                {
                    foreach (SpellModule m in module)
                    {
                        if (m.GetType() == typeof(Mechanics.Modules.MissileTrapLauncher))
                        {
                            m.EnableIfReady();
                            break;
                        }
                    }
                }
                HazardEvasion(hazard);
                return true;
            }
            else return false;
        }
        protected bool TorpedoProtection()
        {
            GameObject hazard = AntiTorpedoRadar();
            if (hazard != null)
            {
                if (module != null && module.Length > 0)
                {
                    foreach (SpellModule m in module)
                    {
                        if (m.GetType() == typeof(Mechanics.Modules.MissileEliminator))
                        {
                            m.EnableIfReady();
                            break;
                        }
                    }
                }
                HazardEvasion(hazard);
                return true;
            }
            else return false;
        }
        protected void HazardEvasion(GameObject hazard)
        {
            if (situation == TacticSituation.SectorСlear)
            {
                Driver.ClearQueue();
                aiStatus = UnitStateType.MoveAI;
                situation = TacticSituation.Defense;
                Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, hazard.transform);
            }
            else if (Driver.PathPoints == 0)
            {
                aiStatus = UnitStateType.MoveAI;
                Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, hazard.transform);
            }
        }
        #endregion
        #region Group interaction
        protected List<Unit> RequestScout()
        {
            List<Unit> enemys = new List<Unit>();
            foreach (Unit x in allies)
            {
                if (x.GetType() == typeof(SpaceShip) && ((SpaceShip)x).Ping(this.transform.position))
                    enemys.AddRange(((SpaceShip)x).GetEnemys());
            }
            return enemys;
        }
        public Unit[] GetEnemys()
        {
            return enemys.ToArray();
        }
        public Unit[] GetAllies()
        {
            return allies.ToArray();
        }
        public bool Ping(Vector3 sender)
        {
            if (Vector3.Distance(this.gameObject.transform.position, sender) < RadarRange * radiolink)
                return true;
            else return false;
        }
        public bool TargetScouting()
        {
            List<Unit> scoutingenemys = RequestScout();
            return scoutingenemys.Contains(CurrentTarget);
        }
        protected void CooperateFire()
        {
            foreach (Unit x in allies)
            {
                if (x.Type == type && x.GetType() == typeof(SpaceShip))
                {
                    ((SpaceShip)x).GetFireSupport(CurrentTarget);
                }
            }
        }
        public override void GetFireSupport(Unit Target)
        {
            if (Target.transform != this.transform)
                Gunner.SetAim(Target, true, 20);
        }
        #endregion
        #region Squad
        protected void FormSquad()
        {
            int inSquadCount = 0;
            Squad[0] = this;
            foreach (Unit x in allies)
            {
                if (x.Type == this.Type&& x.GetType() == typeof(SpaceShip))
                {
                    if (((SpaceShip)x).unitSquadStatus == SquadStatus.Free)
                    {
                        this.unitSquadStatus = SquadStatus.SquadMaster;
                        ((SpaceShip)x).unitSquadStatus = SquadStatus.InSquad;
                        inSquadCount++;
                        this.Squad[inSquadCount] = ((SpaceShip)x);
                        ((SpaceShip)x).Squad[0] = this.Squad[0];
                        ((SpaceShip)x).Squad[1] = this.Squad[1];
                        ((SpaceShip)x).Squad[2] = this.Squad[2];
                    }
                    else if (((SpaceShip)x).unitSquadStatus == SquadStatus.SquadMaster)
                        if (((SpaceShip)x).ComeInSquadRequest(this))
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
        protected Unit FindAllies(UnitClass ofType)
        {
            foreach (Unit x in allies)
            {
                if (x.Type == ofType)
                    return x;
            }
            return null;
        }
        #endregion
        #region Sort functions
        public int EMCSortEnemys(Unit x, Unit y)
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int ScoutSortEnemys(Unit x, Unit y)
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int ReconSortEnemys(Unit x, Unit y)
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int FigtherSortEnemys(Unit x, Unit y)
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
                case UnitClass.Bomber: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Figther: //паритет
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
                case UnitClass.Bomber: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Figther: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Guard_Corvette: //хищник
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int BomberSortEnemys(Unit x, Unit y)
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
                case UnitClass.Turret: //жертва
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
                case UnitClass.Turret: //жертва
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int LRCorvetteSortEnemys(Unit x, Unit y)
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int GuardCorvetteSortEnemys(Unit x, Unit y)
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
                case UnitClass.Figther: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Guard_Corvette: //паритет
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
                case UnitClass.Figther: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Guard_Corvette: //паритет
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int SupportCorvetteSortEnemys(Unit x, Unit y)
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
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        public  int CommandSortEnemys(Unit x, Unit y)
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
                case UnitClass.Support_Corvette:
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Figther:
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.LR_Corvette:
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Guard_Corvette:
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.Bomber:
                    {
                        xPriority = 5;
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
                case UnitClass.Support_Corvette:
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Figther:
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.LR_Corvette:
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Guard_Corvette:
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.Bomber:
                    {
                        yPriority = 5;
                        break;
                    }
                default:
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.transform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.transform.position);
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
        #endregion
        #endregion
        #region Remote control
        public virtual void SendTo(Vector3 destination)
        {
            //Debug.Log("Send To " + destination);
            aiStatus = UnitStateType.UnderControl;
            if (Driver.MoveTo(destination) && Team == Global.playerArmy)
                Driver.BuildPathArrows();
        }
        public virtual void SendToQueue(Vector3 destination)
        {
            //Debug.Log("Add To Quenue " + destination);
            aiStatus = UnitStateType.UnderControl;
            if (Driver.MoveToQueue(destination) && Team == Global.playerArmy)
                Driver.BuildPathArrows();
        }
        protected virtual void SendToQueue(Vector3[] path)
        {
            aiStatus = UnitStateType.UnderControl;
                Driver.MoveToQueue(path);
            if (Team == Global.playerArmy)
                Driver.BuildPathArrows();
        }
        public virtual void AttackThat(Unit target)
        {
            if (target.transform != this.transform)
            {
                Gunner.ResetAim();
                Gunner.SetAim(target, true, 64);
            }
        }
        #endregion
    }
}
