using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisCardsContlloer : MonoBehaviour
{
    List<CardInformation> discards = new List<CardInformation>();

    float offset= .002f;
    int count = 0;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    void Update()
    {
        
    }

    //揃ったトランプが渡される
    public void CardsDump(CardInformation cardInformation)
    {
        discards.Add(cardInformation);
        discards.Last().gameObject.tag = "Trash";

        discards.Last().transform.position = transform.position + new Vector3(0, offset * count, 0);
        discards.Last().transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        count++;
    }
}
