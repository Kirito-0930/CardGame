using UnityEngine;

public class CardInformation : MonoBehaviour
{
	//トランプのマーク(スート)を保持(♣, ♦, ♥, ♠, Joker)
	[SerializeField] string suit;
	public string _suit { get { return suit; } }

	//トランプの数字を保持
	[SerializeField] int number;
	public int _number { get { return number; } }

	//ジョーカーか否かを保持
	[SerializeField] bool isJoker;
	public bool _isJoker { get { return isJoker; } }

	//生成された時のポジション
	public Vector3 originalPos;
	//山を二つに分けた時のポジションが渡される
	public Vector3 shufflePos;

	public bool isShuffleFirst = false;
	public bool isShuffleSecond = false;

	void FixedUpdate()
	{
		if (isShuffleFirst)   //山札を分けた時のポジション
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, shufflePos, Time.deltaTime * 3);

		if (isShuffleSecond)   //二つの山札から合わさるときのポジション
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, originalPos, Time.deltaTime * 10);

		if (gameObject.transform.position == originalPos) 
			isShuffleSecond = false;   //シャッフルが終わったかどうかの判定
	}
}
