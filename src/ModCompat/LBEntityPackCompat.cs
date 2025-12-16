using Allergies.Allergens;
using Allergies.ModCompat.Allergens;
using LBMergedMods.Creatures;
using LBMergedMods.Items;

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
            //AllergySystem.Register(new SimpleEdibleItemAllergen<Durian>(LBMergedMods.Enums.AbstractObjectType.Durian));
            //AllergySystem.Register(new SimpleEdibleItemAllergen<FumeFruit>(LBMergedMods.Enums.AbstractObjectType.FumeFruit));
            AllergySystem.Register(new SimpleEdibleItemAllergen<MarineEye>(LBMergedMods.Enums.AbstractObjectType.MarineEye));
            AllergySystem.Register(new SimpleEdibleItemAllergen<StarLemon>(LBMergedMods.Enums.AbstractObjectType.StarLemon));
            AllergySystem.Register(new SimpleEdibleItemAllergen<ThornyStrawberry>(LBMergedMods.Enums.AbstractObjectType.ThornyStrawberry));
            
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<Hoverfly>(LBMergedMods.Enums.CreatureTemplateType.Hoverfly));
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<Xylo>(LBMergedMods.Enums.CreatureTemplateType.Xylo));
            AllergySystem.Register(new SimpleTouchCreatureAllergen<Caterpillar>(LBMergedMods.Enums.CreatureTemplateType.Killerpillar));
            AllergySystem.Register(new SimpleTouchCreatureAllergen<ThornBug>(LBMergedMods.Enums.CreatureTemplateType.ThornBug));

            AllergySystem.Register(new LBWaterBlobAllergen());
        }
    }
}