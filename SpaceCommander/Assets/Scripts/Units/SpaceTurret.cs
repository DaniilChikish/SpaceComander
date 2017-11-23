using System;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.UI;
using SpaceCommander.AI;
using SpaceCommander.General;

namespace SpaceCommander.Mechanics.Units
{
    public class SpaceTurret : Unit
    {
        public const float AIUpdateRate = 20f; //per second
        //base varibles
        //GUI
        public UI.HUDBase hud;
        public Texture enemyIcon;
        public Texture aliesIcon;
        //public Texture selectedIcon;

        //constants
        protected float radarRange; //set in child
        protected float radarPover; // default 1
        protected float speedRotation;
        //override properties
        public override float Hull { set { armor.Hitpoints = value; } get { return armor.Hitpoints; } }
        public override float MaxHull { get { return armor.MaxHitpoints * (1 + MaxHullMultiplacator); } }
        public override Vector3 Velocity
        {
            get
            {
                return Vector3.zero;
            }
        }
        public override float Acceleration { get { return 0; } }
        public override float Speed { get { return 0; } }
        public override float RotationSpeed { get { return speedRotation * (1 + RotationSpeedMultiplicator); } }
        public override float ShiftSpeed { get { return 0; } }
        public override float RadarRange { get { return radarRange * (1 + RadarRangeMultiplacator); } }
        public override float ShieldForce { get { return 0; } set { } }
        public override float ShieldRecharging { get { return 0; } }
        public override float ShieldCampacity { get { return 1; } }
        public override Unit CurrentTarget { get { return Gunner.Target; } }
        public override float ShellResist { get { return armor.ShellResist * (1 + ResistMultiplacator); } }
        public override float EnergyResist { get { return armor.EnergyResist * (1 + ResistMultiplacator); } }
        public override float BlastResist { get { return armor.BlastResist * (1 + ResistMultiplacator); } }
        public override float Stealthness { get { return stealthness * (1 + StealthnessMultiplacator); } }

        //own properties
        //interface
        public IWeapon[] Weapon { get { return Gunner.Weapon[0]; } }
        public Transform GetTransform()
        {
            return this.gameObject.transform;
        }
        //modules
        protected float stealthness; //set in child
        protected bool detected;
        //controllers
        protected IGunner gunner;
        public IGunner Gunner { get { return gunner; } }

        public override bool ShieldOwerheat { get { return true; } }

        protected GlobalController Global;
        protected IArmor armor;
        protected float synchAction;
        protected List<Unit> enemys = new List<Unit>();
        protected List<Unit> capByTarget;
        public string GunnerTarget; //debug only
        protected SortUnit EnemySortDelegate;
        protected SortUnit AlliesSortDelegate;

        //base interface
        protected void Start()//_______________________Start
        {
            type = UnitClass.Turret;
            Global = GlobalController.Instance;
            EnemySortDelegate = SortEnemysBase;
            radarPover = 1;
            cooldownDetected = 0;
            unitName = type.ToString();
            Global.unitList.Add(this);
            hud = FindObjectOfType<UI.HUDBase>();
            //
            armor = this.gameObject.GetComponent<IArmor>();
            //
            StatsUp();
            //
            gunner = new TurretShootController(this);
            //Driver = new NavmeshMovementController(this.gameObject);
            capByTarget = new List<Unit>();

            this.gameObject.transform.FindChild("MinimapPict").FindChild("AlliesMinimapPict").GetComponent<Renderer>().enabled = false;
            this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;

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
            speedRotation = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "speedRotation"));
            radarRange = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radarRange"));
            radarPover = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "radarPover"));
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
            Gunner.Update();
            //cooldowns
            DecrementBaseCounters();
            //action
            if (synchAction <= 0)
            {
                synchAction = 1f / AIUpdateRate;
                CombatFunction();
            }
        }
        protected void DecrementBaseCounters()
        {
            synchAction -= Time.deltaTime;
            if (this.Team != Global.playerArmy)
                cooldownDetected -= Time.deltaTime;
            if (cooldownDetected < 0)
            {
                this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = false;
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
        public override bool HaveImpact(string impactName)
        {
            return false;
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
        protected bool CombatFunction()
        {
            Scan();//поиск в зоне действия радара
            if (CurrentTarget == null)//текущей цели нет
            {
                if (enemys.Count > 0)
                {
                    bool output;
                    output = OpenFire(GetNearest(), 4);//огонь по ближайшей
                    return output;
                }
                else
                {
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
                    return output;
                }
                else
                {
                    Gunner.ResetAim();
                    return false;//переходим в ожидение
                }
            }
        }
        //sevice function
        //base maneuvers

        //sensors
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
                            if (CheckRelationship(unknown.Team)==RelationshipType.Enemys)
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
            enemys.Sort(delegate (Unit x, Unit y) { return EnemySortDelegate(x, y); });
        }
        //combat
        protected Unit GetNearest()
        {
            return enemys[0];
        }
        protected bool OpenFire(Unit target, float lockdown)
        {
            Gunner.SetAim(target, false, lockdown);
            if (Gunner.SeeTarget())
            {
                for (int i = 0; i < Gunner.Weapon.Length; i++)
                    if (Gunner.TargetInRange(i))//выбор оружия
                        Gunner.ShootHim(i);
            }
            return true;
        } //_________OpenFire
        public override void ResetTarget()
        {
            Gunner.ResetAim();
        }

        public override void GetFireSupport(Unit Target){}
        public override void MakeImpact(IImpact impact){}
        public override void RemoveImpact(IImpact impact) { }

        public override void ArmorCriticalAlarm() { }

        public override void ShieldCriticalAlarm() { }

        public void SetTeam(Army team)
        {
            this.Team = team;
        }
    }
}
