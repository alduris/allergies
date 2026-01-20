namespace Allergies.Allergens
{
    internal class MyceliaAllergen : IAllergen
    {
        public string Name => "Mycelia (airborne)";

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Airborne && (thing is IOwnMycelia || (thing is PhysicalObject { graphicsModule: IOwnMycelia }));
        }
    }
}
