using System;
using System.Collections.Generic;

namespace BazaarBot
{
	[Obsolete("What's the point of this? Save data from file?")]
    class InventoryData
    {
	    public double maxSize;
	    public readonly Dictionary<Good, double> ideal;
	    public readonly Dictionary<Good, double> start;
	    public readonly Dictionary<Good, double> size;

	    public InventoryData(double maxSize, Dictionary<Good, double> ideal, Dictionary<Good, double> start, Dictionary<Good, double> size = null)
	    {
		    this.maxSize = maxSize;
		    this.ideal = ideal;
		    this.start = start;
		    this.size = size;

            if (this.size == null)
            {
                this.size = new Dictionary<Good, double>();
                foreach (var entry in start)
                {
                    this.size[entry.Key] = 1;
                }
            }
	    }
    }
}
