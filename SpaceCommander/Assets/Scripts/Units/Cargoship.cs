using DeusUtility;
using DeusUtility.UI;
using SpaceCommander.AI;
using SpaceCommander.General;
using SpaceCommander.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Mechanics.Units
{
   public class Cargoship : Unit, IEngine
    {
        public const float AIUpdateRate = 20f; //per second
        //GUI
        public UI.HUDBase hud;
        public Texture Icon;
        public Vector3 Anchor;
        //constants
        protected float speedThrust; //set in child
        protected float acceleration;
        protected float speedRotation;
        protected float speedShift;
        protected float radarRange; //set in child
        protected float radarPover; // default 1

        #region Override properties
        public override float Hull { set { armor.Hitpoints = value; } get { return armor.Hitpoints; } }
        public override float MaxHull { get { return armor.MaxHitpoints * (1 + MaxHullMultiplacator); } }
        //public override Army Team { get { return base.Team; } }
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
        public override float RadarRange { get { return radarRange * (1 + RadarRangeMultiplacator); } }
        public override float ShieldForce { get { return 0; } set { } }
        public override float ShieldRecharging { get { return 0; } }
        public override float ShieldCampacity { get { return 1; } }
        public override bool ShieldOwerheat { get { return true; } }
        public override Unit CurrentTarget { get { return Gunner.Target; } }
        public override float ShellResist { get { return armor.ShellResist * (1 + ResistMultiplacator); } }
        public override float EnergyResist { get { return armor.EnergyResist * (1 + ResistMultiplacator); } }
        public override float BlastResist { get { return armor.BlastResist * (1 + ResistMultiplacator); } }
        public override float Stealthness { get { return stealthness * (1 + StealthnessMultiplacator); } }
        #endregion
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
        public IDriver Driver;
        public IGunner Gunner { get { return null; } }
        protected GlobalController Global;
        protected IArmor armor;
        private List<IImpact> impacts;
        public SpellModule[] module;
        protected float synchAction;
        public float movementAIDelay; //Make private after debug;
        public string GunnerTarget; //debug only
        public string[] ImpactList;
        protected List<Unit> enemys = new List<Unit>();

        protected void Start()//_______________________Start
        {
            Global = GlobalController.Instance;
            type = UnitClass.Civil;
            cooldownDetected = 0;
            unitName = type.ToString();
            hud = FindObjectOfType<HUDBase>();
            Anchor = this.transform.position;
            //
            armor = this.gameObject.GetComponent<IArmor>();
            reams = GameObjectUtility.GetChildObjectByName(this.transform, "Jetream").ToArray();
            engineSound = this.gameObject.GetComponent<AudioSource>();
            //
            StatsUp();
            //
            Relationship = Global.GetRalationship(Team);
            Driver = new SpaceMovementController(this.gameObject);

            impacts = new List<IImpact>();

            this.gameObject.transform.Find("MinimapPict").Find("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.Find("MinimapPict").Find("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;

            if (Team == Global.playerArmy)
                this.gameObject.transform.Find("MinimapPict").Find("AlliesMinimapPict").GetComponent<Renderer>().enabled = true;

            Debug.Log("Unit " + this.gameObject.name + " started");
        }
        public void ResetStats()
        {
            Start();
        }
        protected virtual void StatsUp()
        {
            acceleration = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "acceleration"), CultureInfo.InvariantCulture);
            speedThrust = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedThrust"), CultureInfo.InvariantCulture);
            speedRotation = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedRotation"), CultureInfo.InvariantCulture);
            speedShift = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedShift"), CultureInfo.InvariantCulture);
            stealthness = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "stealthness"), CultureInfo.InvariantCulture);
            radarRange = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radarRange"), CultureInfo.InvariantCulture);
            radarPover = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radarPover"), CultureInfo.InvariantCulture);

            float maxHitpoints = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "maxHitpoints"), CultureInfo.InvariantCulture);
            float shellResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "shellResist"), CultureInfo.InvariantCulture);
            float energyResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "energyResist"), CultureInfo.InvariantCulture);
            float blastResist = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "blastResist"), CultureInfo.InvariantCulture);
            armor.StatUp(maxHitpoints, maxHitpoints, shellResist, energyResist, blastResist);
        }
        protected void Update()//______________________Update
        {
            Driver.Update();
            Scan();
            //cooldowns
            DecrementBaseCounters();
            //action
            if (synchAction <= 0)
            {
                synchAction = 1f / AIUpdateRate;
                if (Driver.Status == DriverStatus.Waiting && movementAIDelay <= 0)
                    IdleManeuverFunction();
                SelfDefenceFunction();
            }
        }
        protected void FixedUpdate()
        {
            Driver.FixedUpdate();
        }
        protected void Scan() //___________Scan
        {
            enemys.RemoveAll(x => x == null);
            foreach (Unit unknown in Global.unitList)
                if (unknown != this)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, unknown.transform.position);
                    if (distance < RadarRange)
                    {
                        float multiplicator = Mathf.Pow(((-distance + RadarRange) * 0.02f), (1f / 5f)) * ((2f / (distance + 0.1f)) + 1);
                        if (radarPover * multiplicator > unknown.Stealthness)
                            if (CheckRelationship(unknown.Team) == RelationshipType.Enemys)
                            {
                                if (!enemys.Contains(unknown))
                                    enemys.Add(unknown);
                            }
                    }
                    else
                    {
                        if (enemys.Contains(unknown))
                            enemys.Remove(unknown);
                    }
                }
        }

        protected void DecrementBaseCounters()
        {
            synchAction -= Time.deltaTime;
            if (Driver.Status == DriverStatus.Waiting && movementAIDelay > 0)
                movementAIDelay -= Time.deltaTime;
            //waitingBackCount = Driver.backCount;//синхронизация счетчиков
            if (this.Team != Global.playerArmy)
                cooldownDetected -= Time.deltaTime;
            if (cooldownDetected < 0)
            {
                this.gameObject.transform.Find("MinimapPict").Find("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
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
            if (hud != null)
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
                if (!outOfBorder) frameSize = new Vector2(Global.Prefab.NeutralGUIFrame.width, Global.Prefab.NeutralGUIFrame.height);// * hud.scale;
                else frameSize = new Vector2(Global.Prefab.AlliesOutscreenPoint.width, Global.Prefab.AlliesOutscreenPoint.height);
                Vector2 iconSize = new Vector2(Icon.width, Icon.height);// * hud.scale;
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
                bool drawStatBars = false;
                if (cooldownDetected > 0)
                {
                    style.normal.textColor = Color.white;
                    if (Global.Settings.EnemyUI.ShowUnitFrame)
                    {
                        if (!outOfBorder)
                            GUI.DrawTexture(new Rect(new Vector2(frameX, frameY), frameSize), Global.Prefab.NeutralGUIFrame);
                    }
                    if (Global.Settings.EnemyUI.ShowUnitFrame)
                        GUI.DrawTexture(new Rect(new Vector2(iconX, iconY), iconSize), Icon);
                    if (Global.Settings.EnemyUI.ShowUnitName)
                        GUI.Label(new Rect(crd.x - 100, crd.y - (frameSize.y / 2) * 1.1f, 200, 18), UnitName, style);
                    drawStatBars = Global.Settings.EnemyUI.ShowUnitStatus;
                }
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
        protected void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Corvette);
        }

        //AI logick
        protected virtual void SelfDefenceFunction()
        {
            MissileProtection();
            UseModule(new SpellFunction[] { SpellFunction.Defence, SpellFunction.Self });
        }

        //sevice function
        //base maneuvers
        protected virtual bool IdleManeuverFunction()
        {
            if (Vector3.Distance(this.transform.position, Anchor) > 100)
                return BackToAncour();
            return false;// Driver.ExecetePointManeuver(PointManeuverType.PatroolLine, this.transform.position, this.transform.forward * 50);
        }
        protected bool BackToAncour()
        {
            return Driver.MoveToQueue(Anchor);
        }

        //sensors
        protected virtual GameObject AntiMissileRadar()
        {
            List<GameObject> hazard = new List<GameObject>();
            hazard.AddRange(GameObject.FindGameObjectsWithTag("Missile"));
            foreach (GameObject x in hazard)
            {
                if (Vector3.Distance(x.transform.position, this.transform.position) < 300 && !x.GetComponent<Missile>().Allies(Team))
                    return x;
            }
            return null;
        }
        public override void ShieldCriticalAlarm() { }
        //combat
        public override void ResetTarget()    {     }
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
        protected void HazardEvasion(GameObject hazard)
        {
            if (Driver.PathPoints == 0)
            {
                Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, hazard.transform);
            }
        }
        protected void CooperateFire() { }

        public override void GetFireSupport(Unit Target) { }
        public override void ArmorCriticalAlarm()
        {
            UseModule(new SpellFunction[] { SpellFunction.Emergency, SpellFunction.Hull });
        }
        protected void UseModule(SpellFunction[] functions)
        {
            if (module != null && module.Length > 0)
                foreach (SpellModule m in module)
                    if (m.FunctionsIs(functions))
                        m.EnableIfReady();
        }

    }
}
