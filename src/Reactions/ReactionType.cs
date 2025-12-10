namespace Allergies.Reactions
{
    public class ReactionType(string name, bool register = false) : ExtEnum<ReactionType>(name, register)
    {
        // Reaction ideas:
        // - sneezing
        // - hives
        // - spasming/stunning (not severe)
        // - spasming/stunning (hunter sickness-like?)
        // - death (not immediate)

        public static ReactionType Sneezes = new(nameof(Sneezes), true);
        //public static ReactionType Hives = new(nameof(Hives), true);
        public static ReactionType Spasm = new(nameof(Spasm), true);
        public static ReactionType Sickness = new(nameof(Sickness), true);
        public static ReactionType Anaphylaxis = new(nameof(Anaphylaxis), true);
        public static ReactionType BigHead = new(nameof(BigHead), true);
        public static ReactionType Explode = new(nameof(Explode), true);
    }
}
