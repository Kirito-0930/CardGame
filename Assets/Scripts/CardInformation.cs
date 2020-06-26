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

	public bool isShuffleEnd = false;
	public bool isShuffleStart = false;

	void FixedUpdate()
	{
		if (isShuffleStart)
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, shufflePos, Time.deltaTime * 3);

		if (isShuffleEnd)
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, originalPos, Time.deltaTime * 10);

		if (gameObject.transform.position == originalPos) isShuffleEnd = false;   //シャッフルが終わったかどうかの判定
	}
}
