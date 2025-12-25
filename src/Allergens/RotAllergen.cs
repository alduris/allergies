using Watcher;

namespace Allergies.Allergens
{
    internal class RotAllergen : IAllergen
    {
        public string Name => "Rot (touch)";
        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(CreatureTemplate.Type.DaddyLongLegs, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            if (trigger == TriggerType.Eat || trigger == TriggerType.Touch || trigger == TriggerType.Grab || trigger == TriggerType.Lick)
            {
                return thing is DaddyLongLegs or Lizard { rotModule: not null } or Rattler or Prince or PrinceBulb;
            }

            return trigger == TriggerType.Corruption;
        }
    }
}