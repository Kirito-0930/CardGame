using System.Collections;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] CardsContller cardsContller;
    [SerializeField] PlayerContller[] players;

    int haveMostCards = 0;
    int playersCheck = 1;
    int turn;

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

    //各プレイヤーの準備ができたか判定する
    public void StartCheck()
    {
        StartPlayerCheck();

        if (playersCheck == 4) {
            TurnCheck();
        }

        playersCheck++;
    }

    //誰からスタートするか決める
    void StartPlayerCheck()
    {
        if (haveMostCards <= players[playersCheck - 1].haveCard.Count) {
            haveMostCards = players[playersCheck - 1].haveCard.Count;
            turn = playersCheck - 1;
        }
    }

    //今誰のターンか判定
    public void TurnCheck()
    {
        switch (turn) {
            case 0:
                RequestPlayerTurn();
                break;
            case 1:
                RequestPlayerTurn();
                break;
            case 2:
                RequestPlayerTurn();
                break;
            case 3:
                RequestPlayerTurn();
                turn = 0;
                break;
        }
    }

    //プレイヤーにターンが来たことを知らせる
    void RequestPlayerTurn()
    {
        players[turn].MyTurn();
        players[turn + 1].PreviousTurn();
        turn++;
    }

    //ゲームを始めるまでの準備
    IEnumerator StartMotion()
    {
        cardsContller.CreateCards();
        yield return new WaitForSeconds(1f);
        cardsContller.Shuffle();
    }
}
