using RWCustom;
using UnityEngine;

namespace Allergies.Reactions
{
    internal class SneezeReaction(AbstractCreature player) : Reaction(player)
    {
        private int timeLeft = 1200;

        private int sneezeCooldown = 40;
        private int sneezeWindup = 0;

        public override bool IsStillActive => timeLeft > 0;

        public override void Update()
        {
            if (player.room == null) return;

            if (sneezeCooldown > 0)
            {
                sneezeCooldown--;
            }
            else if (sneezeCooldown == 0 && sneezeWindup == 0 && Random.value < 0.002f)
            {
                sneezeWindup = Random.Range(5, 26);
            }
            else if (sneezeWindup > 0)
            {
                player.Blink(3);
                sneezeWindup--;
                if (sneezeWindup == 0)
                {
                    Sneeze();
                    sneezeCooldown = Random.Range(30, 600);
                }
            }

            timeLeft--;
        }

        private void Sneeze()
        {
            // We can assume that room is not null
            Room room = player.room;
            int drops = Random.Range(4, 7);
            for (int i = 0; i < drops; i++)
            {
                room.AddObject(new WaterDrip(player.firstChunk.pos, Custom.RNV() * Random.value * 14f, false));
            }
            room.PlaySound(SoundID.Rock_Hit_Creature, player.firstChunk, false, 0.6f, 1.6f);
            room.InGameNoise(new Noise.InGameNoise(player.firstChunk.pos, 800f, player, 0.6f));
            player.Blink(Random.Range(10, 21));
            if (Random.value < 0.05f)
            {
                player.Stun(Random.Range(10, 16));
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (sneezeWindup > 0)
            {
                sLeaser.sprites[9].y += 5f * (1f / sneezeWindup);
            }
        }
    }
}
