using UnityEngine;

namespace SpaceCommander.Mechanics
{
    public abstract class EventChecker : MonoBehaviour
    {
        private bool occured;
        public bool Occured {  get { return occured; }  protected set { occured = value; } }
        public EventChecker previous; // if not null, event checking after previous
        public abstract bool Check();
    }
}
