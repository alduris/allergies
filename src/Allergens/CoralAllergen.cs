namespace Allergies.Allergens
{
    internal class CoralAllergen : IAllergen
    {
        public string Name => "Coral Bits (touch)";

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
