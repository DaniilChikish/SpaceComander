using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DeusUtility.UI;
using DeusUtility;

namespace SpaceCommander.Units
{
    public class BattleDrone: Unit, IEngine
    {
        public const float AIUpdateRate = 20f; //per second
        //base varibles
        protected UnitStateType aiStatus;
        protected TargetStateType targetStatus;
        public SpaceShip owner;
        //GUI
        public HUDBase hud;
        public Texture enemyIcon;
        public Texture aliesIcon;

        //constants
        protected float speedThrust; //set in child
        protected float acceleration;
        protected float speedRotation;
        protected float speedShift;
        //override properties
        public override float Hull { set { armor.Hitpoints = value; } get { return armor.Hitpoints; } }
        public override float MaxHull { get { return armor.MaxHitpoints * (1 + MaxHullMultiplacator); } }
        public override Army Team { get { return team; } }
        public override Vector3 Velocity
        {
            get
            {
                return Driver.Velocity;
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
        public override float RadarRange { get { return 0; } }
        public override float ShieldForce { get { return 0; } set { } }
        public override float ShieldRecharging { get { return 0; } }
        public override float ShieldCampacity { get { return 1; } }
        public override bool ShieldOwerheat { get { return true; } }
        public override Unit CurrentTarget { get { if (owner != null) return owner.Gunner.Target; else return null; } }
        public override float ShellResist { get { return armor.ShellResist * (1 + ResistMultiplacator); } }
        public override float EnergyResist { get { return armor.EnergyResist * (1 + ResistMultiplacator); } }
        public override float BlastResist { get { return armor.BlastResist * (1 + ResistMultiplacator); } }
        public override float Stealthness { get { return stealthness * (1 + StealthnessMultiplacator); } }

        //interface
        public IWeapon[] PrimaryWeapon { get { return Gunner.Weapon[0]; } }
        public IWeapon[] SecondaryWeapon { get { return Gunner.Weapon[1]; } }
        public Transform GetTransform()
        {
            return this.gameObject.transform;
        }
        //modules
        protected float stealthness; //set in child
        protected bool detected;
        //controllers
        public bool ManualControl { set; get; }
        public IDriver Driver;
        protected IGunner gunner;
        public IGunner Gunner { get { return gunner; } }
        protected GlobalController Global;
        protected IArmor armor;
        private List<IImpact> impacts;
        protected float synchAction;
        public float movementAIDelay; //Make private after debug;
        public string GunnerTarget; //debug only
        public string[] ImpactList;
        //base interface
        protected void Start()//_______________________Start
        {
            Global = GlobalController.GetInstance();
            type = UnitClass.Drone;
            cooldownDetected = 0;
            aiStatus = UnitStateType.Waiting;
            unitName = type.ToString();
            hud = FindObjectOfType<HUDBase>();
            Global.unitList.Add(this);
            //
            armor = this.gameObject.GetComponent<IArmor>();
            reams = GameObjectUtility.GetChildObjectByName(this.transform, "Jetream").ToArray();
            engineSound = this.gameObject.GetComponent<AudioSource>();
            //
            StatsUp();
            //
            gunner = new ShootController(this.GetTransform());
            Driver = new SpaceMovementController(this.gameObject);

            impacts = new List<IImpact>();

            this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;

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
            acceleration = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "acceleration"));
            speedThrust = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedThrust"));
            speedRotation = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedRotation"));
            speedShift = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedShift"));
            stealthness = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "stealthness"));

