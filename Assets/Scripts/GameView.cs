using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    CardsContller cardsContller;

    void Awake()
    {
        cardsContller = gameObject.GetComponent<CardsContller>();
    }

    void Start()
    {
        cardsContller.CardsCreate();
    }

    void Update()
    {
        
    }
}
