namespace Allergies.Reactions
{
    public class ReactionType(string name, bool register = false) : ExtEnum<ReactionType>(name, register)
    {
        public static readonly ReactionType Sneeze = new(nameof(Sneeze), true);
        //public static readonly ReactionType Hives = new(nameof(Hives), true); // todo: shaderwork for hives ig
        public static readonly ReactionType Spasm = new(nameof(Spasm), true);
        //public static readonly ReactionType Sickness = new(nameof(Sickness), true); // todo: hunter sickness-like
        public static readonly ReactionType Anaphylaxis = new(nameof(Anaphylaxis), true);
        public static readonly ReactionType BigHead = new(nameof(BigHead), true);
        public static readonly ReactionType Explode = new(nameof(Explode), true);
    }
}
