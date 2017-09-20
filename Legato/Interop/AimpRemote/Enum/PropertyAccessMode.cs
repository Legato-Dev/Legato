namespace Legato.Interop.AimpRemote.Enum
{
	[System.Flags]
	public enum PropertyAccessMode : uint
	{
		/// <summary>
		/// プロパティを取得する値です。
		/// OR演算で結びつけます。
		/// </summary>
		Get = 0,

		/// <summary>
		/// プロパティを設定する値です。
		/// OR演算で結びつけます。
		/// </summary>
		Set = 1,
	}
}
