using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    class Base : MonoBehaviour
    {
        public Army team;
        public RepairDock[] docks;
        public Queue<SpaceShip> repairQuenue;
        public void Start()
        {
            docks = this.transform.GetComponentsInChildren<RepairDock>();
            repairQuenue = new Queue<SpaceShip>();
        }
        public void Update()
        {
            if (repairQuenue.Count>0)
            {
                foreach (RepairDock x in docks)
                {
                    if (x.IsFree)
                    {
                        repairQuenue.Dequeue().SendTo(x.transform.position);
                        break;
                    }
                }
            }
        }
        public Vector3 GetInQueue(SpaceShip sender)
        {
            //foreach (RepairDock x in docks)
            //{
            //    if (x.IsFree)
            //        return x.transform.position;
            //}
            repairQuenue.Enqueue(sender);
            return this.transform.position + this.transform.right * repairQuenue.Count * 30;
        }
    }
}
