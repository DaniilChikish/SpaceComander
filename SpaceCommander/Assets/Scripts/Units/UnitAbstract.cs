using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using SpaceCommander;
using UnityEngine.AI;
using DeusUtility.Random;
using DeusUtility.UI;

namespace SpaceCommander
{
    public abstract class Unit : MonoBehaviour
    {
        protected UnitClass type;
        protected string unitName;

        public UnitClass Type { get { return type; } }
        public string UnitName { get { return unitName; } }
        public abstract Army Team { get; }
        public abstract Unit CurrentTarget { get; }
        public abstract float Speed {get; }
        public float SpeedMultiplicator { set; get; }
        public abstract float RotationSpeed {get; }
        public float RotationSpeedMultiplicator { set; get; }
        public abstract float ShiftSpeed {get; }
        public float ShiftSpeedMultiplicator { set; get; }
        public abstract float Health { set; get; }
        public abstract float MaxHealth { get; }
        public float MaxHealthMultiplacator { set; get; }


        public abstract float ShellResist { get; }
        public float ResistMultiplacator { set; get; }
        public abstract float ShieldForce { set; get; }
        public abstract float ShieldRecharging { get; }
        public float ShieldRechargingMultiplacator { set; get; }
        public abstract float ShieldCampacity {get; }
        public float ShieldCampacityMultiplacator { set; get; }

        public abstract float RadarRange { get; }
        public float RadarRangeMultiplacator { set; get; }
        public abstract Vector3 Velocity { get; }
        public abstract void MakeImpact(IImpact impact);
        public abstract bool HaveImpact(string impactName);
        public abstract void RemoveImpact(IImpact impact);
        public abstract void MakeDamage(float damage);
        public abstract void Die();
        public abstract void ResetTarget();
    }
    public delegate int SortUnit(Unit x, Unit y);
    public abstract class SpaceShip : Unit, ISpaceShipObservable
    {
        //base varibles
        protected UnitStateType aiStatus;
        protected TargetStateType targetStatus;
        protected TacticSituation situation;
        public Army team;
        public bool isSelected;
        public Vector3 Anchor;
        //GUI
        public HUDBase hud;
        private ShipManualController manualController;
        public Texture enemyIcon;
        public Texture aliesIcon;
        public Texture selectedIcon;
        //depend varibles
        protected float radiolink;

        //constants
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speedThrust; //set in child
        protected float speedRotation;
        protected float speedShift;
        //override properties
        public override float Health { set { armor.hitpoints = value; } get { return armor.hitpoints; } }
        public override float MaxHealth { get { return armor.maxHitpoints * (1 + MaxHealthMultiplacator); } }
        public override Army Team { get { return team; } }
        public override Vector3 Velocity { get {
                if (ManualControl) return this.gameObject.GetComponent<Rigidbody>().velocity;
                else return Driver.Velocity;
            } }
        public override float Speed { get { return speedThrust * (1 + SpeedMultiplicator); } }
        public override float RotationSpeed { get { return speedRotation * (1 + RotationSpeedMultiplicator); } }
        public override float ShiftSpeed { get { return speedShift * (1 + ShiftSpeedMultiplicator); } }
        public override float RadarRange { get { return radarRange * (1 + RadarRangeMultiplacator); } }
        public override float ShieldForce { set { shield.force = value; } get { return shield.force; } }
        public override float ShieldRecharging { get { return shield.recharging * (1 + ShieldRechargingMultiplacator); } }
        public override float ShieldCampacity { get { return shield.maxCampacity * (1+ShieldCampacityMultiplacator); } }
        public override Unit CurrentTarget { get { return Gunner.Target; } }
        public override float ShellResist { get { return armor.shellResist * (1 + ResistMultiplacator); } }

        //own properties
        public bool ShieldOwerheat { get { return shield.isOwerheat; } }
        public float Stealthness { get { return stealthness; } set { stealthness = value; } }
        public ForceShield GetShieldRef { get { return shield; } }
        public SpellModule[] Module { get { return module; } }

