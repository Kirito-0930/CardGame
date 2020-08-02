using UnityEngine;
using System;
using UniRx;

/// <summary>ゲームの状態（ステート）</summary>
namespace GameState
{
    /// <summary>ステートの実行を管理するクラス</summary>
    public class StateProcessor
    {
        //ステート本体
        public ReactiveProperty<GameState> State { get; set; } 
            = new ReactiveProperty<GameState>();

        //実行ブリッジ
        public void Execute() => State.Value.Execute();
    }

    /// <summary>ステートのクラス</summary>
    public abstract class GameState
    {
        //デリゲート
        public Action ExecAction { get; set; }

        //実行処理
        public virtual void Execute()
        {
            ExecAction?.Invoke();
        }

        //ステート名を取得するメソッド
        public abstract string GetStateName();
    }

    //以下状態クラス=====================================================

    /// <summary>CPUのターン</summary>
    public class GameStateCPUTurn : GameState
    {
        public override string GetStateName()
        {
            return "State:CPUTurn";
        }
    }

    /// <summary>サイコロイベント</summary>
    public class GameStateDiceEvent : GameState
    {
        public override string GetStateName()
        {
            return "State:DiceEvent";
        }

        public override void Execute()
        {
            Debug.Log("なにか特別な処理をしたいときは派生クラスにて処理をしても良い");
            ExecAction?.Invoke();
        }
    }

    public class GameStateNoneTurn : GameState
    {
        public override string GetStateName()
        {
            return "State:noneTurn";
        }
    }

    /// <summary>プレイヤーのターン</summary>
    public class GameStatePlayerTurn : GameState
    {
        public override string GetStateName()
        {
            return "State:PlayerTurn";
        }
    }
}