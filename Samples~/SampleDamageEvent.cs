using KarlBanan.EventBus;

namespace KarlBanan.EventBus.Samples
{
    public readonly struct SampleDamageEvent : IGameEvent
    {
        public readonly int Damage;
        public readonly string Source;

        public SampleDamageEvent(int damage, string source)
        {
            Damage = damage;
            Source = source;
        }
    }

    public readonly struct SampleHealEvent : IGameEvent
    {
        public readonly int Amount;

        public SampleHealEvent(int amount)
        {
            Amount = amount;
        }
    }
}