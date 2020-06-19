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
}
