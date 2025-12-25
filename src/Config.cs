using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Allergies.Allergens;
using Allergies.Reactions;
using Menu;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace Allergies
{
    internal class Config : OptionInterface
    {
        private static Config instance = null!;

        private static string SafeName(string name) => Regex.Replace(name, @"[^\w\d_]", "_").ToLowerInvariant();

        internal static void Register(IAllergen allergen)
        {
            if (!allergenEnabledConfig.ContainsKey(allergen.Name))
            {
                allergenEnabledConfig[allergen.Name] = instance.config.Bind($"AllergenEnabled_{SafeName(allergen.Name)}", true);
            }
        }

        internal static void Register(ReactionType type, int initialWeight)
        {
            if (!reactionWeightConfig.ContainsKey(type))
            {
                reactionWeightConfig[type] = instance.config.Bind($"ReactionWeight_{SafeName(type.value)}", initialWeight, new ConfigAcceptableRange<int>(0, 999));
            }
        }

        private static readonly Dictionary<ReactionType, Configurable<int>> reactionWeightConfig = [];
        private static readonly Dictionary<string, Configurable<bool>> allergenEnabledConfig = [];
        private static Configurable<int> maxAllergensConfig = null!;
        private static Configurable<bool> alwaysMaxAllergensConfig = null!;
        private static Configurable<bool> showAllergensConfig = null!;
        private static Configurable<PlayStyle> playStyleConfig = null!;

        public static int MaxAllergens => maxAllergensConfig.Value;
        public static bool AlwaysMaxAllergens => alwaysMaxAllergensConfig.Value;
        public static bool ShowAllergens => showAllergensConfig.Value;
        public static PlayStyle SelectedPlayStyle => playStyleConfig.Value;

        public static int WeightOf(ReactionType type) => reactionWeightConfig.TryGetValue(type, out var weight) ? weight.Value : 0;
        public static IEnumerable<KeyValuePair<ReactionType, int>> AllWeights() => reactionWeightConfig.Select(x => new KeyValuePair<ReactionType, int>(x.Key, WeightOf(x.Key)));

        public Config()
        {
            instance ??= this;
            maxAllergensConfig = config.Bind("MaxAllergens", 8, new ConfigAcceptableRange<int>(1, 9999));
            alwaysMaxAllergensConfig = config.Bind("AlwaysMax", false);
            showAllergensConfig = config.Bind("ShowAllergens", false);
            playStyleConfig = config.Bind("PlayStyle", PlayStyle.PerWakeUp);
        }

        public override void Initialize()
        {
            base.Initialize();
            var tab = new OpTab(this, "Main");
            Tabs = [tab];

            var sb = new OpScrollBox(tab, 600f, false, true);

            // Title and subtitle
            sb.AddItems(
                new OpShinyLabel(new Vector2(0f, 570f), new Vector2(600f, 30f), "ALLERGIES", FLabelAlignment.Center, true),
                new OpLabel(new Vector2(0f, 550f), new Vector2(600f, 20f), "A mod by Alduris", FLabelAlignment.Center, false),
                new OpRule(new Vector2(10f, 540f), 580f)
                );

            // Basic configs
            UIfocusable temp;
            sb.AddItems(
                new OpLabel(new Vector2(0f, 500f), new Vector2(600f, 30f), "SETTINGS", FLabelAlignment.Center, true),

                temp = new OpDragger(maxAllergensConfig, new Vector2(426f, 470f)),
                new OpLabel(150f, 470f, "Maximum active allergies:", false) { bumpBehav = temp.bumpBehav },
                
                temp = new OpCheckBox(alwaysMaxAllergensConfig, new Vector2(426f, 440f)),
                new OpLabel(150f, 440f, "Always use maximum allergies:", false) { bumpBehav = temp.bumpBehav },
                
                temp = new OpCheckBox(showAllergensConfig, new Vector2(426f, 410f)),
                new OpLabel(150f, 410f, "Show allergens:", false) { bumpBehav = temp.bumpBehav },
                
                temp = new OpResourceSelector2(playStyleConfig, new Vector2(310f, 380f), 140f),
                new OpLabel(150f, 380f, "Randomization frequency:", false) { bumpBehav = temp.bumpBehav },

                new OpRule(new Vector2(10f, 370f), 580f)
                );

            // Reactions
            sb.AddItems(new OpLabel(new Vector2(0f, 330f), new Vector2(600f, 30f), "REACTION WEIGHTS", FLabelAlignment.Center, true));
            float y = 330f;
            foreach (var (type, config) in reactionWeightConfig.OrderBy(x => x.Key.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                y -= 30f;
                var dragger = new OpDragger(config, new Vector2(426f, y));
                sb.AddItems(new OpLabel(150f, y, $"{type}:") { bumpBehav = dragger.bumpBehav }, dragger);
            }

            y -= 10f;
            sb.AddItems(new OpRule(new Vector2(10f, y), 580f));
            
            // Allergens
            y -= 40f;
            sb.AddItems(new OpLabel(new Vector2(0f, y), new Vector2(600f, 30f), "ENABLED ALLERGENS", FLabelAlignment.Center, true));
            y -= 10f;

            const float TOTAL_WIDTH = 580f;
            const float SPACE_WIDTH = 22f;
            const float LEFT_OFFSET = 600f / 2f - TOTAL_WIDTH / 2f;
            float columnsTop = y;
            int totalColumns = Math.Max(1, Mathf.FloorToInt((TOTAL_WIDTH + SPACE_WIDTH) / (SPACE_WIDTH + allergenEnabledConfig.Keys.Max(x => LabelTest.GetWidth(x) + 34f))));
            int column = 0;
            float columnWidth = (TOTAL_WIDTH - SPACE_WIDTH * (totalColumns - 1)) / totalColumns;
            foreach (var (allergen, config) in allergenEnabledConfig.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (column == 0) y -= 30f;
                float columnOffset = LEFT_OFFSET + (columnWidth + SPACE_WIDTH) * column;
                var cb = new OpCheckBox(config, new Vector2(columnOffset + columnWidth - 24f, y));
                sb.AddItems(new OpLabel(columnOffset, y, $"{allergen}:") { bumpBehav = cb.bumpBehav }, cb);

                column = (column + 1) % totalColumns;
            }
            
            // Allergen column bars
            for (int i = 0; i < totalColumns - 1; i++)
            {
                float x = LEFT_OFFSET + columnWidth * (i + 1) + SPACE_WIDTH * i + 10f;
                sb.AddItems(new OpRule(new Vector2(x, y), columnsTop - y, true));
            }

            // Set height for scrolling
            sb.SetContentSize(610f - y);
            sb.ScrollToTop();
        }

        public enum PlayStyle
        {
            PerWakeUp,
            PerCycle,
            PerSession,
            PerCampaign
        }

        private class OpShinyLabel : OpLabel
        {
            public OpShinyLabel(Vector2 pos, Vector2 size, string text = "TEXT", FLabelAlignment alignment = FLabelAlignment.Center, bool bigText = false, FTextParams? textParams = null) : base(pos, size, text, alignment, bigText, textParams)
            {
                label.shader = Custom.rainWorld.Shaders["MenuText"];
            }
        }

        private class OpRule : OpImage
        {
            public OpRule(Vector2 pos, float size, bool vertical = false) : base(pos, "pixel")
            {
                scale = vertical ? new Vector2(2f, size) : new Vector2(size, 2f);
                color = MenuColorEffect.rgbMediumGrey;
            }
        }

        private class OpResourceSelector2 : OpResourceSelector
        {
            public OpResourceSelector2(ConfigurableBase config, Vector2 pos, float width) : base(config, pos, width)
            {
            }

            public OpResourceSelector2(Configurable<string> config, Vector2 pos, float width, SpecialEnum listType) : base(config, pos, width, listType)
            {
            }

            public override void GrafUpdate(float timeStacker)
            {
                base.GrafUpdate(timeStacker);
                if (_rectList != null && !_rectList.isHidden)
                {
                    myContainer.MoveToFront();
                    for (int i = 0; i < 9; i++)
                    {
                        _rectList.sprites[i].alpha = 1f;
                    }
                }
            }
        }
    }
}
