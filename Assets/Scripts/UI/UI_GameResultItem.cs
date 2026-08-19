using UnityEngine;
using TMPro;

public class UI_GameResultItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _killText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _expText;

    public void SetData(int level, string nickname, int score, int kill, int exp)
    {
        _nameText.text = $"{nickname}({level})";
        _killText.text = kill.ToString();
        _scoreText.text = score.ToString("N0");
        _expText.text = exp.ToString("N0");
    }
}
