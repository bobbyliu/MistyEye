using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace logic
{
    public class Product3ModifierLogic : BoardRuleLogicBase
    {
        public Product3ModifierLogic(LevelData level_data)
            : base(level_data)
        { }

        protected int Calculate(List<int> materials)
        {
            if (materials.Count != 3)
            {
                return -1;
            }
            return materials[0] * materials[1] * materials[2];
        }
        protected bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 3)
            {
                return true;
            }
            return false;
        }

        int modifierCount = 0;
        // Generator param list: {material_list}, {target_list}, {+1 count}
        public override void Generator(List<int> param)
        {
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(materialCount);

            cardDeck = new List<logic.CardData>(new logic.CardData[targetCount + materialCount]);

            int plus_one = param[0];
            for (int card_count = 0; card_count < param.Count - 1; card_count++)
            {
                var new_card = (card_count < materialCount) ?
                    CardData.MaterialPublicCard(param[card_count]) : CardData.TargetCard(param[card_count]);

                if (card_count < materialCount)
                {
                    Debug.Log("cardDeck.add " + param[card_count] + " at " + random_mapping[card_count]);
                    cardDeck[random_mapping[card_count]] = new_card;
                }
                else
                {
                    Debug.Log("cardDeck.add " + param[card_count] + " at " + card_count);
                    cardDeck[card_count] = new_card;
                }
            }
            modifierCount = param[param.Count - 1];
            var new_modifier = CardData.SpecialCardPlusOne(modifierCount);
            cardDeck[cardDeck.Count - 1] = new_modifier;
        }

        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            bool is_special_modifier = false;
            foreach (int card_id in cardsId)
            {
                if (cardDeck[card_id].cardType == CardData.CardType.MODIFIER)
                {
                    is_special_modifier = true;
                }
            }
            if (is_special_modifier)
            {
                if (cardsId.Count == 1)
                {
                    return JudgeState.PENDING;
                }
                if (cardsId.Count > 2)
                {
                    return JudgeState.INVALID;
                }
                if (cardDeck[cardsId[0]].cardType == CardData.CardType.MODIFIER &&
                    cardDeck[cardsId[0]].cardValue == 1 &&
                    cardDeck[cardsId[1]].cardType == CardData.CardType.MATERIAL_PUBLIC)
                {
                    return JudgeState.SPECIAL;
                } else
                {
                    return JudgeState.INVALID;
                }
            }
            
            var last_card = cardDeck[cardsId[cardsId.Count - 1]];
            if (last_card.cardType != CardData.CardType.TARGET)
            {
                if (PrematureFail(cardsId.Select(id => cardDeck[id].cardValue).ToList()))
                {
                    return JudgeState.INVALID;
                }
                return JudgeState.PENDING;
            }

            // TODO: this copy is pointless. Change to passing cards or even original 
            List<int> card_value_list = new List<int>(cardsId.Count - 1);
            for (int i = 0; i < cardsId.Count - 1; i++)
            {
                card_value_list.Add(cardDeck[cardsId[i]].cardValue);
            }
            if (Calculate(card_value_list) == last_card.cardValue)
            {
                return JudgeState.VALID;
            }
            else
            {
                return JudgeState.INVALID;
            }
        }

        public override void UndoRemove()
        {
        }

        public override bool CheckCompletion(List<List<int>> already_removed)
        {
            // TODO: ugh, ugly..
            return already_removed.Count == targetCount + modifierCount - 1;
        }
    }
}