namespace Allergies.Triggers
{
    public class TriggerType(string name, bool register = false) : ExtEnum<TriggerType>(name, register)
    {
        public static TriggerType Eat = new(nameof(Eat), true);
        public static TriggerType Touch = new(nameof(Touch), true);
        public static TriggerType Impale = new(nameof(Impale), true);
        public static TriggerType Spores = new(nameof(Spores), true);
    }
}
