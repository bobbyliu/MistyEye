using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetPreLoad
{
    private const string CARD_ASSET_PREFIX = "Assets/Data/Cards/";
    private const string BACKGROUND_PREFIX = "Assets/Data/Levels/";

    public static void PreloadAssets() {
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "GreySquare.png");
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "groundbig.png");
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "card3.png");
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "MaterialBase.png");
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "MaterialSelected.png");
        Addressables.LoadAssetAsync<Sprite>(CARD_ASSET_PREFIX + "TargetBase.png");
    }
}
