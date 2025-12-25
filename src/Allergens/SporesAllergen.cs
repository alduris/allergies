namespace Allergies.Allergens
{
    internal class SporesAllergen : IAllergen
    {
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
            return trigger == TriggerType.Spores;
        }
    }
}
