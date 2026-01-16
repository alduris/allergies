using Allergies.Allergens;
using Allergies.Reactions;
using Allergies.Triggers;
using BepInEx;
using BepInEx.Logging;
using MoreSlugcats;
using System;
using System.Linq;
using System.Security.Permissions;
using Allergies.ModCompat;
using Watcher;
using UnityEngine;
using RWCustom;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace Allergies;

[BepInPlugin("alduris.allergies", "Allergies", "1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger = null!;
    private bool IsInit;
    private Config options = null!;

    internal static int sessionSeed;

    public void OnEnable()
    {
        Logger = base.Logger;
        sessionSeed = unchecked((int)DateTime.Now.Ticks);
        options = new Config();

        // Basic hooks
        On.Player.NewRoom += Player_NewRoom;
        On.Player.Update += Player_Update;
        On.RoomCamera.SpriteLeaser.Update += SpriteLeaser_Update;
        On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
        On.AbstractPhysicalObject.Destroy += AbstractPhysicalObject_Destroy;
        On.RainWorld.OnModsInit += OnModsInit;
        On.CreatureSymbol.SpriteNameOfCreature += CreatureSymbol_SpriteNameOfCreature;

        // Hooks for triggers
        BiteTriggerHooks.Apply();
        EdibleTriggerHooks.Apply();
        ImpaleTriggerHooks.Apply();
        TouchTriggerHooks.Apply();
        AirborneTriggerHooks.Apply();
    }

    private void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
    {
        orig(self, newRoom);
        AllergySystem.Initiate(newRoom.world.game, self, newRoom);
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

    private void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
    {
        orig(self);
        AllergySystem.Destroy(null);
    }

    private void AbstractPhysicalObject_Destroy(On.AbstractPhysicalObject.orig_Destroy orig, AbstractPhysicalObject self)
    {
        orig(self);
        if (self is AbstractCreature creature)
        {
            AllergySystem.Destroy(creature);
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
        AllergySystem.Register(new SimpleEdibleItemAllergen<SwollenWaterNut>(AbstractPhysicalObject.AbstractObjectType.WaterNut));
        
        AllergySystem.Register(new SimpleAirborneItemAllergen<FlyLure>(AbstractPhysicalObject.AbstractObjectType.FlyLure));

        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Centipede>(CreatureTemplate.Type.Centipede, 2));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Fly>(CreatureTemplate.Type.Fly));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<Hazer>(CreatureTemplate.Type.Hazer));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<NeedleWorm>(CreatureTemplate.Type.SmallNeedleWorm));
        AllergySystem.Register(new SimpleEdibleCreatureAllergen<VultureGrub>(CreatureTemplate.Type.VultureGrub));
        
        AllergySystem.Register(new SimpleAirborneCreatureAllergen<Deer>(CreatureTemplate.Type.Deer));
        AllergySystem.Register(new SimpleAirborneCreatureAllergen<Scavenger>(CreatureTemplate.Type.Scavenger));

        AllergySystem.Register(new BeesAllergen());
        AllergySystem.Register(new ClothAllergen());
        AllergySystem.Register(new CoralAllergen());
        AllergySystem.Register(new DartMaggotAllergen());
        AllergySystem.Register(new LizardLickAllergen());
        AllergySystem.Register(new PolePlantAllergen());
        AllergySystem.Register(new RotAllergen());
        AllergySystem.Register(new SeedAllergen());
        AllergySystem.Register(new SlimeMoldAllergen());
        AllergySystem.Register(new SpiderBiteAllergen());
        AllergySystem.Register(new SporesAllergen());
        AllergySystem.Register(new VoidAllergen());

        if (ModManager.DLCShared)
        {
            AllergySystem.Register(new SimpleEdibleItemAllergen<DandelionPeach>(DLCSharedEnums.AbstractObjectType.DandelionPeach));
            AllergySystem.Register(new SimpleEdibleItemAllergen<GlowWeed>(DLCSharedEnums.AbstractObjectType.GlowWeed));
            AllergySystem.Register(new SimpleEdibleItemAllergen<GooieDuck>(DLCSharedEnums.AbstractObjectType.GooieDuck));
            AllergySystem.Register(new SimpleEdibleItemAllergen<LillyPuck>(DLCSharedEnums.AbstractObjectType.LillyPuck));
            
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<Yeek>(DLCSharedEnums.CreatureTemplateType.Yeek));
        }

        if (ModManager.Watcher)
        {
            AllergySystem.Register(new SimpleEdibleItemAllergen<BoxWorm.Larva>(WatcherEnums.AbstractObjectType.FireSpriteLarva));

            AllergySystem.Register(new SimpleEdibleCreatureAllergen<Barnacle>(WatcherEnums.CreatureTemplateType.Barnacle));
            AllergySystem.Register(new SimpleEdibleCreatureAllergen<Frog>(WatcherEnums.CreatureTemplateType.Frog));
            AllergySystem.Register(new SimpleEdibleCreatureAllergen<Rat>(WatcherEnums.CreatureTemplateType.Rat));
            AllergySystem.Register(new SimpleEdibleCreatureAllergen<SandGrub>(WatcherEnums.CreatureTemplateType.SandGrub));
            
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<BigMoth>(WatcherEnums.CreatureTemplateType.BigMoth));
            AllergySystem.Register(new SimpleAirborneCreatureAllergen<FireSprite>(WatcherEnums.CreatureTemplateType.FireSprite));
        }

        // Register reactions
        AllergySystem.Register(ReactionType.Sneeze, (player) => new SneezeReaction(player.abstractCreature), 4);
        AllergySystem.Register(ReactionType.Spasm, (player) => new SpasmReaction(player.abstractCreature), 5);
        AllergySystem.Register(ReactionType.Vomit, (player) => new VomitReaction(player.abstractCreature), 3);
        AllergySystem.Register(ReactionType.Anaphylaxis, (player) => new AnaphylaxisReaction(player.abstractCreature), 3);
        AllergySystem.Register(ReactionType.BigHead, (player) => new BigHeadReaction(player.abstractCreature), 1);
        AllergySystem.Register(ReactionType.Explode, (player) => new ExplodeReaction(player.abstractCreature), 0);
        AllergySystem.Register(ReactionType.Hives, (player) => new HivesReaction(player.abstractCreature), 4);
        
        try
        {
            // Mod compatibility
            if (ModManager.ActiveMods.Any(x => x.id == "lb-fgf-m4r-ik.modpack"))
            {
                LBEntityPackCompat.Register();
            }

            // Also shaders
            AssetBundle bundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("shaders/allergies"));
            Custom.rainWorld.Shaders["HivesAllergy"] = FShader.CreateShader("HivesAllergy", bundle.LoadAsset<Shader>("assets/shaders/HivesAllergy.shader"));
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }

        // Register remix menu
        MachineConnector.SetRegisteredOI("alduris.allergies", options);
    }
}
