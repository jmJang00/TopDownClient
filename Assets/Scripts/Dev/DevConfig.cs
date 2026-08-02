using System.IO;
using UnityEngine;

[System.Serializable]
public class DevConfig
{
    public string StartScene;

    public static DevConfig Load()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "DevConfig.json");

        if (!File.Exists(path))
            return null;

        return JsonUtility.FromJson<DevConfig>(File.ReadAllText(path));
    }
}
