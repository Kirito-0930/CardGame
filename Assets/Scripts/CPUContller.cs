using System.Collections.Generic;
using UnityEngine;

public class CPUContller : MonoBehaviour
{
    [SerializeField] DisCardsContlloer trash;
    [SerializeField] GameObject startpos;   //手札の基準ポジション
    [SerializeField] GameView gameView;

    [SerializeField] float offset;                  //手札をずらす幅

    public List<CardInformation> haveCard
        = new List<CardInformation>();     //このListにトランプが渡される

    public bool isMyTurn;
    public bool isNextMtTurn;

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
        GetCardCheck();
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

        gameView.StartCheck();
    }

    //TODO:相手のトランプを取ってきたとき同じ数字があるか確認するための関数
    void GetCardCheck()
    {

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
        //CPUが持っている(配札時にかぶっていた)トランプ
        trash.CardsDump(haveCard[myCardIndex]);
        haveCard.RemoveAt(myCardIndex);

        //相手からとってきた(配札時にかぶっていた)トランプ
        trash.CardsDump(haveCard[checkCardIndex]);
        haveCard.RemoveAt(checkCardIndex);
    }
}
