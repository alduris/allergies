using Allergies.Allergens;
using Allergies.Triggers;
using BepInEx;
using BepInEx.Logging;
using MoreSlugcats;
using System.Security.Permissions;
using Watcher;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace Allergies;

[BepInPlugin("alduris.allergies", "Allergies", "1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger = null!;
    private bool IsInit;
    private Config options = null!;

    public void OnEnable()
    {
        Logger = base.Logger;
        options = new Config();

        // Basic hooks
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;
        On.RoomCamera.SpriteLeaser.Update += SpriteLeaser_Update;
        On.RainWorld.OnModsInit += OnModsInit;
        On.CreatureSymbol.SpriteNameOfCreature += CreatureSymbol_SpriteNameOfCreature;

        // Hooks for triggers
        BiteTriggerHooks.Apply();
        EdibleTriggerHooks.Apply();
        ImpaleTriggerHooks.Apply();
        TouchTriggerHooks.Apply();
        AirborneTriggerHooks.Apply();
    }


    private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        AllergySystem.Initiate(world.game, abstractCreature);
    }

    private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        AllergySystem.Update(self);
    }

    private void SpriteLeaser_Update(On.RoomCamera.SpriteLeaser.orig_Update orig, RoomCamera.SpriteLeaser self, float timeStacker, RoomCamera rCam, UnityEngine.Vector2 camPos)
    {
        orig(self, timeStacker, rCam, camPos);
        if (self.drawableObject is PlayerGraphics { player: Player player } && !self.deleteMeNextFrame)
        {
            AllergySystem.DrawSprites(player, self, rCam, timeStacker, camPos);
        }
    }

    private string CreatureSymbol_SpriteNameOfCreature(On.CreatureSymbol.orig_SpriteNameOfCreature orig, IconSymbol.IconSymbolData iconData)
    {
        if (iconData.critType == CreatureTemplate.Type.LizardTemplate)
        {
            return CreatureSymbol.LizardSpriteName("Kill_Standard_Lizard", iconData.intData);
        }
        return orig(iconData);
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        // Register allergens
        AllergySystem.Register(new SimpleEdibleItemAllergen<DangleFruit>(AbstractPhysicalObject.AbstractObjectType.DangleFruit));
        AllergySystem.Register(new SimpleEdibleItemAllergen<EggBugEgg>(AbstractPhysicalObject.AbstractObjectType.EggBugEgg));
        AllergySystem.Register(new SimpleEdibleItemAllergen<Mushroom>(AbstractPhysicalObject.AbstractObjectType.Mushroom));
        AllergySystem.Register(new SimpleEdibleItemAllergen<OracleSwarmer>(AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer));
        AllergySystem.Register(new SimpleEdibleItemAllergen<Pomegranate>(AbstractPhysicalObject.AbstractObjectType.Pomegranate));
        AllergySystem.Register(new SeedAllergen());
        AllergySystem.Register(new SlimeMoldAllergen());
        AllergySystem.Register(new SimpleEdibleItemAllergen<SwollenWaterNut>(AbstractPhysicalObject.AbstractObjectType.WaterNut));

        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Centipede>(CreatureTemplate.Type.Centipede, 2));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Fly>(CreatureTemplate.Type.Fly));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Hazer>(CreatureTemplate.Type.Hazer));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Lizard>(CreatureTemplate.Type.LizardTemplate));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<NeedleWorm>(CreatureTemplate.Type.SmallNeedleWorm));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<VultureGrub>(CreatureTemplate.Type.VultureGrub));

        AllergySystem.Register(new SpiderBiteAllergen());
        AllergySystem.Register(new DartMaggotAllergen());
        AllergySystem.Register(new CoralAllergen());
        AllergySystem.Register(new ClothAirborneAllergen());
        AllergySystem.Register(new LizardLickAllergen());
        AllergySystem.Register(new PolePlantAllergen());
        AllergySystem.Register(new SporesAllergen());

        if (ModManager.DLCShared)
        {
            AllergySystem.Register(new SimpleEdibleItemAllergen<DandelionPeach>(DLCSharedEnums.AbstractObjectType.DandelionPeach));
            AllergySystem.Register(new SimpleEdibleItemAllergen<GlowWeed>(DLCSharedEnums.AbstractObjectType.GlowWeed));
            AllergySystem.Register(new SimpleEdibleItemAllergen<GooieDuck>(DLCSharedEnums.AbstractObjectType.GooieDuck));
            AllergySystem.Register(new SimpleEdibleItemAllergen<LillyPuck>(DLCSharedEnums.AbstractObjectType.LillyPuck));
        }

        if (ModManager.MSC)
        {
            AllergySystem.Register(new SimpleEdibleItemAllergen<FireEgg>(MoreSlugcatsEnums.AbstractObjectType.FireEgg));
        }

        if (ModManager.Watcher)
        {
            AllergySystem.Register(new SimpleEdibleItemAllergen<BoxWorm.Larva>(WatcherEnums.AbstractObjectType.FireSpriteLarva));

            AllergySystem.Register(new SimpleEdibleCreatureAllergen<Barnacle>(WatcherEnums.CreatureTemplateType.Barnacle));
            AllergySystem.Register(new SimpleEdibleCreatureAllergen<Frog>(WatcherEnums.CreatureTemplateType.Frog));
            AllergySystem.Register(new SimpleEdibleCreatureAllergen<Rat>(WatcherEnums.CreatureTemplateType.Rat));
            AllergySystem.Register(new SimpleEdibleCreatureAllergen<SandGrub>(WatcherEnums.CreatureTemplateType.SandGrub));

            AllergySystem.Register(new MothAirborneAllergen());
        }

        // Register reactions

        // Register remix menu
        MachineConnector.SetRegisteredOI("alduris.allergies", options);
    }
}
