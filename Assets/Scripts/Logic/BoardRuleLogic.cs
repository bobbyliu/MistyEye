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
        protected int target_count = 0;

        public TargetMatchingLogic(LevelData level_data)
            : base(level_data)
        { }

        // TODO: mark abstract?
        protected virtual int Calculate(List<int> materials)
        {
            if (materials.Count != 1)
            {
                return -1;
            }
            return materials[0];
        }
            
        // Start is called before the first frame update
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
                    } else
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
            } else
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

    public class TargetSumLogic : TargetMatchingLogic
    {
        public TargetSumLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            int sum = 0;
            for (int i = 0; i < materials.Count; i++)
            {
                sum += materials[i];
            }
            return sum;
        }
    }

    public class RandomSumLogic : TargetMatchingLogic
    {
        public RandomSumLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count == 0)
            {
                return -1;
            }
            int sum = 0;
            for (int i = 0; i < materials.Count; i++)
            {
                sum += materials[i];
            }
            return sum;
        }

        // Start is called before the first frame update
        public override void Generator(List<int> param)
        {
            int material_count = param[0];
            target_count = param[1];
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(material_count);

            cardDeck = new List<logic.CardData>(new logic.CardData[target_count + material_count]);
            int material_initialized = 0;

            // TODO: change this to card type?
            for (int i = 0; i < target_count; i++)
            {
                // 2 or 3, basically.
                int matgroup = UnityEngine.Random.Range(2, 4);
                if (material_count - material_initialized <= 2 * (target_count - i))
                {
                    matgroup = 2;
                }
                int partial_sum = 0;
                for (int j = 0; j < matgroup; j++)
                {
                    int temp = UnityEngine.Random.Range(1, 99);
                    partial_sum += temp;
                    var new_card = new CardData
                    {
                        cardValue = temp,
                        cardType = CardData.CardType.MATERIAL,
                        imageName = "MaterialBase.png"  // TODO: ugh..
                    };
                    Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                    cardDeck[random_mapping[material_initialized]] = new_card;
                    material_initialized++;
                }
                var new_target = new CardData
                {
                    cardValue = partial_sum,
                    cardType = CardData.CardType.TARGET,
                    imageName = "MaterialBase.png"  // TODO: ugh..
                };
                Debug.Log("cardDeck.add " + partial_sum + " at " + (material_count + i));
                cardDeck[material_count + i] = new_target;
            }
            for (int j = material_initialized; j < material_count; j++)
            {
                int temp = UnityEngine.Random.Range(1, 99);
                var new_card = new CardData
                {
                    cardValue = temp,
                    cardType = CardData.CardType.MATERIAL,
                    imageName = "MaterialBase.png"  // TODO: ugh..
                };
                Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                cardDeck[random_mapping[material_initialized]] = new_card;
                material_initialized++;
            }
        }
    }

    public class RandomProductLogic : TargetMatchingLogic
    {
        public RandomProductLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count == 0)
            {
                return -1;
            }
            int product = 1;
            for (int i = 0; i < materials.Count; i++)
            {
                product *= materials[i];
            }
            return product;
        }

        // Start is called before the first frame update
        public override void Generator(List<int> param)
        {
            int material_count = param[0];
            target_count = param[1];
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(material_count);

            cardDeck = new List<logic.CardData>(new logic.CardData[target_count + material_count]);
            int material_initialized = 0;

            // TODO: change this to card type?
            for (int i = 0; i < target_count; i++)
            {
                // 2 or 3, basically.
                int matgroup = UnityEngine.Random.Range(2, 4);
                if (material_count - material_initialized <= 2 * (target_count - i))
                {
                    matgroup = 2;
                }
                int partial_product = 1;
                for (int j = 0; j < matgroup; j++)
                {
                    int temp = UnityEngine.Random.Range(1, 9);
                    partial_product *= temp;
                    var new_card = new CardData
                    {
                        cardValue = temp,
                        cardType = CardData.CardType.MATERIAL,
                        imageName = "MaterialBase.png"  // TODO: ugh..
                    };
                    Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                    cardDeck[random_mapping[material_initialized]] = new_card;
                    material_initialized++;
                }
                var new_target = new CardData
                {
                    cardValue = partial_product,
                    cardType = CardData.CardType.TARGET,
                    imageName = "MaterialBase.png"  // TODO: ugh..
                };
                Debug.Log("cardDeck.add " + partial_product + " at " + (material_count + i));
                cardDeck[material_count + i] = new_target;
            }
            for (int j = material_initialized; j < material_count; j++)
            {
                int temp = UnityEngine.Random.Range(1, 9);
                var new_card = new CardData
                {
                    cardValue = temp,
                    cardType = CardData.CardType.MATERIAL,
                    imageName = "MaterialBase.png"  // TODO: ugh..
                };
                Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                cardDeck[random_mapping[material_initialized]] = new_card;
                material_initialized++;
            }
        }
    }
}