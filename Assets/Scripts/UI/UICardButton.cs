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

    // TODO: check if we should use this or cardType directly?
    private bool hideWhenRemoved;
    private bool showNumberWhenNormal;

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
        if (cardData.cardType == logic.CardData.CardType.MATERIAL)
        {
            SetMaterialCardAndEnable(i);
            showNumberWhenNormal = false;
            return;
        }
        if (cardData.cardType == logic.CardData.CardType.MATERIAL_PUBLIC)
        {
            SetMaterialCardAndEnable(i);
            showNumberWhenNormal = true;
            return;
        }
        if (cardData.cardType == logic.CardData.CardType.TARGET)
        {
            SetTargetCardAndEnable(i);
            showNumberWhenNormal = true;
            return;
        }

        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + cardData.imageName).Completed +=
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

    // TODO: this should not be a separate function.
    public void SetMaterialCardAndEnable(int i)
    {
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "MaterialBase.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    // TODO: is this too many Refresh calls? Maybe do this only at Refresh time? 
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
        cardData.cardStatus = logic.CardStatus.NORMAL;
        cardId = i;
        Refresh();
    }
    public void SetTargetCardAndEnable(int i)
    {
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
            if (hideWhenRemoved)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        string show_data = cardData.cardValue.ToString();
        if (cardData.remainingCount > 1)
        {
            show_data = string.Format("{0}({1})", show_data, cardData.remainingCount);
        }

        if (cardData.cardStatus == logic.CardStatus.NORMAL)
        {
            cardButton.GetComponent<Image>().sprite = clickableImage;
            if (showNumberWhenNormal)
            {
                cardText.text = show_data;
            } else
            {
                cardText.text = "";
            }
            cardButton.interactable = true;
            if (hideWhenRemoved)
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
