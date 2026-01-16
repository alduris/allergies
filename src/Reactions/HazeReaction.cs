using UnityEngine;

namespace Allergies.Reactions
{
    internal class HazeReaction : Reaction
    {
        private Haze visuals = null!;

        private int fadeInTime;
        private int activeTime;
        private int fadeOutTime;
        private readonly int totalFadeInTime;
        private readonly int totalFadeOutTime;

        public HazeReaction(AbstractCreature player) : base(player)
        {
            totalFadeInTime = fadeInTime = Random.Range(3 * 40, 6 * 40);
            totalFadeOutTime = fadeOutTime = Random.Range(3 * 40, 8 * 40);
            activeTime = Random.Range(15 * 40, 30 * 40);
        }

        public override bool IsStillActive => fadeInTime > 0 || activeTime > 0 || fadeOutTime >= 0;

        public float Intensity
        {
            get
            {
                float value;
                if (fadeInTime > 0)
                {
                    value = 1f - (float)fadeInTime / totalFadeInTime;
                }
                else if (activeTime > 0)
                {
                    value = 1f;
                }
                else
                {
                    value = (float)fadeOutTime / totalFadeOutTime;
                }
                return value;
            }
        }

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

            visuals ??= new Haze(this, player.room);
            if (visuals.room != player.room)
            {
                visuals.MoveRoom(player.room);
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
        }

        private class Haze : CosmeticSprite
        {
            private float lastIntensity;
            private float intensity;
            private readonly HazeReaction reaction;

            public Haze(HazeReaction reaction, Room room)
            {
                this.room = room;
                this.reaction = reaction;
                room.AddObject(this);
            }

            public override void Update(bool eu)
            {
                base.Update(eu);
                lastIntensity = intensity;
                intensity = reaction.Intensity;
                if (reaction.slatedForDeletion)
                {
                    Destroy();
                }
            }

            public void MoveRoom(Room newRoom)
            {
                room.RemoveObject(this);
                room = newRoom;
                room.AddObject(this);
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                sLeaser.sprites = [new FSprite("Futile_White")
                {
                    shader = rCam.game.rainWorld.Shaders["HazeAllergy"],
                    scaleX = rCam.game.rainWorld.options.ScreenSize.x,
                    scaleY = rCam.game.rainWorld.options.ScreenSize.y,
                    x = 0,
                    y = 0,
                    color = new Color(1.25f, Random.value, 0f),
                    alpha = 0,
                }];

                base.InitiateSprites(sLeaser, rCam);
                AddToContainer(sLeaser, rCam, null!);
            }

            public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
            {
                rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[0]);
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                sLeaser.sprites[0].alpha = Mathf.Lerp(lastIntensity, intensity, timeStacker);
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
        }
    }
}
