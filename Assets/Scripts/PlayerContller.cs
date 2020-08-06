using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContller : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] DisCardsContlloer trash;
    [SerializeField] GameObject startpos;   //手札の基準ポジション
    [SerializeField] GameView gameView;
    [SerializeField] Shuffle shuffle;

    [SerializeField] Quaternion right;
    [SerializeField] Quaternion left;

    [SerializeField] float offset;                  //手札をずらす幅
    [SerializeField] int diceProbability;        //サイコロを振る確率の初期値

    public List<CardInformation> haveCard 
        = new List<CardInformation>();     //このListにトランプが渡される

    Transform obj;                                   //選択したトランプが格納される
    Quaternion originalRotation;

    bool canEvent = true;
    bool isPlayer;
    int defaultLayer = 1;

    public void Init()
    {
        isPlayer = (gameObject.tag == "Player") ? true : false;

        originalRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        right = (gameObject.tag == "Player") ? Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 30, transform.eulerAngles.z)
                                                                : Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 20, transform.eulerAngles.z);

        left = (gameObject.tag == "Player") ? Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 30, transform.eulerAngles.z)
                                                              : Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 20, transform.eulerAngles.z);
    }

    /// <summary>デバック用</summary>
    public void DebugView()
    {
        if (obj != null) {
           Debug.Log(obj.gameObject.GetComponent<CardInformation>()._number);
        }
    }

    #region 向きを変える処理
    /// <summary>元の向きに戻る</summary>
    public void DefaultRotation()
    {
        transform.rotation = originalRotation;

        if (isPlayer) {
            for (int i = 0; i < haveCard.Count; i++) {
                haveCard[i].tag = "Untagged";
            }
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
            TagChange();
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, left, rotationTime);
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
    public void HaveCardsChange(List<CardInformation> otherPlayerCards)
    {
        haveCard = otherPlayerCards;
        CardsLineUp();
    }

    //TODO:diceProbabilityに増減値を足す
    /// <summary>サイコロを振る確率を変動させる</summary>
    public void ProbabilityUP()
    {
        if (haveCard.Exists(h => h._isJoker == true))
        {

        }
    }
    #endregion

    #region 手札処理
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

    //CPUのみにこの処理を行う
    void TagChange()
    {
        /*アウトラインをつけるためにトランプのタグを変える*/
        if (!isPlayer) {
            for (int i = 0; i < haveCard.Count; i++) {
                haveCard[i].tag = gameObject.tag;
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

        gameView.StartCheck();   //準備ができたことを知らせる
    }
    #endregion

    #region ターンが回ってきたときの処理
    /// <summary>自分のターンの時処理される</summary>
    public void NowSelectCard()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        LayerChange(ray);

        /*どのトランプを取ったかGameViewに伝える*/
        if (Input.GetMouseButtonDown(0) && obj != null) {
            obj.gameObject.layer = defaultLayer;
            StartCoroutine(gameView.ExchangeCard(obj.gameObject.GetComponent<CardInformation>()));
            obj = null;
        }
    }

    /// <summary>トランプが引かれた時の処理</summary>
    public void TakenCard(int haveCardIndex)
    {
        haveCard.RemoveAt(haveCardIndex);
        CardsLineUp();
    }

    //選択しているトランプのレイヤーを変える
    void LayerChange(Ray ray)
    {
        RaycastHit hit;

        /*マウスポジションからレイを飛ばして当たったオブジェクトにアウトラインをつける*/
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject.tag != "CPU") return;

            if (obj == null) {
                obj = hit.transform;
                obj.localPosition += new Vector3(0, 0, 0.1f);
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
            else {
                obj.gameObject.layer = defaultLayer;
                obj.localPosition -= new Vector3(0, 0, 0.1f);
                obj = hit.transform;
                obj.localPosition += new Vector3(0, 0, 0.1f);
                obj.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
        }
    }

    /// <summary>CPUがトランプを引く処理</summary>
    public IEnumerator CPUTurn(List<CardInformation> cardInformations)
    {
        if (diceProbability >= Random.Range(1, 101)) {
            if (canEvent) {
                gameView.isEvent = true;
                canEvent = false;
            }
        }

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        StartCoroutine(gameView.ExchangeCard(cardInformations[Random.Range(0, cardInformations.Count)]));
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
}
