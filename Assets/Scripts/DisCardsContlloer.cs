using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisCardsContlloer : MonoBehaviour
{
    List<CardInformation> discards = new List<CardInformation>();

    float offset = .001f;
    int cardCount = 0;   //捨てられたトランプが何枚目か

    /// <summary>トランプの捨て場</summary>
    /// <param name="cardInformation">揃ったトランプが渡される</param>
    public void CardsDump(CardInformation cardInformation)
    {
        discards.Add(cardInformation);
        discards.Last().gameObject.tag = gameObject.tag;

        discards.Last().transform.parent = gameObject.transform;
        discards.Last().transform.position = transform.position + new Vector3(0, offset * cardCount, 0);
        discards.Last().transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        cardCount++;
    }
}
