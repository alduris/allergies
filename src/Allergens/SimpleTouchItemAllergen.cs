using RWCustom;

namespace Allergies.Allergens
{
    public class SimpleTouchItemAllergen<T>(AbstractPhysicalObject.AbstractObjectType itemType, int iconIntData = 0) : SimpleTouchAllergen<T> where T : PhysicalObject
    {
        private readonly AbstractPhysicalObject.AbstractObjectType _itemType = itemType;
        private readonly int _iconIntData = iconIntData;

        public override string Name =>
            (Custom.rainWorld.inGameTranslator.TryTranslate($"objecttype-{_itemType.value}", out string translated)
                ? translated
                : _itemType.value) + " (touch)";

        public override FSprite GetIcon()
        {
            return new FSprite(ItemSymbol.SpriteNameForItem(_itemType, _iconIntData))
            {
                color = ItemSymbol.ColorForItem(_itemType, _iconIntData),
            };
        }
    }
}