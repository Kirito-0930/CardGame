using GameState;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;

public class GameView : MonoBehaviour
{
    //他スクリプト
    [SerializeField] CardsContller cardsContller;
    [SerializeField] DiceContller diceContller;
    [SerializeField] OutlinePostProcess outLinePost;
    [SerializeField] PlayerContller[] players;

    //ゲーム終了時のボタン表示とサイコロイベント用のボタン
    [SerializeField] GameObject button;
    [SerializeField] GameObject diceButton;

    //各テキスト表示用
    [SerializeField] TextMeshProUGUI diceDisplay;
    [SerializeField] TextMeshProUGUI turnDisplay;
    [SerializeField] TextMeshProUGUI winerDisplay;

    #region ステート変数
    //各ステート
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public GameStateCPUTurn StateCPUTurn { get; set; } = new GameStateCPUTurn();
    public GameStateDiceEvent StateDiceEvent { get; set; } = new GameStateDiceEvent();
    public GameStateNoneTurn StateNoneTurn { get; set; } = new GameStateNoneTurn();
    public GameStatePlayerTurn StatePlayerTurn { get; set; } = new GameStatePlayerTurn();
    
    //変更前のステート名
    string prevStateName;
    #endregion

    public bool isEvent = false;

    bool isDebug;
    bool iswiner = false;
    int haveMostCards = 0;
    int playersCheck = 0;
    int playercount = 4;   //プレイヤーの人数

    ReactiveProperty<int> turn = new ReactiveProperty<int>();
    CompositeDisposable disposable = new CompositeDisposable();

    void Awake()
    {
        //各スクリプトの初期化
        Init();
        diceContller.Init();
        outLinePost.Init();
        for (int i = 0; i < players.Length; i++) {
            players[i].Init();
        }
    }

    void Start()
    {
        #region デバック用
        this.UpdateAsObservable()
            .Where(check => Input.GetKeyDown(KeyCode.F1))
            .Subscribe(_ => isDebug = !isDebug);

        this.UpdateAsObservable()
            .Where(check => isDebug)
            .Subscribe(_ => players[0].DebugView());
        #endregion

        StartMotion();

        //ステートの値が変更されたら実行処理を行うようにする
        StateProcessor.State
            .Where(name => StateProcessor.State.Value.GetStateName() != prevStateName)
            .Subscribe(_ =>
            {
                Debug.Log("Now State:" + StateProcessor.State.Value.GetStateName());
                prevStateName = StateProcessor.State.Value.GetStateName();
                StateProcessor.Execute();
            })
            .AddTo(this);

        //サイコロイベント判定
        this.UpdateAsObservable()
            .Where(check => isEvent && StateProcessor.State.Value != StateDiceEvent)
            .Subscribe(_ => StateProcessor.State.Value = StateDiceEvent)
            .AddTo(this);

        //勝利判定
        this.UpdateAsObservable()
            .Where(check => iswiner)
            .Subscribe(_ => WinerDisplay())
            .AddTo(this);
    }

    void Init()
    {
        //ステートの初期化
        StateProcessor.State.Value = StateNoneTurn;
        StateCPUTurn.ExecAction = CPUTurn;
        StateDiceEvent.ExecAction = DiceEvent;
        StateNoneTurn.ExecAction = NoneTurn;
        StatePlayerTurn.ExecAction = PlayerTurn;
    }

    //引かれる相手を分ける
    int IndexSet(int turn)
    {
        if (turn == 0) return 3;
        else return turn - 1;
    }

    # region ボタン処理
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

    #region ターン判定
    //ステートを変える
    void ChangeState(int turn)
    {
        turnDisplay.text = $"Turn:Player{turn + 1}";

        //プレイヤーかCPUかの判定
        _ = (players[turn]._isPlayer) ? StateProcessor.State.Value = StatePlayerTurn
                                                   : StateProcessor.State.Value = StateCPUTurn;
    }

    //ターンが変わると実行
    void CheckTurn()
    {
        turn
            .Subscribe(_ => ChangeState(turn.Value))
            .AddTo(this);
    }

    /// <summary>各プレイヤーの準備ができたらターン開始</summary>
    public void CheckTurnStart()
    {
        StartedTurn();
        playersCheck++;

        if (playersCheck == playercount) {
            diceButton.SetActive(true);
            CheckTurn();
        }
    }

