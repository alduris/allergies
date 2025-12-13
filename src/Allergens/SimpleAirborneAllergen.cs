namespace Allergies.Allergens
{
    public abstract class SimpleAirborneAllergen<T> : IAllergen where T : PhysicalObject
    {
        public bool Equals(IAllergen other)
        {
            return other is SimpleAirborneAllergen<T>;
        }

        public abstract string Name { get; }
        public abstract FSprite GetIcon();

        public virtual bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Airborne && thing is T;
        }
    }
}