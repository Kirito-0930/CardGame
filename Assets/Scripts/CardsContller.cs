using System.Collections.Generic;
using UnityEngine;

public class CardsContller : MonoBehaviour
{
    /// <summary>トランプのプレファブ</summary>
    [SerializeField] List<GameObject> cardsPrefab = new List<GameObject>();
    [SerializeField] Vector3 start;   //最初のカードの位置

    Distribute distribute;
    Shuffle shuffle;

    List<GameObject> cardsObject = new List<GameObject>();
    /// <summary>カード情報</summary>
    List<Card> cardsInformation = new List<Card>();
    /// <summary>カードの内部番号</summary>
    List<int> cardsNumber;
    public float cardOffset;   //カードをずらす幅

    void Awake()
    {
        CardsCreate();
        GetScripts();
    }

    /// <summary>
    /// カードの生成
    /// </summary>
    void CardsCreate()
    {
        for (int i = 0; i < 53; i++)   //カードオブジェクト生成
        {
            cardsObject.Add(Instantiate(cardsPrefab[i], Vector3.zero, Quaternion.identity));

            if (i == 0) cardsInformation.Add(new Card("Joker", null, true));
        }

        for (int i = 0; i < 4; i++)   //カード情報生成
        {
            switch (i)
            {
                case 0:
                    Suit("♣");
                    break;
                case 1:
                    Suit("♦");
                    break;
                case 2:
                    Suit("♥");
                    break;
                case 3:
                    Suit("♠");
                    break;
            }
        }
    }

    void GetScripts()
    {
        distribute = gameObject.GetComponent<Distribute>();
        shuffle = gameObject.GetComponent<Shuffle>();
    }

    void Suit(string suit)
    {
        for (int i = 1; i < 14; i++)
        {
            cardsInformation.Add(new Card(suit, i, false));
        }
    }

    /// <summary>
    /// シャッフルが要求された時の処理
    /// </summary>
    public void RequestShuffle()
    {
        cardsNumber = shuffle.CardShuffle(cardsNumber);
        Generate();
    }

    /// <summary>
    /// カードの並べ替えを行う関数
    /// </summary>
    void Generate()
    {
        for (int i = 0; i < 53; i++)
        {
            cardsObject[cardsNumber[i]].transform.position = start + new Vector3(cardOffset, 0.001f, 0) * i;
        }
    }

    /// <summary>
    /// プレイヤーにトランプを配る時の処理
    /// </summary>
    public void RequestDistribute()
    {
        for (int i = 0; i < 53; i++) 
        {
            distribute.CardDistribute(i, cardsObject[cardsNumber[i]]);
        }
    }
}
