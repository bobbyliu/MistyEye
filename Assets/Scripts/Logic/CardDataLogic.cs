using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    public enum CardStatus
    {
        REMOVED,
        NORMAL,
        SELECTED,
    }

    [Serializable]
    public class CardData
    {
        public int cardValue;
        public int remainingCount;
        // TODO: for explicit, may need multiple image name?
        public string imageName;

        public enum CardType
        {
            INVALID,
            MATERIAL,  // Material type, hidden -> show
            MATERIAL_PUBLIC,  // Material type, always show
            TARGET  // Target type, always show
        }
        public CardType cardType = CardType.INVALID;

        private Stack<int> cardValueHistory = new Stack<int>();
        private Stack<int> remainingCountHistory = new Stack<int>();

        // UICardButton should declare a linkage to this.
        public event Action onRefresh;

        private CardStatus m_cardStatus = CardStatus.REMOVED;
        public CardStatus cardStatus
        {
            get { return m_cardStatus; }
            set
            {
                m_cardStatus = value;
                // TODO: not sure if this is better to put here or all the callers.
                onRefresh();
            }
        }

        // Return true if card is fully removed; false if not.
        public void ReduceCard()
        {
            cardValueHistory.Push(cardValue);
            remainingCountHistory.Push(remainingCount);
            if (remainingCount >= 0)
            {
                remainingCount--;
            }

            Debug.Log("remainingCount=" + remainingCount);
            if (remainingCount == 0)
            {
                cardStatus = CardStatus.REMOVED;
            } else
            {
                cardStatus = CardStatus.NORMAL;
            }
            onRefresh();
            return;
        }
        public void RevertCard()
        {
            // Preset: History has size >= 1.
            cardValue = cardValueHistory.Pop();
            remainingCount = remainingCountHistory.Pop();
            cardStatus = CardStatus.NORMAL;
            onRefresh();
        }

        public static CardData MaterialCard(int cardValue)
        {
            return new CardData
            {
                cardValue = cardValue,
                cardType = CardData.CardType.MATERIAL,
                imageName = "MaterialBase.png",
                remainingCount = 1
            };
        }
        public static CardData MaterialPublicCard(int cardValue)
        {
            return new CardData
            {
                cardValue = cardValue,
                cardType = CardData.CardType.MATERIAL_PUBLIC,
                imageName = "MaterialBase.png",
                remainingCount = 1
            };
        }
        public static CardData TargetCard(int cardValue)
        {
            return new CardData
            {
                cardValue = cardValue,
                cardType = CardData.CardType.TARGET,
                imageName = "MaterialBase.png",  // Ugh..
                remainingCount = 1
            };
        }
        public static CardData RepeatedTargetCard(int cardValue, int repeat)
        {
            return new CardData
            {
                cardValue = cardValue,
                cardType = CardData.CardType.TARGET,
                imageName = "MaterialBase.png",  // Ugh..
                remainingCount = repeat
            };
        }
    }
}
