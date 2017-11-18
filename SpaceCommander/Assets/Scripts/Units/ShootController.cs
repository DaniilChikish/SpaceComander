using SpaceCommander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Units
{
    public class ShootController : IGunner
    {
        private IWeapon[][] weapons;
        protected Unit target;
        public Transform ownerTransform;
        protected float targetLockdownCount;
        protected float[] synchWeapons;
        protected int[] indexWeapons;
        public ShootController(){ }
        public ShootController(Transform body)
        {
            this.ownerTransform = body;
            List<IWeapon[]> buffer = new List<IWeapon[]>();
            for (int i = 0; i < body.transform.childCount; i++)
            {
                IWeapon[] buffer2 = body.transform.GetChild(i).GetComponentsInChildren<IWeapon>();
                if (buffer2.Length > 0)
                {
                    buffer.Add(buffer2);
                }
            }

            Weapon = buffer.ToArray();
            synchWeapons = new float[Weapon.Length];
            indexWeapons = new int[Weapon.Length];
        }
        public Unit Target { get { return target; } }
        public IWeapon[][] Weapon { get { return weapons; } protected set { weapons = value; } }
        public virtual bool ShootHim(int slot)
        {
            bool output = false;
            if (synchWeapons[slot] <= 0)
            {
                float angel;
                if (target != null) angel = Vector3.Angle(target.transform.position - ownerTransform.position, ownerTransform.forward);
                else angel = 0;
                if (angel < Weapon[slot][0].Dispersion * 5 || angel < Weapon[slot][0].AimAngle * 1.1f)
                {
                    if (indexWeapons[slot] >= Weapon[slot].Length)
                        indexWeapons[slot] = 0;
                    if (Weapon[slot][indexWeapons[slot]].IsReady)
                    {
                        synchWeapons[slot] = (60f / this.Weapon[slot][0].Firerate) / this.Weapon[slot].Length;
                        output = Weapon[slot][indexWeapons[slot]].Fire();
                        indexWeapons[slot]++;
                    }
                    else indexWeapons[slot]++;
                }
            }
            return output;
        }
        public virtual bool Volley(int slot)
        {
            bool output = false;
            if (synchWeapons[slot] <= 0)
            {
                float angel;
                if (target != null) angel = Vector3.Angle(target.transform.position - ownerTransform.position, ownerTransform.forward);
                else angel = 0;
                if (angel < Weapon[slot][0].Dispersion * 5 || angel < Weapon[slot][0].AimAngle * 1.1f)
                {
                    synchWeapons[slot] = (60f / this.Weapon[slot][0].Firerate) / this.Weapon[slot].Length;
                    for (int i = 0; i < Weapon[slot].Length; i++)
                        if (Weapon[slot][i].IsReady)
                        {
                            output = Weapon[slot][i].Fire();
                        }
                }
            }
            return output;
        }
        public virtual void Update()
        {
            for (int slot = 0; slot < weapons.Length; slot++)
                if (synchWeapons[slot] > 0)
                    synchWeapons[slot] -= Time.deltaTime;
            if (targetLockdownCount > 0)
                targetLockdownCount -= Time.deltaTime;
        }
        public bool SeeTarget()
        {
            if (target != null)
            {
                RaycastHit hit;
                Physics.Raycast(ownerTransform.position, target.transform.position - ownerTransform.position, out hit, 100000, (1 << 0 | 1 << 8)); //1 - default layer, 9 - terrain layer -1
                return (hit.transform == target.transform);
            }
            else return false;
        }
        public bool AimOnTarget()
        {
            if (target != null)
                return (Vector3.Angle(ownerTransform.forward, target.transform.position - ownerTransform.position) < 5f);
            else return false;
        }
        public bool TargetInRange(int slot)
        {
            return (target != null && Vector3.Distance(target.transform.position, ownerTransform.position) < weapons[slot][0].Range);
        }
        public bool CanShoot(int slot)
        {
            if (indexWeapons[slot] >= weapons[slot].Length)
                indexWeapons[slot] = 0;
            return (weapons[slot][indexWeapons[slot]].IsReady || weapons[slot][indexWeapons[slot]].BackCounter <= 2);
        }
        public bool NeedAim()
        {
            for (int i = 0; i < Weapon.Length; i++)
            {
                if (TargetInRange(i) && CanShoot(i))
                    return true;
            }
            return false;
        }
        public bool SetAim(Unit target, bool immediately, float lockdown)
        {
            if (this.target == null || targetLockdownCount < 0 || immediately)
            {
                this.target = target;
                targetLockdownCount = lockdown;
                for (int j = 0; j < weapons.Length; j++)
                    for (int i = 0; i < weapons[j].Length; i++)
                    {
                        weapons[j][i].Target = target;
                    }
                //Debug.Log("set target - " + Target.transform.position);
                //Debug.Log("set aim - " + oldTargetPosition);
                return true;
            }
            else return false;
        }
        public bool ResetAim()
        {
            this.target = null;
            for (int j = 0; j < weapons.Length; j++)
                for (int i = 0; i < weapons[j].Length; i++)
                {
                    weapons[j][i].Target = null;
                }
            return true;
        }
        public void ReloadWeapons()
        {
            for (int i = 0; i < weapons.Length; i++)
                for (int j = 0; j < weapons[i].Length; j++)
                {
                    weapons[i][j].Reset();
                }
        }
        public float GetRange(int slot)
        {
            return weapons[slot][0].Range;
        }
    }
    public class SpaceShipShootController : ShootController
    {
        private IShield shield;
        public SpaceShipShootController(SpaceShip body) : base(body.GetTransform())
        {
            shield = body.GetShieldRef;
        }
        public override bool ShootHim(int slot)
        {
            if (synchWeapons[slot] <= 0)
            {
                float angel;
                if (target != null) angel = Vector3.Angle(target.transform.position - ownerTransform.position, ownerTransform.forward);
                else angel = 0;
                if (angel < Weapon[slot][0].Dispersion * 5 || angel < Weapon[slot][0].AimAngle * 1.1f)
                {
                    if (indexWeapons[slot] >= Weapon[slot].Length)
                        indexWeapons[slot] = 0;
                    if (Weapon[slot][indexWeapons[slot]].IsReady)
                    {
                        shield.Blink(Weapon[slot][indexWeapons[slot]].ShildBlink);
                        synchWeapons[slot] = (60f / this.Weapon[slot][0].Firerate) / this.Weapon[slot].Length;
                        bool output = Weapon[slot][indexWeapons[slot]].Fire();
                        indexWeapons[slot]++;
                        return output;
                    }
                    else indexWeapons[slot]++;
                }
            }
            return false;
        }
        public override bool Volley(int slot)//relative cooldown indexWeapon, ignore angel;
        {
            if (indexWeapons[slot] >= Weapon[slot].Length)
                indexWeapons[slot] = 0;
            if (synchWeapons[slot] <= 0 && Weapon[slot][indexWeapons[slot]].IsReady)
            {
                int i = 0;
                shield.Blink(Weapon[slot][indexWeapons[slot]].ShildBlink);
                synchWeapons[slot] = 60f / this.Weapon[slot][0].Firerate;
                indexWeapons[slot]++;
                for (i = 0; i < Weapon[slot].Length; i++)
                {
                    Weapon[slot][i].Fire();
                }
                return true;
            }
            else return false;
        }
    }
    public class TurretShootController : ShootController
    {
        private float rotationSpeed;
        public TurretShootController(SpaceCommander.Units.SpaceTurret body) : base(body.GetTransform())
        {
            rotationSpeed = body.RotationSpeed;
        }
        public override void Update()
        {
            for (int slot = 0; slot < Weapon.Length; slot++)
                if (synchWeapons[slot] > 0)
                    synchWeapons[slot] -= Time.deltaTime;
            if (targetLockdownCount > 0)
                targetLockdownCount -= Time.deltaTime;
            if (SeeTarget() && TargetInRange(0) && CanShoot(0))
                LookOn(Target.transform.position - ownerTransform.position);
        }
        private void LookOn(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, new Vector3(0, 1, 0));
                ownerTransform.rotation = Quaternion.RotateTowards(ownerTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
