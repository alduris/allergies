using System.Linq;
using MoreSlugcats;

namespace Allergies.Allergens
{
    internal class ClothAirborneAllergen : IAllergen
    {
        public string Name => "Cloth";

        public bool Equals(IAllergen other)
        {
            return other is ClothAirborneAllergen;
        }

        public FSprite GetIcon()
        {
            return null!;
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            if (trigger != TriggerType.Airborne) return false;

            if (thing is MoonCloak) return true;
            if (thing is Oracle oracle)
            {
                var gowns = (oracle.graphicsModule as OracleGraphics)!.gowns;
                return gowns != null && gowns.Any(x => x is not null);
            }
            if (thing is Player otherPlayer)
            {
                var gown = (otherPlayer.graphicsModule as PlayerGraphics)!.gown;
                return gown != null && gown.visible;
            }
            return false;
        }
    }
}
