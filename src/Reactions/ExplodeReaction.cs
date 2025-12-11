using System.Linq;
using MoreSlugcats;
using Noise;
using RWCustom;
using UnityEngine;

namespace Allergies.Reactions
{
    internal class ExplodeReaction(AbstractCreature player) : Reaction(player)
    {
        public int countdown = Random.Range(40, 401); // 1-10 seconds

        public override bool IsStillActive => countdown >= 0;

        public override void Update()
        {
            if (countdown > 0)
            {
                if (player?.room != null)
                {
                    countdown--;
                    if (Random.value < 0.012f)
                    {
                        player.Blink(Random.Range(5, 31));
                    }
                    if (Random.value < 0.33f)
                    {
                        player.room.AddObject(new Spark(player.firstChunk.pos, Custom.RNV(), Color.white, null, 4, 8));
                    }
                }
            }
            else if (countdown == 0 && player?.room != null)
            {
                countdown--;
                var room = player.room;
                var pos = player.bodyChunks.Average(x => x.pos);
                var color = player.ShortCutColor();

                if (Random.value > 0.01f)
                {
                    // Normal sized explosion
                    room.AddObject(new SootMark(room, pos, 80f, true));
                    room.AddObject(new Explosion(room, player, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, null, 0.7f, 160f, 1f));
                    room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                    room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                    room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                    room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
                }
                else
                {
                    // Singularity bomb explosion :3c
                    room.AddObject(new SingularityBomb.SparkFlash(player.firstChunk.pos, 300f, color));
                    room.AddObject(new Explosion(room, player, pos, 7, 450f, 6.2f, 10f, 280f, 0.25f, null, 0.3f, 160f, 1f));
                    room.AddObject(new Explosion(room, player, pos, 7, 2000f, 4f, 0f, 400f, 0.25f, null, 0.3f, 200f, 1f));
                    room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                    room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                    room.AddObject(new Explosion.ExplosionLight(pos, 2000f, 2f, 60, color));
                    room.AddObject(new ShockWave(pos, 350f, 0.485f, 300, true));
                    room.AddObject(new ShockWave(pos, 2000f, 0.185f, 180, false));
                }

                for (int i = 0; i < 25; i++)
                {
                    Vector2 dir = Custom.RNV();
                    if (room.GetTile(pos + dir * 20f).Solid)
                    {
                        if (!room.GetTile(pos - dir * 20f).Solid)
                        {
                            dir *= -1f;
                        }
                        else
                        {
                            dir = Custom.RNV();
                        }
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        room.AddObject(new Spark(pos + dir * Mathf.Lerp(30f, 60f, Random.value), dir * Mathf.Lerp(7f, 38f, Random.value) + Custom.RNV() * 20f * Random.value, Color.Lerp(color, new Color(1f, 1f, 1f), Random.value), null, 11, 28));
                    }
                    room.AddObject(new Explosion.FlashingSmoke(pos + dir * 40f * Random.value, dir * Mathf.Lerp(4f, 20f, Mathf.Pow(Random.value, 2f)), 1f + 0.05f * Random.value, new Color(1f, 1f, 1f), color, Random.Range(3, 11)));
                }

                room.ScreenMovement(pos, Vector2.zero, 0.9f);
                for (int m = 0; m < abstractPlayer.stuckObjects.Count; m++)
                {
                    abstractPlayer.stuckObjects[m].Deactivate();
                }
                room.PlaySound(SoundID.Bomb_Explode, pos, abstractPlayer);
                room.InGameNoise(new InGameNoise(pos, 9000f, player, 1f));
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
        }
    }
}
