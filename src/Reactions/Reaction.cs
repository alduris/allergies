using UnityEngine;

namespace Allergies.Reactions
{
    public abstract class Reaction(AbstractCreature player)
    {
        protected AbstractCreature abstractPlayer = player;
        protected Player player => abstractPlayer.realizedCreature as Player;
        protected PlayerGraphics playerGraphics => player.graphicsModule as PlayerGraphics;
        
        public abstract bool IsStillActive { get; }

        internal bool hasInitSprites = false;

        public abstract void Update();
        public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);
        public abstract void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
        public abstract void Destroy();
    }
}
