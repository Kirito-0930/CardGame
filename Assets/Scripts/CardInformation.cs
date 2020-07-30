using UnityEngine;

public class CardInformation : MonoBehaviour
{
	[SerializeField] string suit;     //トランプのマーク(スート)を保持(♣, ♦, ♥, ♠, Joker)
	public string _suit { get { return suit; } }

	[SerializeField] int number;    //トランプの数字を保持
	public int _number { get { return number; } }

	[SerializeField] bool isJoker;   //ジョーカーか否かを保持
	public bool _isJoker { get { return isJoker; } }

	public Vector3 originalPos;     //生成された時のポジション
	public Vector3 shufflePos;      //山を二つに分けた時のポジションが渡される

	public bool isShuffleFirst = false;
	public bool isShuffleSecond = false;

	void Update()
	{
		if (isShuffleFirst)   //山札を分けた時のポジション
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, shufflePos, Time.deltaTime * 3);

		if (isShuffleSecond)   //二つの山札から合わさるときのポジション
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, originalPos, Time.deltaTime * 10);

		if (gameObject.transform.position == originalPos) 
			isShuffleSecond = false;   //シャッフルが終わったかどうかの判定
	}
}
