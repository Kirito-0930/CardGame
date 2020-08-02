using System.Collections;
using System.Collections.Generic;
using GameState;
using UnityEngine;
using UniRx;

public class Test : MonoBehaviour
{
    //変更前のステート名
    private string prevStateName;

    //ステート
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public GameStateCPUTurn StateCPUTurn { get; set; } = new GameStateCPUTurn();
    public GameStateDiceEvent StateDiceEvent { get; set; } = new GameStateDiceEvent();
    public GameStateNoneTurn StateNoneTurn { get; set; } = new GameStateNoneTurn();
    public GameStatePlayerTurn StatePlayerTurn { get; set; } = new GameStatePlayerTurn();

    void Start()
    {

        //ステートの初期化
        StateProcessor.State.Value = StateNoneTurn;
        StateCPUTurn.ExecAction = CPU;
        StateDiceEvent.ExecAction = Dice;
        StateNoneTurn.ExecAction = None;
        StatePlayerTurn.ExecAction = Player;

        //ステートの値が変更されたら実行処理を行うようにする
        StateProcessor.State
            .Where(_ => StateProcessor.State.Value.GetStateName() != prevStateName)
            .Subscribe(_ =>
            {
                Debug.Log("Now State:" + StateProcessor.State.Value.GetStateName());
                prevStateName = StateProcessor.State.Value.GetStateName();
                StateProcessor.Execute();
            })
            .AddTo(this);
    }


    public void CPUTurn()
    {
        StateProcessor.State.Value = StateCPUTurn;
    }

    void CPU()
    {
        Debug.Log("StateがCPUTurnに状態遷移しました。");
    }

    public void DiceEvent()
    {
        StateProcessor.State.Value = StateDiceEvent;
    }

    void Dice()
    {
        Debug.Log("StateがDiceEventに状態遷移しました。");
    }

    public void NoneTurn()
    {
        StateProcessor.State.Value = StateNoneTurn;
    }

    void None()
    {
        Debug.Log("StateがNoneTurnに状態遷移しました。");
    }

    public void PlayerTurn()
    {
        StateProcessor.State.Value = StatePlayerTurn;
    }

    void Player()
    {
        Debug.Log("StateがPlayerTurnに状態遷移しました。");
    }
}