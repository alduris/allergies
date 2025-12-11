namespace Allergies.Allergens
{
    public class TriggerType(string name, bool register = false) : ExtEnum<TriggerType>(name, register)
    {
        public static TriggerType Eat = new(nameof(Eat), true);
        public static TriggerType Bite = new(nameof(Bite), true);
        public static TriggerType Lick = new(nameof(Lick), true);
        public static TriggerType Void = new(nameof(Void), true);
        public static TriggerType Touch = new(nameof(Touch), true);
        public static TriggerType Coral = new(nameof(Coral), true);
        public static TriggerType Impale = new(nameof(Impale), true);
        public static TriggerType Spores = new(nameof(Spores), true);
        public static TriggerType Airborne = new(nameof(Airborne), true);
    }
}
