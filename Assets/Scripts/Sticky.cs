using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Sticky : MonoBehaviour {
    public static int CurrentLevel = 0;
    public static string CurrentLevelName = "level3";

    public static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, TextAsset> LoadedTextAssets = new Dictionary<string, TextAsset>();
    public static Dictionary<string, AssetBundleLoadAssetOperation> RunningRequests = new Dictionary<string, AssetBundleLoadAssetOperation>();

    public Button TitleButton;

    public static LevelList level_list;

    public static List<int> UnlockedLevels;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return StartCoroutine(Initialize());
        Load();
        TitleButton.interactable = true;
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        AssetBundleManager.SetDevelopmentAssetBundleServer();
#else
		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		AssetBundleManager.SetSourceAssetBundleDirectory("/" + Utility.GetPlatformName() + "/");
		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
#endif

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();
        if (request != null)
        {
            yield return StartCoroutine(request);
        }
    }

    static public void LoadSpriteAsync(string bundle_name, string item_name)
    {
        string index = bundle_name + ":" + item_name;

        Sprite try_sprite;
        LoadedSprites.TryGetValue(index, out try_sprite);
        if (try_sprite != null)
        {
            return;
        }

        AssetBundleLoadAssetOperation try_operation;
        Sticky.RunningRequests.TryGetValue(index, out try_operation);
        if (try_operation != null)
        {
            return;
        }

        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(
            bundle_name, item_name, typeof(Sprite));
        RunningRequests.Add(index, request);
    }

    static public Sprite GetSprite(string bundle_name, string item_name)
    {
        string index = bundle_name + ":" + item_name;

        // Sprite is already loaded
        Sprite try_sprite;
        LoadedSprites.TryGetValue(index, out try_sprite);
        if (try_sprite != null)
        {
            return try_sprite;
        }

        AssetBundleLoadAssetOperation try_operation;
        RunningRequests.TryGetValue(index, out try_operation);
        if (try_operation != null)
        {
            try_sprite = try_operation.GetAsset<Sprite>();
            if (try_sprite == null)
            {
                return null;
            } else
            {
                LoadedSprites.Add(index, try_sprite);
                RunningRequests.Remove(index);
                return try_sprite;
            }
        }

        LoadSpriteAsync(bundle_name, item_name);
        return null;
    }

    static public void LoadTextAssetAsync(string bundle_name, string item_name)
    {
        string index = bundle_name + ":" + item_name;

        TextAsset try_sprite;
        LoadedTextAssets.TryGetValue(index, out try_sprite);
        if (try_sprite != null)
        {
            return;
        }

        AssetBundleLoadAssetOperation try_operation;
        Sticky.RunningRequests.TryGetValue(index, out try_operation);
        if (try_operation != null)
        {
            return;
        }

        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(
            bundle_name, item_name, typeof(TextAsset));
        RunningRequests.Add(index, request);
    }

    static public TextAsset GetTextAsset(string bundle_name, string item_name)
    {
        string index = bundle_name + ":" + item_name;

        // Sprite is already loaded
        TextAsset try_sprite;
        LoadedTextAssets.TryGetValue(index, out try_sprite);
        if (try_sprite != null)
        {
            return try_sprite;
        }

        AssetBundleLoadAssetOperation try_operation;
        RunningRequests.TryGetValue(index, out try_operation);
        if (try_operation != null)
        {
            try_sprite = try_operation.GetAsset<TextAsset>();
            if (try_sprite == null)
            {
                return null;
            }
            else
            {
                LoadedTextAssets.Add(index, try_sprite);
                RunningRequests.Remove(index);
                return try_sprite;
            }
        }

        LoadTextAssetAsync(bundle_name, item_name);
        return null;
    }

    static public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.OpenOrCreate);

        PlayerData data = new PlayerData();
        data.UnlockedLevels = Sticky.UnlockedLevels;
        bf.Serialize(file, data);
        file.Close();
    }

    static public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            Sticky.UnlockedLevels = data.UnlockedLevels;
        } else
        {
            Sticky.UnlockedLevels = new List<int>();
            Sticky.UnlockedLevels.Add(1);
        }
    }
}

[System.Serializable]
class PlayerData
{
    public List<int> UnlockedLevels;
}
