using System.Collections.Generic;
using UnityEngine;

public struct PlayerResult
{
    public string Nickname;
    public int Level;
    public int Score;
    public int Kill;
    public int Exp;
}

public class UI_GameResult : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private UI_GameResultItem _itemPrefab;

    private readonly List<UI_GameResultItem> _items = new();
    private readonly List<PlayerResult> _sortedResults = new();

    public void SetResults(IReadOnlyList<PlayerResult> results)
    {
        Clear();

        _sortedResults.Clear();

        for (int i = 0; i < results.Count; ++i)
        {
            _sortedResults.Add(results[i]);
        }

        _sortedResults.Sort((a, b) => b.Score.CompareTo(a.Score));

        for (int i = 0; i < _sortedResults.Count; ++i)
        {
            UI_GameResultItem item = Instantiate(_itemPrefab, _content);

            item.SetData(
                _sortedResults[i].Level,
                _sortedResults[i].Nickname,
                _sortedResults[i].Score,
                _sortedResults[i].Kill,
                _sortedResults[i].Exp
            );

            _items.Add(item);
        }
    }

    private void Clear()
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            Destroy(_items[i].gameObject);
        }

        _items.Clear();
    }
}
