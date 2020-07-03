using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsContller : MonoBehaviour
{
    [SerializeField] Distribute distribute;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject cardsSetPos;
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

    //トランプ情報(スート、番号、ジョーカーか否か)の生成
    void CardsCreate()
    {
        for (int i = 0; i < 53; i++)
        {
            cardsInformation.Add(Instantiate(cardsPrefab[i]).GetComponent<CardInformation>());
            cardsInformation[i].transform.rotation = Quaternion.Euler(180, 0, 0);   //トランプを裏側にしておく
            cardsInformation[i].transform.position = cardsSetPos.transform.position;
            cardsSetPos.transform.position += new Vector3(0, .002f, 0);
        }
    }

    //プレイヤーにトランプを配る時の処理
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
        camera.SetActive(false);
    }

    //シャッフルボタンが押された時の処理
    public void RequestShuffle()
    {
        StartCoroutine(ShuffleAnimation());      //見た目のトランプシャッフル
        cardsNumber = shuffle.CardShuffle();   //内部的なトランプシャッフル
    }

    IEnumerator ShuffleAnimation()
    {
        for (int i = 0; i < 53; i++)
        {
            cardsInformation[i].originalPos = cardsInformation[i].transform.position;

            cardsInformation[i].shufflePos = i % 2 == 0 ? new Vector3(-0.3f, 2f + .002f * i, 0) 
                                                                             : new Vector3(0.3f, 2f + .002f * (i - 1), 0);   //トランプデッキを二つに分ける
            cardsInformation[i].isShuffleFirst = true;
        }

        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < 53; i++)
        {
            cardsInformation[i].isShuffleFirst = false;

            yield return new WaitForSeconds(0.05f);

            if (i % 2 == 0) cardsInformation[i].isShuffleSecond = true;
            else cardsInformation[i].isShuffleSecond = true;
        }
    }
}
