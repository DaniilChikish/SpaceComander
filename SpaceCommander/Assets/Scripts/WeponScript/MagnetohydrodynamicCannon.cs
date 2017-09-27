using DeusUtility.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class MagnetohydrodynamicCannon : EnergyWeapon
    {
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.MagnetohydrodynamicGun;
        }
        protected override void UpdateLocal()
        {
            firerate = (200 + 800 * (heat / maxHeat)) * (1 + FirerateMultiplacator);
        }
        protected override void Shoot(Transform target)
        {
            float speed = roundSpeed;
            float damage = 10f;
            float armorPiersing = 9f;
            float mass = 20f;
            heat += 3;
            float localDisp = (Dispersion * heat / 50);
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * localDisp);
            else
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * -localDisp);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * localDisp);
            else
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * -localDisp);
            GameObject shell = Instantiate(Global.MagnetoShell, gameObject.transform.position, direction);
            shell.GetComponent<IShell>().StatUp(speed * (1 + RoundspeedMultiplacator), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass, false, null);
        }
    }
}


