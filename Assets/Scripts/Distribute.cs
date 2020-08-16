using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distribute : MonoBehaviour
{
    [SerializeField] List<GameObject> players = new List<GameObject>();
 
    int count = 0;   //どのプレイヤーに配るか判定用

    /// <summary>どのプレイヤーに配るか分ける</summary>
    /// <param name="cardCardInformation">i枚目のトランプオブジェクト</param>
    public void DistributeCard(CardInformation cardCardInformation)
    {
        switch (count % 4) {
            case 0:
                HandOver(cardCardInformation, players[count % 4]);
                count++;
                break;
            case 1:
                HandOver(cardCardInformation, players[count % 4]);
                count++;
                break;
            case 2:
                HandOver(cardCardInformation, players[count % 4]);
                count++;
                break;
            case 3:
                HandOver(cardCardInformation, players[count % 4]);
                count = 0;
                break;
        }
    }

    //各プレイヤーの手札用ListにAddしていく
    void HandOver(CardInformation cardCardInformation, GameObject players)
    {
            players.GetComponent<PlayerContller>().haveCard.Add(cardCardInformation);
    }
}
