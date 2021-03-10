using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    public class L04SumBoardRuleLogicBase : BoardRuleLogicBase
    {
        public L04SumBoardRuleLogicBase(LevelData level_data)
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

            int sum = 0;
            for (int i = 0; i < cardsId.Count - 1; i++) {
                sum += cardDeck[cardsId[i]].cardValue;
            }
            if (sum == last_card.cardValue)
            {
                return JudgeState.VALID;
            } else
            {
                return JudgeState.INVALID;
            }
        }

        public override bool CheckCompletion(List<List<int>> already_removed)
        {
            return already_removed.Count == materialCount / numberPerGroup;
        }
    }

    public class L04SumBoardRuleLogicEasy : L04SumBoardRuleLogicBase
    {
        public L04SumBoardRuleLogicEasy(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            numberPerGroup = 2;
            targetNumber = 11;
            isAllShown = true;

            materialCardDeck = new List<int>(12);
            for (int i = 0; i <= 11; i++)
            {
                materialCardDeck.Add(i);
            }

            GeneratorBase();
        }
    }
    public class L04SumBoardRuleLogicMemory : L04SumBoardRuleLogicBase
    {
        public L04SumBoardRuleLogicMemory(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            numberPerGroup = 2;
            targetNumber = 11;
            isAllShown = false;

            materialCardDeck = new List<int>(12);
            for (int i = 0; i <= 11; i++)
            {
                materialCardDeck.Add(i);
            }

            GeneratorBase();
        }
    }
    public class L04SumBoardRuleLogicCalculate : L04SumBoardRuleLogicBase
    {
        public L04SumBoardRuleLogicCalculate(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            numberPerGroup = 3;
            targetNumber = 20;
            isAllShown = true;

            materialCardDeck = new List<int>(24);
            for (int i = 0; i < 8; i++)
            {
                int x1 = Random.Range(0, targetNumber);
                int x2 = Random.Range(0, targetNumber);
                if (x1 < x2)
                {
                    materialCardDeck.Add(x1);
                    materialCardDeck.Add(x2 - x1);
                    materialCardDeck.Add(targetNumber - x2);
                } else
                {
                    materialCardDeck.Add(x2);
                    materialCardDeck.Add(x1 - x2);
                    materialCardDeck.Add(targetNumber - x1);
                }
            }

            GeneratorBase();
        }
    }
}
