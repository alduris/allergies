using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Allergies.Allergens;
using Allergies.Reactions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Allergies
{
    public static class AllergySystem
    {
        private static readonly List<IAllergen> allAllergens = [];
        private static readonly Dictionary<ReactionType, Func<Player, Reaction>> allReactions = [];
        private static readonly ConditionalWeakTable<AbstractCreature, List<ActiveAllergy>> activeAllergies = new();

        public static void Register(IAllergen allergen)
        {
            if (allergen is null) throw new ArgumentNullException(nameof(allergen));
            if (allAllergens.All(x => x.Name != allergen.Name))
            {
                allAllergens.Add(allergen);
            }
            Config.Register(allergen);
        }

        public static void Register(ReactionType type, Func<Player, Reaction> factory, int defaultWeight = 3)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (!allReactions.ContainsKey(type))
            {
                allReactions.Add(type, factory);
            }
            Config.Register(type, defaultWeight);
        }

        public static bool TriggerAllergy(Player player, PhysicalObject? initiator, TriggerType type)
        {
            bool flag = false;
            if (activeAllergies.TryGetValue(player.abstractCreature, out var allergies))
            {
                foreach (var allergy in allergies)
                {
                    flag = allergy.TryApply(player, initiator, type) | flag; // order of operations
                }
            }
            return flag;
        }

        public static IEnumerable<Reaction> ActiveReactionsFor(Player player)
        {
            if (activeAllergies.TryGetValue(player.abstractCreature, out var allergies))
            {
                return allergies.SelectMany(x => x.activeReactions);
            }
            return [];
        }

        internal static void Initiate(RainWorldGame game, Player player, Room room)
        {
            if (!activeAllergies.TryGetValue(player.abstractCreature, out _))
            {
                int seed = new System.Random().Next(); // pick a default random seed
                
                if (game.IsStorySession)
                {
                    // ONLY if the game is a story session, determine a way to generate random seed if need be
                    // (arena, for example, is always random)
                    var saveState = game.GetStorySession.saveState;
                    int id = player.abstractCreature.ID.number + player.abstractCreature.ID.spawner * 100;
                    switch (Config.SelectedPlayStyle)
                    {
                        case Config.PlayStyle.PerCycle:
                            seed = saveState.totTime * 100000 + saveState.cycleNumber * 1000 + saveState.saveStateNumber.Index * 100 + id + Plugin.sessionSeed;
                            break;
                        case Config.PlayStyle.PerSession:
                            seed = saveState.saveStateNumber.Index * 100 + id + Plugin.sessionSeed;
                            break;
                        case Config.PlayStyle.PerCampaign:
                            // Try to get the seed out of the save state, or add it if not present (or if invalid)
                            string? allergySeedString = saveState.unrecognizedSaveStrings.FirstOrDefault(x => x.StartsWith("AllergySeed:"));
                            if (allergySeedString != null)
                            {
                                string[] split = allergySeedString.Split([':'], 2);
                                if (int.TryParse(split[1], out seed))
                                {
                                    Plugin.Logger.LogDebug($"Parsed seed from save: {seed}");
                                    break;
                                }
                                else
                                {
                                    Plugin.Logger.LogWarning($"FOUND INVALID SAVED ALLERGY SEED: '{split[1]}'");
                                }
                            }

                            Plugin.Logger.LogDebug($"Assigned seed to save: {seed}");
                            saveState.AddUnrecognized([$"AllergySeed:{seed}"]);
                            break;
                    }

                    if (Config.SelectedPlayStyle != Config.PlayStyle.PerCampaign)
                    {
                        // Reset seed in between per-campaign play style runs
                        saveState.unrecognizedSaveStrings.RemoveAll(x => x.StartsWith("AllergySeed:"));
                    }
                }

                Random.State oldState = Random.state;
                Random.InitState(seed);

                var allergies = RandomAllergies();
                activeAllergies.Add(player.abstractCreature, allergies);

                foreach (var allergy in allergies)
                {
                    Plugin.Logger.LogDebug($"{player.abstractCreature.ID} has allergy \"{allergy.allergen.Name}\" ({allergy.allergen.GetType().FullName}) with reaction {allergy.reactionType}");
                }

                Random.state = oldState;

                if (Config.ShowAllergens)
                {
                    var playerHSL = player.ShortCutColor().ToHSL();
                    room.AddObject(new AllergyDisplay(room, player.abstractCreature,
                        allergies.Select(x => x.allergen).ToList(),
                        (playerHSL with { lightness = Mathf.Lerp(0.55f, 1f, playerHSL.lightness) }).rgb));
                }
            }
        }

        private static List<ActiveAllergy> RandomAllergies()
        {
            List<IAllergen> unpickedAllergens = [.. allAllergens];
            List<ReactionType> weightedReactions = [];
            foreach (var (type, weight) in Config.AllWeights())
            {
                for (int i = 0; i < weight; i++)
                {
                    weightedReactions.Add(type);
                }
            }

            if (unpickedAllergens.Count == 0 || weightedReactions.Count == 0)
                return [];

            List<ActiveAllergy> allergies = [];
            int allergiesToPick = Config.AlwaysMaxAllergens ? Config.MaxAllergens : Random.Range(1, Config.MaxAllergens + 1);
            for (int i = 0; i < allergiesToPick && unpickedAllergens.Count > 0; i++)
            {
                int index = Random.Range(0, unpickedAllergens.Count);
                IAllergen allergen = unpickedAllergens[index];
                unpickedAllergens.RemoveAt(index);

                allergies.Add(new ActiveAllergy(allergen, weightedReactions[Random.Range(0, weightedReactions.Count)]));
            }

            return allergies;
        }

        internal static void Update(Player player)
        {
            if (activeAllergies.TryGetValue(player.abstractCreature, out var allergies))
            {
                foreach (var allergy in allergies)
                {
                    allergy.Update();
                }
            }
        }

        internal static void DrawSprites(Player player, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (activeAllergies.TryGetValue(player.abstractCreature, out var allergies))
            {
                foreach (var allergy in allergies)
                {
                    allergy.DrawSprites(sLeaser, rCam, timeStacker, camPos);
                }
            }
        }

        internal static void Destroy(AbstractCreature? player)
        {
            if (player != null)
            {
                if (activeAllergies.TryGetValue(player, out var allergies))
                {
                    foreach (var allergy in allergies)
                    {
                        allergy.Destroy();
                    }
                    allergies.Clear();
                }
            }
            else
            {
                List<ActiveAllergy> activeAllergiesCopy = [.. ActiveAllergy.allActiveAllergies];
                foreach (var activeAllergy in activeAllergiesCopy)
                {
                    activeAllergy.Destroy();
                }
            }
        }

        private class ActiveAllergy(IAllergen allergy, ReactionType reactionType)
        {
            public readonly IAllergen allergen = allergy;
            public readonly ReactionType reactionType = reactionType;
            public readonly List<Reaction> activeReactions = [];

            private int allergyCooldown = 0;

            internal static readonly HashSet<ActiveAllergy> allActiveAllergies = [];

            public bool TryApply(Player player, PhysicalObject? physicalObject, TriggerType trigger)
            {
                if (allergyCooldown <= 0 && !player.dead && allergen.MatchesCriteria(physicalObject, trigger) && allReactions.TryGetValue(reactionType, out var reactionFactory))
                {
#if DEBUG
                    Plugin.Logger.LogDebug($"Allergy trigger! \"{allergen.Name}\" for reaction \"{reactionType}\"");
#endif
                    allActiveAllergies.Add(this);
                    Reaction reaction = reactionFactory.Invoke(player);
                    activeReactions.Add(reaction);
                    allergyCooldown = reaction.setCooldown;
                    return true;
                }
                return false;
            }

            private readonly HashSet<Reaction> reactionsToRemove = [];
            public void Update()
            {
                foreach (var reaction in activeReactions)
                {
                    if (!reaction.slatedForDeletion)
                    {
                        reaction.Update();
                        if (!reaction.IsStillActive || reaction.slatedForDeletion)
                        {
                            reactionsToRemove.Add(reaction);
                            reaction.Destroy();
                        }
                    }
                    else
                    {
                        reactionsToRemove.Add(reaction);
                    }
                }
                activeReactions.RemoveAll(reactionsToRemove.Contains);
                reactionsToRemove.Clear();

                if (allergyCooldown > 0) allergyCooldown--;
            }

            public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                foreach (var reaction in activeReactions)
                {
                    if (!reaction.hasInitSprites)
                    {
                        reaction.hasInitSprites = true;
                        reaction.InitiateSprites(sLeaser, rCam);
                    }
                    reaction.DrawSprites(sLeaser, rCam, timeStacker, camPos);
                }
            }

            public void Destroy()
            {
                foreach (var reaction in activeReactions)
                {
                    reaction.Destroy();
                }
                activeReactions.Clear();
                allActiveAllergies.Remove(this);
            }
        }
    }
}
