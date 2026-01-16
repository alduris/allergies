using System;
using System.Collections.Generic;
using System.Linq;
using MoreSlugcats;

namespace Allergies.Allergens
{
    internal class VoidAllergen : IAllergen
    {
        public delegate bool VoidDelegate(PhysicalObject? thing, TriggerType trigger);
        private static readonly List<VoidDelegate> _customPredicates = [];
        public static event VoidDelegate ExtraConditions
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

        public string Name => "Void Essence";

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            if (_customPredicates.Any(x => x.Invoke(thing, trigger))) return true;

            if (trigger == TriggerType.Eat && thing is FireEgg)
            {
                return true;
            }
            if (trigger == TriggerType.Touch || trigger == TriggerType.Lick || trigger == TriggerType.Grab)
            {
                return thing is TempleGuard || (thing is Creature critter && critter.abstractCreature.voidCreature);
            }
            return trigger == TriggerType.Void;
        }
    }
}
