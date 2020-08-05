using GameState;
using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameView : MonoBehaviour
{
    [SerializeField] CardsContller cardsContller;
    [SerializeField] DiceContller diceContller;
    [SerializeField] GameObject button;
    [SerializeField] GameObject diceButton;
    [SerializeField] OutlinePostProcess outLinePost;
    [SerializeField] PlayerContller[] players;
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

    public CardInformation tradedCardInformation;

    ReactiveProperty<int> turn = new ReactiveProperty<int>();
    CompositeDisposable disposable = new CompositeDisposable();

    void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

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

        //勝利判定
        this.UpdateAsObservable()
            .First(check => iswiner)
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

    //最初のプレイヤーが引く相手とそれ以外を分ける
    int IndexSet(int turn)
    {
        if (turn == 0) return 3;
        else return turn - 1;
    }

    /// <summary>トランプの取引をする</summary>
    /// <param name="cardInformation">取引されるトランプ</param>
    public void ExchangeCard(CardInformation cardInformation)
    {
        cardInformation.gameObject.tag = "Untagged";

        int index;
        index = players[IndexSet(turn.Value)].haveCard.FindIndex(h => h._number == cardInformation._number);

        players[IndexSet(turn.Value)].TakenCard(index);
        StartCoroutine(players[turn.Value].GetCard(cardInformation));

        Observable.Timer(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => players[IndexSet(turn.Value)].DefaultRotation())
            .AddTo(this);

        Observable.Timer(System.TimeSpan.FromSeconds(0.2f))
            .Subscribe(_ => TurnCheck())
            .AddTo(this);
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
    /// <summary>各プレイヤーの準備ができたらターン開始</summary>
    public void TurnStartCheck()
    {
        StartedTurn();

        if (playersCheck == 3) {
            diceButton.SetActive(true);
            TurnCheck();
        }

        playersCheck++;
    }

    //ステートを変える
    void StateChange(int turn)
    {
        turnDisplay.text = $"Turn:Player{turn + 1}";

        //プレイヤーかCPUかの判定
        _ = (turn == 0) ? StateProcessor.State.Value = StatePlayerTurn 
                                 : StateProcessor.State.Value = StateCPUTurn;
    }

    //トランプを一番持ってる人からスタートする
    void StartedTurn()
    {
        if (haveMostCards <= players[playersCheck].haveCard.Count) {
            haveMostCards = players[playersCheck].haveCard.Count;
            turn.Value = playersCheck;
        }
    }

    //ターン中か判定
    void TurnCheck()
    {
        turn
            .Subscribe(_ => StateChange(turn.Value))
            .AddTo(this);
    }
    #endregion

    #region 各ターンの処理
    void CPUTurn()
    {
        
    }

    void NoneTurn()
    {
        Debug.Log("StateがNoneTurnに状態遷移しました。");
    }

    void PlayerTurn()
    {
        this.UpdateAsObservable()
              .Subscribe(_ => 
              {
                  players[0].NowSelectCard();
                  players[0].GetTurnRotation(Time.deltaTime);
              })
              .AddTo(disposable);
    }
    #endregion

    #region サイコロイベント処理
    void DiceEvent()
    {
        disposable.Dispose();
        diceContller.DiceEvent();
    }

    /// <summary>サイコロの出目によって各プレイヤーの手札を入れ替える</summary>
    /// <param name="diceNumber">出目が送られる</param>
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

        disposable = new CompositeDisposable();
    }
    #endregion

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

        Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
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
