using RWCustom;

namespace Allergies.Allergens
{
    internal class SeedAllergen : IAllergen
    {
        public string Name => Custom.rainWorld.inGameTranslator.Translate("objecttype-SeedCob");

        public FSprite GetIcon()
        {
            return new FSprite(ItemSymbol.SpriteNameForItem(AbstractPhysicalObject.AbstractObjectType.SeedCob, 0))
            {
                color = ItemSymbol.ColorForItem(AbstractPhysicalObject.AbstractObjectType.SeedCob, 0)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Eat && (thing is SeedCob || (ModManager.DLCShared && thing is SlimeMold sm && sm.abstractPhysicalObject.type == DLCSharedEnums.AbstractObjectType.Seed));
        }
    }
}
