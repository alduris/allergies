using Allergies.Triggers;

namespace Allergies.Allergens
{
    internal class LizardLickAllergen : IAllergen
    {
        public string Name => "Lizard Spit";

        public bool Equals(IAllergen other)
        {
            return other is LizardLickAllergen;
        }

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Lick && thing is Lizard;
        }
    }
}
