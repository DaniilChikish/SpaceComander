using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class CannonShell : Round, IShell
    {
        ShellType type;
        public void StatUp(ShellType type)
        {
            this.type = type;
            switch (type)
                {
                case ShellType.Solid:
                    {
                        this.speed = 133.33f;
                        this.damage = 30f;
                        this.armorPiersing = 2f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 3f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.SolidBig:
                    {
                        this.speed = 133.33f;
                        this.damage = 30f * 4;
                        this.armorPiersing = 2f + 3;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 3f * 4;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.SolidAP:
                    {
                        this.speed = 88.89f;
                        this.damage = 27f;
                        this.armorPiersing = 4f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 4.5f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.Subcaliber:
                    {
                        this.speed = 177.78f;
                        this.damage = 22f;
                        this.armorPiersing = 5f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 2.25f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.SubcaliberBig:
                    {
                        this.speed = 177.78f;
                        this.damage = 22f * 4;
                        this.armorPiersing = 5f + 3;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 2.25f * 4;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.Camorous:
                    {
                        this.speed = 148.15f;
                        this.damage = 28.5f;
                        this.armorPiersing = 1.8f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 2.7f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.CamorousBig:
                    {
                        this.speed = 148.15f;
                        this.damage = 28.5f * 4;
                        this.armorPiersing = 1.8f + 3;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 2.7f * 4;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.CamorousAP:
                    {
                        this.speed = 98.77f;
                        this.damage = 24f;
                        this.armorPiersing = 3.6f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 4.05f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.Сumulative:
                    {
                        this.speed = 111.11f;
                        this.damage = 25f;
                        this.armorPiersing = 7f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 3.6f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.HightExplosive:
                    {
                        this.speed = 102.56f;
                        this.damage = 3f;
                        this.armorPiersing = 0.2f;
                        this.ttl = 2f;
                        this.GetComponent<Rigidbody>().mass = 3.9f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.Uranium:
                    {
                        this.speed = 40f;
                        this.damage = 39f;
                        this.armorPiersing = 1f;
                        this.ttl = 3f;
                        this.GetComponent<Rigidbody>().mass = 10f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
                case ShellType.Railgun:
                    {
                        this.speed = 300f;
                        this.damage = 250f;
                        this.armorPiersing = 6f;
                        this.ttl = 1.5f;
                        this.GetComponent<Rigidbody>().mass = 15f;
                        this.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
                        break;
                    }
            }
        }
        protected override void Explode()
        {
            switch (type)
            {
                //case ShellType.Solid:
                //    {
                //        break;
                //    }
                //case ShellType.SolidAP:
                //    {
                //        break;
                //    }
                //case ShellType.Subcaliber:
                //    {
                //        break;
                //    }
                case ShellType.Camorous:
                    {
                        Instantiate(FindObjectOfType<GlobalController>().ShellBlast, gameObject.transform.position, gameObject.transform.rotation);
                        break;
                    }
                case ShellType.CamorousAP:
                    {
                        Instantiate(FindObjectOfType<GlobalController>().ShellBlast, gameObject.transform.position + (gameObject.transform.forward * 0.5f), gameObject.transform.rotation);
                        break;
                    }
                case ShellType.Сumulative:
                    {
                        Instantiate(FindObjectOfType<GlobalController>().ShellBlast, gameObject.transform.position + gameObject.transform.forward, gameObject.transform.rotation);
                        break;
                    }
                case ShellType.HightExplosive:
                    {
                        GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
                        blast.GetComponent<Explosion>().StatUp(BlastType.ExplosiveShell);
                        break;
                    }
                case ShellType.Uranium:
                    {
                        Instantiate(FindObjectOfType<GlobalController>().ShellBlast, gameObject.transform.position, gameObject.transform.rotation);
                        break;
                    }
                //case ShellType.Railgun:
                //    {
                //        break;
                //    }
            }
            Destroy(this.gameObject);
        }
        protected override void OnCollisionEnter(Collision collision)
        {
            if (type == ShellType.Camorous || type == ShellType.CamorousAP || type == ShellType.Сumulative || type == ShellType.HightExplosive || type == ShellType.Uranium)
                Explode();
        }

    }
}
