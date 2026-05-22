using UnityEngine;
using UnityEngine.InputSystem;

namespace KarlBanan.EventBus.Samples
{
    public sealed class EventBusSamplePublisher : MonoBehaviour
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private int healing = 5;

        public void RaiseDamageEvent()
        {
            EventBus.Raise(new SampleDamageEvent(damage, name));
        }

        public void RaiseHealEvent()
        {
            EventBus.Raise(new SampleHealEvent(healing));
        }

        private void Update()
        {
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                RaiseDamageEvent();
            }

            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                RaiseHealEvent();
            }
        }
    }
}