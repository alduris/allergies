using System;

namespace Allergies.Allergens
{
    public interface IAllergen : IEquatable<IAllergen>
    {
        public string Name { get; }

        public FSprite GetIcon();

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger);
    }
}
