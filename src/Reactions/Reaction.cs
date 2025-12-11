using UnityEngine;

namespace Allergies.Reactions
{
    public abstract class Reaction(AbstractCreature player)
    {
        protected AbstractCreature abstractPlayer = player;
        protected Player player => (abstractPlayer.realizedCreature as Player)!;
        protected PlayerGraphics playerGraphics => (player.graphicsModule as PlayerGraphics)!;

        public abstract bool IsStillActive { get; }

        public bool slatedForDeletion = false;
        internal bool hasInitSprites = false;

        public abstract void Update();
        public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam) { }
        public abstract void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
        public virtual void Destroy()
        {
            slatedForDeletion = true;
        }
    }
}
