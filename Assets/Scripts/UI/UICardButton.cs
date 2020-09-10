using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICardButton : MonoBehaviour
{
    private const string CARD_ASSET_PREFIX = "Assets/Data/Cards/";
    // TODO: this is wrong.
    private const string BACKGROUND_PREFIX = "Assets/Data/Levels/";
    private Sprite backgroundImage;
    private Sprite clickableImage;
    private Sprite selectionImage;
    private string valueText;

    [SerializeField] [Tooltip("按钮")] private Button cardButton;
    [SerializeField] [Tooltip("数字")] private Text cardText;
    // Used to pass level loading status.
    public int cardId { get; set; }

    public enum CardStatus
    {
        REMOVED,
        NORMAL,
        SELECTED,
    }
    private CardStatus m_cardStatus;
    public CardStatus cardStatus
    {
        private get { return m_cardStatus; }
        set
        {
            m_cardStatus = value;
            Refresh();
        }
    }

    public void SetCardAndEnable(int i) //int card_id, string level_image)
    {
        logic.CardData card_data = LevelManager.Instance.boardRuleLogic.cardDeck[i];

        if (card_data.cardType == logic.CardData.CardType.MATERIAL)
        {
            SetMaterialCardAndEnable(i);
            return;
        }
        if (card_data.cardType == logic.CardData.CardType.TARGET)
        {
            SetTargetCardAndEnable(i);
            return;
        }

        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + card_data.imageName).Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    selectionImage = handle.Result;
                    Refresh();
                }
            };
        m_cardStatus = CardStatus.NORMAL;
        valueText = card_data.cardValue.ToString();
        cardId = i;
        Refresh();
    }

    // TODO: this should not be a separate function.
    public void SetMaterialCardAndEnable(int i) //int card_id, string level_image)
    {
        logic.CardData card_data = LevelManager.Instance.boardRuleLogic.cardDeck[i];
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "MaterialBase.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    Refresh();
                }
            };
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "MaterialSelected.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    selectionImage = handle.Result;
                    Refresh();
                }
            };
        m_cardStatus = CardStatus.NORMAL;
        valueText = card_data.cardValue.ToString();
        cardId = i;
        Refresh();
    }
    public void SetTargetCardAndEnable(int i) //int card_id, string level_image)
    {
        logic.CardData card_data = LevelManager.Instance.boardRuleLogic.cardDeck[i];
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "TargetBase.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    Refresh();
                }
            };
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "TargetBase.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    selectionImage = handle.Result;
                    Refresh();
                }
            };
        m_cardStatus = CardStatus.NORMAL;
        valueText = card_data.cardValue.ToString();
        cardId = i;
        Refresh();
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        cardStatus = CardStatus.REMOVED;
        cardButton.onClick.AddListener(ClickCard);
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "GreySquare.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    backgroundImage = handle.Result;
                    Refresh();
                }
            };
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "groundbig.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    Refresh();
                }
            };
    }
    // TODO: there must be a way to merge LevelImage_Completed with LockedImage_Completed.
    void Update()
    {
    }

    void Refresh()
    {
        if (m_cardStatus == CardStatus.NORMAL)
        {
            cardButton.GetComponent<Image>().sprite = clickableImage;
            //            cardText.text = "";
            cardText.text = valueText;
            cardButton.interactable = true;
        }
        else if (m_cardStatus == CardStatus.SELECTED)
        {
            cardButton.GetComponent<Image>().sprite = selectionImage;
            cardText.text = valueText;
            cardButton.interactable = false;
        }
        else  // m_cardStatus == CardStatus.REMOVED
        {
            cardButton.GetComponent<Image>().sprite = backgroundImage;
            cardText.text = "";
            cardButton.interactable = false;
        }
    }

    public void ClickCard()
    {
        if (LevelManager.Instance.pendingCheck)
        {
            return;
        }
        m_cardStatus = CardStatus.SELECTED;
        Refresh();

        LevelManager.Instance.FlipCard(cardId);
    }
}
