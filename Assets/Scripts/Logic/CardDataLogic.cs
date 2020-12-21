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
        public string showValue;
        public int remainingCount;

        public string clickableImageName;
        public string selectionImageName;

        public enum CardType
        {
            INVALID,
            MATERIAL,  // Material type, hidden -> show
            MATERIAL_PUBLIC,  // Material type, always show
            TARGET,  // Target type, always show
            MODIFIER  // Modifier type like +1. Always show. Card value and 
        }
        public CardType cardType = CardType.INVALID;

        public bool hideWhenRemoved = false;
        public bool showNumberWhenNormal = false;

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
        public void ModifyCard(int new_value)
        {
            cardValueHistory.Push(cardValue);
            remainingCountHistory.Push(remainingCount);

            cardValue = new_value;
            showValue = new_value.ToString();
            Debug.Log("ModifyCard=" + new_value);
            cardStatus = CardStatus.NORMAL;
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
                showValue = cardValue.ToString(),
                cardType = CardData.CardType.MATERIAL,
                clickableImageName = "MaterialBase.png",
                selectionImageName = "MaterialSelected.png",
                showNumberWhenNormal = false,
                remainingCount = 1
            };
        }
        public static CardData MaterialPublicCard(int cardValue)
        {
            return new CardData
            {
                cardValue = cardValue,
                showValue = cardValue.ToString(),
                cardType = CardData.CardType.MATERIAL_PUBLIC,
                clickableImageName = "MaterialBase.png",
                selectionImageName = "MaterialSelected.png",
                showNumberWhenNormal = true,
                remainingCount = 1
            };
        }
        public static CardData TargetCard(int cardValue)
        {
            return new CardData
            {
                cardValue = cardValue,
                showValue = cardValue.ToString(),
                cardType = CardData.CardType.TARGET,
                clickableImageName = "TargetBase.png",
                selectionImageName = "TargetBase.png",  // Hmm
                showNumberWhenNormal = true,
                remainingCount = 1
            };
        }
        public static CardData RepeatedTargetCard(int cardValue, int repeat)
        {
            return new CardData
            {
                cardValue = cardValue,
                showValue = cardValue.ToString(),
                cardType = CardData.CardType.TARGET,
                clickableImageName = "TargetBase.png",
                selectionImageName = "TargetBase.png",  // Hmm
                showNumberWhenNormal = true,
                remainingCount = repeat
            };
        }

        // TODO: maybe this should be able to check all special cards.
        public static CardData SpecialCardPlusOne(int repeat)
        {
            return new CardData
            {
                cardValue = 1,
                showValue = "+1",
                cardType = CardData.CardType.MODIFIER,
                clickableImageName = "TargetBase.png",
                selectionImageName = "TargetBase.png",  // Hmm
                showNumberWhenNormal = true,
                remainingCount = repeat
            };
        }
    }
}
