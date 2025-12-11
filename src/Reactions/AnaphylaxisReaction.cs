using UnityEngine;

namespace Allergies.Reactions
{
    internal class AnaphylaxisReaction(AbstractCreature player) : Reaction(player)
    {
        private int countdown = Random.Range(30, 120);
        private int beginningSuffocationPhase = Random.Range(100, 300);
        private int activelyDyingPhase = Random.Range(50, 160);

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
                float currentIntensity = Mathf.Pow(1f / beginningSuffocationPhase, 0.25f);
                player.aerobicLevel = Mathf.Max(player.aerobicLevel, currentIntensity);
                if (currentIntensity > 0.42f)
                {
                    player.Blink(6);
                }
                beginningSuffocationPhase--;
            }
            else if (activelyDyingPhase > 0)
            {
                player.aerobicLevel = Mathf.Max(player.aerobicLevel, 1);
                player.Stun(6);
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
