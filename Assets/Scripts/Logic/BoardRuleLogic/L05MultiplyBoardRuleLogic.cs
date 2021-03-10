using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    public class L05MultiplyBoardRuleLogicBase : BoardRuleLogicBase
    {
        public L05MultiplyBoardRuleLogicBase(LevelData level_data)
            : base(level_data)
        { }

        protected int targetNumber;
        protected int numberPerGroup;
        protected bool isAllShown;

        protected List<int> materialCardDeck;

        private int alreadyRemoved;

        public void GeneratorBase()
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount + targetCount]);
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int number_of_groups = materialCount / numberPerGroup;
            for (int card_id = 0; card_id < materialCardDeck.Count; card_id++)
            {
                int card_value = materialCardDeck[card_id];
                var new_card = isAllShown ? CardData.MaterialPublicCard(card_value) : CardData.MaterialCard(card_value);
                cardDeck[random_mapping[card_id]] = new_card;
                Debug.Log("Add card" + random_mapping[card_id]);
            }
            Debug.Log("Add card" + materialCardDeck.Count);
            cardDeck[materialCardDeck.Count] = CardData.RepeatedTargetCard(targetNumber, number_of_groups);
        }

        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            var last_card = cardDeck[cardsId[cardsId.Count - 1]];
            if (last_card.cardType != CardData.CardType.TARGET)
            {
                if (cardsId.Count > numberPerGroup)
                {
                    return JudgeState.INVALID;
                }
                return JudgeState.PENDING;
            }

            if (cardsId.Count < numberPerGroup + 1)
            {
                return JudgeState.INVALID;
            }

            int prod = 1;
            for (int i = 0; i < cardsId.Count - 1; i++)
            {
                prod *= cardDeck[cardsId[i]].cardValue;
            }
            if (prod == last_card.cardValue)
            {
                return JudgeState.VALID;
            }
            else
            {
                return JudgeState.INVALID;
            }
        }

        public override bool CheckCompletion(List<List<int>> already_removed)
        {
            return already_removed.Count == materialCount / numberPerGroup;
        }
    }

    public class L05MultiplyBoardRuleLogicEasy : L05MultiplyBoardRuleLogicBase
    {
        public L05MultiplyBoardRuleLogicEasy(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            numberPerGroup = 2;
            targetNumber = 72;
            materialCardDeck = new List<int> {1, 2, 3, 4, 6, 8, 9, 12, 18, 24, 36, 72};
            isAllShown = true;

            GeneratorBase();
        }
    }

    public class L05MultiplyBoardRuleLogicMemory : L05MultiplyBoardRuleLogicBase
    {
        public L05MultiplyBoardRuleLogicMemory(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            numberPerGroup = 2;
            targetNumber = 72;
            materialCardDeck = new List<int> { 1, 2, 3, 4, 6, 8, 9, 12, 18, 24, 36, 72 };
            isAllShown = false;

            GeneratorBase();
        }
    }

    public class L05MultiplyBoardRuleLogicCalculate : L05MultiplyBoardRuleLogicBase
    {
        public L05MultiplyBoardRuleLogicCalculate(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            numberPerGroup = 3;
            targetNumber = 120;
            List<int> candidates = new List<int> {
                1, 1, 120,
                1, 2, 60,
                1, 3, 40,
                1, 4, 30,
                1, 5, 24,
                1, 6, 20,
                1, 8, 15,
                1, 10, 12,
                2, 2, 30,
                2, 3, 20,
                2, 4, 15,
                2, 5, 12,
                2, 6, 10,
                3, 4, 10,
                3, 5, 8,
                4, 5, 6,
            };
            int candidate_group_count = candidates.Count / 3;
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(candidate_group_count);

            //            materialCardDeck = new List<int> { 1, 1, 120, 1, 2, 60, 1, 3, 40, 1, 4, 30,
            //                                               1};
            materialCardDeck = new List<int>(new int[materialCount]);
            isAllShown = true;

            for (int i = 0; i < materialCount / 3; i++)
            {
                materialCardDeck[i * 3 + 0] = candidates[random_mapping[i] * 3 + 0];
                materialCardDeck[i * 3 + 1] = candidates[random_mapping[i] * 3 + 1];
                materialCardDeck[i * 3 + 2] = candidates[random_mapping[i] * 3 + 2];
            }

            GeneratorBase();
        }
    }
}
