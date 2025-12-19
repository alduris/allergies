using MoreSlugcats;

namespace Allergies.Allergens
{
    internal class VoidAllergen : IAllergen
    {
        public string Name => "Void Essence";

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            if (trigger == TriggerType.Eat && thing is FireEgg)
            {
                return true;
            }
            return trigger == TriggerType.Void;
        }
    }
}
