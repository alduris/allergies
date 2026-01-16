using System;
using Allergies.Allergens;
using Allergies.ModCompat.Allergens;
using LBMergedMods.Creatures;
using LBMergedMods.Items;
using MonoMod.RuntimeDetour;
using RWCustom;

namespace Allergies.ModCompat
{
    internal static class LBEntityPackCompat
    {
        private static bool _active = false;
        public static void Register()
        {
            if (_active) return;
            _active = true;

            // Allergens
            AllergySystem.Register(new SimpleEdibleItemAllergen<BouncingMelon>(LBMergedMods.Enums.AbstractObjectType.BouncingMelon));
            AllergySystem.Register(new SimpleEdibleItemAllergen<LimeMushroom>(LBMergedMods.Enums.AbstractObjectType.LimeMushroom));
            AllergySystem.Register(new SimpleEdibleItemAllergen<MarineEye>(LBMergedMods.Enums.AbstractObjectType.MarineEye));
            AllergySystem.Register(new SimpleEdibleItemAllergen<StarLemon>(LBMergedMods.Enums.AbstractObjectType.StarLemon));
            AllergySystem.Register(new SimpleEdibleItemAllergen<ThornyStrawberry>(LBMergedMods.Enums.AbstractObjectType.ThornyStrawberry));
            
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<Hoverfly>(LBMergedMods.Enums.CreatureTemplateType.Hoverfly));
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<Xylo>(LBMergedMods.Enums.CreatureTemplateType.Xylo));
            AllergySystem.Register(new SimpleTouchItemAllergen<Durian>(LBMergedMods.Enums.AbstractObjectType.Durian));
            AllergySystem.Register(new SimpleTouchCreatureAllergen<Caterpillar>(LBMergedMods.Enums.CreatureTemplateType.Killerpillar));
            AllergySystem.Register(new SimpleTouchCreatureAllergen<ThornBug>(LBMergedMods.Enums.CreatureTemplateType.ThornBug));

            AllergySystem.Register(new LBWaterBlobAllergen());

            _ = new Hook(typeof(FumeFruitCloud).GetMethod(nameof(FumeFruitCloud.Update)), FumeFruitCloud_Update);
        }

        private static void FumeFruitCloud_Update(Action<FumeFruitCloud, bool> orig, FumeFruitCloud self, bool eu)
        {
            orig(self, eu);
            if (!self.slatedForDeletetion)
            {
                // Loose copy of calculations for fume fruit so that we can trigger allergy on a player
                var crits = self.room.abstractRoom.creatures;
                for (var i = 0; i < crits.Count; i++)
                {
                    if (crits[i] is not AbstractCreature acr || acr.realizedCreature is not Creature crit || crit.Submersion > .3f || (acr.rippleLayer != self.RippleLayer && !acr.rippleBothSides))
                        continue;
                    foreach (var chunk in crit.bodyChunks)
                    {
                        if (Custom.DistLess(self.pos, chunk.pos, self.Rad + chunk.rad + 20f) && crit is Player player)
                        {
                            AllergySystem.TriggerAllergy(player, null, TriggerType.Spores);
                        }
                    }
                }
            }
        }
    }
}