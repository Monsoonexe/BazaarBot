
namespace EconomySim
{
    public struct Offer
    {
		/// <summary>
		/// The thing that is up for sale.
		/// </summary>
	    public readonly string Good;

		/// <summary>
		/// Quantity of <see cref="Good"/>.
		/// </summary>
	    public readonly double Units;

		/// <summary>
		/// The purchase cost in currency per <see cref="Unit"/>.
		/// </summary>
		public readonly double UnitPrice;

		/// <summary>
		/// The offerer.
		/// </summary>
		public readonly int AgentID;

		/// <summary>
		/// <see cref="Units"/> * <see cref="UnitPrice"/>.
		/// </summary>
		public double TotalStackPrice => Units * UnitPrice;

	    public Offer(int agentID, string good, double units, double UnitPrice)
	    {
		    AgentID = agentID;
		    Good = good;
		    Units = units;
		    this.UnitPrice = UnitPrice;
	    }

	    public override string ToString()
		    => $"({AgentID}): {Good} x {Units} @ {UnitPrice})";
    }
}
