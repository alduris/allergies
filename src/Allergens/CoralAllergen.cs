using Allergies.Triggers;

namespace Allergies.Allergens
{
    internal class CoralAllergen : IAllergen
    {
        public string Name => "Coral Bits";

        public bool Equals(IAllergen other)
        {
            return other is CoralAllergen;
        }

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Coral;
        }
    }
}
