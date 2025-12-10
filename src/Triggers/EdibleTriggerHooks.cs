using System;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace Allergies.Triggers
{
    internal static class EdibleTriggerHooks
    {
        public static void Apply()
        {
            try
            {
                On.Player.ObjectEaten += Player_ObjectEaten;
                On.Player.EatMeatUpdate += Player_EatMeatUpdate;
                On.Player.MaulingUpdate += Player_MaulingUpdate;
                IL.Player.Update += Player_Update;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
        {
            orig(self, edible);
            if (edible is PhysicalObject physicalObject)
            {
                AllergySystem.TriggerAllergy(self, physicalObject, TriggerType.Eat);
            }
        }

        private static void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
        {
            orig(self, graspIndex);
            if (self.grasps[graspIndex] == null || self.grasps[graspIndex].grabbed is not Creature) return;
            if (self.eatMeat == 20)
            {
                AllergySystem.TriggerAllergy(self, self.grasps[graspIndex].grabbed, TriggerType.Eat);
            }
        }

        private static void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
        {
            orig(self, graspIndex);
            if (self.maulTimer == 15 && self.grasps[graspIndex]?.grabbed is not null)
            {
                AllergySystem.TriggerAllergy(self, self.grasps[graspIndex].grabbed, TriggerType.Eat);
            }
        }

        private static void Player_Update(ILContext il)
        {
            var c = new ILCursor(il);
            
            // Go to where the game does external food source logic and emit a delegate to:
            //  1. Actually find the food source
            //  2. Trigger allergic reaction if necessary
            if (c.TryGotoNext(x => x.MatchLdfld<Player>(nameof(Player.eatExternalFoodSourceCounter)))
                && c.TryGotoNext(MoveType.After, x => x.MatchStfld<Player>(nameof(Player.dontEatExternalFoodSourceCounter))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate((Player self) =>
                {
                    PhysicalObject? triggerObj = null;
                    if (self.handOnExternalFoodSource is Vector2 foodSourcePos)
                    {
                        triggerObj = self.room.physicalObjects.SelectMany(x => x)
                                                              .SelectMany(x => x.bodyChunks)
                                                              .MinBy(x => Vector2.Distance(x.pos, foodSourcePos))
                                                              .owner;
                    }
                    AllergySystem.TriggerAllergy(self, triggerObj, TriggerType.Eat);
                });
            }
            else
            {
                Plugin.Logger.LogError("IL.Player.Update in EdibleTriggerHooks failed to match target!");
            }
        }
    }
}
