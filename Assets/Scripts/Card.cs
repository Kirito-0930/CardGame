public struct Card
{
	/// <summary>カードのマーク(スート)を保持(♣, ♦, ♥, ♠, Joker)</summary>
	public string suit;

	/// <summary>カードの数字</summary>
	public int? number;

	/// <summary>ジョーカーかどうか</summary>
	public bool isJoker;

	public Card(string suit , int? number, bool isJoker)
	{
		this.suit = suit;
		this.number = number;
		this.isJoker = isJoker;
	}
}
