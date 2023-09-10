using SpaceCommander.AI;
using SpaceCommander.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Mechanics
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        public float DamageMultiplacator { set; get; }
        public float FirerateMultiplacator { set; get; }
        public float RangeMultiplacator { set; get; }
        public float DispersionMultiplicator { set; get; }
        public float RoundspeedMultiplacator { set; get; }
        public float APMultiplacator { set; get; }

        protected WeaponType type;
        protected GlobalController Global;
        protected Unit owner;
        protected Rigidbody ownerBody;
        private ParticleSystem particle;
        protected new AudioSource audio;
        private bool useSoundDelay = true;
        protected Unit target;
        public Unit Target { set { target = value; } get { return target; } }
        private float dispersion; //dafault 0;
        private float shildBlinkTime; //default 0.01
        private float roundSpeed; //default 1000;
        private float firerate;
        private float range;
        private float aimAngle;

        protected bool PreAiming;
        protected float backCount;

        public float Range { get { return range * (1 + RangeMultiplacator); } protected set { range = value; } }
        public float AimAngle { get { return aimAngle; } }
        public float RoundSpeed { get { return roundSpeed * (1 + RoundspeedMultiplacator); } protected set { roundSpeed = value; } }
        public float Dispersion { get { return dispersion * (1 + DispersionMultiplicator); } protected set { dispersion = value; } }
        public float Firerate { get { return firerate * (1 + FirerateMultiplacator); } protected set { firerate = value; } }
        public float ShildBlink { get { return shildBlinkTime; } protected set { shildBlinkTime = value; } }
        public float BackCounter { get { return backCount; } }
        public WeaponType Type { get { return type; } }
        private void OnEnable()
        {

        }
        protected void Start()
        {
            Global = GlobalController.Instance;
            owner = this.transform.GetComponentInParent<Unit>();
            ownerBody = owner.GetComponent<Rigidbody>();

            roundSpeed = 150;
            shildBlinkTime = 0.01f;
            particle = this.GetComponentInChildren<ParticleSystem>();
            {
                audio = this.gameObject.GetComponent<AudioSource>();
                if (audio == null) CreateAudioSourse();
            }
            StatUp();
        }
        private void CreateAudioSourse()
        {
            audio = this.gameObject.AddComponent<AudioSource>();
            audio.clip = Global.Sound.GetShot(this.type);
            audio.playOnAwake = false;
            audio.spatialBlend = 1;
            audio.priority = 120;
            audio.minDistance = 10;
            audio.maxDistance = 2000;
            audio.rolloffMode = AudioRolloffMode.Logarithmic;
        }
        protected abstract void StatUp();
        protected void StatUp(float dispersion, float shildBlinkTime, float roundSpeed, float firerate, float range)
        {
            this.dispersion = dispersion;
            this.shildBlinkTime = shildBlinkTime;
            this.roundSpeed = roundSpeed;
            this.firerate = firerate;
            this.range = range;
            this.aimAngle = 10;
        }
        // Update is called once per frame
        public virtual void Update()
        {
            if (PreAiming) Preaiming();
            if (backCount > 0)
                backCount -= Time.deltaTime;
            UpdateLocal();
        }
        protected virtual void UpdateLocal()
        { }
        private void Preaiming()
        {
            float angel = Vector3.Angle(this.transform.forward, this.gameObject.GetComponentInParent<Unit>().transform.forward);
            if (angel < aimAngle && target != null)
            {
                try
                {
                    float distance;
                    float approachTime;
                    Vector3 aimPoint = target.transform.position;
                    //Debug.Log(target.GetComponent<NavMeshAgent>().velocity);
                    Vector3 targetVelocity = target.Velocity - owner.Velocity;

                    distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до цели
                    approachTime = distance / RoundSpeed;
                    aimPoint = target.transform.position + targetVelocity * approachTime; //первое приближение

                    distance = Vector3.Distance(this.gameObject.transform.position, aimPoint); //расстояние до точки первого приближения
                    approachTime = distance / RoundSpeed;
                    aimPoint = target.transform.position + targetVelocity * approachTime * 1.01f; //второе приближение

                    //distance = Vector3.Distance(this.gameObject.transform.position, aimPoint);
                    //approachTime = distance / averageRoundSpeed;
                    //aimPoint = target.transform.position + target.GetComponent<NavMeshAgent>().velocity * approachTime; //третье приближение

                    Quaternion targetRotation = Quaternion.LookRotation((aimPoint - this.transform.position).normalized, new Vector3(0, 1, 0)); //донаводка
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, Time.deltaTime * aimAngle * 1.2f);
                }
                catch (MissingReferenceException)
                {
                    target = null;
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(Vector3.zero);//возврат
                this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, targetRotation, Time.deltaTime * 6);
            }
        }
        public abstract void Reset();
        public abstract bool IsReady { get; }
        public abstract float ShootCounter { get; }
        public abstract float MaxShootCounter { get; }
        public virtual bool Fire()
        {
            particle.Play();
            //if (useSoundDelay)
            //    audio.PlayDelayed((Vector3.Distance(this.transform.position, Camera.main.transform.position) / 343f));
            //else
            audio.PlayOneShot(Global.Sound.GetShot(this.type), Global.Settings.SoundLevel);
            return true;
        }
        protected abstract void Shoot(Transform target);
        public static Quaternion RandomDirection(float dispersion)
        {
            Vector3 direction = Vector3.zero;
            direction.x += dispersion * UnityEngine.Random.Range(-1, 1);
            direction.y += dispersion * UnityEngine.Random.Range(-1, 1);
            return Quaternion.Euler(direction);
        }
        //public static Quaternion RandomDirectionNormal(float dispersion, GlobalController Global)
        //{
        //    float vertComp = UnityEngine.Random.Range(-(Global.RandomNormalPool.Length - 2), Global.RandomNormalPool.Length - 2);
        //    float horComp = UnityEngine.Random.Range(-(Global.RandomNormalPool.Length - 2), Global.RandomNormalPool.Length - 2);
        //    Vector3 direction = Vector3.zero;
        //    direction.x += (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(Mathf.Abs(vertComp))]) - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion * Mathf.Sign(vertComp);
        //    direction.y += (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(Mathf.Abs(horComp))]) - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion * Mathf.Sign(horComp);
        //    return Quaternion.Euler(direction);
        //}
        public static Quaternion RandomDirectionNormal(float dispersion)
        {
            GlobalController Global = GlobalController.Instance;
            float vertComp = UnityEngine.Random.Range(-(Global.RandomNormalPool.Length - 2), Global.RandomNormalPool.Length - 2);
            float horComp = UnityEngine.Random.Range(-(Global.RandomNormalPool.Length - 2), Global.RandomNormalPool.Length - 2);
            Vector3 direction = Vector3.zero;
            direction.x += (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(Mathf.Abs(vertComp))]) - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion * Mathf.Sign(vertComp);
            direction.y += (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(Mathf.Abs(horComp))]) - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion * Mathf.Sign(horComp);
            return Quaternion.Euler(direction);
        }
    }
    public abstract class MagWeapon : WeaponBase
    {
        public float ReloadMultiplacator { set; get; }
        public float AmmocampacityMultiplacator { set; get; }
        public float ShellmassMultiplacator { set; get; }

        private float reloadingTime;
        private int ammoCampacity;
        private int ammo;
        protected int Ammo { get { return ammo; } }
        protected int AmmoCampacity { get { return Mathf.RoundToInt(ammoCampacity * (1 + AmmocampacityMultiplacator)); } }
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //Global.SpecINI.Write(this.GetType().ToString(), "range", range.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "dispersion", dispersion.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "shildBlinkTime", shildBlinkTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "firerate", firerate.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "ammoCampacity", ammoCampacity.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "reloadingTime", reloadingTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "PreAiming", PreAiming.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "roundSpeed", roundSpeed.ToString());

            float dispersion = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "dispersion"), CultureInfo.InvariantCulture);
            float shildBlinkTime = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "shildBlinkTime"), CultureInfo.InvariantCulture);
            float firerate = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "firerate"), CultureInfo.InvariantCulture);
            float roundSpeed = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "roundSpeed"), CultureInfo.InvariantCulture);
            float range = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "range"), CultureInfo.InvariantCulture);
            base.StatUp(dispersion, shildBlinkTime, roundSpeed, firerate, range);

            ammoCampacity = Convert.ToInt32(Global.SpecINI.GetValue(this.GetType().ToString(), "ammoCampacity"), CultureInfo.InvariantCulture);
            reloadingTime = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "reloadingTime"), CultureInfo.InvariantCulture);
            PreAiming = Convert.ToBoolean(Global.SpecINI.GetValue(this.GetType().ToString(), "PreAiming"), CultureInfo.InvariantCulture);

            ammo = AmmoCampacity;
        }
        public override bool Fire()
        {
            if (IsReady)
            {
                base.Fire();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                ammo--;
                backCount = 60f / Firerate;
                return true;
            }
            else return false;
        }
        public override void Update()
        {
            base.Update();
            if (ammo <= 0 && backCount <= 0)
            {
                ammo = AmmoCampacity;
                backCount = reloadingTime * (1 + ReloadMultiplacator);
            }
        }
        public override bool IsReady
        {
            get
            {
                return (ammo > 0 && backCount <= 0);
            }
        }
        public override float ShootCounter { get { return ammo; } }
        public override float MaxShootCounter { get { return reloadingTime * (1 + ReloadMultiplacator); } }
        public override void Reset()
        {
            ammo = AmmoCampacity;
            backCount = 0;
        }
    }
    public abstract class ShellWeapon : WeaponBase
    {
        public float ReloadMultiplacator { set; get; }
        public float AmmocampacityMultiplacator { set; get; }
        public float ShellmassMultiplacator { set; get; }

        protected float reloadingTime;
        protected float reloadBackCount;
        protected int ammo;
        protected int ammoCampacity;
        protected int Ammo { get { return ammo; } }
        protected int AmmoCampacity { get { return Mathf.RoundToInt(ammoCampacity * (1 + AmmocampacityMultiplacator)); } }
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //Global.SpecINI.Write(this.GetType().ToString(), "range", range.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "dispersion", dispersion.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "shildBlinkTime", shildBlinkTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "firerate", firerate.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "ammoCampacity", ammoCampacity.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "reloadingTime", reloadingTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "PreAiming", PreAiming.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "roundSpeed", roundSpeed.ToString());

            float dispersion = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "dispersion"), CultureInfo.InvariantCulture);
            float shildBlinkTime = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "shildBlinkTime"), CultureInfo.InvariantCulture);
            float firerate = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "firerate"), CultureInfo.InvariantCulture);
            float roundSpeed = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "roundSpeed"), CultureInfo.InvariantCulture);
            float range = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "range"), CultureInfo.InvariantCulture);
            base.StatUp(dispersion, shildBlinkTime, roundSpeed, firerate, range);

            ammoCampacity = Convert.ToInt32(Global.SpecINI.GetValue(this.GetType().ToString(), "ammoCampacity"), CultureInfo.InvariantCulture);
            reloadingTime = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "reloadingTime"), CultureInfo.InvariantCulture);
            PreAiming = Convert.ToBoolean(Global.SpecINI.GetValue(this.GetType().ToString(), "PreAiming"), CultureInfo.InvariantCulture);

            ammo = AmmoCampacity;
        }
        public override bool Fire()
        {
            if (IsReady)
            {
                base.Fire();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                ammo--;
                backCount = reloadingTime * (1 + ReloadMultiplacator);
                reloadBackCount = 60f / Firerate;
                return true;
            }
            else return false;
        }
        public override void Update()
        {
            base.Update();
            if (ammo < AmmoCampacity && backCount <= 0)
                ammo++;
            if (ammo < AmmoCampacity && backCount <= 0)
                backCount = reloadingTime * (1 + ReloadMultiplacator);
            if (reloadBackCount > 0) reloadBackCount -= Time.deltaTime;
        }
        public override bool IsReady
        {
            get
            {
                return (ammo > 0 && reloadBackCount <= 0);
            }
        }
        public override float ShootCounter { get { return ammo; } }
        public override float MaxShootCounter { get { return reloadingTime * (1 + ReloadMultiplacator); } }
        public override void Reset()
        {
            ammo = AmmoCampacity;
            backCount = 0;
            reloadBackCount = 0;
        }
    }
    public abstract class EnergyWeapon : WeaponBase
    {
        protected float coolingMultiplacator;
        protected float maxheatMultiplacator;

        protected float heat;
        protected float maxHeat;
        protected bool overheat;
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;

            //Global.SpecINI.Write(this.GetType().ToString(), "range", range.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "dispersion", dispersion.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "shildBlinkTime", shildBlinkTime.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "firerate", firerate.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "maxHeat", maxHeat.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "PreAiming", PreAiming.ToString());
            //Global.SpecINI.Write(this.GetType().ToString(), "roundSpeed", roundSpeed.ToString());

            float dispersion = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "dispersion"), CultureInfo.InvariantCulture);
            float shildBlinkTime = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "shildBlinkTime"), CultureInfo.InvariantCulture);
            float firerate = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "firerate"), CultureInfo.InvariantCulture);
            float roundSpeed = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "roundSpeed"), CultureInfo.InvariantCulture);
            float range = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "range"), CultureInfo.InvariantCulture);
            base.StatUp(dispersion, shildBlinkTime, roundSpeed, firerate, range);

            maxHeat = Convert.ToInt32(Global.SpecINI.GetValue(this.GetType().ToString(), "maxHeat"), CultureInfo.InvariantCulture);
            PreAiming = Convert.ToBoolean(Global.SpecINI.GetValue(this.GetType().ToString(), "PreAiming"), CultureInfo.InvariantCulture);
        }
        public override bool Fire()
        {
            if (IsReady)
            {
                base.Fire();
                if (target != null)
                    Shoot(target.transform);
                else Shoot(null);
                backCount = 60f / Firerate;
                return true;
            }
            else return false;
        }
        public override void Update()
        {
            base.Update();
            if (heat > 0)
            {
                heat -= Time.deltaTime * 4 * (1 + coolingMultiplacator);
                if (heat > maxHeat)
                    overheat = true;
            }
            else overheat = false;
        }
        public override bool IsReady { get { return (!overheat && backCount <= 0); } }
        public override float ShootCounter { get { return heat; } }
        public override float MaxShootCounter { get { return maxHeat * (1 + maxheatMultiplacator); } }
        public override void Reset()
        {
            heat = 0;
            backCount = 0;
        }
    }
    public abstract class Round : MonoBehaviour
    {
        protected float speed;
        protected float damage;
        protected float armorPiersing;
        protected float ttl;
        protected bool canRicochet;

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
                Destroy();
        }
        protected virtual void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        if (!canRicochet)
                            Destroy();
                        break;
                    }
            }
        }
        public abstract void Destroy();
    }
    public abstract class Missile : MonoBehaviour
    {
        protected GlobalController Global;
        protected Rigidbody body;
        protected Army Team;
        protected float acceleration;// ускорение ракеты           
        protected float dropImpulse;//импульс сброса          
        protected float lifeTime;// время жизни
        protected float explosionTime;// длительность жизни

        protected virtual void Start()
        {
            Global = GlobalController.Instance;
            body = gameObject.GetComponent<Rigidbody>();
            acceleration = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "acceleration"), CultureInfo.InvariantCulture);
            dropImpulse = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "dropImpulse"), CultureInfo.InvariantCulture);
            explosionTime = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "explosionTime"), CultureInfo.InvariantCulture);
            GetComponent<AudioSource>().volume = Global.Settings.SoundLevel;
        }
        protected virtual void Update()
        {
            if (lifeTime > explosionTime)
                Explode();
            else
                lifeTime += Time.deltaTime;
            //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity.magnitude);

            //полет по прямой
            Vector3 shift = Vector3.zero;
            //полет по прямой
            float signVelocity;
            float mainSpeed = Vector3.Project(body.velocity, body.transform.right).magnitude;
            if (Vector3.Angle(body.velocity, body.transform.right) < 90)
                signVelocity = 1;
            else signVelocity = -1;
            shift.z = Mathf.Clamp((acceleration * 10) - (mainSpeed * signVelocity), -acceleration, acceleration);
            //shift.x = acceleration;
            //компенсирование боковых инерций

            float horisontalSpeed = Vector3.Project(body.velocity, body.transform.right).magnitude;
            if (Vector3.Angle(body.velocity, body.transform.right) < 90)
                signVelocity = 1;
            else signVelocity = -1;
            shift.x = Mathf.Clamp(-(horisontalSpeed * signVelocity), -acceleration, acceleration);

            float verticalSpeed = Vector3.Project(body.velocity, body.transform.up).magnitude;
            if (Vector3.Angle(body.velocity, body.transform.up) < 90)
                signVelocity = 1;
            else signVelocity = -1;
            shift.y = Mathf.Clamp(-(verticalSpeed * signVelocity), -acceleration, acceleration);
            body.AddRelativeForce(shift, ForceMode.Acceleration);
        }
        protected virtual void UpdateLocal() { }
        public abstract void Explode();
        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Unit":
                    {
                        if (lifeTime > explosionTime / 10)
                            Explode();
                        break;
                    }
                default:
                    {
                        Explode();
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
                        if (lifeTime > explosionTime / 20)
                            Explode();
                        break;
                    }
            }
        }
        private void OnGUI()
        {
            if (true)
            {
                float scaleLocal = 0.25f * Global.Settings.IconsScale;

                float distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
                if (distance < 400)
                {
                    float border = 40;
                    bool outOfBorder = false;
                    Vector3 crd;
                    if (Global.ManualController.enabled) crd = DeusUtility.UI.UIUtil.WorldToScreenCircle(this.transform.position, border, out outOfBorder);
                    else crd = DeusUtility.UI.UIUtil.WorldToScreenFrame(this.transform.position, border, out outOfBorder);
                    if (!outOfBorder)
                    {
                        Vector2 frameSize = new Vector2(Global.Prefab.AlliesGUIFrame.width, Global.Prefab.AlliesGUIFrame.height);
                        frameSize = frameSize * scaleLocal;
                        float frameY = crd.y - frameSize.y / 2f - (12 * scaleLocal);
                        float frameX = crd.x - frameSize.x / 2f;
                        Texture frameToDraw;
                        if (Team == Global.playerArmy)
                            frameToDraw = Global.Prefab.AlliesGUIFrame;
                        else
                            frameToDraw = Global.Prefab.EnemyGUIFrame;
                        GUI.DrawTexture(new Rect(new Vector2(frameX, frameY), frameSize), frameToDraw);
                    }
                }
            }
        }
        public void SetTeam(Army allies)
        {
            this.Team = allies;
        }
        public bool Allies(Army army)
        {
            return (Team == army);
        }
    }
    public abstract class SelfguidedMissile : Missile
    {
        public Transform target;// цель для ракеты       
        //public GameObject Blast;// префаб взрыва   
        protected float turnSpeed;// скорость поворота ракеты            
        protected float aimCone;
        protected float explosionRange; //расстояние детонации

        protected override void Start()
        {
            base.Start();

            turnSpeed = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "turnSpeed"), CultureInfo.InvariantCulture);
            aimCone = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "aimCone"), CultureInfo.InvariantCulture);
            explosionRange = Convert.ToSingle(Global.SpecINI.GetValue(this.GetType().ToString(), "explosionRange"), CultureInfo.InvariantCulture);
        }
        protected override void Update()
        {
            base.Update();
            UpdateLocal();
            if (target != null && Vector3.Distance(this.transform.position, target.transform.position) < explosionRange)
                Explode();

            if (lifeTime > 0.5)//задержка старта
            {
                if (target != null)//наведение
                {
                    Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, new Vector3(0, 1, 0));
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    // угол между направлением на цель и направлением ракеты порядок имеет значение.
                    Weapons.MissileTrap[] traps = FindObjectsOfType<Weapons.MissileTrap>();
                    if (traps.Length > 0)
                    {
                        foreach (Weapons.MissileTrap x in traps)
                        {
                            if (Vector3.Angle(x.transform.position - this.transform.position, this.transform.forward) < aimCone)
                            {
                                target = null;
                                break;
                            }
                        }
                    }
                }
                if (target != null && Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) > aimCone)
                    target = null;
            }
        }
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

    }
}
