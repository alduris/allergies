using System;
using Allergies.Allergens;

namespace Allergies.Triggers
{
    internal static class ImpaleTriggerHooks
    {
        public static void Apply()
        {
            try
            {
                On.DartMaggot.ChangeMode += DartMaggot_ChangeMode;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        private static void DartMaggot_ChangeMode(On.DartMaggot.orig_ChangeMode orig, DartMaggot self, DartMaggot.Mode newMode)
        {
            orig(self, newMode);
            if (newMode == DartMaggot.Mode.StuckInChunk && self.stuckInChunk?.owner is Player player)
            {
                AllergySystem.TriggerAllergy(player, self, TriggerType.Impale);
            }
        }
    }
}
