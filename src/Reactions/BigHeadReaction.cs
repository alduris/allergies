using System.Collections.Generic;
using System.Linq;
using RWCustom;
using UnityEngine;

namespace Allergies.Reactions
{
    internal class BigHeadReaction : Reaction
    {
        private int ticksLeft = Random.Range(1100, 1301); // approximately 30 seconds, give or take 2.5 seconds
        private float intensity = 0f;
        private float lastIntensity = 0f;

        private static List<BigHeadReaction> activeBigHeads = [];

        public BigHeadReaction(AbstractCreature player) : base(player)
        {
            // Trim list if need be and add ourselves
            activeBigHeads.RemoveAll(x => x.abstractPlayer.world.game != abstractPlayer.world.game);
            activeBigHeads.Add(this);
        }

        public override bool IsStillActive => ticksLeft > 0 || intensity > 0.01;

        public override void Update()
        {
            ticksLeft--;
            lastIntensity = intensity;
            if (ticksLeft > 0)
            {
                intensity = Custom.LerpAndTick(intensity, 1f, 0.07f, 0.02f);
            }
            else
            {
                intensity = Custom.LerpAndTick(intensity, 0f, 0.05f, 0.01f);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            activeBigHeads.Remove(this);
        }

        private float SumOfIntensities(float timeStacker) =>
            activeBigHeads.Sum(x => x.abstractPlayer == abstractPlayer ? Mathf.Lerp(x.lastIntensity, x.intensity, timeStacker) : 0f);

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (activeBigHeads[0] == this) // we only want to do it once, so we keep track of all others 
            {
                // head sprite: 3
                // face sprite: 9
                float sumOfIntensities = SumOfIntensities(timeStacker) * 0.3f + 1f;
                sLeaser.sprites[3].scaleX *= sumOfIntensities;
                sLeaser.sprites[3].scaleY = sumOfIntensities;
                sLeaser.sprites[9].scaleX *= sumOfIntensities;
                sLeaser.sprites[9].scaleY = sumOfIntensities;
            }
        }
    }
}
