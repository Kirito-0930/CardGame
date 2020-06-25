using System.Collections.Generic;
using UnityEngine;

public class PlayerContller : MonoBehaviour
{
    [SerializeField] Vector3 startpos;        //手札の基準ポジション

    [SerializeField] float offset;                //手札をずらす幅

    public List<CardInformation> haveCard 
        = new List<CardInformation>();   //このListにトランプが渡される

    void Start()
    {
        
    }

    void Update()
    {
        CheckSameCard();
    }

    void CheckSameCard()
    { 
    
    }
}
