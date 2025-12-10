using BepInEx;
using BepInEx.Logging;
using System.Security.Permissions;

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

        // Hooks for triggers

        // Register allergens and reactions
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

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        MachineConnector.SetRegisteredOI("alduris.allergies", options);
    }
}
