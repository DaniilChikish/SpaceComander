using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using SpaceCommander;
using UnityEngine.AI;

namespace SpaceCommander
{
    public delegate int SortSpaceShip(IUnit x, IUnit y);
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
        private List<IImpact> impacts;
        public Vector3 Anchor;

        //depend varibles
        protected float radiolink;

        //constants
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speed; //set in child
        public float Health { set { armor.hitpoints = value; } get { return armor.hitpoints; } }
        public float ShellResist { /*set { armor.shellResist = value; }*/ get { return armor.shellResist; } }
        public float MaxHealth { get { return armor.maxHitpoints; } }
        public bool ShieldOwerheat { get { return shield.isOwerheat; } }
        public float ShieldForce { set { shield.force = value; } get { return shield.force; } }
        public float ShieldMaxCampacity { set { shield.maxCampacity = value; } get { return shield.maxCampacity; } }
        public float ShieldRecharging { set { shield.recharging = value; } get { return shield.recharging; } }
        public float RadarRange { set { radarRange = value; } get { return radarRange; } }
        public virtual float Stealthness { get { return stealthness; } }
        public float Speed { set { speed = value; } get { return speed; } }
        public Vector3 Velocity { get { return Driver.Velocity; } }
        public Army Team { get { return team; } }
        public Transform ObjectTransform { get { if (this.gameObject != null) return this.gameObject.transform; else return null; } }
        //modules
        public bool movementAiEnabled; // default true
        public bool combatAIEnabled;  // default true
        public bool selfDefenceModuleEnabled;  // default true
        public bool roleModuleEnabled; // default true
        protected float stealthness; //set in child
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
        public string[] ImpactList;
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
            impacts = new List<IImpact>();

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
                                    else if (orderBackCount < 0)
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
        public void MakeImpact(IImpact impact)
        {
            impacts.Add(impact);
        }
        public bool HaveImpact(string impactName)
        {
            if (this.impacts.Exists(x => x.Name == impactName)) return true;
            else return false;
        }
        public void RemoveImpact(IImpact impact)
        {
            impacts.Remove(impact);
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
            if (aiStatus == UnitStateType.UnderControl)
                situation = TacticSituation.Defense;
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
                        if (Randomizer.Uniform(0, 100, 1)[0] < unknown.Stealthness * radarPover * multiplicator * 100)
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
                    float angel = Vector3.Angle(this.transform.position - x.transform.position, x.transform.forward);
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
                Gunner.SetAim(CurrentTarget);
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
            else targetStatus = TargetStateType.BehindABarrier;
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
                if (x.Ping(this.transform.position))
                    enemys.AddRange(x.GetEnemys());
            }
            return enemys;
        }
        public SpaceShip[] GetEnemys()
        {
            return enemys.ToArray();
        }
        public SpaceShip[] GetAllies()
        {
            return allies.ToArray();
        }
        public bool Ping(Vector3 sender)
        {
            if (Vector3.Distance(this.gameObject.transform.position, sender) < RadarRange * radiolink)
                return true;
            else return false;
        }
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
            if (Target != this)
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
        protected int FigtherSortEnemys(IUnit x, IUnit y)
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
}
