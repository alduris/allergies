using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static string SafeName(string name) => Regex.Replace(name, "[^\\w\\d_]", "_").ToLowerInvariant();

        internal static void Register(ReactionType type, int initialWeight)
        {
            if (!reactionWeightConfig.ContainsKey(type))
            {
                reactionWeightConfig[type] = instance.config.Bind($"ReactionWeight_{SafeName(type.value)}", initialWeight, new ConfigAcceptableRange<int>(0, 999));
            }
        }

        private static readonly Dictionary<ReactionType, Configurable<int>> reactionWeightConfig = [];
        private static Configurable<int> maxAllergensConfig = null!;
        private static Configurable<bool> alwaysMaxAllergensConfig = null!;

        public static int MaxAllergens => maxAllergensConfig.Value;
        public static bool AlwaysMaxAllergens => alwaysMaxAllergensConfig.Value;

        public static int WeightOf(ReactionType type) => reactionWeightConfig.TryGetValue(type, out var weight) ? weight.Value : 0;
        public static IEnumerable<KeyValuePair<ReactionType, int>> AllWeights() => reactionWeightConfig.Select(x => new KeyValuePair<ReactionType, int>(x.Key, WeightOf(x.Key)));

        public Config()
        {
            instance ??= this;
            maxAllergensConfig = instance.config.Bind("MaxAllergens", 8, new ConfigAcceptableRange<int>(1, 9999));
            alwaysMaxAllergensConfig = instance.config.Bind("AlwaysMax", false);
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
            sb.AddItems(
                new OpLabel(new Vector2(0f, 500f), new Vector2(600f, 30f), "SETTINGS", FLabelAlignment.Center, true),

                new OpLabel(150f, 470f, "Maximum active allergies:", false),
                new OpDragger(maxAllergensConfig, new Vector2(426f, 470f)),
                new OpLabel(150f, 440f, "Always use maximum allergies:", false),
                new OpCheckBox(alwaysMaxAllergensConfig, new Vector2(426f, 440f)),

                new OpRule(new Vector2(10f, 430f), 580f)
                );

            // Reactions
            sb.AddItems(new OpLabel(new Vector2(0f, 390f), new Vector2(600f, 30f), "REACTION WEIGHTS", FLabelAlignment.Center, true));
            float y = 390f;
            foreach (var (type, config) in reactionWeightConfig.OrderBy(x => x.Key.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                y -= 30f;
                sb.AddItems(new OpLabel(150f, y, $"{type}:"), new OpDragger(config, new Vector2(426f, y)));
            }

            // Set height for scrolling
            sb.SetContentSize(600f - y);
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
            public OpRule(Vector2 pos, float width) : base(pos, "pixel")
            {
                scale = new Vector2(width, 2f);
                color = MenuColorEffect.rgbMediumGrey;
            }
        }
    }
}
