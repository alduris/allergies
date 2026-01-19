using System.Linq;
using MoreSlugcats;

namespace Allergies.Allergens
{
    internal class ClothAllergen : IAllergen
    {
        public string Name => "Cloth (touch)";

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            if (trigger != TriggerType.Touch) return false;

            if (thing is MoonCloak) return true;
            if (thing is Oracle oracle && oracle.graphicsModule is OracleGraphics oracleGraphics)
            {
                var gowns = oracleGraphics.gowns;
                return gowns != null && gowns.Any(x => x is not null);
            }
            if (thing is Player otherPlayer && otherPlayer.graphicsModule is PlayerGraphics playerGraphics)
            {
                var gown = playerGraphics.gown;
                return gown != null && gown.visible;
            }
            return false;
        }
    }
}
