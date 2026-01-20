using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Allergies.Reactions
{
    internal class HazeReaction : Reaction
    {
        private static WeakReference<HazeReaction>? activeHazeReaction;

        private int fadeInTime;
        private int activeTime;
        private int fadeOutTime;
        private readonly int totalFadeInTime;
        private readonly int totalFadeOutTime;

        private float lastIntensity;
        private float intensity;
        private FSprite? sprite;

        public HazeReaction(AbstractCreature player) : base(player)
        {
            // Since Haze is a full screen effect, we don't care about disambiguating between players. If there is an active effect, no other effects can spawn.
            // One exception to this is if the player is an NPC, whereupon we don't trigger the effect because that doesn't make sense.
            if ((this.player != null && this.player.isNPC) || (activeHazeReaction != null && activeHazeReaction.TryGetTarget(out var reaction) && reaction.IsStillActive))
            {
                Destroy();
                return;
            }
            activeHazeReaction = new WeakReference<HazeReaction>(this);
            totalFadeInTime = fadeInTime = Random.Range(3 * 40, 6 * 40);
            totalFadeOutTime = fadeOutTime = Random.Range(6 * 40, 9 * 40);
            activeTime = Random.Range(15 * 40, 30 * 40);
        }

        public override bool IsStillActive => fadeInTime > 0 || activeTime > 0 || fadeOutTime >= 0;

        public override void Update()
        {
            if (player?.room == null) return;

            if (fadeInTime > 0)
            {
                fadeInTime--;
            }
            else if (activeTime > 0)
            {
                activeTime--;
            }
            else if (fadeOutTime >= 0)
            {
                fadeOutTime--;
            }
            else
            {
                Destroy();
            }

            lastIntensity = intensity;
            if (fadeInTime > 0)
            {
                intensity = 1f - (float)fadeInTime / totalFadeInTime;
            }
            else if (activeTime > 0)
            {
                intensity = 1f;
            }
            else
            {
                intensity = (float)fadeOutTime / totalFadeOutTime;
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            sprite = new FSprite("Futile_White")
            {
                shader = rCam.game.rainWorld.Shaders["HazeAllergy"],
                scaleX = rCam.game.rainWorld.options.ScreenSize.x,
                scaleY = rCam.game.rainWorld.options.ScreenSize.y,
                x = 0,
                y = 0,
                color = new Color(1.25f, Random.value, 0f),
                alpha = 0,
            };
            rCam.ReturnFContainer("Bloom").AddChild(sprite);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sprite!.alpha = Mathf.Lerp(lastIntensity, intensity, timeStacker);
        }

        public override void Destroy()
        {
            base.Destroy();
            sprite?.RemoveFromContainer();
        }
    }
}
