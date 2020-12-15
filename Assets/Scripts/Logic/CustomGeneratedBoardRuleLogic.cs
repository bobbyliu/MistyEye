using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace logic
{
    public class ModBoardRuleLogic : BoardRuleLogicBase
    {
        public ModBoardRuleLogic(LevelData level_data)
            : base(level_data)
        { }

        private int modBase;
        private int numberPerGroup;

        // Parameter format: [Mod, NumberPerGroup, MinMultiplier, MaxMultiplier]
        // Number range is [MinMultiplier*Mod, MaxMultiplier*Mod)
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount]);

            modBase = param[0];
            numberPerGroup = param[1];
            int min_multiplier = param[2];
            int max_multiplier = param[3];
            int number_of_groups = materialCount / numberPerGroup;

            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int[] mod_group = BoardRuleLogicUtil.GetRandomShuffler(modBase, number_of_groups);
            int card_count = 0;
            for (int group_id = 0; group_id < number_of_groups; group_id++)
            {
                int[] multiplier = BoardRuleLogicUtil.GetRandomShuffler(max_multiplier - min_multiplier, numberPerGroup);
                for (int card_id = 0; card_id < numberPerGroup; card_id++)
                {
                    int card_value = (multiplier[card_id] + min_multiplier) * modBase + mod_group[group_id];
                    var new_card = CardData.MaterialCard(card_value);
                    Debug.Log("cardDeck.add " + card_value + " at " + card_count);
                    cardDeck[random_mapping[card_count]] = new_card;
                    card_count++;
                }
            }
        }
        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            var first_card = cardDeck[cardsId[0]];
            var last_card = cardDeck[cardsId[cardsId.Count - 1]];
            if (first_card.cardValue % modBase != last_card.cardValue % modBase)
            {
                return JudgeState.INVALID;
            }
            if (cardsId.Count < numberPerGroup)
            {
                return JudgeState.PENDING;
            }

            return JudgeState.VALID;
        }
    }

    public class DivisionRuleLogic : TargetMatchingLogic
    {
        public DivisionRuleLogic(LevelData level_data)
            : base(level_data)
        { }

        private int modBase;

        // Parameter format: [Mod, Min, Max]
        // Number range is [Min, Max)
        // Material count = target count * 2
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount + targetCount]);

            modBase = param[0];
            int min = param[1];
            int max = param[2];
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int[] target_value_mapping = BoardRuleLogicUtil.GetRandomShuffler(max - min, targetCount);
            for (int i = 0; i < targetCount; i++)
            {
                int card_value = target_value_mapping[i] + min;
                cardDeck[random_mapping[i * 2]] = CardData.MaterialCard(card_value % modBase);
                cardDeck[random_mapping[i * 2 + 1]] = CardData.MaterialCard(card_value / modBase);
                cardDeck[materialCount + i] = CardData.TargetCard(card_value);
            }
        }
        protected override int Calculate(List<int> materials)
        {
            if (materials.Count != 2)
            {
                return -1;  // TODO: check if we can return a status.
            }
            return materials[0] * modBase + materials[1];
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 2)
            {
                return true;
            }
            return false;
        }
    }

    public class StepUpNLogic : TargetMatchingLogic
    {
        public StepUpNLogic(LevelData level_data)
            : base(level_data)
        { }

        private int step;
        // Parameter format: [Min, Max, N]
        // Target count = Max - Min
        // Material count = Target count * N
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount + targetCount]);

            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int min = param[0];
            int max = param[1];
            step = param[2];
            int card_count = 0;
            for (int i = min; i <= max; i++)
            {
                for (int j = 1; j <= step; j++)
                {
                    cardDeck[random_mapping[card_count]] = CardData.MaterialCard(i - j);
                    card_count++;
                }
                cardDeck[materialCount + i - min] = CardData.TargetCard(i);
            }
        }
        protected override int Calculate(List<int> materials)
        {
            if (materials.Count != step)
            {
                return -1;
            }
            return materials[materials.Count - 1] + 1;
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials[materials.Count - 1] != materials[0] + materials.Count - 1)
            {
                return true;
            }
            if (materials.Count > step)
            {
                return true;
            }
            return false;
        }
    }

    public class StepUpLogic : TargetMatchingLogic
    {
        public StepUpLogic(LevelData level_data)
            : base(level_data)
        { }
        // Parameter format: [Max]
        // Target count = Max
        // Material count = Max * (Max + 1) / 2
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount + targetCount]);

            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int max = param[0];
            int card_count = 0;
            for (int i = 1; i <= max; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    cardDeck[random_mapping[card_count]] = CardData.MaterialCard(j);
                    card_count++;
                }
                cardDeck[materialCount + i - 1] = CardData.TargetCard(i);
            }
        }
        protected override int Calculate(List<int> materials)
        {
            return materials.Count;
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials[materials.Count - 1] != materials.Count - 1)
            {
                return true;
            }
            return false;
        }
    }

    public class NumberReverseLogic : BoardRuleLogicBase
    {
        public NumberReverseLogic(LevelData level_data)
            : base(level_data)
        { }

        private int NumberReverse(int num)
        {
            int temp = 0;
            while (num > 0)
            {
                temp = temp * 10 + num % 10;
                num = num / 10;
            }
            return temp;
        }

        // Parameter format: [Min, Max)
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount]);
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);

            int min = param[0];
            int max = param[1];
            int number_of_groups = materialCount / 2;

            HashSet<int> dupe = new HashSet<int>();

            for (int i = 0; i < number_of_groups; i++)
            {
                int main = UnityEngine.Random.Range(min, max);
                while (main % 10 == 0 || dupe.Contains(main)) {
                    main = UnityEngine.Random.Range(min, max);
                }
                int reverse = NumberReverse(main);
                dupe.Add(main);
                dupe.Add(reverse);

                cardDeck[random_mapping[i * 2]] = CardData.MaterialCard(main);
                cardDeck[random_mapping[i * 2 + 1]] = CardData.MaterialCard(reverse);
            }
        }
        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            if (cardsId.Count < 2)
            {
                return JudgeState.PENDING;
            }
            var first_card = cardDeck[cardsId[0]];
            var last_card = cardDeck[cardsId[1]];
            if (NumberReverse(first_card.cardValue) == last_card.cardValue)
            {
                return JudgeState.VALID;
            } else
            {
                return JudgeState.INVALID;
            }
        }
    }

    public class NumberCycleLogic : BoardRuleLogicBase
    {
        public NumberCycleLogic(LevelData level_data)
            : base(level_data)
        { }

        private int NumberCycle(int num1, int num2)
        {
            if (num1 % 10 == num2 / 10)
            {
                return num1 / 10 + (num2 % 10) * 10;
            } else if (num1 / 10 == num2 % 10)
            {
                return num2 / 10 + (num1 % 10) * 10;
            }
            return -1;
        }

        private bool Validate(int num)
        {
            return num % 10 != 0;
        }

        // No parameter. Fixed at 11-99.
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount]);
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int number_of_groups = materialCount / 3;

            // Range is 11-99, so 0-88.
            const int randomizer_size = 89;

            int group_count = 0;
            while (group_count < number_of_groups)
            {
                group_count = 0;
                int[] randomizer = BoardRuleLogicUtil.GetRandomShuffler(randomizer_size);  // When using, + 11
                HashSet<int> previous = new HashSet<int>();
                HashSet<int> forbidden = new HashSet<int>();
                for (int randomizer_index = 0; randomizer_index < randomizer_size; randomizer_index++)
                {
                    int main = randomizer[randomizer_index] + 11;
                    if (forbidden.Contains(main) || !Validate(main))
                    {
                        continue;
                    }
                    int[] randomizer_small = BoardRuleLogicUtil.GetRandomShuffler(9);
                    int group2 = 0, group3 = 0;
                    bool found_group = false;
                    for (int randomizer_small_index = 0; randomizer_small_index < 9; randomizer_small_index++)
                    {
                        int auxillary = randomizer_small[randomizer_small_index] + 1;  // 1 to 9
                        group2 = main / 10 + auxillary * 10;
                        group3 = (main % 10) * 10 + auxillary;
                        if (!forbidden.Contains(group2) && !forbidden.Contains(group3))
                        {
                            found_group = true;
                            break;
                        }
                    }
                    if (!found_group)
                    {
                        continue;
                    }

                    forbidden.Add(main);
                    forbidden.Add(group2);
                    forbidden.Add(group3);
                    previous.Add(main);
                    previous.Add(group2);
                    previous.Add(group3);
                    foreach (int item in previous)
                    {
                        forbidden.Add(NumberCycle(item, main));
                        forbidden.Add(NumberCycle(item, group2));
                        forbidden.Add(NumberCycle(item, group3));
                    }

                    cardDeck[random_mapping[group_count * 3]] = CardData.MaterialPublicCard(main);
                    cardDeck[random_mapping[group_count * 3 + 1]] = CardData.MaterialPublicCard(group2);
                    cardDeck[random_mapping[group_count * 3 + 2]] = CardData.MaterialPublicCard(group3);
                    group_count++;
                    if (group_count >= number_of_groups)
                    {
                        break;
                    }
                }
                Debug.Log("Try initializing NumberCycleLogic with " + group_count + " groups.");
            }
        }

        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            if (cardsId.Count == 1)
            {
                return JudgeState.PENDING;
            }
            if (cardsId.Count == 2)
            {
                if (NumberCycle(cardDeck[cardsId[0]].cardValue, cardDeck[cardsId[1]].cardValue) == -1)
                {
                    return JudgeState.INVALID;
                } else
                {
                    return JudgeState.PENDING;
                }
            }
            if (NumberCycle(cardDeck[cardsId[0]].cardValue, cardDeck[cardsId[1]].cardValue) ==
                cardDeck[cardsId[2]].cardValue ||
                NumberCycle(cardDeck[cardsId[1]].cardValue, cardDeck[cardsId[0]].cardValue) ==
                cardDeck[cardsId[2]].cardValue)
            {
                return JudgeState.VALID;
            } else
            {
                return JudgeState.INVALID;
            }
        }
    }

    public class MixedNineLogic : BoardRuleLogicBase
    {
        public MixedNineLogic(LevelData level_data)
            : base(level_data)
        { }

        // Return unique digits within the group. Return -1 if there is a dupe.
        private int MixedNine(List<int> nums)
        {
            bool[] hasNum = new bool[10];
            int totes = 0;
            foreach (int num in nums)
            {
                int temp = num;
                while (temp > 0)
                {
                    int digit = temp % 10;
                    // Dupe
                    if (hasNum[digit])
                    {
                        return -1;
                    } else
                    {
                        hasNum[digit] = true;
                        totes++;
                    }
                    temp = temp / 10;
                }
            }
            return totes;
        }
        private int AssembleNumber(int[] num, int start, int end)
        {
            int temp = 0;
            for (int i = start; i < end; i++)
            {
                temp = temp * 10 + num[i] + 1;
            }
            return temp;
        }

        private int maxNum;
        // Parameter format: maxNum, number_per_group;
        public override void Generator(List<int> param)
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount]);
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);

            maxNum = param[0];
            int number_per_group = param[1];
            int number_of_groups = materialCount / number_per_group;

            HashSet<int> dupe = new HashSet<int>();

            for (int i = 0; i < number_of_groups; i++)
            {
                List<int> assembled = new List<int>(new int[number_per_group]);
                bool found_valid_group = false;
                while (!found_valid_group)
                {
                    found_valid_group = true;
                    int[] randomizer = BoardRuleLogicUtil.GetRandomShuffler(maxNum);  // +1
                    for (int j = 0; j < number_per_group; j++)
                    {
                        int value = AssembleNumber(randomizer,
                            (maxNum * j) / number_per_group, (maxNum * (j+1)) / number_per_group);
                        assembled[j] = value;
                        if (dupe.Contains(assembled[j]))
                        {
                            found_valid_group = false;
                            break;
                        }
                    }
                }

                for (int j = 0; j < number_per_group; j++)
                {
                    cardDeck[random_mapping[i * number_per_group + j]] = CardData.MaterialPublicCard(assembled[j]);
                    dupe.Add(assembled[j]);
                }
            }
        }
        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            int totes = MixedNine(cardsId.Select(id => cardDeck[id].cardValue).ToList());
            if (totes < 0)
            {
                return JudgeState.INVALID;
            }
            if (totes < maxNum)
            {
                return JudgeState.PENDING;
            }
            else
            {
                return JudgeState.VALID;
            }
        }
    }
}
