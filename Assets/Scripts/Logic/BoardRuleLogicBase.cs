using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace logic
{
    [Serializable]
    public class LevelData
    {
        public int materialCount;
        public int columnCount;
        public int targetCount;

        public List<int> deckGeneratorParams;
        //        public List<GroupInfo> groups_info_;
        public string boardRuleLogicName;

//        public float TimeLimit;

        public string ruleImageName;

        public string backgroundImagePrefix;
    }

    public class BoardRuleLogicBase
    {
        public BoardRuleLogicBase(LevelData level_data)
        {
            materialCount = level_data.materialCount;
            columnCount = level_data.columnCount;
            targetCount = level_data.targetCount;
            Generator(level_data.deckGeneratorParams);
        }

        // Base class for generating stuff.
        public virtual void Generator(List<int> param)
        {

        }
        public enum JudgeState
        {
            PENDING,
            VALID,
            INVALID,
            SPECIAL
        }

        // Determine the matching status of cardsId
        public virtual JudgeState JudgeAndFlip(List<int> cardsId)
        {
            return JudgeState.VALID;
        }
        // TODO: is this universal?
        public virtual void SpecialModify(List<int> cardsId)
        {
            if (cardDeck[cardsId[0]].cardType == CardData.CardType.MODIFIER &&
                cardDeck[cardsId[0]].cardValue == 1)
            {
                cardDeck[cardsId[0]].ReduceCard();
                cardDeck[cardsId[1]].ModifyCard(cardDeck[cardsId[1]].cardValue + 1);
            }
            return;
        }
        public virtual string GetPartialText(List<int> cardsId)
        {
            return string.Join(",", cardsId.Select(id => cardDeck[id].cardValue).ToArray());
        }

        public virtual void UndoRemove() { }

        public virtual bool CheckCompletion(List<List<int>> already_removed)
        {
            int total_count = 0;
            foreach (List<int>already_removed_group in already_removed)
            {
                total_count += already_removed_group.Count;
            }
            if (total_count == cardDeck.Count)
            {
                return true;
            }
            return false;
        }

        public int materialCount;
        public int columnCount;
        public int targetCount;
        public List<CardData> cardDeck = new List<CardData>();
    }

    public class BoardRuleLogicUtil
    {
        public static int[] GetRandomShuffler(int size, int steps = -1)
        {
            int[] random_mapping = new int[size];
            if (steps == -1)
            {
                steps = size;
            }
            for (int i = 0; i < size; i++)
            {
                random_mapping[i] = i;
            }
            for (int i = 0; i < steps; i++)
            {
                int temp = UnityEngine.Random.Range(i, size);
                int valuetemp;
                valuetemp = random_mapping[temp];
                random_mapping[temp] = random_mapping[i];
                random_mapping[i] = valuetemp;
            }

            return random_mapping;
        }
    }
}