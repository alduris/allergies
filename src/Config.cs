using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Allergies.Reactions;

namespace Allergies
{
    internal class Config : OptionInterface
    {
        private static Config instance;

        private static string SafeName(string name) => Regex.Replace(name, "[^\\w\\d_]", "_").ToLowerInvariant();

        internal static void Register(ReactionType type, int initialWeight)
        {
            reactionWeightConfig[type] = instance.config.Bind($"ReactionWeight_{SafeName(type.value)}", initialWeight, new ConfigAcceptableRange<int>(0, 999));
        }

        private static readonly Dictionary<ReactionType, Configurable<int>> reactionWeightConfig = [];
        private static Configurable<int> maxAllergensConfig;

        public static int MaxAllergens => maxAllergensConfig.Value;

        public static int WeightOf(ReactionType type) => reactionWeightConfig.TryGetValue(type, out var weight) ? weight.Value : 0;
        public static IEnumerable<KeyValuePair<ReactionType, int>> AllWeights() => reactionWeightConfig.Select(x => new KeyValuePair<ReactionType, int>(x.Key, WeightOf(x.Key)));

        public Config()
        {
            instance ??= this;
            maxAllergensConfig = instance.config.Bind("MaxAllergens", 5, new ConfigAcceptableRange<int>(1, 9999));
        }

        public override void Initialize()
        {
            base.Initialize();
            // todo
        }
    }
}
