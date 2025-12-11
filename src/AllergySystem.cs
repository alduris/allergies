using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Allergies.Allergens;
using Allergies.Reactions;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Allergies
{
    public static class AllergySystem
    {
        internal static readonly List<IAllergen> allAllergens = [];
        internal static readonly Dictionary<ReactionType, Func<Player, Reaction>> allReactions = [];
        private static readonly ConditionalWeakTable<AbstractCreature, List<ActiveAllergy>> activeAllergies = new();

        public static void Register(IAllergen allergen)
        {
            if (allergen is null) throw new ArgumentNullException(nameof(allergen));
            if (!allAllergens.Contains(allergen))
            {
                allAllergens.Add(allergen);
            }
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
                    flag |= allergy.TryApply(player, initiator, type);
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

        internal static void Initiate(RainWorldGame game, AbstractCreature player)
        {
            if (!activeAllergies.TryGetValue(player, out _))
            {
                int seed = new System.Random().Next();
                /*if (game.IsStorySession)
                {
                    var saveState = game.GetStorySession.saveState;
                    seed = saveState.totTime * 1000 + saveState.cycleNumber * saveState.saveStateNumber.Index + player.ID.number + player.ID.spawner + Plugin.sessionSeed;
                }*/

                Random.State oldState = Random.state;
                Random.InitState(seed);

                var allergies = RandomAllergies();
                activeAllergies.Add(player, allergies);

#if DEBUG
                foreach (var allergy in allergies)
                {
                    Plugin.Logger.LogDebug($"{player.ID} has allergy \"{allergy.allergen.Name}\" ({allergy.allergen.GetType().FullName}) with reaction {allergy.reactionType}");
                }
#endif

                Random.state = oldState;
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

            List<ActiveAllergy> activeAllergies = [];
            int allergiesToPick = Random.Range(0, Config.MaxAllergens + 1);
            for (int i = 0; i < allergiesToPick && unpickedAllergens.Count > 0; i++)
            {
                int index = Random.Range(0, unpickedAllergens.Count);
                IAllergen allergen = unpickedAllergens[index];
                unpickedAllergens.RemoveAt(index);

                activeAllergies.Add(new ActiveAllergy(allergen, weightedReactions[Random.Range(0, weightedReactions.Count)]));
            }

            return activeAllergies;
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

            internal static HashSet<ActiveAllergy> allActiveAllergies = [];

            public bool TryApply(Player player, PhysicalObject? physicalObject, TriggerType trigger)
            {
                if (!player.dead && allergen.MatchesCriteria(physicalObject, trigger) && allReactions.TryGetValue(reactionType, out var reactionFactory))
                {
                    allActiveAllergies.Add(this);
                    activeReactions.Add(reactionFactory.Invoke(player));
                    allergyCooldown = 400; // 10 seconds
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
