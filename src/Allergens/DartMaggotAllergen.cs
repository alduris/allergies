namespace Allergies.Allergens
{
    internal class DartMaggotAllergen : IAllergen
    {
        public string Name => "Dart Maggot";

        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(CreatureTemplate.Type.SpitterSpider, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Impale && thing is DartMaggot;
        }
    }
}
