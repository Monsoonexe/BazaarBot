
namespace BazaarBot
{
	internal class GoodStack
	{
		public Good Good { get; private set; }

		public double Quantity { get; set; }

		public GoodStack(Good good, double quantity)
		{
			Good = good;
			Quantity = quantity;
		}
	}
}
