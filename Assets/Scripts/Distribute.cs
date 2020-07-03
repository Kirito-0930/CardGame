using UnityEngine;

public class Distribute : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject cpu1;
    [SerializeField] GameObject cpu2;
    [SerializeField] GameObject cpu3;

    int count = 0;

    /// <summary>
    /// どのプレイヤーに配るか分ける
    /// </summary>
    /// <param name="cardCardInformation">i枚目のトランプオブジェクト</param>
    public void CardDistribute(CardInformation cardCardInformation)
    {
        switch (count % 4) {
            case 0:
                HandOver(cardCardInformation, player);
                count++;
                break;
            case 1:
                HandOver(cardCardInformation, cpu1);
                count++;
                break;
            case 2:
                HandOver(cardCardInformation, cpu2);
                count++;
                break;
            case 3:
                HandOver(cardCardInformation, cpu3);
                count = 0;
                break;
        }
    }

    //各プレイヤーの手札用ListにAddしていく
    void HandOver(CardInformation cardCardInformation, GameObject players)
    {
        if (players.GetComponent<PlayerContller>() != null)
        {
            players.GetComponent<PlayerContller>().haveCard.Add(cardCardInformation);
        }
        else 
        {
            players.GetComponent<CPUContller>().haveCard.Add(cardCardInformation);
        }
    }
}
