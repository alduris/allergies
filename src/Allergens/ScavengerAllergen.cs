namespace Allergies.Allergens
{
    internal class ScavengerAllergen : IAllergen
    {
        public string Name => "Scavenger Fur";

        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(CreatureTemplate.Type.Scavenger, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Airborne && thing is Scavenger;
        }
    }
}
