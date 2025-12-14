namespace Allergies.Allergens
{
    public class TriggerType(string name, bool register = false) : ExtEnum<TriggerType>(name, register)
    {
        public static readonly TriggerType Eat = new(nameof(Eat), true);
        public static readonly TriggerType Grab = new(nameof(Grab), true);
        public static readonly TriggerType Lick = new(nameof(Lick), true);
        public static readonly TriggerType Void = new(nameof(Void), true);
        public static readonly TriggerType Touch = new(nameof(Touch), true);
        public static readonly TriggerType Coral = new(nameof(Coral), true);
        public static readonly TriggerType Impale = new(nameof(Impale), true);
        public static readonly TriggerType Spores = new(nameof(Spores), true);
        public static readonly TriggerType Airborne = new(nameof(Airborne), true);
        public static readonly TriggerType Corruption = new(nameof(Corruption), true);
    }
}
