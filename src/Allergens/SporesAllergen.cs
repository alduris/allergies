using RWCustom;

namespace Allergies.Allergens
{
    internal class SporesAllergen : IAllergen
    {
        public string Name => Custom.rainWorld.inGameTranslator.Translate("objecttype-PuffBall");

        public bool Equals(IAllergen other)
        {
            return other is SporesAllergen;
        }

        public FSprite GetIcon()
        {
            return new FSprite(ItemSymbol.SpriteNameForItem(AbstractPhysicalObject.AbstractObjectType.PuffBall, 0))
            {
                color = ItemSymbol.ColorForItem(AbstractPhysicalObject.AbstractObjectType.PuffBall, 0)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Spores;
        }
    }
}
