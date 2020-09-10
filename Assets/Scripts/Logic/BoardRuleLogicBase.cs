using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    [Serializable]
    public class CardData
    {
        public int cardValue;
        // TODO: for explicit, may need multiple image name?
        public string imageName;

        public enum CardType
        {
            INVALID,
            PUBLIC, // TODO: maybe useless?
            SECRET,
            MATERIAL,
            TARGET
        }
        public CardType cardType = CardType.INVALID;
    }
    [Serializable]
    public class LevelData
    {
        public int rowCount;
        public int columnCount;

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
            rowCount = level_data.rowCount;
            columnCount = level_data.columnCount;
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
            INVALID
        }

        // Determine the matching status of cardsId
        public virtual JudgeState JudgeAndFlip(List<int> cardsId)
        {
            return JudgeState.VALID;
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

        public int rowCount;
        public int columnCount;
        public List<CardData> cardDeck = new List<CardData>();
    }

    public class BoardRuleLogicUtil
    {
        public static int[] GetRandomShuffler(int size)
        {
            int[] random_mapping = new int[size];
            for (int i = 0; i < size; i++)
            {
                random_mapping[i] = i;
            }
            for (int i = 0; i < size; i++)
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