        //interface
        public IWeapon[] PrimaryWeapon { get { return Gunner.Weapon[0]; } }
        public IWeapon[] SecondaryWeapon { get { return Gunner.Weapon[1]; } }
        public Transform GetTransform()
        {
            return this.gameObject.transform;
        }
        //modules
        public bool movementAiEnabled; // default true
        public bool combatAIEnabled;  // default true
        public bool selfDefenceModuleEnabled;  // default true
        protected float stealthness; //set in child
        protected bool detected;
        public float cooldownDetected;
        //controllers
        public bool ManualControl { set; get; }
        public IDriver Driver;
        protected IGunner gunner;
        public IGunner Gunner { get { return gunner; } }
        protected GlobalController Global;
        protected Armor armor;
        protected ForceShield shield;
        private List<IImpact> impacts;
        public SpellModule[] module;
        protected float synchAction;
        public float movementAIDelay; //Make private after debug;
        protected List<Unit> enemys = new List<Unit>();
        protected List<Unit> allies = new List<Unit>();
        protected List<Unit> capByTarget;
        public string ManeuverName; //debug only
        public string GunnerTarget; //debug only
        protected SortUnit EnemySortDelegate;
        protected SortUnit AlliesSortDelegate;
        protected SquadStatus unitSquadStatus;
        protected SpaceShip[] Squad;
        public string[] ImpactList;
        //base interface
        protected void Start()//_______________________Start
        {
            Global = FindObjectOfType<GlobalController>();
            movementAiEnabled = true;
            combatAIEnabled = true;
            selfDefenceModuleEnabled = true;
            EnemySortDelegate = SortEnemysBase;
            radarPover = 1;
            StatsUp();//

            //Global.SpecINI.Write(this.GetType().ToString(), "speedThrust", speedThrust.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "speedRotation", speedRotation.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "speedShift", speedShift.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "radarRange", speedShift.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "radarPover", speedShift.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "stealthness", stealthness.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "radiolink", radiolink.ToString());

            //Health = MaxHealth;
            cooldownDetected = 0;
            //waitingBackCount = 0.2f;
            aiStatus = UnitStateType.Waiting;
            unitName = type.ToString();
            hud = FindObjectOfType<HUDBase>();
            Global.unitList.Add(this);
            Anchor = this.transform.position;
            //
            armor = this.gameObject.GetComponent<Armor>();
            shield = this.gameObject.GetComponent<ForceShield>();
            //
            gunner = new ShootController(this);
            //Driver = new NavmeshMovementController(this.gameObject);
            Driver = new SpaceMovementController(this.gameObject);
            capByTarget = new List<Unit>();
            Squad = new SpaceShip[3];
            //List<SpaceShip> enemys = new List<SpaceShip>();
            //List<SpaceShip> allies = new List<SpaceShip>();
            impacts = new List<IImpact>();

            manualController = FindObjectOfType<ShipManualController>();

            this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("SelectedMinimapPict").GetComponent<Renderer>().enabled = false;

            if (team == Global.playerArmy)
                this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = true;

            Debug.Log("Unit " + this.gameObject.name + " started");
        }
        public void ResetStats()
        {
            Start();
        }
        protected virtual void StatsUp()
        {
            speedThrust = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "speedThrust"));
            speedRotation = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "speedRotation"));
            speedShift = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "speedShift"));
            radarRange = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "radarRange"));
            radarPover = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "radarPover"));
            stealthness = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "stealthness"));
            radiolink = Convert.ToSingle(Global.SpecINI.ReadINI(this.GetType().ToString(), "radiolink"));
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
                    if (Driver.Status == DriverStatus.Waiting && movementAIDelay<=0)
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
                                    else if (movementAIDelay < 0)
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
                        if (selfDefenceModuleEnabled)
                            SelfDefenceFunction();
                    }
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
            if (hud!=null&&!ManualControl)
            {
                //GUI.skin = hud.Skin;
                //if (Global.StaticProportion && hud.scale != 1)
                //    GUI.matrix = Matrix4x4.Scale(Vector3.one * hud.scale);

                float scaleLocal = hud.scale / 1.5f;
                float border = 40;
                bool outOfBorder = false;
                bool outOfPlane = false;
                Vector3 crd = Camera.main.WorldToScreenPoint(transform.position);
                float distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
                crd.y = Screen.height - crd.y;

                if (crd.x > Screen.width -40 || crd.x < 40 || crd.y > Screen.height -5 || crd.y < 40)
                    outOfBorder = true;
                if (crd.z < 0)
                    outOfPlane = true;
                //crd.z = 0;

                if (outOfBorder || outOfPlane)
                {
                    Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
                    Vector3 originC = crd - center;
                    Vector3 vC = originC;
                    float a = center.x - border;
                    float b = center.y - border;
                    float r = 0;

                    //float comX = a, comY = b;

                    if (vC.x < 0) vC.x = -vC.x;
                    if (vC.y < 0) vC.y = -vC.y;
                    if (vC.x / vC.y > a / b || vC.x / vC.y < -(a / b))
                    {
                        r = vC.magnitude * a / vC.x;
                    }
                    else
                    {
                        r = vC.magnitude * b / vC.y;
                    }
                    crd = center + originC.normalized * r;

                    if (manualController.enabled && true) //in hud elipse
                    {
                        crd = Camera.main.WorldToScreenPoint(transform.position);
                        if (crd.x < 0)
                        {
                            crd.x = border;
                        }
                        else
                        if (crd.x > Screen.width)
                        {
                            crd.x = Screen.width - border;
                        }
                            crd.y = center.y + (b *(1- crd.z / distance));

                    }
                }
                if (outOfPlane)
                {
                    crd.x = Screen.width - crd.x;
                    crd.y = Screen.height - border;
                    outOfBorder = true;
                }

                Vector2 frameSize;
                if (!outOfBorder) frameSize = new Vector2(Global.AlliesGUIFrame.width, Global.AlliesGUIFrame.height);// * hud.scale;
                else frameSize = new Vector2(Global.AlliesOutscreenPoint.width, Global.AlliesOutscreenPoint.height);
                Vector2 iconSize = new Vector2(aliesIcon.width, aliesIcon.height);// * hud.scale;
                float frameX;
                float frameY = crd.y - frameSize.y / 2f - 12;
                float iconX;
                float iconY;

                if (!true || distance < 200) //perspective
                    distance = 200;

                frameSize = frameSize * (200 / distance) * scaleLocal;
                frameY = crd.y - frameSize.y / 2f - (12 * (200 / distance) * scaleLocal);
                iconSize = iconSize * (200 / distance) * scaleLocal;
                frameX = crd.x - frameSize.x / 2f;
                iconX = crd.x - iconSize.x / 2f;
                if (!outOfBorder)
                    iconY = frameY - iconSize.y * 1.2f;
                else
                    iconY = frameY - (iconSize.y - ((border + 20) * (200 / distance) * scaleLocal)) * 1.2f;
                GUIStyle style = new GUIStyle();
                style.fontSize = Mathf.RoundToInt(12 * scaleLocal);
                //style.font = GuiProcessor.getI.rusfont;
                style.normal.textColor = Color.red;
                style.alignment = TextAnchor.MiddleCenter;
                //style.fontStyle = FontStyle.Italic;
                Texture frameToDraw = null;
                Texture iconToDraw = null;
                if (team == Global.playerArmy)
                {
                    if (isSelected)
                    {
                        style.normal.textColor = Color.cyan;

                        if (!outOfBorder&&!outOfPlane)
                            frameToDraw = Global.AlliesSelectedGUIFrame;
                        else
                            frameToDraw = Global.AlliesSelectedOutscreenPoint;
                        iconToDraw = selectedIcon;
                        GUI.Label(new Rect(crd.x - 120, crd.y - (frameSize.y / 2) * 1.1f, 240, 18), UnitName, style);
                    }
                    else
                    {
                        if (!outOfBorder && !outOfPlane)
                            frameToDraw = Global.AlliesGUIFrame;
                        else
                            frameToDraw = Global.AlliesOutscreenPoint;
                        iconToDraw = aliesIcon;
                    }
                }
                else if (cooldownDetected > 0)
                {
                    style.normal.textColor = Color.red;

                    if (!outOfBorder && !outOfPlane)
                        frameToDraw = Global.EnemyGUIFrame;
                    else
                        frameToDraw = Global.EnemyOutscreenPoint;
                    iconToDraw = enemyIcon;
                    //GUI.Label(new Rect(crd.x - 120, crd.y - Global.NameFrameOffset, 240, 18), UnitName, style);
                }

                if (frameToDraw != null)
                    GUI.DrawTexture(new Rect(new Vector2(frameX, frameY), frameSize), frameToDraw);
                if (iconToDraw != null)
                    GUI.DrawTexture(new Rect(new Vector2(iconX, iconY), iconSize), iconToDraw);

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
            this.Health = this.Health - damage;
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
            Global.selectedList.Remove(this);
            Global.unitList.Remove(this);
            Destroy(this.gameObject);
        }
        protected virtual void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }

        //AI logick
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
                /*target2 = new Vector3(60, 0, 0);*/
                target2 = this.transform.right * 60f;
            else
                /*target2 = new Vector3(-60, 0, 0);*/
                target2 = this.transform.right * -60f;
            target2 += this.transform.position + new Vector3(0, 0.5f, 0);
            //      Debug.Log(target2);
            Driver.MoveToQueue(target2);

            Vector3 target4;
            if (random < 0)
                /*target4 = new Vector3(60, 0, 0);*/
                target4 = this.transform.right * 60f;
            else
                /*target4 = new Vector3(-60, 0, 0);*/
                target4 = this.transform.right * -60f;
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
        private bool CanPatrool(Vector3[] path)
        {
            for (int i = 1; i < path.Length; i++)
            {
                if (!CanWalk(path[i - 1], path[i]))
                    return false;
            }
            return true;
        }
        private bool CanWalk(Vector3 position, Vector3 destination)
        {
            RaycastHit[] hits = Physics.RaycastAll(position, (destination - position), (destination - position).magnitude); //9 is Terrain layer
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.tag == "Terrain")
                    return false;
            }
            return true;
        }
        //sensors
        protected void Scan() //___________Scan
        {
            enemys.Clear();
            allies.Clear();
            foreach (SpaceShip unknown in Global.unitList)
                if (unknown != this)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, unknown.transform.position);
                    if (distance < RadarRange * radiolink)
                    {
                        float multiplicator = Mathf.Pow(((-distance + RadarRange) * 0.02f), (1f / 5f)) * ((2f / (distance + 0.1f)) + 1);
                        if (radarPover * multiplicator > unknown.Stealthness)
                            if (!unknown.Allies(team))
                            {
                                enemys.Add(unknown);
                            }
                            else
                            {
                                if (!allies.Contains(unknown))// if ((distance < this.RadarRange * this.radiolink) && (distance < unknown.RadarRange * unknown.radiolink))
                                {
                                    allies.Add(unknown);
                                    //foreach (SpaceShip all2 in unknown.GetAllies())
                                    //    if (!allies.Contains(all2) && all2 != this)
                                    //        allies.Add(all2);
                                }
                            }
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
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.5 && !x.GetComponent<Torpedo>().Allies(team))
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
                if (Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.3)
                {
                    float angel = Vector3.Angle(this.transform.position - x.transform.position, x.transform.forward);
                    if (angel < 10)
                    {
                        return x;
                    }
                }
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
        private int SortEnemysBase(Unit x, Unit y)
        {
            if (Vector3.Distance(this.transform.position, x.transform.position) > Vector3.Distance(this.transform.position, y.transform.position))
                return 1;
            else return -1;
        }
        public void ArmorCriticalAlarm()
        {
            UseModule(new SpellFunction[] { SpellFunction.Emergency, SpellFunction.Health });
            situation = TacticSituation.ExitTheBattle;
        }
        public void ShieldCriticalAlarm()
        {
            UseModule(new SpellFunction[] { SpellFunction.Emergency, SpellFunction.Shield });
            situation = TacticSituation.Retreat;
        }

        //combat
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
            bool shot = false;
            float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
            RaycastHit hit;
            Physics.Raycast(this.transform.position, CurrentTarget.transform.position - this.transform.position, out hit);
            if (hit.transform == CurrentTarget.transform)
            {
                targetStatus = TargetStateType.Captured;//наведение
                if (distance < Gunner.GetRange(1) && distance > 50)
                {
                    targetStatus = TargetStateType.InSecondaryRange;
                    shot = Gunner.ShootHim(1);
                }
                if (distance < Gunner.GetRange(0))//выбор оружия
                {
                    targetStatus = TargetStateType.InPrimaryRange;
                    shot = Gunner.ShootHim(0);
                }
            }
            else targetStatus = TargetStateType.BehindABarrier;
            return shot;
        } //_________OpenFire
        public override void ResetTarget()
        {
            Gunner.ResetAim();
        }
        protected virtual void SelfDefenceFunction()
        {
            MissileProtection();
            TorpedoProtection();
            UseModule(new SpellFunction[] {SpellFunction.Defence, SpellFunction.Self });
        }
        protected bool MissileProtection()
        {
            GameObject hazard = AntiMissileRadar();
            if (hazard != null)
            {
                if (module!=null&&module.Length>0)
                {
                    foreach (SpellModule m in module)
                    {
                        if (m.GetType() == typeof(MissileTrapLauncher))
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
                        if (m.GetType() == typeof(TorpedoEliminator))
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
                    Evasion(hazard.transform.right);
                }
                else if (Driver.PathPoints == 0)
                {
                    aiStatus = UnitStateType.MoveAI;
                    Evasion(hazard.transform.right);
                }
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
        protected List<Unit> RequestScout()
        {
            List<Unit> enemys = new List<Unit>();
            foreach (SpaceShip x in allies)
            {
                if (x.Ping(this.transform.position))
                    enemys.AddRange(x.GetEnemys());
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
            foreach (SpaceShip x in allies)
            {
                if (x.Type == type)
                {
                    x.GetFireSupport(CurrentTarget);
                }
            }
        }
        public void GetFireSupport(Unit Target)
        {
            if (Target.transform != this.transform)
                Gunner.SetAim(Target, true, 20);
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
        // sort functions
        protected int EMCSortEnemys(Unit x, Unit y)
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
        protected int ScoutSortEnemys(Unit x, Unit y)
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
        protected int ReconSortEnemys(Unit x, Unit y)
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
        protected int FigtherSortEnemys(Unit x, Unit y)
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
        protected int BomberSortEnemys(Unit x, Unit y)
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
        protected int LRCorvetteSortEnemys(Unit x, Unit y)
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
        protected int GuardCorvetteSortEnemys(Unit x, Unit y)
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
        protected int SupportCorvetteSortEnemys(Unit x, Unit y)
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
        protected int CommandSortEnemys(Unit x, Unit y)
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
        //remote control
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
            if(Driver.MoveToQueue(destination) && Team == Global.playerArmy)
                Driver.BuildPathArrows();
        }
        protected virtual void SendToQueue(Vector3[] path)
        {
            aiStatus = UnitStateType.UnderControl;
            for (int i = 0; i < path.Length; i++)
            {
                Driver.MoveToQueue(path[i]);
            }
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
    }
}
