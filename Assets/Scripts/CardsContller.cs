using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsContller : MonoBehaviour
{
    [SerializeField] Distribute distribute;
    /// <summary>トランプ生成するときの元オブジェクト</summary>
    [SerializeField] List<GameObject> cardsPrefab = new List<GameObject>();
    [SerializeField] List<GameObject> player;
    [SerializeField] Shuffle shuffle;

    /// <summary>トランプ情報(スート、番号、ジョーカーか否か)</summary>
    List<CardInformation> cardsInformation = new List<CardInformation>();
    /// <summary>cardsInformationのindexに入れる</summary>
    List<int> cardsNumber;

    void Awake()
    {
        CardsCreate();
    }

    //トランプオブジェクト生成
    void CardsCreate()
    {
        for (int i = 0; i < 53; i++)
        {
            cardsInformation.Add(Instantiate(cardsPrefab[i]).GetComponent<CardInformation>());
        }
    }

    /// <summary>
    /// プレイヤーにトランプを配る時の処理
    /// </summary>
    public void RequestDistribute()
    {
        for (int index = 0; index < 53; index++)
        {
            distribute.CardDistribute(cardsInformation[cardsNumber[index]]);
        }

        for (int playerIndex = 0; playerIndex < player.Count; playerIndex++)
        {
            player[playerIndex].SetActive(true);
        }
    }

    /// <summary>
    /// シャッフルが要求された時の処理
    /// </summary>
    public void RequestShuffle()
    {
        cardsNumber = shuffle.CardShuffle();
    }
}
