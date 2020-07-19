using UnityEngine;

public class Distribute : MonoBehaviour
{
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject player3;
    [SerializeField] GameObject player4;

    int count = 0;

    /// <summary>
    /// どのプレイヤーに配るか分ける
    /// </summary>
    /// <param name="cardCardInformation">i枚目のトランプオブジェクト</param>
    public void CardDistribute(CardInformation cardCardInformation)
    {
        switch (count % 4) {
            case 0:
                HandOver(cardCardInformation, player1);
                count++;
                break;
            case 1:
                HandOver(cardCardInformation, player2);
                count++;
                break;
            case 2:
                HandOver(cardCardInformation, player3);
                count++;
                break;
            case 3:
                HandOver(cardCardInformation, player4);
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
