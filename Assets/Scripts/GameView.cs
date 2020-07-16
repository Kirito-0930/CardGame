using System.Collections;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] CardsContller cardsContller;
    [SerializeField] CPUContller cpu1;
    [SerializeField] CPUContller cpu2;
    [SerializeField] CPUContller cpu3;
    [SerializeField] PlayerContller player;

    void Awake()
    {
        StartCoroutine(StartMotion());
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    //ゲームを始めるまでの準備
    IEnumerator StartMotion()
    {
        cardsContller.CardsCreate();
        yield return new WaitForSeconds(1f);
        cardsContller.Shuffle();
    }
}
