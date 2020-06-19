using UnityEngine;

public class Distribute : MonoBehaviour
{
    [SerializeField] GameObject spwan1;
    [SerializeField] GameObject spwan2;
    [SerializeField] GameObject spwan3;
    [SerializeField] GameObject spwan4;
    public float cardOffset;   //カードをずらす幅

    /// <summary>
    /// カードを配る
    /// </summary>
    /// <param name="i">カード番号</param>
    /// <param name="cardObject">カード番号のトランプオブジェクト</param>
    public void CardDistribute(int i, GameObject cardObject)
    {
        switch (i % 4) {
            case 0:
                Spwans(i, cardObject, spwan1);
                break;
            case 1:
                Spwans(i, cardObject, spwan2);
                break;
            case 2:
                Spwans(i, cardObject, spwan3);
                break;
            case 3:
                Spwans(i, cardObject, spwan4);
                break;
        }
    }

    void Spwans(int i,GameObject cardObject, GameObject spwan)
    {
        cardObject.transform.position = spwan.transform.position + new Vector3(cardOffset, 0.001f, 0) * i;
        cardObject.transform.parent = spwan.transform;
    }
}
