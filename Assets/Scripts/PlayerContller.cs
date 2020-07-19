using System.Collections.Generic;
using UnityEngine;

public class PlayerContller : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] DisCardsContlloer trash;
    [SerializeField] GameObject startpos;   //手札の基準ポジション
    [SerializeField] GameView gameView;

    [SerializeField] float offset;                  //手札をずらす幅

    public List<CardInformation> haveCard 
        = new List<CardInformation>();     //このListにトランプが渡される

    Transform obj;

    bool isMyTurn;
    bool isPlayer;
    int defaultLayer = 1;

    void Start()
    {
        isPlayer = gameObject.tag == "Player" ? true : false;
    }

    void Update()
    {
        if (isMyTurn && isPlayer)
            NowSelectCard();
    }

    //手元に残ったトランプを並べる P != C
    void CardsLineUp()
    {
        for (int i = 0; i < haveCard.Count; i++)
        {
            haveCard[i].transform.parent = startpos.transform;
            haveCard[i].transform.localPosition = new Vector3(offset * i, 0.001f * i, 0);
            haveCard[i].transform.eulerAngles = startpos.transform.eulerAngles;

            if(isPlayer)
                haveCard[i].gameObject.tag = gameObject.tag;
        }

        gameView.StartCheck();
    }

    //配られたトランプの番号が揃っているか調べる P = C
    void FristCardsSameCheck(int checkCardIndex)
    {
        for (int i = checkCardIndex + 1; i < haveCard.Count; i++) {
            if (haveCard[checkCardIndex]._number == haveCard[i]._number) {
                ThrowAwayCards(i, checkCardIndex);
                FristCardsSameCheck(checkCardIndex);
            }
        }
    }

    //TODO:相手のトランプを取ってきたとき同じ数字があるか確認するための関数 P = C
    void GetCardCheck()
    {

    }

    public void MyTurn()
    {
        
    }

    //選んでいるトランプにアウトラインをつける P
    void NowSelectCard()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))   //マウスポジションからレイを飛ばして当たったオブジェクトにアウトラインをつける
        {
            if (obj == null && hit.transform.gameObject.tag == "CPU")
            {
                obj = hit.transform;
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
            else if (obj != hit.transform && hit.transform.gameObject.tag == "CPU")
            {
                obj.gameObject.layer = defaultLayer;
                obj = hit.transform;
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
        }
        else
        {
            if (obj != null)
            {
                obj.gameObject.layer = defaultLayer;
                obj = null;
            }
        }
    }

    //配られたトランプの整理 P != C
    public void Prepare()
    {
        if (isPlayer)
            playerCamera.gameObject.SetActive(true);

        for (int i = 0; i < haveCard.Count; i++) {
            FristCardsSameCheck(i);
        }

        CardsLineUp();
    }

    public void PreviousTurn()
    {

    }

    //トランプの番号がそろったら捨てる P = C
    void ThrowAwayCards(int myCardIndex, int checkCardIndex)
    {
        //自分が持っている(配札時にかぶっていた)トランプ
        trash.CardsDump(haveCard[myCardIndex]);
        haveCard.RemoveAt(myCardIndex);

        //相手からとってきた(配札時にかぶっていた)トランプ
        trash.CardsDump(haveCard[checkCardIndex]);
        haveCard.RemoveAt(checkCardIndex);
    }
}
