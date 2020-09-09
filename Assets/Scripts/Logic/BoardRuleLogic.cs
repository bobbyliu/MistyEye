using System.Collections;
using System.Collections.Generic;
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
                        imageName = param[i] + ".png"
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
        private List<List<int>> groups = new List<List<int>>();

        private List<int> material_cards = new List<int>();
        private List<int> target_cards = new List<int>();

        // Record a list of groups that have been removed, so that we don't match them in the future. 
        private List<int> flipped_id = new List<int>();

        public TargetMatchingLogic(LevelData level_data)
            : base(level_data)
        { }

        // TODO: mark abstract?
        protected virtual int Calculate(List<int> materials)
        {
            return 0;
        }
            
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
                    var new_card = new CardData
                    {
                        cardValue = param[i],
                        cardType = CardData.CardType.SECRET,
                        imageName = param[i] + ".png"
                    };

                    Debug.Log("cardDeck.add " + param[i] + " at " + random_mapping[card_count]);
                    cardDeck[random_mapping[card_count]] = new_card;
                    current_group.Add(param[i]);
                    card_count++;
                }
                else
                {
                    groups.Add(current_group);
                    current_group = new List<int>();
                }
            }
        }

        public override JudgeState JudgeAndFlip(List<int> cardsId)
        {
            bool partial_match_found = false;
            for (int group_index = 0; group_index < groups.Count; group_index++)
            {
                if (flipped_id.Contains(group_index))
                {
                    continue;
                }
                if (cardsId.Count > groups[group_index].Count)
                {
                    continue;
                }
                bool group_partial_match = true;
                for (int i = 0; i < cardsId.Count; i++)
                {
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

        public override void UndoRemove()
        {
            flipped_id.RemoveAt(flipped_id.Count - 1);
        }

        public override bool CheckCompletion(List<List<int>> already_removed)
        {
            return already_removed.Count == groups.Count;
        }

    }
}