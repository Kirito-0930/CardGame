﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameView : MonoBehaviour
{
    //各スクリプト変数
    [SerializeField] CardsContller cardsContller;
    [SerializeField] DiceContller diceContller;
    [SerializeField] OutlinePostProcess outLinePost;
    [SerializeField] PlayerContller[] players;

    [SerializeField] GameObject button;
    [SerializeField] GameObject diceButton;
    [SerializeField] TextMeshProUGUI diceDisplay;
    [SerializeField] TextMeshProUGUI turnDisplay;
    [SerializeField] TextMeshProUGUI winerDisplay;

    //サイコロイベント用変数
    [SerializeField] Light diceLight;
    [SerializeField] AnimationCurve animationCurve;

    //SE・BGM用変数
    [SerializeField] AudioClip diceSE;
    AudioSource audioSource;

    public bool isEvent = false;

    bool isCPU;
    bool isDebug;
    bool isTurn = false;
    bool winer = false;
    float diceEventTime = 0;
    float time = 1;
    int haveMostCards = 0;
    int playersCheck = 0;
    int turn;

    void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        audioSource = gameObject.GetComponent<AudioSource>();
        isTurn = false;

        diceContller.Init();
        outLinePost.Init();
        for (int i = 0; i < players.Length; i++) {
            players[i].Init();
        }
    }

    void Start()
    {
        StartCoroutine(StartMotion());
    }

    void Update()
    {
        //デバック用
        if (Input.GetKeyDown(KeyCode.F1)) {
            isDebug = !isDebug;
        }
        if (isDebug) {
            players[0].DebugView();
        }

        //サイコロイベント判定
        if (isEvent) {
            isTurn = false;
            EventEffect();
            diceContller.DiceEvent();
        }

        if (!isTurn) return;

        if (CPUCheck(turn)) {
            StartCoroutine(players[turn].CPUTurn(players[IndexSet(turn)].haveCard));
            isTurn = false;
        }
        else {
            players[0].NowSelectCard();
        }
    }

    void FixedUpdate()
    {
        PlayerRotation();
    }

    #region プレイヤーを判定
    //CPUかどうか判定
    bool CPUCheck(int checknum)
    {
        if (checknum == 0) isCPU = false;
        else isCPU = true;

        return isCPU;
    }

    //最初のプレイヤーが引く相手とそれ以外を分ける
    int IndexSet(int turn)
    {
        if (turn == 0) return 3;
        else return turn - 1;
    }
    #endregion

    #region ボタン処理
    //タイトル画面に戻る
    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }

    //もう一度ゲームを開始する
    public void ReStartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region ターン処理
    /// <summary>各プレイヤーの準備ができたらターン開始</summary>
    public void StartCheck()
    {
        StartPlayerCheck();

        if (playersCheck == 3) {
            diceButton.SetActive(true);
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

    //ターンが来たプレイヤーを向き合わせる
    void PlayerRotation()
    {
        if (isTurn || time < 1) {
            time += Time.deltaTime / 2;

            players[turn].GetTurnRotation(time);
            players[IndexSet(turn)].TakenTurnRotation(time);
        }
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
    #endregion

    #region サイコロイベント処理
    /// <summary>サイコロの出目によって各プレイヤーの手札を入れ替える</summary>
    public void DiceCheck(int diceNumber)
    {
        var tmp = players[0].haveCard;

        switch (diceNumber) {
            case 1:
                players[0].HaveCardsChange(players[3].haveCard);
                players[3].HaveCardsChange(players[2].haveCard);
                players[2].HaveCardsChange(players[1].haveCard);
                players[1].HaveCardsChange(tmp);
                diceDisplay.text = "Left 1";
                break;
            case 2:
                diceDisplay.text = "Out";
                break;
            case 3:
                players[0].HaveCardsChange(players[1].haveCard);
                players[1].HaveCardsChange(players[2].haveCard);
                players[2].HaveCardsChange(players[3].haveCard);
                players[3].HaveCardsChange(tmp);
                diceDisplay.text = "Right 1";
                break;
            case 4:
                players[0].HaveCardsChange(players[1].haveCard);
                players[1].HaveCardsChange(players[2].haveCard);
                players[2].HaveCardsChange(players[3].haveCard);
                players[3].HaveCardsChange(tmp);
                diceDisplay.text = "Right 1";
                break;
            case 5:
                diceDisplay.text = "Out";
                break;
            case 6:
                players[0].HaveCardsChange(players[3].haveCard);
                players[3].HaveCardsChange(players[2].haveCard);
                players[2].HaveCardsChange(players[1].haveCard);
                players[1].HaveCardsChange(tmp);
                diceDisplay.text = "Left 1";
                break;
            default:
                break;
        }

        isEvent = false;
        isTurn = true;
    }

    //サイコロイベントが発生した時の演出
    void EventEffect()
    {
        for (int i = 0; i < players.Length; i++) {
            if(i == 0) audioSource.PlayOneShot(diceSE);
            players[i].DefaultRotation();
        }

        diceEventTime += Time.deltaTime;
        diceLight.range = 10 * animationCurve.Evaluate(diceEventTime);
        if (diceEventTime >= 1) diceEventTime = 0;
    }
    #endregion

    /// <summary>トランプの取引をする</summary>
    /// <param name="cardInformation">取引されるトランプ</param>
    public IEnumerator ExchangeCard(CardInformation cardInformation)
    {
        isTurn = false;
        cardInformation.gameObject.tag = "Untagged";

        int index;
        index = players[IndexSet(turn)].haveCard.FindIndex(h => h.number == cardInformation.number);

        players[IndexSet(turn)].TakenCard(index);
        StartCoroutine(players[turn].GetCard(cardInformation));
        yield return new WaitForSeconds(1f);

        players[IndexSet(turn)].DefaultRotation();
        yield return new WaitForSeconds(0.2f);

        if (players[turn].haveCard.Count == 0) {
            StartCoroutine(WinCheck());
        }
        else if (players[IndexSet(turn)].haveCard.Count == 0) {
            StartCoroutine(WinCheck());
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

        if (players[0].haveCard.Count == 0) {
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
        button.SetActive(true);
    }
}
