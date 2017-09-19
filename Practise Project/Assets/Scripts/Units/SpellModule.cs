using DeusUtility.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander
{
    public enum SpellModuleState { Ready, Active, Cooldown};
    public enum SpellType { Passive, Activated, LongAction }
    public enum SpellFunction { Self, Enemy, Allies, Shield, Health, Buff, Debuff, Attack, Support, Defence, Emergency}
    public abstract class SpellModule
    {
        protected float backCount;
        protected float activeTime;
        protected float coolingTime;
        protected SpellModuleState state;
        protected SpellType type;
        protected List<SpellFunction> function;
        protected SpaceShip owner;
        public SpellModuleState State { get { return state; } }
        public float BackCount {  get { return backCount; } }
        public float CoolingTime { get { return coolingTime; } }
        public float ActiveTime { get { return activeTime; } }
        public SpellType Type { get { return type; } }
        public bool FunctionsIs(SpellFunction[] functions)
        {
            foreach (SpellFunction f in functions)
                if (!this.function.Contains(f))
                    return false;
            return true;
        }
        public bool FunctionOneOf(SpellFunction[] functions)
        {
            foreach (SpellFunction f in functions)
                if (this.function.Contains(f))
                    return true;
            return false;
        }
        public SpellModule(SpaceShip owner)
        {
            this.owner = owner;
            this.function = new List<SpellFunction>();
        }
        public virtual void Enable()
        {
            backCount = activeTime;
            state = SpellModuleState.Active;
        }
        public virtual void EnableIfReady()
        {
            if (state == SpellModuleState.Ready)
                Enable();
        }
        public void Update()
        {
            if (backCount > 0)
                backCount -= Time.deltaTime;
            else
                switch (state)
                {
                    case SpellModuleState.Ready:
                        {
                            break;
                        }
                    case SpellModuleState.Active:
                        {
                            state = SpellModuleState.Cooldown;
                            backCount = coolingTime;
                            Disable();
                            break;
                        }
                    case SpellModuleState.Cooldown:
                        {
                            state = SpellModuleState.Ready;
                            break;
                        }
                }
            if (type == SpellType.Passive || (type == SpellType.LongAction && state == SpellModuleState.Active))
                Act();
        }
        protected virtual void Act() { }
        protected abstract void Disable();
    }
    public class Jammer : SpellModule
    {
        protected GlobalController Global;
        private float ownerStealthnessBefore;
        public Jammer(SpaceShip owner) : base(owner)
        {
            Global = GameObject.FindObjectOfType<GlobalController>();
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 100;
            activeTime = 30;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Defence);
            function.Add(SpellFunction.Buff);
        }

        protected override void Disable()
        {
            owner.Stealthness = ownerStealthnessBefore;
        }

        public override void Enable()
        {
            base.Enable();
            ownerStealthnessBefore = owner.Stealthness;
            owner.Stealthness = owner.Stealthness * 3.5f;
            foreach (SpaceShip x in Global.unitList)
            {
                if ((Vector3.Distance(x.transform.position, owner.transform.position) < owner.RadarRange / 2) && (x.CurrentTarget != null && x.CurrentTarget.transform == owner.transform))
                {
                    x.ResetTarget();
                }
            }
        }
    }
    public class Transponder : SpellModule
    {
        public Transponder(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 60;
            activeTime = 15;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Buff);
            function.Add(SpellFunction.Defence);
        }

        protected override void Disable()
        {

        }
        public override void Enable()
        {
            base.Enable();
        }
    }
    public class MissileTrapLauncher : SpellModule
    {
        protected GlobalController Global;
        private float brustCount;

        public MissileTrapLauncher(SpaceShip owner) : base(owner)
        {
            Global = GameObject.FindObjectOfType<GlobalController>();
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 15;
            activeTime = 2;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Defence);
            function.Add(SpellFunction.Emergency);
        }
        public override void Enable()
        {
            Debug.Log("MissileTrap Lauched");
            base.Enable();
            brustCount = 0;
        }

        protected override void Disable()
        {
        }

        protected override void Act()
        {
            if (brustCount <= 0)
            {
                brustCount = activeTime / 10f;
                float dispersion = 45;
                Quaternion direction = owner.transform.rotation;
                double[] randomOffset = Randomizer.Uniform(10, 90, 2);
                if (randomOffset[0] > 50)
                    direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion);
                else
                    direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * -dispersion);
                if (randomOffset[1] > 50)
                    direction.y = direction.z + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion);
                else
                    direction.y = direction.z + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * -dispersion);

                GameObject trap = GameObject.Instantiate(Global.MissileTrap, owner.transform.position, direction);


                GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");
                if (missiles.Length > 0)
                {
                    foreach (GameObject x in missiles)
                    {
                        if (x.GetComponent<SelfguidedMissile>().target == owner.transform && Vector3.Distance(x.transform.position, owner.transform.position) < owner.RadarRange / 2f)
                        {
                            x.GetComponent<SelfguidedMissile>().target = trap.transform;
                            break;
                        }
                    }
                }
            }
            else brustCount -= Time.deltaTime;
        }
    }
    public class Warp : SpellModule
    {
        public Warp(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 30;
            activeTime = 5;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Buff);
            function.Add(SpellFunction.Attack);
        }

        protected override void Disable()
        {
        }

        public override void Enable()
        {
            base.Enable();
            owner.MakeImpact(new WarpImpact(owner));
        }
    }
    public class WarpImpact : IImpact
    {
        public string Name { get { return "WarpImpact"; } }
        float ttl;
        SpaceShip owner;
        private float ownerMassPrev;
        public WarpImpact(SpaceShip owner)
        {
            this.owner = owner;
            ownerMassPrev = owner.GetComponent<Rigidbody>().mass;
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("TrusterInhibitorImpact"))
                    ttl = 0;
                else
                {
                    ttl = 4;
                    owner.SpeedMultiplicator += 6;
                    owner.GetComponent<Rigidbody>().mass = owner.GetComponent<Rigidbody>().mass / 10;
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }

        public void CompleteImpact()
        {
            owner.SpeedMultiplicator -= 6;
            owner.GetComponent<Rigidbody>().mass = ownerMassPrev;
            owner.RemoveImpact(this);
        }
    }
    public class RadarBooster : SpellModule
    {
        protected GlobalController Global;
        public RadarBooster(SpaceShip owner) : base(owner)
        {
            Global = GameObject.FindObjectOfType<GlobalController>();
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 60;
            activeTime = 20;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Allies);
            function.Add(SpellFunction.Buff);
            function.Add(SpellFunction.Support);
        }

        protected override void Disable()
        {
        }

        public override void Enable()
        {
            base.Enable();
            owner.MakeImpact(new RadarBoosterImpact(owner, activeTime));
            foreach (SpaceShip x in Global.unitList)
            {
                if (x.Allies(owner.Team) && (Vector3.Distance(x.transform.position, owner.transform.position) < owner.RadarRange / 4f))
                {
                    x.MakeImpact(new RadarBoosterImpact(owner, activeTime));
                }
            }
        }
    }
    public class RadarBoosterImpact : IImpact
    {
        public bool Act = false;
        public string Name { get { return "RadarBoosterImpact"; } }
        float ttl;
        SpaceShip owner;
        public RadarBoosterImpact(SpaceShip owner, float time)
        {
            this.owner = owner;
            if (owner.HaveImpact(this.Name))
                return;//ttl = 0;
            else
            {
                if (owner.HaveImpact("RadarInhibitorImpact"))
                    return;//ttl = 0;
                else
                {
                    Act = true;
                    ttl = time;
                    owner.RadarRangeMultiplacator +=  1;
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }

        public void CompleteImpact()
        {
            if (Act) owner.RadarRangeMultiplacator -= 1;
            owner.RemoveImpact(this);
        }
    }
    public class ShieldStunner : SpellModule
    {
        protected GlobalController Global;
        public ShieldStunner(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 30;
            activeTime = 1;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Enemy);
            function.Add(SpellFunction.Shield);
            function.Add(SpellFunction.Attack);
        }

        protected override void Disable()
        {
        }
        public override void EnableIfReady()
        {
            if (state == SpellModuleState.Ready)
            {
                if (owner.CurrentTarget != null && owner.CurrentTarget.ShieldForce > 50)
                    Enable();
            }
        }
        public override void Enable()
        {
            base.Enable();
            owner.CurrentTarget.MakeImpact(new ShildStunImpact(owner.CurrentTarget, 5));
        }
    }
    public class ShildStunImpact : IImpact
    {
        public bool Act = false;
        public string Name { get { return "ShildStunImpact"; } }
        private float ttl;
        private Unit owner;
        private float ownerShildRechargingPrev;
        public ShildStunImpact(Unit owner, float time)
        {
            this.owner = owner;
            ownerShildRechargingPrev = owner.ShieldRecharging;
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("ShildBoosterImpact"))
                    ttl = 0;
                else
                {
                    Act = true;
                    ttl = time;
                    owner.ShieldForce -= 25;
                    owner.ShieldRechargingMultiplacator -= (1 + (owner.ShieldCampacity / 200));
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            if (Act) owner.ShieldRechargingMultiplacator += (1 + (owner.ShieldCampacity / 200));
            owner.RemoveImpact(this);
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class TrusterStunner : SpellModule
    {
        protected GlobalController Global;
        public TrusterStunner(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 45;
            activeTime = 1;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Enemy);
            function.Add(SpellFunction.Attack);
        }

        protected override void Disable()
        {
        }
        public override void EnableIfReady()
        {
            if (state == SpellModuleState.Ready)
            {
                if (owner.CurrentTarget.Speed > 5)
                    Enable();
            }
        }
        public override void Enable()
        {
            base.Enable();
            owner.CurrentTarget.MakeImpact(new TrusterStunImpact(owner.CurrentTarget, 10));
        }
    }
    public class TrusterStunImpact : IImpact
    {
        public bool Act = false;
        public string Name { get { return "TrusterInhibitorImpact"; } }
        private float ttl;
        private Unit owner;
        public TrusterStunImpact(Unit owner, float time)
        {
            this.owner = owner;
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("WarpImpact"))
                    ttl = 0;
                else
                {
                    Act = true;
                    UnityEngine.AI.NavMeshAgent agent = owner.transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.speed = 0;
                        agent.ResetPath();
                        agent.velocity = Vector3.zero;
                    }
                    ttl = time;
                    owner.SpeedMultiplicator += (-1f);
                    owner.RotationSpeedMultiplicator += (-0.9f);
                    owner.ShiftSpeedMultiplicator += (-1f);
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            if (Act)
            {
                owner.SpeedMultiplicator -= (-1f);
                owner.RotationSpeedMultiplicator -= (-0.9f);
                owner.ShiftSpeedMultiplicator -= (-1f);
            }
            owner.RemoveImpact(this);
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class RadarInhibitor : SpellModule
    {
        protected GlobalController Global;
        public RadarInhibitor(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 45;
            activeTime = 0;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Enemy);
            function.Add(SpellFunction.Debuff);
            function.Add(SpellFunction.Attack);
        }

        protected override void Disable()
        {
        }
        public override void Enable()
        {
            base.Enable();
            owner.CurrentTarget.MakeImpact(new RadarInhibitorImpact(owner.CurrentTarget, 10));
        }
    }
    public class RadarInhibitorImpact : IImpact
    {
        public bool Act = false;
        public string Name { get { return "RadarInhibitorImpact"; } }
        float ttl;
        Unit owner;
        private float ownerRadarRangePrev;
        public RadarInhibitorImpact(Unit owner, float time)
        {
            this.owner = owner;
            ownerRadarRangePrev = owner.RadarRange;
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("RadarBoosterImpact"))
                    ttl = 0;
                else
                {
                    Act = true;
                    ttl = time;
                    owner.RadarRangeMultiplacator += (-0.6f);
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }

        public void CompleteImpact()
        {
            if (Act) owner.RadarRangeMultiplacator -= (-0.6f);
            owner.RemoveImpact(this);
        }
    }
    public class EmergencySelfRapairing : SpellModule
    {
        private float repairSpeed;
        public EmergencySelfRapairing(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 120;
            activeTime = 3;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Emergency);
            function.Add(SpellFunction.Health);
        }
        public override void EnableIfReady()
        {
            if (owner.Health < owner.MaxHealth * 0.4f)
                if (state == SpellModuleState.Ready)
                    Enable();
            //base.EnableIfReady();
        }
        protected override void Disable()
        {

        }
        public override void Enable()
        {
            base.Enable();
            repairSpeed = owner.MaxHealth / 5f;
        }

        protected override void Act()
        {
            owner.Health += repairSpeed * Time.deltaTime;
        }
    }
    public class EmergencyShieldRecharging : SpellModule
    {
        private float rechargingSpeed;
        public EmergencyShieldRecharging(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.LongAction;
            coolingTime = 90;
            activeTime = 5;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Emergency);
            function.Add(SpellFunction.Shield);
        }
        public override void EnableIfReady()
        {
            if (owner.ShieldOwerheat)
                base.EnableIfReady();
        }
        protected override void Disable()
        {

        }
        public override void Enable()
        {
            base.Enable();
            rechargingSpeed = owner.ShieldCampacity / 10f;
        }

        protected override void Act()
        {
            owner.ShieldForce += rechargingSpeed * Time.deltaTime;
        }
    }
    public class RepairingImpact : IImpact
    {
        public string Name { get { return "Repairing"; } }
        private float ttl;
        private float repairSpeed;
        private SpaceShip owner;
        public RepairingImpact(SpaceShip owner, float time)
        {
            this.owner = owner;
            switch (owner.Type)
            {
                case UnitClass.Bomber:
                    {
                        repairSpeed = 10;
                        break;
                    }
                case UnitClass.Command:
                    {
                        repairSpeed = 14;
                        break;
                    }
                case UnitClass.ECM:
                    {
                        repairSpeed = 4;
                        break;
                    }
                case UnitClass.Figther:
                    {
                        repairSpeed = 8;
                        break;
                    }
                case UnitClass.Guard_Corvette:
                    {
                        repairSpeed = 20;
                        break;
                    }
                case UnitClass.LR_Corvette:
                    {
                        repairSpeed = 20;
                        break;
                    }
                case UnitClass.Recon:
                    {
                        repairSpeed = 4;
                        break;
                    }
                case UnitClass.Scout:
                    {
                        repairSpeed = 6;
                        break;
                    }
                case UnitClass.Support_Corvette:
                    {
                        repairSpeed = 20;
                        break;
                    }
            }
            //if (owner.Impacts.Exists(x => x.ImpactName == this.ImpactName))
            //    ttl = 0;
            //else
            //{
            ttl = time;
            //}
        }
        public void ActImpact()
        {
            if (ttl > 0 && owner.Health < (owner.MaxHealth * 1.2))
            {
                owner.Health += repairSpeed * Time.deltaTime;
                ttl -= Time.deltaTime;
            }
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            owner.RemoveImpact(this);
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class ShieldsBraker : SpellModule
    {
        public ShieldsBraker(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 60;
            activeTime = 1;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Enemy);
            function.Add(SpellFunction.Shield);
            function.Add(SpellFunction.Attack);
        }

        public override void Enable()
        {
            base.Enable();
            //GlobalController Global = GameObject.FindObjectOfType<GlobalController>();
            foreach (Unit x in owner.GetEnemys())
            {
                x.ShieldForce -= (x.ShieldCampacity / 2);
            }
            //Instantiate(Global.EMIExplosionPrefab, this.transform.position, this.transform.rotation);
        }
        protected override void Disable()
        {
        }
    }
    public class AcceleratingCoils : SpellModule
    {
        public AcceleratingCoils(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 90;
            activeTime = 5;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Self);
            function.Add(SpellFunction.Buff);
            function.Add(SpellFunction.Attack);
        }
        public override void Enable()
        {
            base.Enable();
            Weapon[] weapons = owner.transform.GetComponentsInChildren<Weapon>();
            foreach (Weapon x in weapons)
                x.RoundspeedMultiplacator += 0.3f;
        }

        protected override void Disable()
        {
            Weapon[] weapons = owner.transform.GetComponentsInChildren<Weapon>();
            foreach (Weapon x in weapons)
                x.RoundspeedMultiplacator -= 0.3f;
        }
    }
    public class AcceleratingCoilsPassive : SpellModule
    {
        public AcceleratingCoilsPassive(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Passive;
            coolingTime = 0;
            activeTime = 0;
            state = SpellModuleState.Cooldown;
            Weapon[] weapons = owner.transform.GetComponentsInChildren<Weapon>();
            foreach (Weapon x in weapons)
                x.RoundspeedMultiplacator += 0.1f;
        }
        public override void Enable()
        {
            base.Enable();
        }

        protected override void Disable()
        {
        }
    }
    public class RechargeAcceleratorPassive : SpellModule
    {
        public RechargeAcceleratorPassive(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Passive;
            coolingTime = 0;
            activeTime = 0;
            state = SpellModuleState.Cooldown;
            MagWeapon[] weapons = owner.transform.GetComponentsInChildren<MagWeapon>();
            foreach (MagWeapon x in weapons)
                x.ReloadMultiplacator += 0.15f;
        }
        public override void Enable()
        {
            base.Enable();
        }

        protected override void Disable()
        {
        }
    }
    public class ExtendedAmmoPack : SpellModule
    {
        public ExtendedAmmoPack(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Passive;
            coolingTime = 0;
            activeTime = 0;
            state = SpellModuleState.Cooldown;
            MagWeapon[] weapons = owner.transform.GetComponentsInChildren<MagWeapon>();
            foreach (MagWeapon x in weapons)
            {
                x.AmmocampacityMultiplacator += 0.5f;
                x.Reset();
            }
        }
        public override void Enable()
        {
            base.Enable();
        }

        protected override void Disable()
        {
        }
    }
    public class TeamSpirit : SpellModule
    {
        Unit[] alies;
        public TeamSpirit(SpaceShip owner) : base(owner)
        {
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 120;
            activeTime = 20;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Allies);
            function.Add(SpellFunction.Buff);
            function.Add(SpellFunction.Attack);
        }
        public override void Enable()
        {
            base.Enable();
            alies = owner.GetAllies();
            List<Weapon> weapons = new List<Weapon>();
            weapons.AddRange(owner.transform.GetComponentsInChildren<Weapon>());
            owner.SpeedMultiplicator += 0.1f;
            owner.RotationSpeedMultiplicator += 0.2f;
            owner.ShieldRechargingMultiplacator += 0.5f;
            foreach (Unit x in alies)
            {
                weapons.AddRange(x.transform.GetComponentsInChildren<Weapon>());
                x.SpeedMultiplicator += 0.15f;
                x.RotationSpeedMultiplicator += 0.2f;
                x.ShieldRechargingMultiplacator += 0.5f;
            }
            foreach (Weapon x in weapons)
            {
                x.DamageMultiplacator += 0.1f;
                x.FirerateMultiplacator += 0.3f;
            }
        }

        protected override void Disable()
        {
            List<Weapon> weapons = new List<Weapon>();
            weapons.AddRange(owner.transform.GetComponentsInChildren<Weapon>());
            owner.SpeedMultiplicator -= 0.1f;
            owner.RotationSpeedMultiplicator -= 0.2f;
            owner.ShieldRechargingMultiplacator -= 0.5f;
            foreach (Unit x in alies)
            {
                weapons.AddRange(x.transform.GetComponentsInChildren<Weapon>());
                x.SpeedMultiplicator -= 0.15f;
                x.RotationSpeedMultiplicator -= 0.2f;
                x.ShieldRechargingMultiplacator -= 0.5f;
            }
            foreach (Weapon x in weapons)
            {
                x.DamageMultiplacator -= 0.1f;
                x.FirerateMultiplacator -= 0.3f;
            }
        }
    }
    public class ForcedTargetDesignator : SpellModule
    {
        List<SpaceShip> alies;
        public ForcedTargetDesignator(SpaceShip owner) : base(owner)
        {
            alies = new List<SpaceShip>();
            backCount = 0;
            type = SpellType.Activated;
            coolingTime = 60;
            activeTime = 15;
            state = SpellModuleState.Ready;
            function.Add(SpellFunction.Allies);
            function.Add(SpellFunction.Buff);
            function.Add(SpellFunction.Attack);
        }
        public override void Enable()
        {
            base.Enable();
            GlobalController global = GameObject.FindObjectOfType<GlobalController>();
            foreach (SpaceShip x in global.unitList)
            {
                if (x.Allies(owner.Team))
                    alies.Add(x);
            }
            List<Weapon> weapons = new List<Weapon>();
            owner.RotationSpeedMultiplicator += 0.3f;
            foreach (SpaceShip x in alies)
            {
                weapons.AddRange(x.transform.GetComponentsInChildren<Weapon>());
                x.RotationSpeedMultiplicator += 0.3f;
                x.RadarRangeMultiplacator += 0.5f;
                x.GetFireSupport(owner.CurrentTarget);
            }
            foreach (Weapon x in weapons)
            {
                x.RangeMultiplacator += 0.3f;
                x.DispersionMultiplicator += (-0.5f);
                x.APMultiplacator += 1.5f;
            }
        }

        protected override void Disable()
        {
            GlobalController global = GameObject.FindObjectOfType<GlobalController>();
            List<Weapon> weapons = new List<Weapon>();
            weapons.AddRange(owner.transform.GetComponentsInChildren<Weapon>());
            owner.RotationSpeedMultiplicator -= 0.3f;
            foreach (SpaceShip x in alies)
            {
                weapons.AddRange(x.transform.GetComponentsInChildren<Weapon>());
                x.RotationSpeedMultiplicator -= 0.3f;
                x.RadarRangeMultiplacator -= 0.5f;
            }
            foreach (Weapon x in weapons)
            {
                x.RangeMultiplacator -= 0.3f;
                x.DispersionMultiplicator -= (-0.5f);
                x.APMultiplacator -= 1.5f;
            }
        }
    }

    //public class ForsageImpact : IImpact
    //{
    //    public string Name { get { return "WarpImpact"; } }
    //    float ttl;
    //    SpaceShip owner;
    //    private float ownerSpeedPrev;
    //    public ForsageImpact(SpaceShip owner)
    //    {
    //        this.owner = owner;
    //        ownerSpeedPrev = owner.Speed;
    //        if (owner.HaveImpact(this.Name))
    //            ttl = 0;
    //        else
    //        {
    //            if (owner.HaveImpact("TrusterInhibitorImpact"))
    //                ttl = 0;
    //            else
    //            {
    //                ttl = 15;
    //                owner.Speed = owner.Speed * 2;
    //            }
    //        }
    //    }
    //    public void ActImpact()
    //    {
    //        if (ttl > 0)
    //            ttl -= Time.deltaTime;
    //        else CompleteImpact();
    //    }

    //    public void CompleteImpact()
    //    {
    //        owner.Speed = ownerSpeedPrev;
    //        owner.RemoveImpact(this);
    //    }
    //}
    //public class ShieldBoosterImpact : IImpact
    //{
    //    public string Name { get { return "ShieldBoosterImpact"; } }
    //    float ttl;
    //    SpaceShip owner;
    //    private float ownerShieldMaxPowerPrew;
    //    private float ownerShieldRechargingPrew;
    //    public ShieldBoosterImpact(SpaceShip owner, float time)
    //    {
    //        this.owner = owner;
    //        ownerShieldMaxPowerPrew = owner.ShieldCampacity;
    //        ownerShieldRechargingPrew = owner.ShieldRecharging;
    //        if (owner.HaveImpact(this.Name))
    //            ttl = 0;
    //        else
    //        {
    //            if (owner.HaveImpact("ShieldInhibitorImpact"))
    //                ttl = 0;
    //            else
    //            {
    //                ttl = time;
    //                owner.ShieldCampacity = owner.ShieldCampacity * 4;
    //                owner.ShieldRecharging = owner.ShieldRecharging * 2;
    //            }
    //        }
    //    }
    //    public void ActImpact()
    //    {
    //        if (ttl > 0)
    //            ttl -= Time.deltaTime;
    //        else CompleteImpact();
    //    }

    //    public void CompleteImpact()
    //    {
    //        owner.ShieldCampacity = ownerShieldMaxPowerPrew;
    //        owner.ShieldRecharging = ownerShieldRechargingPrew;
    //        owner.RemoveImpact(this);
    //    }
    //}
}