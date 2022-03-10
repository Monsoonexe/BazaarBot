
namespace BazaarBot.Agents
{
	/// <summary>
	/// The most fundamental agent class, and has as little implementation as possible.
	///	In most cases you should start by extending Agent instead of this.
	///	@author larsiusprime
	/// </summary>
	internal class AgentData
	{
		public readonly string ClassName;
		public readonly string LogicName;
		public readonly double Money;

		public InventoryData inventory;
		public Logic logic;
		public int? lookBack;

		public AgentData(string className, double money, string logicName)
		{
			ClassName = className;
			Money = money;
			LogicName = logicName;
		}
	}
}
