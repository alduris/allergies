using System;
using Allergies.Allergens;
using CoralBrain;
using UnityEngine;

namespace Allergies.Triggers
{
    internal static class TouchTriggerHooks
    {
        public static void Apply()
        {
            try
            {
                On.Player.SlugcatGrab += Player_OnSlugcatGrab;
                On.Player.Collide += Player_Collide;
                On.Player.CollideWithCoralCircuitBit += Player_CollideWithCoralCircuitBit;
                On.ClimbableVinesSystem.VineBeingClimbedOn += ClimbableVinesSystem_VineBeingClimbedOn;
                On.LizardTongue.Update += LizardTongue_Update;
                On.VoidSea.VoidSeaScene.UpdatePlayerInVoidSea += VoidSeaScene_UpdatePlayerInVoidSea;
                On.DaddyCorruption.Update += DaddyCorruption_Update;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        private static void Player_OnSlugcatGrab(On.Player.orig_SlugcatGrab orig, Player self, PhysicalObject obj, int graspUsed)
        {
            orig(self, obj, graspUsed);
            AllergySystem.TriggerAllergy(self, obj, TriggerType.Touch);
        }

        private static void Player_Collide(On.Player.orig_Collide orig, Player self, PhysicalObject? otherObject, int myChunk, int otherChunk)
        {
            orig(self, otherObject, myChunk, otherChunk);
            if (otherObject is not null)
            {
                AllergySystem.TriggerAllergy(self, otherObject, TriggerType.Touch);
            }
        }

        private static void Player_CollideWithCoralCircuitBit(On.Player.orig_CollideWithCoralCircuitBit orig, Player self, int chunk, CoralCircuit.CircuitBit bit, float overLapFac)
        {
            orig(self, chunk, bit, overLapFac);
            AllergySystem.TriggerAllergy(self, null, TriggerType.Coral);
        }

        private static void ClimbableVinesSystem_VineBeingClimbedOn(On.ClimbableVinesSystem.orig_VineBeingClimbedOn orig, ClimbableVinesSystem self, ClimbableVinesSystem.VinePosition vPos, Creature crit)
        {
            if (crit is Player player)
            {
                if (vPos.vine is PhysicalObject physicalObject)
                {
                    AllergySystem.TriggerAllergy(player, physicalObject, TriggerType.Touch);
                }
                else if (vPos.vine is CoralStem)
                {
                    AllergySystem.TriggerAllergy(player, null, TriggerType.Coral);
                }
                else if (vPos.vine is DaddyCorruption.ClimbableCorruptionTube)
                {
                    AllergySystem.TriggerAllergy(player, null, TriggerType.Corruption);
                }
            }
        }

        private static void LizardTongue_Update(On.LizardTongue.orig_Update orig, LizardTongue self)
        {
            BodyChunk lastAttached = self.attached;
            orig(self);
            if (lastAttached is null && self.attached is { owner: Player player })
            {
                AllergySystem.TriggerAllergy(player, self.lizard, TriggerType.Lick);
            }
        }

        private static void VoidSeaScene_UpdatePlayerInVoidSea(On.VoidSea.VoidSeaScene.orig_UpdatePlayerInVoidSea orig, VoidSea.VoidSeaScene self, Player voidSeaPlayer)
        {
            orig(self, voidSeaPlayer);
            AllergySystem.TriggerAllergy(voidSeaPlayer, null, TriggerType.Void);
        }

        private static void DaddyCorruption_Update(On.DaddyCorruption.orig_Update orig, DaddyCorruption self, bool eu)
        {
            orig(self, eu);
            foreach (AbstractCreature abstrPlayer in self.room.game.Players)
            {
                if (abstrPlayer.realizedCreature is Player player && player.room == self.room)
                {
                    foreach (DaddyCorruption.Bulb bulb in self.allBulbs)
                    {
                        if (Vector2.Distance(bulb.pos, player.firstChunk.pos) < 100f)
                        {
                            AllergySystem.TriggerAllergy(player, null, TriggerType.Corruption);
                        }
                    }
                }
            }
        }
    }
}
