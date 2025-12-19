namespace Allergies.Allergens
{
    public abstract class SimpleTouchAllergen<T> : IAllergen where T : PhysicalObject
    {
        public abstract string Name { get; }
        public abstract FSprite GetIcon();

        public virtual bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Touch && thing is T;
        }
    }
}