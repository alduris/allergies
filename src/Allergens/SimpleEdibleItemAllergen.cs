using RWCustom;

namespace Allergies.Allergens
{
    public class SimpleEdibleItemAllergen<T>(AbstractPhysicalObject.AbstractObjectType itemType, int iconIntData = 0) : EdibleAllergen<T> where T : PhysicalObject
    {
        private readonly AbstractPhysicalObject.AbstractObjectType itemType = itemType;
        private readonly int iconIntData = iconIntData;

        public override string Name => Custom.rainWorld.inGameTranslator.TryTranslate($"objecttype-{itemType.value}", out string translated) ? translated : itemType.value;

        public override FSprite GetIcon()
        {
            return new FSprite(ItemSymbol.SpriteNameForItem(itemType, iconIntData))
            {
                color = ItemSymbol.ColorForItem(itemType, iconIntData),
            };
        }
    }
}
