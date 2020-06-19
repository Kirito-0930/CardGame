using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    List<int> cardsIndex;

    public List<int> CardShuffle()
    {
        if (cardsIndex == null) cardsIndex = new List<int>();
        else cardsIndex.Clear();

        for (int i = 0; i < 53; i++)
        {
            cardsIndex.Add(i);
        }

        int indexCount = cardsIndex.Count;
        Random.InitState(System.DateTime.Now.Millisecond);

        while (indexCount > 1)   //トランプ入れ替えの処理
        {
            indexCount--;
            int ri = Random.Range(0, indexCount + 1);
            int tmp = cardsIndex[ri];
            cardsIndex[ri] = cardsIndex[indexCount];
            cardsIndex[indexCount] = tmp;
        }

        return cardsIndex;
    }
}
