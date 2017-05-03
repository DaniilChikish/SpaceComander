using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    class RepairDock : MonoBehaviour
    {
        public bool IsFree;
        protected void Start()
        {
            IsFree = true;
        }
        protected void OnTriggerEnter(Collider other)
        {
            if (/*IsFree == true && */other.gameObject.tag == "Unit")
            {
                SpaceShip x = other.GetComponent<SpaceShip>();
                if (x.Health != x.MaxHealth)
                    x.Impacts.Add(new FullRepairing(x, 0));
            }
        }
        protected void OnTriggerStay(Collider other)
        {
            IsFree = false;
        }
        protected void OnTriggerExit(Collider other)
        {
            if (IsFree == false)
                IsFree = true;
        }
    }
    public class FullRepairing : IImpact
    {
        public string Name { get { return "FullRepairing"; } }
        private float ttl;
        private float repairSpeed;
        private bool ownerMovementAiEnabledPrev;
        private SpaceShip owner;
        public FullRepairing(SpaceShip owner, float time)
        {
            this.owner = owner;

            switch (owner.Type)
            {
                case UnitClass.Bomber:
                    {
                        repairSpeed = 5;
                        break;
                    }
                case UnitClass.Command:
                    {
                        repairSpeed = 7;
                        break;
                    }
                case UnitClass.ECM:
                    {
                        repairSpeed = 2;
                        break;
                    }
                case UnitClass.Figther:
                    {
                        repairSpeed = 4;
                        break;
                    }
                case UnitClass.Guard_Corvette:
                    {
                        repairSpeed = 10;
                        break;
                    }
                case UnitClass.LR_Corvette:
                    {
                        repairSpeed = 10;
                        break;
                    }
                case UnitClass.Recon:
                    {
                        repairSpeed = 2;
                        break;
                    }
                case UnitClass.Scout:
                    {
                        repairSpeed = 3;
                        break;
                    }
                case UnitClass.Support_Corvette:
                    {
                        repairSpeed = 10;
                        break;
                    }
            }
            if (owner.Impacts.Exists(x => x.Name == this.Name))
                repairSpeed = 0;
            else
            {
                ownerMovementAiEnabledPrev = owner.movementAiEnabled;
                owner.movementAiEnabled = false;
                Debug.Log(owner.name + " full repaering start");
            }
        }
        public void ActImpact()
        {
            if (owner.Health < owner.MaxHealth)
                owner.Health += repairSpeed * Time.deltaTime;
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            owner.movementAiEnabled = ownerMovementAiEnabledPrev;
            Debug.Log(owner.name + " full repaering complete");
            owner.Impacts.Remove(this);
            owner.ResetStats();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
