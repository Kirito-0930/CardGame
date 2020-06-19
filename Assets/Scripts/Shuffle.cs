using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{

    public List<int> CardShuffle(List<int> cardsNumber)
    {
        if (cardsNumber == null) cardsNumber = new List<int>();
        else cardsNumber.Clear();

        for (int i = 0; i < 53; i++)   //カード0番から52番までのList生成
        {
            cardsNumber.Add(i);
        }

        int n = cardsNumber.Count;
        Random.InitState(System.DateTime.Now.Millisecond);   //ランダムのシード値

        while (n > 1)   //カード番号のランダム処理
        {
            n--;
            int k = Random.Range(0, n + 1);
            int temp = cardsNumber[k];
            cardsNumber[k] = cardsNumber[n];
            cardsNumber[n] = temp;
        }

        return cardsNumber;
    }
}
