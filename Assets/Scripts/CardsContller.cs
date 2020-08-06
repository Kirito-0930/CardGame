using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsContller : MonoBehaviour
{
    [SerializeField] Distribute distribute;
    /// <summary>トランプ生成するときの元オブジェクト</summary>
    [SerializeField] List<GameObject> cardsPrefab;
    [SerializeField] List<PlayerContller> players;
    [SerializeField] Shuffle shuffle;
    [SerializeField] Transform cardsSetPos;

    /// <summary>トランプ情報(スート、番号、ジョーカーか否か)を入れる</summary>
    List<CardInformation> cardsInformation = new List<CardInformation>();
    /// <summary>cardsInformationのindexに入れる</summary>
    List<int> cardsNumber;

    /// <summary>トランプの生成</summary>
    public void CreateCards()
    {
        for (int i = 0; i < 53; i++) {
            cardsInformation.Add(Instantiate(cardsPrefab[i]).GetComponent<CardInformation>());
            cardsInformation[i].transform.rotation = Quaternion.Euler(180, 0, 0);   //トランプを裏側にしておく
            cardsInformation[i].transform.position = cardsSetPos.position;
            cardsSetPos.position += new Vector3(0, .002f, 0);
        }
    }

    /// <summary>ゲームが開始したら自動でシャッフルされる</summary>
    public void Shuffle()
    {
        StartCoroutine(ShuffleAnimation());      //見た目のトランプシャッフル
        cardsNumber = shuffle.CardShuffle();   //内部的なトランプシャッフル
    }

    //シャッフル完了後自動でトランプを配る
    void Distribute()
    {
        for (int index = 0; index < 53; index++) {
            distribute.CardDistribute(cardsInformation[cardsNumber[index]]);
        }

        for (int playerIndex = 0; playerIndex < players.Count; playerIndex++) {
            StartCoroutine(players[playerIndex].Prepare());
        }
    }

    //シャッフルの演出を制御
    IEnumerator ShuffleAnimation()
    {
        for (int i = 0; i < 53; i++) {
            cardsInformation[i].originalPos = cardsInformation[i].transform.position;

            //トランプデッキを二つに分ける
            cardsInformation[i].shufflePos = i % 2 == 0 ? new Vector3(-0.3f, 2f + .002f * i, 0) 
                                                                              : new Vector3(0.3f, 2f + .002f * (i - 1), 0);

            cardsInformation[i].isShuffleFirst = true;
        }
        
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < 53; i++) {
            cardsInformation[i].isShuffleFirst = false;

            yield return new WaitForSeconds(0.05f);

            if (i % 2 == 0) cardsInformation[i].isShuffleSecond = true;
            else cardsInformation[i].isShuffleSecond = true;
        }

        yield return new WaitForSeconds(1.5f);
        Distribute();
    }
}
