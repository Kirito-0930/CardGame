using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiceContller : MonoBehaviour
{
    [SerializeField] GameView gameView;

    [SerializeField] float power;

    Rigidbody rb;

 /*
Left1   = 1
Out     = 2
Right1 = 3
Right1 = 4
Out     = 5
Left1   = 6
*/
    //サイコロの出目判定
    int GetNumber(Transform diceTransform)
    {
        int result;

        float innerProductX = Vector3.Dot(diceTransform.right, Vector3.up);
        float innerProductY = Vector3.Dot(diceTransform.up, Vector3.up);
        float innerProductZ = Vector3.Dot(diceTransform.forward, Vector3.up);

        if ((Mathf.Abs(innerProductX) > Mathf.Abs(innerProductY)) &&
            (Mathf.Abs(innerProductX) > Mathf.Abs(innerProductZ))) {
            // X軸が一番近い
            if (innerProductX > 0f) {
                result = 4;
            }
            else {
                result = 3;
            }
        }
        else if ((Mathf.Abs(innerProductY) > Mathf.Abs(innerProductX)) &&
                   (Mathf.Abs(innerProductY) > Mathf.Abs(innerProductZ))) {
            // Y軸が一番近い
            if (innerProductY > 0f) {
                result = 5;
            }
            else {
                result = 2;
            }
        }
        else {
            // Z軸が一番近い
            if (innerProductZ > 0f) {
                result = 1;
            }
            else {
                result = 6;
            }
        }

        return result;
    }

    public void Init()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        transform.position = new Vector3(0, 3.5f, 0);
    }

    /// <summary>サイコロイベントが起こった時の処理</summary>
    public void DiceEvent()
    {
        if (rb.IsSleeping() && rb.useGravity) {
            gameView.DiceCheck(GetNumber(transform));
            rb.useGravity = false;
            gameView.isEvent = false;
            transform.position = new Vector3(0, 3.5f, 0);
            return;
        }

        if (rb.IsSleeping()) {
            rb.useGravity = true;
            rb.velocity = Vector3.up * 5f;
            rb.angularVelocity = new Vector3(Random.Range(5, power), Random.Range(5, power), Random.Range(5, power));
        }
    }
}
