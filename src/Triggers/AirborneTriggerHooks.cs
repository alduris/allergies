using System;
using System.Linq;
using Allergies.Allergens;
using RWCustom;
using UnityEngine;

namespace Allergies.Triggers
{
    internal static class AirborneTriggerHooks
    {
        public static void Apply()
        {
            try
            {
                On.Player.Update += Player_Update;
                On.SporeCloud.Update += SporeCloud_Update;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (eu && self.room != null)
            {
                var myPos = self.firstChunk.pos;
                var myRad = self.firstChunk.rad;
                foreach (var obj in self.room.physicalObjects.SelectMany(x => x))
                {
                    if (obj.abstractPhysicalObject.IsSameRippleLayer(self.abstractPhysicalObject.rippleLayer)
                        && obj.bodyChunks.Any(x => Vector2.Distance(x.pos, myPos) < 80f + x.rad + myRad)) // 4 tiles
                    {
                        AllergySystem.TriggerAllergy(self, obj, TriggerType.Airborne);
                    }
                }
            }
        }

        private static void SporeCloud_Update(On.SporeCloud.orig_Update orig, SporeCloud self, bool eu)
        {
            orig(self, eu);
            if (!self.nonToxic)
            {
                foreach (var abstrPlayer in self.room.game.Players)
                {
                    if (abstrPlayer.realizedCreature is Player player && player.room == self.room && abstrPlayer.IsSameRippleLayer(self.rippleLayer))
                    {
                        foreach (var chunk in player.bodyChunks)
                        {
                            if (Custom.DistLess(self.pos, chunk.pos, self.rad + chunk.rad + 20f))
                            {
                                AllergySystem.TriggerAllergy(player, null, TriggerType.Spores);
                            }
                        }
                    }
                }
            }
        }
    }
}
