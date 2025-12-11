using UnityEngine;

namespace Allergies.Reactions
{
    internal class SpasmReaction(AbstractCreature player) : Reaction(player)
    {
        private bool hasSpawned = false;
        private int countdown = Random.Range(5, 80);

        public override bool IsStillActive => !hasSpawned;

        public override void Update()
        {
            if (player.room == null) return;
            if (countdown > 0)
            {
                countdown--;
            }
            else if (!hasSpawned)
            {
                int duration = Random.Range(60, 121);
                player.room.AddObject(new CreatureSpasmer(player, false, duration));
                player.Stun(duration + 20);
                hasSpawned = true;
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
        }
    }
}
