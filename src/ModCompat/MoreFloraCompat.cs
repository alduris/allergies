using Allergies.Allergens;
using MoreFlora;

namespace Allergies.ModCompat
{
    internal static class MoreFloraCompat
    {
        private static bool _active = false;
        public static void Register()
        {
            if (_active) return;
            _active = true;

            AllergySystem.Register(new SimpleEdibleItemAllergen<WalnutPiece>(AbstractObjectType.WalnutPiece));

            VoidAllergen.ExtraConditions += VoidAllergen_ExtraConditions; // for depths fruit
        }

        private static bool VoidAllergen_ExtraConditions(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Eat && thing is DangleFruit && thing.abstractPhysicalObject.type == AbstractObjectType.DepthsFruit;
        }
    }
}
