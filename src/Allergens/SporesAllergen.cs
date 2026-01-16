using System.Collections.Generic;
using System.Linq;

namespace Allergies.Allergens
{
    public class SporesAllergen : IAllergen
    {
        public delegate bool SporesDelegate(PhysicalObject? thing, TriggerType trigger);
        private static readonly List<SporesDelegate> _customPredicates = [];
        public static event SporesDelegate ExtraConditions
        {
            add
            {
                _customPredicates.Add(value);
            }
            remove
            {
                _customPredicates.Remove(value);
            }
        }

        public string Name => "Spores";

        public FSprite GetIcon()
        {
            return new FSprite(ItemSymbol.SpriteNameForItem(AbstractPhysicalObject.AbstractObjectType.PuffBall, 0))
            {
                color = ItemSymbol.ColorForItem(AbstractPhysicalObject.AbstractObjectType.PuffBall, 0)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            if (_customPredicates.Any(x => x.Invoke(thing, trigger))) return true;
            return trigger == TriggerType.Spores;
        }
    }
}
