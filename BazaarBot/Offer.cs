
namespace BazaarBot
{
    public struct Offer
    {
	    public Good good;
	    public double units;			//how many units
	    public double unit_price;	//price per unit
	    public int agent_id;		//who offered this

	    public Offer(int agent_id, Good good, double units, double unit_price)
	    {
		    this.agent_id = agent_id;
		    this.good = good;
		    this.units = units;
		    this.unit_price = unit_price;
	    }

	    public override string ToString()
		    =>  $"({agent_id}): {good} x {units} @ {unit_price}";
    }
}
