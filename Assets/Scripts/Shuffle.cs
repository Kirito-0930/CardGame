using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    List<int> cardsIndex;

    /// <summary>ゲーム開始時のシャッフル</summary>
    /// <returns>0~52のシャッフルされたインデックスを返す</returns>
    public List<int> CardShuffle()
    {
        if (cardsIndex == null) cardsIndex = new List<int>();
        else cardsIndex.Clear();

        for (int i = 0; i < 53; i++) {
            cardsIndex.Add(i);
        }

        var indexCount = cardsIndex.Count;

        while (indexCount > 1) {   //トランプ入れ替えの処理
            indexCount--;
            var random = Random.Range(0, indexCount + 1);
            var tmp = cardsIndex[random];
            cardsIndex[random] = cardsIndex[indexCount];
            cardsIndex[indexCount] = tmp;
        }

        return cardsIndex;
    }

    /// <summary>プレイヤーがゲーム中にするシャッフル</summary>
    /// <param name="playercards">プレイヤーの手札が渡される</param>
    /// <returns>シャッフルされたリストを返す</returns>
    public List<CardInformation> HandShuffle(List<CardInformation> playercards)
    {
        List<CardInformation> havecards = playercards;
        int indexCount = havecards.Count;

        while (indexCount > 1) {
            indexCount--;
            int random = Random.Range(0, indexCount + 1);
            var tmp = havecards[random];
            havecards[random] = havecards[indexCount];
            havecards[indexCount] = tmp;
        }

        return havecards;
    }
}
