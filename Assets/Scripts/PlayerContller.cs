using System.Collections.Generic;
using UnityEngine;

public class PlayerContller : MonoBehaviour
{
    [SerializeField] GameObject startpos;   //手札の基準ポジション

    [SerializeField] float offset;                  //手札をずらす幅

    public List<CardInformation> haveCard 
        = new List<CardInformation>();     //このListにトランプが渡される

    void Start()
    {
        for (int i = 0; i < haveCard.Count; i++)
        {
            haveCard[i].transform.position = startpos.transform.position + new Vector3(offset * i, 0.001f * i, 0);
            haveCard[i].transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void Update()
    {
        CheckSameCard();
    }

    //TODO:相手のトランプを取ってきたとき同じ数字があるか確認するための関数
    void CheckSameCard()
    { 
    
    }
}
