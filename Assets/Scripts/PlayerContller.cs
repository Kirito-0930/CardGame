using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UniRx;

public class PlayerContller : MonoBehaviour
{
    //他スクリプト
    [SerializeField] DisCardsContlloer trash;
    [SerializeField] GameView gameView;
    [SerializeField] Shuffle shuffle;

    //手札に使う
    [SerializeField] GameObject startpos;   //手札の基準ポジション
    [SerializeField] float offset;                  //手札をずらす幅
    public List<CardInformation> haveCard 
        = new List<CardInformation>();     //このListにトランプが渡される

    //プレイヤーだけが使う
    [SerializeField] Camera playerCamera;
    Transform obj;                                   //選択したトランプが格納される
    
    //向きを変える時に使用する
    [SerializeField] Quaternion right;
    [SerializeField] Quaternion left;
    Quaternion originalRotation;

    //CPUのみに使うサイコロイベント発動確率
    [SerializeField] int diceProbability;        //サイコロを振る確率の初期値
    uint XorShift;

    bool canEvent = true;
    public bool _isPlayer { private set; get; }
    int defaultLayer = 1;

    /// <summary>デバック用</summary>
    public void DebugView()
    {
        if (obj != null) {
           Debug.Log(obj.gameObject.GetComponent<CardInformation>()._number);
        }
    }

    public void Init()
    {
        //暗号論的疑似乱数
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] nonce = new byte[sizeof(int)];

