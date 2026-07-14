using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    public string baseUrl = "https://localhost:5001/api";

    private static WebManager s_instance;
    public static WebManager Instance { get { Init(); return s_instance; } }

    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("NetworkManager");
            s_instance = go.GetComponent<WebManager>();
        }
    }

	public void SendPostRequest<T>(string url, object obj, Action<T> res)
	{
		StartCoroutine(CoSendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, res));
	}

    IEnumerator CoSendWebRequest<T>(string url, string method, object obj, Action<T> res)
	{
		string sendUrl = $"{baseUrl}/{url}";

		byte[] jsonBytes = null;
		if (obj != null)
		{
			string jsonStr = JsonUtility.ToJson(obj);
			jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
		}

		using (var uwr = new UnityWebRequest(sendUrl, method))
		{
			uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");

			yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                T resObj = JsonUtility.FromJson<T>(uwr.downloadHandler.text);
                res.Invoke(resObj);
            }
		}
	}
}
