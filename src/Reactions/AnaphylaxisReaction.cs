using UnityEngine;

namespace Allergies.Reactions
{
    internal class AnaphylaxisReaction(AbstractCreature player) : Reaction(player)
    {
        private int countdown = Random.Range(30, 120);
        private int beginningSuffocationPhase = Random.Range(100, 300);
        private int activelyDyingPhase = Random.Range(70, 160);

        public override bool IsStillActive => !player.dead;

        public override void Update()
        {
            if (player.room == null) return;

            if (countdown > 0)
            {
                countdown--;
            }
            else if (beginningSuffocationPhase > 0)
            {
                float currentIntensity = Mathf.Pow(Mathf.Max(0f, 1f - beginningSuffocationPhase / 60f), 0.333f);
                player.aerobicLevel = Mathf.Max(player.aerobicLevel, currentIntensity);
                player.exhausted = true;
                player.Blink(6);
                beginningSuffocationPhase--;
                if (beginningSuffocationPhase == 0)
                {
                    player.room.AddObject(new CreatureSpasmer(player, false, activelyDyingPhase));
                    player.Stun(activelyDyingPhase * 2);
                }
            }
            else if (activelyDyingPhase > 0)
            {
                player.aerobicLevel = Mathf.Max(player.aerobicLevel, 1);
                activelyDyingPhase--;
            }
            else
            {
                player.Die();
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
        }
    }
}
