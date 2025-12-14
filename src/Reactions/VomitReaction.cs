using MoreSlugcats;
using UnityEngine;

namespace Allergies.Reactions
{
    internal class VomitReaction : Reaction
    {
        internal static readonly AbstractPhysicalObject.AbstractObjectType VomitObject =
            new AbstractPhysicalObject.AbstractObjectType("VomitObject", true);
        
        private int countdown = Random.Range(20, 81);
        private int throwUpCounter = 0;

        public VomitReaction(AbstractCreature player) : base(player)
        {
            VomitReactionHooks.Apply();
        }

        public override bool IsStillActive => throwUpCounter <= 110;
        
        public override void Update()
        {
            if (player.room == null) return;
            
            if (countdown > 0)
            {
                countdown--;
                player.swallowAndRegurgitateCounter = 0;
            }
            else
            {
                if (player.objectInStomach is null)
                {
                    if (ModManager.MSC && Random.value < 0.001f)
                    {
                        player.objectInStomach = new AbstractCreature(abstractPlayer.world,
                            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null,
                            abstractPlayer.pos, abstractPlayer.world.game.GetNewID());
                    }
                    else
                    {
                        player.objectInStomach = new AbstractConsumable(player.room.world, VomitObject, null,
                            abstractPlayer.pos, player.room.game.GetNewID(), -1, -1, null)
                        {
                            destroyOnAbstraction = true
                        };
                    }
                }
                throwUpCounter++;
                player.swallowAndRegurgitateCounter = throwUpCounter;

                if (throwUpCounter > 60)
                {
                    player.standing = false;
                }

                if (throwUpCounter > 100)
                {
                    player.Stun(20);
                }

                if (throwUpCounter == 110)
                {
                    player.SubtractFood(1);
                    player.Regurgitate();
                }
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
        }
    }
}