            float maxHitpoints = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "maxHitpoints"));
            float shellResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "shellResist"));
            float energyResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "energyResist"));
            float blastResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "blastResist"));
            armor.StatUp(maxHitpoints, maxHitpoints, shellResist, energyResist, blastResist);
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
            //action
            if (synchAction <= 0 && owner != null)
            {
                synchAction = 1f / AIUpdateRate;
                CombatFunction();
                if (targetStatus == TargetStateType.NotFinded)
                {
                    if (aiStatus == UnitStateType.Waiting)
                    {
                        GoToOwner();
                    }
                }
                if (Driver.Status == DriverStatus.Waiting && movementAIDelay <= 0)
                    switch (aiStatus)
                    {
                        case UnitStateType.MoveAI:
                            {
                                aiStatus = UnitStateType.MoveAI;
                                AttackManeuver();
                                break;
                            }
                        case UnitStateType.Waiting:
                            {
                                if (movementAIDelay <= 0)
                                {
                                    aiStatus = UnitStateType.MoveAI;
                                    AttackManeuver();
                                }
                                break;
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
            if (cooldownDetected < 0)
            {
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
                if (team == Global.playerArmy)
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
            Explosion();
            Destroy(this.gameObject);
        }
        private void OnDestroy()
        {
            Global.unitList.Remove(this);
        }
        protected virtual void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }

        //AI logick
        protected virtual bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        return ToPrimaryDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.IncreaseDistance, Gunner.Target.transform);
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return Rush();
                    }
                default:
                    return false;
            }
        }
        protected bool CombatFunction()
        {
            targetStatus = TargetStateType.NotFinded;
            if (CurrentTarget == null)//текущей цели нет
            {
                Gunner.ResetAim();
                return false;
            }
            else
                return OpenFire(CurrentTarget, 5);
        }
        //sevice function
        //base maneuvers

        protected bool ToPrimaryDistance()
        {
            Vector3 target = CurrentTarget.transform.position + (this.transform.position - CurrentTarget.transform.position).normalized * Gunner.GetRange(0) * 0.9f + new Vector3(0, 0.5f, 0);
            //target += this.transform.position + new Vector3(0, 0.5f, 0);
            return Driver.MoveToQueue(target);
        }
        protected bool Rush()
        {
            //waitingBackCount = 5f;
            Vector3 target = CurrentTarget.transform.position + CurrentTarget.transform.forward * Gunner.GetRange(0) * 0.4f;
            return Driver.MoveToQueue(target);
        }
        protected bool GoToOwner()
        {
            return Driver.MoveTo(owner.transform.position + owner.transform.forward * 50);
        }
        //sensors
        protected virtual GameObject AntiMissileRadar()
        {
            List<GameObject> hazard = new List<GameObject>();
            hazard.AddRange(GameObject.FindGameObjectsWithTag("Missile"));
            foreach (GameObject x in hazard)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < 300 && !x.GetComponent<Missile>().Allies(team))
                    return x;
            }
            return null;
        }
        public override void ArmorCriticalAlarm()        {        }
        public override void ShieldCriticalAlarm()       {       }

        //combat
        protected bool OpenFire(Unit target, float lockdown)
        {
            Gunner.SetAim(target, false, lockdown);
            if (Gunner.SeeTarget())
            {
                targetStatus = TargetStateType.Captured;//наведение
                for (int i = 0; i < Gunner.Weapon.Length; i++)
                    if (Gunner.TargetInRange(i))//выбор оружия
                    {
                        targetStatus = TargetStateType.InPrimaryRange;
                        Gunner.ShootHim(i);
                    }
                return true;
            }
            else targetStatus = TargetStateType.BehindABarrier;
            return false;
        } //_________OpenFire
        public override void ResetTarget()
        {
            Gunner.ResetAim();
        }
        protected bool MissileProtection()
        {
            GameObject hazard = AntiMissileRadar();
            if (hazard != null)
            {
                HazardEvasion(hazard);
                return true;
            }
            else return false;
        }
        protected void HazardEvasion(GameObject hazard)
        {
            if (Driver.PathPoints == 0)
            {
                aiStatus = UnitStateType.MoveAI;
                Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, hazard.transform);
            }
        }
        protected void CooperateFire()       {      }

        public override void GetFireSupport(Unit Target)        {       }
        public void SetTeam(Army team)
        {
            this.team = team;
        }
    }
}