    //トランプを一番持ってる人からスタートする
    void StartedTurn()
    {
        if (haveMostCards <= players[playersCheck].haveCard.Count) {
            haveMostCards = players[playersCheck].haveCard.Count;
            turn.Value = playersCheck;
        }
    }
    #endregion

    #region 各ターンの処理
    void CPUTurn()
    {
        float time = 0;
        this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                time += Time.deltaTime;
                players[turn.Value].GetTurnRotation(time);
                players[turn.Value - 1].TakenTurnRotation(time);
            })
            .AddTo(this);

        int index = players[turn.Value].CPUTurn(players[turn.Value - 1].haveCard.Count);

        ExchangeCard(players[turn.Value - 1].haveCard[index]);
    }

    void NoneTurn()
    {
        Debug.Log("StateがNoneTurnに状態遷移しました。");
    }

    void PlayerTurn()
    {
        disposable = new CompositeDisposable();

        float time = 0;
        this.UpdateAsObservable()
            .Subscribe(_ =>   
            {
                players[0].NowSelectCard();

                time += Time.deltaTime;
                players[0].GetTurnRotation(time);
                players[3].TakenTurnRotation(time);
            })
            .AddTo(disposable);
    }
    #endregion

    #region サイコロイベント処理
    /// <summary>サイコロの出目によって各プレイヤーの手札を入れ替える</summary>
    /// <param name="diceNumber">出目が送られる</param>
    public void DiceCheck(int diceNumber)
    {
        var tmp = players[0].haveCard;

        switch (diceNumber) {
            case 1:
                players[0].ChangeHaveCards(players[3].haveCard);
                players[3].ChangeHaveCards(players[2].haveCard);
                players[2].ChangeHaveCards(players[1].haveCard);
                players[1].ChangeHaveCards(tmp);
                diceDisplay.text = "Left 1";
                break;
            case 2:
                diceDisplay.text = "Out";
                break;
            case 3:
                players[0].ChangeHaveCards(players[1].haveCard);
                players[1].ChangeHaveCards(players[2].haveCard);
                players[2].ChangeHaveCards(players[3].haveCard);
                players[3].ChangeHaveCards(tmp);
                diceDisplay.text = "Right 1";
                break;
            case 4:
                players[0].ChangeHaveCards(players[1].haveCard);
                players[1].ChangeHaveCards(players[2].haveCard);
                players[2].ChangeHaveCards(players[3].haveCard);
                players[3].ChangeHaveCards(tmp);
                diceDisplay.text = "Right 1";
                break;
            case 5:
                diceDisplay.text = "Out";
                break;
            case 6:
                players[0].ChangeHaveCards(players[3].haveCard);
                players[3].ChangeHaveCards(players[2].haveCard);
                players[2].ChangeHaveCards(players[1].haveCard);
                players[1].ChangeHaveCards(tmp);
                diceDisplay.text = "Left 1";
                break;
            default:
                break;
        }

        isEvent = false;
        turn.Value++;
    }

    void DiceEvent()
    {
        disposable.Dispose();
        diceContller.DiceEvent();
    }
    #endregion

    /// <summary>トランプの取引をする</summary>
    /// <param name="cardInformation">取引されるトランプ</param>
    public void ExchangeCard(CardInformation cardInformation)
    {
        cardInformation.gameObject.tag = "Untagged";

        int index;
        index = players[IndexSet(turn.Value)].haveCard.FindIndex(h => h._number == cardInformation._number);

        players[IndexSet(turn.Value)].TakenCard(index);
        StartCoroutine(players[turn.Value].GetCard(cardInformation));

        Observable.Timer(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => players[IndexSet(turn.Value)].DefaultRotation())
            .AddTo(this);

        for (int i = 1; i < players.Length; i++) {
            players[i].DiceProbability();
            if (isEvent) break;
        }

        Observable.Timer(TimeSpan.FromSeconds(0.2f))
            .Where(check => !isEvent)
            .Subscribe(nextTurn => turn.Value++)
            .AddTo(this);
    }

    //勝利演出
    void WinerDisplay()
    {
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

        Observable.Timer(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => button.SetActive(true))
            .AddTo(this);
    }

    //ゲームを始めるまでの準備
    void StartMotion()
    {
        cardsContller.CreateCards();

        Observable.Timer(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => cardsContller.Shuffle())
            .AddTo(this);
    }
}
