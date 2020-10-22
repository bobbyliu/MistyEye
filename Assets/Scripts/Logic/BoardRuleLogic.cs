using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace logic
{

    public class OrderedGroupMatcherLogic : BoardRuleLogicBase
    {
        public OrderedGroupMatcherLogic(LevelData level_data)
            : base(level_data)
        { }

        // Start is called before the first frame update
        public override void Generator(List<int> param)
        {
            List<int> current_group = new List<int>();

            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(rowCount * columnCount);

            cardDeck = new List<logic.CardData>(new logic.CardData[rowCount * columnCount]);
            int card_count = 0;
            for (int i = 0; i < param.Count; i++)
            {
                if (param[i] != 0)
                {
                    var new_card = new CardData {
                        cardValue = param[i],
                        cardType = CardData.CardType.SECRET,
                        imageName = "MaterialBase.png"
//                        imageName = param[i] + ".png"
                    };

                    Debug.Log("cardDeck.add " + param[i] + " at " + random_mapping[card_count]);
                    cardDeck[random_mapping[card_count]] = new_card;
                    current_group.Add(param[i]);
                    card_count++;
                } else
                {
                    groups.Add(current_group);
                    current_group = new List<int>();
                }
            }
        }

        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            bool partial_match_found = false;
            for (int group_index = 0; group_index < groups.Count; group_index++) {
                if (flipped_id.Contains(group_index))
                {
                    continue;
                }
                if (cardsId.Count > groups[group_index].Count)
                {
                    continue;
                }
                bool group_partial_match = true;
                for (int i = 0; i < cardsId.Count; i++) {
                    if (cardDeck[cardsId[i]].cardValue != groups[group_index][i])
                    {
                        group_partial_match = false;
                        break;
                    }
                }
                if (!group_partial_match)
                {
                    continue;
                }
                partial_match_found = true;
                // Fully matched. 
                if (cardsId.Count == groups[group_index].Count)
                {
                    flipped_id.Add(group_index);
                    return JudgeState.VALID;
                }
            }
            if (partial_match_found)
            {
                return JudgeState.PENDING;
            }
            return JudgeState.INVALID;
        }

        public override void UndoRemove() {
            flipped_id.RemoveAt(flipped_id.Count - 1);
        }

        public override bool CheckCompletion(List<List<int>> already_removed)
        {
            return already_removed.Count == groups.Count;
        }

        private List<List<int>> groups = new List<List<int>>();

        // Record a list of groups that have been removed, so that we don't match them in the future. 
        private List<int> flipped_id = new List<int>();
    }

    public class TargetMatchingLogic : BoardRuleLogicBase
    {
        protected int target_count = 0;

        public TargetMatchingLogic(LevelData level_data)
            : base(level_data)
        { }

        // TODO: mark abstract?
        protected virtual int Calculate(List<int> materials)
        {
            if (materials.Count != 1)
            {
                return -1;  // TODO: check if we can return a status.
            }
            return materials[0];
        }
        protected virtual bool PrematureFail(List<int> materials)
        {
            return false;
        }

        // Generator param list: material_count, target_count, {material_list}, 0, {target_list}
        public override void Generator(List<int> param)
        {
            int material_count = param[0];
            target_count = param[1];
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(material_count);

            cardDeck = new List<logic.CardData>(new logic.CardData[target_count + material_count]);
            int card_count = 0;

            // TODO: change this to card type?
            int phase = 1;
            for (int i = 2; i < param.Count; i++)
            {
                if (param[i] != 0)
                {
                    var new_card = new CardData
                    {
                        cardValue = param[i],
                        cardType = (phase == 1) ? CardData.CardType.MATERIAL : CardData.CardType.TARGET,
                        imageName = "MaterialBase.png"  // TODO: ugh..
                    };

                    if (phase == 1)
                    {
                        Debug.Log("cardDeck.add " + param[i] + " at " + random_mapping[card_count]);
                        cardDeck[random_mapping[card_count]] = new_card;
                    }
                    else
                    {
                        Debug.Log("cardDeck.add " + param[i] + " at " + card_count);
                        cardDeck[card_count] = new_card;
                    }

                    card_count++;
                }
                else
                {
                    phase = 2;
                }
            }
        }

        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
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
            return already_removed.Count == target_count;
        }
    }
}