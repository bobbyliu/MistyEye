using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    public class L03SameLadderBoardRuleLogicBase : BoardRuleLogicBase
    {
        public L03SameLadderBoardRuleLogicBase(LevelData level_data)
            : base(level_data)
        { }

        protected int range;
        protected bool isAllShown;

        public void GeneratorBase()
        {
            cardDeck = new List<logic.CardData>(new logic.CardData[materialCount]);
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);
            int card_count = 0;
            for (int card_value = 1; card_value <= range; card_value++)
            {
                for (int card_id = 0; card_id < card_value; card_id++)
                {
                    var new_card = isAllShown ? CardData.MaterialPublicCard(card_value) : CardData.MaterialCard(card_value);
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
            if (first_card.cardValue != last_card.cardValue)
            {
                return JudgeState.INVALID;
            }
            if (cardsId.Count < first_card.cardValue)
            {
                return JudgeState.PENDING;
            }

            return JudgeState.VALID;
        }
    }

    public class L03SameLadderBoardRuleLogicEasy : L03SameLadderBoardRuleLogicBase
    {
        public L03SameLadderBoardRuleLogicEasy(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            range = 5;
            isAllShown = true;

            GeneratorBase();
        }
    }
    public class L03SameLadderBoardRuleLogicMemory : L03SameLadderBoardRuleLogicBase
    {
        public L03SameLadderBoardRuleLogicMemory(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            range = 5;
            isAllShown = false;

            GeneratorBase();
        }
    }
    public class L03SameLadderBoardRuleLogicCalculate : L03SameLadderBoardRuleLogicBase
    {
        public L03SameLadderBoardRuleLogicCalculate(LevelData level_data)
            : base(level_data)
        { }
        public override void Generator(List<int> param)
        {
            range = 8;
            isAllShown = true;

            GeneratorBase();
        }
    }
}
