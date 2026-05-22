using UnityEngine;

namespace KarlBanan.EventBus.Samples
{
    public sealed class EventBusSampleListener : MonoBehaviour
    {
        [SerializeField] private int health = 100;

        private void OnEnable()
        {
            EventBus.Subscribe<SampleDamageEvent>(OnDamage, EventPriority.NORMAL);
            EventBus.Subscribe<SampleHealEvent>(OnHeal, EventPriority.NORMAL);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SampleDamageEvent>(OnDamage);
            EventBus.Unsubscribe<SampleHealEvent>(OnHeal);
        }

        private void OnDamage(SampleDamageEvent gameEvent)
        {
            health -= gameEvent.Damage;
            Debug.Log($"{name} took {gameEvent.Damage} from {gameEvent.Source}. Health: {health}");
        }

        private void OnHeal(SampleHealEvent gameEvent)
        {
            health += gameEvent.Amount;
            Debug.Log($"{name} healed {gameEvent.Amount}. Health: {health}");
        }
    }
}