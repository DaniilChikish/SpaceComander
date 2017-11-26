using UnityEngine;

namespace SpaceCommander.Scenarios
{
    class OnKeyDownOrderAssert : OrderAssert
    {
        public OrderAssert previous;
        [SerializeField]
        KeyCode key;
        private void Update()
        {
            if (
                (Input.GetKeyDown(key) || (key == KeyCode.None && Input.anyKeyDown))
                && (previous == null || (previous != null && previous.State == OrderAccertState.Complete))
                )
                if (methodToCall == Method.Pass)
                    State = OrderAccertState.Complete;
                else State = OrderAccertState.Fail;
        }
    }
}
