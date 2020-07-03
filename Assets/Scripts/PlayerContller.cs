using System.Collections.Generic;
using UnityEngine;

public class PlayerContller : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject startpos;   //手札の基準ポジション

    [SerializeField] float offset;                  //手札をずらす幅

    Transform obj;

    public List<CardInformation> haveCard 
        = new List<CardInformation>();     //このListにトランプが渡される

    int defaultLayer = 1;

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
        SelectCardsObject();
    }

    //手元に残ったトランプを並べる
    void CardsLineUp()
    {
        for (int i = 0; i < haveCard.Count; i++)
        {
            haveCard[i].transform.parent = startpos.transform;
            haveCard[i].transform.position = startpos.transform.position + new Vector3(offset * i, 0.001f * i, 0);
            haveCard[i].transform.eulerAngles = startpos.transform.eulerAngles;
        }
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

    //選んでいるトランプにアウトラインをつける
    void SelectCardsObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))   //マウスポジションからレイを飛ばして当たったオブジェクトにアウトラインをつける
        {
            if (obj == null)
            {
                obj = hit.transform;
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
            else if (obj != hit.transform)
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

    //トランプの番号がそろったら捨てる
    void ThrowAwayCards(int myCardIndex, int checkCardIndex)
    {
        haveCard.RemoveAt(myCardIndex);
        haveCard.RemoveAt(checkCardIndex);
    }
}
