using Allergies.Allergens;
using LBMergedMods.Creatures;
using LBMergedMods.Enums;
using LBMergedMods.Items;

namespace Allergies.ModCompat.Allergens
{
    internal class LBWaterBlobAllergen : IAllergen
    {
        public string Name => "Water Blob";
        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(CreatureTemplateType.WaterBlob, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return (trigger == TriggerType.Touch && thing is WaterBlob) ||
                   (trigger == TriggerType.Eat && thing is BlobPiece);
        }
    }
}