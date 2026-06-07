using MoreMountains.Tools;
using UnityEngine;

public class GameMatch : MonoBehaviour
{
    private void Update()
    {
        if (NetworkManager.State == NetworkState.Connected)
        {
            //TODO: 네트워크 상태에 따른 버튼 Enalbe, Disable
        }
    }

    public void MatchStart()
    {
        StartCoroutine(NetworkManager.Instance.FindGame());
    }
}