            rng.GetBytes(nonce);
            XorShift = BitConverter.ToUInt32(nonce, 0);
        }

        _isPlayer = (gameObject.tag == "Player") ? true : false;

        originalRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        right = (_isPlayer) ? Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 30, transform.eulerAngles.z)
                                  : Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 20, transform.eulerAngles.z);

        left = (_isPlayer) ? Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 30, transform.eulerAngles.z)
                                : Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 20, transform.eulerAngles.z);
    }

    #region プレイヤーだけに行う処理 
    /// <summary>自分のターンの時処理される</summary>
    public void NowSelectCard()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        LayerChange(ray);

        /*どのトランプを取ったかGameViewに伝える*/
        if (Input.GetMouseButtonDown(0) && obj != null) {
            obj.gameObject.layer = defaultLayer;
            gameView.ExchangeCard(obj.GetComponent<CardInformation>());
            obj = null;
        }
    }

    //選択しているトランプのレイヤーを変える
    void LayerChange(Ray ray)
    {
        RaycastHit hit;

        /*マウスポジションからレイを飛ばして当たったオブジェクトにアウトラインをつける*/
        if (Physics.Raycast(ray, out hit)) {
            if (obj == null && hit.transform.gameObject.tag == "CPU") {
                obj = hit.transform;
                obj.localPosition += new Vector3(0, 0, 0.1f);
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
            else if (obj != hit.transform && hit.transform.gameObject.tag == "CPU") {
                obj.gameObject.layer = defaultLayer;
                obj.localPosition -= new Vector3(0, 0, 0.1f);
                obj = hit.transform;
                obj.localPosition += new Vector3(0, 0, 0.1f);
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
        }
    }
    #endregion

    #region CPUだけに行う処理
    //TODO:diceProbabilityに増減値を足す
    /// <summary>サイコロを振る確率を変動させる</summary>
    public void ProbabilityUP()
    {
        if (haveCard.Exists(h => h._isJoker == true))
        {

        }
    }

    //アウトラインをつけるためにタグを変える
    void ChangeTag()
    {
        if (!_isPlayer) {
            for (int i = 0; i < haveCard.Count; i++) {
                haveCard[i].tag = gameObject.tag;
            }
        }
    }

    public void DiceProbability()
    {
        if (!canEvent) return;

        //XorShift
        XorShift = (XorShift + 43) % 367;
        XorShift %= 100;

        if (diceProbability >= XorShift) {
            gameView.isEvent = true;
            canEvent = false;
        }
    }

    /// <summary>トランプを引く処理</summary>
    public int CPUTurn(int haveCardCount)
    {
        Observable.Timer(TimeSpan.FromSeconds(ThinkingTime()));

        return UnityEngine.Random.Range(1, haveCardCount);
    }

    float ThinkingTime()
    {
        float time = 0.3f;
        if (haveCard.Count <= 4)
            time += 0.4f;
        if (haveCard.Exists(h => h._isJoker == true))
            time += 0.6f;
        return time;
    }
    #endregion

    #region 手札に行う処理
    //トランプを並べる
    void CardsLineUp()
    {
        for (int i = 0; i < haveCard.Count; i++) {
            haveCard[i].transform.parent = startpos.transform;
            haveCard[i].transform.localPosition = new Vector3(offset * i, 0.001f * i, 0);
            haveCard[i].transform.eulerAngles = startpos.transform.eulerAngles;
        }
    }

    //トランプの番号がそろったら捨てる
    void CardThrowAway(int myCardIndex, int checkCardIndex)
    {
        /*相手からとってきた(配札時にかぶっていた後の要素)トランプ*/
        trash.CardsDump(haveCard[checkCardIndex]);
        haveCard.RemoveAt(checkCardIndex);

        /*自分が持っている(配札時にかぶっていた先の要素)トランプ*/
        trash.CardsDump(haveCard[myCardIndex]);
        haveCard.RemoveAt(myCardIndex);
    }

    //配られたトランプの番号が揃っているか調べる
    void FristCardsSameCheck(int checkCardIndex)
    {
        for (int i = checkCardIndex + 1; i < haveCard.Count; i++) {
            if (haveCard[checkCardIndex]._number == haveCard[i]._number) {
                CardThrowAway(checkCardIndex, i);
                FristCardsSameCheck(checkCardIndex);
            }
        }
    }

    /// <summary>最初に配られたトランプの整理</summary>
    public IEnumerator Prepare()
    {
        CardsLineUp();
        yield return new WaitForSeconds(1f);

        /*揃っているトランプがあるか調べる*/
        for (int i = 0; i < haveCard.Count; i++) {
            FristCardsSameCheck(i);
        }

        CardsLineUp();
        yield return new WaitForSeconds(1f);

        gameView.CheckTurnStart();
    }
    #endregion

    #region サイコロイベント処理
    /// <summary>サイコロを振るボタンを押したとき</summary>
    public void DiceButton()
    {
        if (canEvent) {
            gameView.isEvent = true;
            canEvent = false;
        }
    }

    /// <summary>サイコロの出目によって手札を変える</summary>
    /// <param name="otherPlayerCards">入れ替える相手の手札</param>
    public void ChangeHaveCards(List<CardInformation> otherPlayerCards)
    {
        haveCard = otherPlayerCards;
        CardsLineUp();
    }
    #endregion

    #region トランプの取引処理
    /// <summary>トランプが引かれた時の処理</summary>
    public void TakenCard(int haveCardIndex)
    {
        haveCard.RemoveAt(haveCardIndex);
        CardsLineUp();
    }

    /// <summary>引いたトランプを手札に追加し、そろっていたら捨てる</summary>
    public IEnumerator GetCard(CardInformation cardInformation)
    {
        /*数字が揃っているか判定する*/
        if (haveCard.Exists(h => h._number == cardInformation._number)) {
            int index = haveCard.FindIndex(h => h._number == cardInformation._number);
            haveCard.Add(cardInformation);
            CardsLineUp();
            yield return new WaitForSeconds(0.5f);

            CardThrowAway(index, haveCard.Count - 1);
            yield return new WaitForSeconds(0.5f);

            shuffle.HandShuffle(haveCard);
            CardsLineUp();
        }
        else {
            haveCard.Add(cardInformation);
            CardsLineUp();
            yield return new WaitForSeconds(0.5f);

            shuffle.HandShuffle(haveCard);
            CardsLineUp();
        }
    }
    #endregion

    #region ターンによって向きを変える
    /// <summary>元の向きに戻る</summary>
    public void DefaultRotation()
    {
        transform.rotation = originalRotation;

        for (int i = 0; i < haveCard.Count; i++) {
            haveCard[i].tag = "Untagged";
        }
    }

    /// <summary>トランプを引く相手の方を向く</summary>
    public void GetTurnRotation(float rotationTime)
    {
        transform.rotation = Quaternion.Slerp(originalRotation, right, rotationTime * 2);
    }

    /// <summary>トランプを引かれる方を向く</summary>
    public void TakenTurnRotation(float rotationTime)
    {
        if (rotationTime < 1) {
            ChangeTag();
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, left, rotationTime);
    }
    #endregion
}
