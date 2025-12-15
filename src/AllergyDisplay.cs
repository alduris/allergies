using System.Collections.Generic;
using Allergies.Allergens;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace Allergies
{
    internal class AllergyDisplay : CosmeticSprite
    {
        private readonly int totalDisplayTime;
        private readonly List<AllergenLabel> _labels;
        private readonly AbstractCreature _followCreature;
        private int _displayTime;
        private int _countdown;
        
        public AllergyDisplay(Room room, AbstractCreature followCreature, List<IAllergen> allergens, Color color)
        {
            this.room = room;
            _followCreature = followCreature;

            _countdown = room.game.IsStorySession && room.game.GetStorySession.saveState.cycleNumber == 0 ? 200 : 80;

            int displayTimePerLabel = allergens.Count * 20 + 140;
            totalDisplayTime = displayTimePerLabel + 5 * allergens.Count;
            _labels =
            [
                new AllergenLabel("ALLERGENS:", 0, allergens.Count, displayTimePerLabel, color)
            ];
            for (int i = 0; i < allergens.Count; i++)
            {
                _labels.Add(new AllergenLabel(allergens[i].Name, i + 1, allergens.Count, displayTimePerLabel, color));
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (_followCreature.realizedCreature?.room == null) return;
            if (_followCreature.realizedCreature.room != room)
            {
                RemoveFromRoom();
                room = _followCreature.realizedCreature.room;
                room.AddObject(this);
            }

            if (_countdown > 0)
            {
                _countdown--;
                return;
            }
            
            _displayTime++;
            if (_displayTime >= totalDisplayTime)
            {
                Destroy();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            var container = new FContainer();
            foreach (AllergenLabel label in _labels)
            {
                label.AddToContainer(container);
            }

            sLeaser.containers = [container];
            sLeaser.sprites = [];

            AddToContainer(sLeaser, rCam, null);
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContatiner)
        {
            newContatiner = rCam.ReturnFContainer("Water");
            newContatiner.AddChild(sLeaser.containers[0]);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (_followCreature.realizedCreature?.room != null && _countdown == 0)
            {
                Vector2 headPos = Vector2.Lerp(_followCreature.realizedCreature.firstChunk.lastPos,
                    _followCreature.realizedCreature.firstChunk.pos, timeStacker);
                sLeaser.containers[0].SetPosition(headPos + Vector2.up * 20f - camPos);
                foreach (AllergenLabel label in _labels)
                {
                    label.DrawSprites(_displayTime + timeStacker);
                }
            }
            else
            {
                foreach (AllergenLabel label in _labels)
                {
                    label.Hide();
                }
            }
        }

        private class AllergenLabel
        {
            private readonly FLabel _label;
            private readonly float yPos;
            private readonly int beginShowTime;
            private readonly int endShowTime;
            private readonly int beginHideTime;
            private readonly int endHideTime;

            public AllergenLabel(string text, int i, int outOf, int displayTime, Color color)
            {
                _label = new FLabel(Custom.GetFont(), text)
                {
                    color = color,
                    alignment = FLabelAlignment.Center
                };
                yPos = LabelTest.LineHeight(false) * (outOf - i);
                beginShowTime = i * 5;
                endShowTime = beginShowTime + 10;
                endHideTime = beginShowTime + displayTime;
                beginHideTime = beginShowTime + displayTime - 10;
            }

            public void AddToContainer(FContainer container)
            {
                _label.RemoveFromContainer();
                container.AddChild(_label);
            }

            public void DrawSprites(float time)
            {
                float alpha = Mathf.Min(Mathf.InverseLerp(beginShowTime, endShowTime, time),
                    Mathf.InverseLerp(endHideTime, beginHideTime, time));

                _label.alpha = alpha;
                _label.isVisible = true;
                _label.SetPosition(0f, yPos);
            }

            public void Hide()
            {
                _label.isVisible = false;
            }
        }
    }
}