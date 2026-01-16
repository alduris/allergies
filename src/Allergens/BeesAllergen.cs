namespace Allergies.Allergens
{
    internal class BeesAllergen : IAllergen
    {
        public string Name => "Bee stings";

        public FSprite GetIcon()
        {
            return new FSprite(ItemSymbol.SpriteNameForItem(AbstractPhysicalObject.AbstractObjectType.SporePlant, 0))
            {
                color = ItemSymbol.ColorForItem(AbstractPhysicalObject.AbstractObjectType.SporePlant, 0),
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Impale && thing is SporePlant.AttachedBee;
        }
    }
}
