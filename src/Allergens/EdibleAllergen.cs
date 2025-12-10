using System.Runtime.CompilerServices;
using Allergies.Triggers;

namespace Allergies.Allergens
{
    public abstract class EdibleAllergen<T> : IAllergen where T : PhysicalObject
    {
        public abstract string Name { get; }

        public bool Equals(IAllergen other)
        {
            return other is EdibleAllergen<T>;
        }

        public abstract FSprite GetIcon();

        public virtual bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Eat && thing is T;
        }
    }
}
