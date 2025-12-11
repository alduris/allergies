using System;

namespace Allergies.Allergens
{
    public interface IAllergen : IEquatable<IAllergen>
    {
        public string Name { get; }

        public FSprite GetIcon();

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger);

        public enum TriggerGroup
        {
            Eat = 1,
            Contact = 2,
            Airborne = 4
        }
    }
}
