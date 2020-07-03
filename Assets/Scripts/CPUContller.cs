using System.Collections.Generic;
using UnityEngine;

public class CPUContller : MonoBehaviour
{
    [SerializeField] GameObject startpos;   //手札の基準ポジション

    [SerializeField] float offset;                  //手札をずらす幅

    public List<CardInformation> haveCard
        = new List<CardInformation>();     //このListにトランプが渡される

    List<CardInformation> discards = new List<CardInformation>();

    void Start()
    {
        for (int i = 0; i < haveCard.Count; i++)
        {
            SameFristCardsCheck(i);
        }

        CardsLineUp();
    }

    void Update()
    {
        
    }

    //手元に残ったトランプを並べる
    void CardsLineUp()
    {
        for (int i = 0; i < haveCard.Count; i++)
        {
            haveCard[i].transform.parent = startpos.transform;
            haveCard[i].transform.localPosition = startpos.transform.localPosition + new Vector3(offset * i, 0.001f * i, 0);
            haveCard[i].transform.eulerAngles = startpos.transform.eulerAngles;
        }
    }

    //配られたトランプの番号が揃っているか調べる
    void SameFristCardsCheck(int checkCardIndex)
    {
        for (int i = checkCardIndex + 1; i < haveCard.Count; i++)
        {
            if (haveCard[checkCardIndex]._number == haveCard[i]._number)
            {
                ThrowAwayCards(i, checkCardIndex);
                SameFristCardsCheck(checkCardIndex);
            }
        }
    }

    //トランプの番号がそろったら捨てる
    void ThrowAwayCards(int myCardIndex, int checkCardIndex)
    {
        haveCard.RemoveAt(myCardIndex);
        haveCard.RemoveAt(checkCardIndex);
    }
}
