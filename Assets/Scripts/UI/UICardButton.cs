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

    [SerializeField] [Tooltip("按钮")] private Button cardButton;
    [SerializeField] [Tooltip("数字")] private Text cardText;
    // Used to pass level loading status.
    public int cardId { get; set; }

    private logic.CardData cardData;


    private void OnDestroy()
    {
        if (cardData != null)
        {
            cardData.onRefresh -= Refresh;
        }
    }

    public void BindCardData(logic.CardData card_data)
    {
        cardData = card_data;
        card_data.onRefresh += Refresh;
    }

    public void SetCardAndEnable(int i)
    {
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + cardData.clickableImageName).Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    Refresh();
                }
            };
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + cardData.selectionImageName).Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    selectionImage = handle.Result;
                    Refresh();
                }
            };
        cardData.cardStatus = logic.CardStatus.NORMAL;
        cardId = i;
        Refresh();
    }
    
    // Start is called before the first frame update
    void Awake()
    {
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
        if (cardData == null || cardData.cardStatus == logic.CardStatus.REMOVED)
        {
            cardButton.GetComponent<Image>().sprite = backgroundImage;
            cardText.text = "";
            cardButton.interactable = false;
            if (cardData != null && cardData.hideWhenRemoved)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        string show_data = cardData.showValue;
        if (cardData.remainingCount > 1)
        {
            show_data = string.Format("{0}({1})", show_data, cardData.remainingCount);
        }

        if (cardData.cardStatus == logic.CardStatus.NORMAL)
        {
            cardButton.GetComponent<Image>().sprite = clickableImage;
            if (cardData.showNumberWhenNormal)
            {
                cardText.text = show_data;
            } else
            {
                cardText.text = "";
            }
            cardButton.interactable = true;
            if (cardData.hideWhenRemoved)
            {
                gameObject.SetActive(true);
            }
        }
        else // m_cardStatus == logic.CardStatus.SELECTED
        {
            cardButton.GetComponent<Image>().sprite = selectionImage;
            cardText.text = show_data;
            cardButton.interactable = false;
        }

    }

    public void ClickCard()
    {
        if (LevelManager.Instance.pendingCheck)
        {
            return;
        }
        cardData.cardStatus = logic.CardStatus.SELECTED;
        Refresh();

        LevelManager.Instance.FlipCard(cardId);
    }
}
