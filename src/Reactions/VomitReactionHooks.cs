using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Allergies.Reactions
{
    internal static class VomitReactionHooks
    {
        private static bool _applied = false;
        public static void Apply()
        {
            if (_applied) return;
            _applied = true;

            try
            {
                On.AbstractPhysicalObject.Realize += AbstractPhysicalObject_Realize;
                On.SlimeMold.ctor += SlimeMold_ctor;
                IL.SlimeMold.DrawSprites += SlimeMold_DrawSprites;
                On.Player.Grabability += Player_Grabability;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            return obj is SlimeMold sm && sm.abstractPhysicalObject.type == VomitReaction.VomitObject ? Player.ObjectGrabability.CantGrab : orig(self, obj);
        }

        private static void SlimeMold_DrawSprites(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(x => x.MatchLdfld<SlimeMold>(nameof(SlimeMold.JellyfishMode)));
            c.GotoNext(MoveType.After, x => x.MatchLdcR4(2.4f));

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((float origVal, SlimeMold self) =>
            {
                return self.abstractPhysicalObject.type == VomitReaction.VomitObject ? 1.3f : origVal;
            });
        }

        private static void AbstractPhysicalObject_Realize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
        {
            orig(self);
            if (self.type == VomitReaction.VomitObject)
            {
                self.realizedObject ??= new SlimeMold(self);
            }
        }

        private static void SlimeMold_ctor(On.SlimeMold.orig_ctor orig, SlimeMold self, AbstractPhysicalObject abstractPhysicalObject)
        {
            orig(self, abstractPhysicalObject);
            if (abstractPhysicalObject.type == VomitReaction.VomitObject)
            {
                self.JellyfishMode = true;
            }
        }
    }
}