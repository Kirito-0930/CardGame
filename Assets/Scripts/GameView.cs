using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameView : MonoBehaviour
{
    [SerializeField] CardsContller cardsContller;
    [SerializeField] GameObject Buttons;
    [SerializeField] OutlinePostProcess outLinePost;
    [SerializeField] PlayerContller[] players;
    [SerializeField] TextMeshProUGUI turnDisplay;
    [SerializeField] TextMeshProUGUI winerDisplay;

    bool isTurn = false;
    bool winer = false;
    float time = 1;
    int haveMostCards = 0;
    int playersCheck = 0;
    int turn;

    void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        
        outLinePost.Init();
        for (int i = 0; i < players.Length; i++) {
            players[i].Init();
        }
    }

    void Start()
    {
        StartCoroutine(StartMotion());
        isTurn = false;
    }

    void Update()
    {
        if (turn == 0 && isTurn) {
            players[0].NowSelectCard();
        }
        else if (turn != 0 && isTurn) {
            StartCoroutine(players[turn].CPUSelectCard(players[IndexSet(turn)].haveCard));
            isTurn = false;
        }
    }

    void FixedUpdate()
    {
        if (isTurn || time < 1) {
            time += Time.deltaTime / 2;

            players[turn].GetTurnRotation(time);
            players[IndexSet(turn)].TakenTurnRotation(time);
        }
    }

    //最初のプレイヤーが引く相手とそれ以外を分ける
    int IndexSet(int turn) 
    {
        if (turn == 0) return 3;
        else return turn - 1;
    }

    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }

    public void ReStartButton()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>各プレイヤーの準備ができたらターン開始</summary>
    public void StartCheck()
    {
        StartPlayerCheck();

        if (playersCheck == 3) {
            TurnCheck();
        }

        playersCheck++;
    }

    //プレイヤーにターンが来たことを知らせる
    void RequestPlayerTurn(int turn)
    {
        turnDisplay.text = $"Turn:Player{turn + 1}";
        isTurn = true;
        time = 0;
    }

    //トランプを一番持ってる人からスタートする
    void StartPlayerCheck()
    {
        if (haveMostCards <= players[playersCheck].haveCard.Count) {
            haveMostCards = players[playersCheck].haveCard.Count;
            turn = playersCheck;
        }
    }

    //今誰のターンか判定
    void TurnCheck()
    {
        if (turn == 4) turn = 0;

        switch (turn) {
            case 0:
                RequestPlayerTurn(turn);
                break;
            case 1:
                RequestPlayerTurn(turn);
                break;
            case 2:
                RequestPlayerTurn(turn);
                break;
            case 3:
                RequestPlayerTurn(turn);
                break;
        }
    }

    /// <summary>トランプの取引をする</summary>
    /// <param name="cardInformation">取引されるトランプ</param>
    public IEnumerator ExchangeCard(CardInformation cardInformation)
    {
        isTurn = false;
        cardInformation.gameObject.tag = "Untagged";

        int index;
        index = players[IndexSet(turn)].haveCard.FindIndex(h => h._number == cardInformation._number);

        players[IndexSet(turn)].TakenCard(index);
        StartCoroutine(players[turn].GetCard(cardInformation));
        yield return new WaitForSeconds(1f);

        players[IndexSet(turn)].DefaultRotation();
        yield return new WaitForSeconds(0.2f);

        if (players[turn].haveCard.Count == 0) {
            WinCheck();
        }
        else if (players[IndexSet(turn)].haveCard.Count == 0) {
            WinCheck();
        }

        if (!winer) {
            turn++;
            TurnCheck();
        }
    }

    //ゲームを始めるまでの準備
    IEnumerator StartMotion()
    {
        cardsContller.CreateCards();
        yield return new WaitForSeconds(1f);

        cardsContller.Shuffle();
    }

    //勝利判定
    IEnumerator WinCheck()
    {
        winer = true;

        if (turn == 0) {
            winerDisplay.colorGradientPreset.bottomLeft 
                = winerDisplay.colorGradientPreset.topRight = Color.white;
            winerDisplay.colorGradientPreset.bottomRight 
                = winerDisplay.colorGradientPreset.topLeft = Color.yellow;

            winerDisplay.text = "You Winer!!";
        }
        else {
            winerDisplay.colorGradientPreset.bottomLeft
               = winerDisplay.colorGradientPreset.topRight = Color.black;
            winerDisplay.colorGradientPreset.bottomRight
                = winerDisplay.colorGradientPreset.topLeft = Color.red;

            winerDisplay.text = "You Lose.";
        }

        yield return new WaitForSeconds(0.5f);
        Buttons.SetActive(true);
    }
}
