using System;
using System.Runtime.CompilerServices;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Allergies.Reactions
{
    internal class HivesReaction : Reaction
    {
        private static readonly ConditionalWeakTable<AbstractCreature, HivesReaction> _instances = new();
        private static readonly ConditionalWeakTable<PlayerGraphics, HivesOverlay> _overlays = new();

        private int fadeInTime = 0;
        private int activeTime = 0;
        private int fadeOutTime = 0;
        private readonly int totalFadeInTime = 0;
        private readonly int totalFadeOutTime = 0;

        public HivesReaction(AbstractCreature player) : base(player)
        {
            if (!_instances.TryGetValue(player, out var activeInstance))
            {
                _instances.Add(player, this);
                totalFadeInTime = fadeInTime = Random.Range(3 * 40, 6 * 40);
                totalFadeOutTime = fadeOutTime = Random.Range(3 * 40, 8 * 40);
                activeTime = Random.Range(15 * 40, 30 * 40);
            }
            else
            {
                activeInstance.LengthenTimer();
                Destroy();
            }

            setCooldown = 400;
        }

        public override bool IsStillActive => fadeInTime > 0 || activeTime > 0 || fadeOutTime > 0;

        private void LengthenTimer()
        {
            if (activeTime > 0) // also covers _fadeInTime > 0
            {
                activeTime += Random.Range(10 * 40, 20 * 40);
            }
            else if (fadeOutTime > 0)
            {
                fadeInTime = (int)(totalFadeInTime * ((float)fadeOutTime / totalFadeOutTime));
                activeTime = Random.Range(20 * 40, 40 * 40);
                fadeOutTime = totalFadeOutTime;
            }
        }

        public override void Update()
        {
            if (player?.graphicsModule is PlayerGraphics graphics && _overlays.TryGetValue(graphics, out var hivesOverlay))
            {
                if (hivesOverlay.slatedForDeletetion)
                {
                    Plugin.Logger.LogDebug("Hives overlay slated for deletion!");
                    _overlays.Remove(graphics);
                }
                else if (hivesOverlay.room == null && player.room != null)
                {
                    player.room.AddObject(hivesOverlay);
                }
            }

            if (player?.room == null) return;

            if (fadeInTime > 0)
            {
                fadeInTime--;
            }
            else if (activeTime > 0)
            {
                activeTime--;
            }
            else if (fadeOutTime > 0)
            {
                fadeOutTime--;
            }
            else
            {
                Plugin.Logger.LogDebug($"Destroying because out of time");
                Destroy();
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (sLeaser.drawableObject is PlayerGraphics pGraphics && !_overlays.TryGetValue(pGraphics, out _))
            {
                _overlays.Add(pGraphics, new HivesOverlay(this, player, sLeaser));
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Plugin.Logger.LogDebug("Destroying hives reaction");
            _instances.Remove(abstractPlayer);
        }

        private class HivesOverlay : MudOverlay
        {
            private readonly HivesReaction reaction;

            public HivesOverlay(HivesReaction ownerReaction, Creature ownerCrit, RoomCamera.SpriteLeaser ownerSleaser) : base(ownerCrit, ownerSleaser)
            {
                Plugin.Logger.LogDebug($"Created hives overlay for player {ownerCrit.abstractCreature.ID}");
                reaction = ownerReaction;
            }

            public override void Update(bool eu)
            {
                base.Update(eu);
                if (!slatedForDeletetion && reaction.slatedForDeletion)
                {
                    Destroy();
                }
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                base.InitiateSprites(sLeaser, rCam);
                Plugin.Logger.LogDebug("Creating hives sprites");
                foreach (var sprite in sLeaser.sprites)
                {
                    sprite.shader = rCam.game.rainWorld.Shaders["HivesAllergy"];
                }

                // Adjust tail
                TriangleMesh tailSprite = (sLeaser.sprites[2] as TriangleMesh)!;
                PlayerGraphics pg = (ownerCrit.graphicsModule as PlayerGraphics)!;
                Color seed = tailSprite.verticeColors[0]; // verticeColors[0] has wrong calculations that makes the uv 0 + the seed, aka just the seed

                // First: fix the uvs for the tail by reversing the order of the green channel
                for (int i = 0; i < tailSprite.verticeColors.Length / 2; i++)
                {
                    int j = tailSprite.verticeColors.Length - 1 - i;
                    (tailSprite.verticeColors[i].g, tailSprite.verticeColors[j].g) = (tailSprite.verticeColors[j].g, tailSprite.verticeColors[i].g);
                }

                // Then add some scaling multipliers to fix weird stuff with mud sprites
                float tailBaseRad = pg.tail[0].rad * 0.5f;
                for (int i = 0; i < tailSprite.verticeColors.Length; i++)
                {
                    tailSprite.verticeColors[i].r = (tailSprite.verticeColors[i].r - seed.r) * tailBaseRad + seed.r;
                    tailSprite.verticeColors[i].g = (tailSprite.verticeColors[i].g - seed.g) * tailBaseRad + seed.g;
                }
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
                if (!slatedForDeletetion)
                {
                    float alpha;
                    if (reaction.fadeInTime > 0)
                    {
                        alpha = 1f - (reaction.fadeInTime + 1f - timeStacker) / reaction.totalFadeInTime;
                    }
                    else if (reaction.activeTime > 0)
                    {
                        alpha = 1f;
                    }
                    else
                    {
                        alpha = (reaction.fadeOutTime + 1f - timeStacker) / reaction.totalFadeOutTime;
                    }
                    alpha *= 0.2f;

                    var array = MuddableSprites();
                    for (int i = 0; i < sLeaser.sprites.Length; i++)
                    {
                        UpdateHivesSprite(sLeaser.sprites[i], array[i], alpha);
                    }
                }
            }

            private void UpdateHivesSprite(FSprite dst, FSprite src, float alpha)
            {
                dst.isVisible = src.isVisible;
                dst.SetPosition(src.GetPosition());
                dst.SetAnchor(src.GetAnchor());
                dst.element = src.element;
                dst.scaleX = src.scaleX;
                dst.scaleY = src.scaleY;
                dst.rotation = src.rotation;
                float packedColor = MudUtils.PackColor(src.color);

                switch (dst)
                {
                    case TriangleMesh mudMesh:
                        {
                            var srcMesh = (src as TriangleMesh)!;
                            mudMesh.vertices = srcMesh.vertices;
                            for (int i = 0; i < mudMesh.verticeColors.Length; i++)
                            {
                                mudMesh.verticeColors[i].b = srcMesh.verticeColors != null ? MudUtils.PackColor(srcMesh.verticeColors[i]) : packedColor;
                                mudMesh.verticeColors[i].a = alpha;
                            }
                            mudMesh.Refresh();
                            break;
                        }
                    case MudUtils.MudOverlaySprite mudSprite:
                        {
                            FAtlasElement element = src.element;
                            Rect srcUV = new Rect();
                            srcUV.width = element.sourceSize.x;
                            srcUV.height = element.sourceSize.y;
                            srcUV.x = -src.anchorX * srcUV.width;
                            srcUV.y = -src.anchorY * srcUV.height;
                            float width = element.sourceRect.width;
                            float height = element.sourceRect.height;
                            float top = srcUV.x + element.sourceRect.x;
                            float right = srcUV.y + (srcUV.height - element.sourceRect.y - element.sourceRect.height);
                            Vector2[] vertices = mudSprite.vertices;
                            vertices[0] = new Vector2(top, right + height);
                            vertices[1] = new Vector2(top + width, right + height);
                            vertices[2] = new Vector2(top + width, right);
                            vertices[3] = new Vector2(top, right);
                            mudSprite.flipX = src.scaleX < 0f;
                            mudSprite.flipY = src.scaleY < 0f;
                            mudSprite.packedColor = packedColor;
                            mudSprite.alpha = alpha;
                            break;
                        }
                    default:
                        dst.alpha = alpha;
                        break;
                }
            }

            public override void Destroy()
            {
                base.Destroy();
                Plugin.Logger.LogDebug("Hives reaction actually destroyed!");
            }
        }
    }
}
