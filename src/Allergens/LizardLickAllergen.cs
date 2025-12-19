namespace Allergies.Allergens
{
    internal class LizardLickAllergen : IAllergen
    {
        public string Name => "Lizard Spit";

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
