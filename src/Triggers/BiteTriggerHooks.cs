using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Allergies.Triggers
{
    internal static class BiteTriggerHooks
    {
        public static void Apply()
        {
            try
            {
                On.Player.Grabbed += Player_Grabbed;
                IL.BigSpider.Collide += BigSpider_Collide;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }


        private static void Player_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            // Assume it is a bite
            orig(self, grasp);
            AllergySystem.TriggerAllergy(self, grasp.grabber, TriggerType.Bite);
        }

        private static void BigSpider_Collide(ILContext il)
        {
            var c = new ILCursor(il);

            // Try go to where it attacks other creature
            if (c.TryGotoNext(MoveType.After, x => x.MatchCallvirt<Creature>(nameof(Creature.Violence))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((BigSpider self, PhysicalObject otherObject) =>
                {
                    if (otherObject is Player player)
                    {
                        AllergySystem.TriggerAllergy(player, self, TriggerType.Bite);
                    }
                });
            }
            else
            {
                Plugin.Logger.LogError("IL.BigSpider.Collide in BiteTriggerHooks failed to match target!");
            }
        }
    }
}
