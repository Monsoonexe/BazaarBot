using System;
using System.Collections.Generic;

namespace BazaarBot
{
    class InventoryData
    {
	    public double maxSize;
	    public readonly Dictionary<string, double> ideal;
	    public readonly Dictionary<string, double> start;
	    public readonly Dictionary<string, double> size;

	    public InventoryData(double maxSize, Dictionary<string, double> ideal, Dictionary<string, double> start, Dictionary<string, double> size)
	    {
		    this.maxSize = maxSize;
		    this.ideal = ideal;
		    this.start = start;
		    this.size = size;
            if (this.size == null)
            {
                this.size = new Dictionary<string, double>();
                foreach (var entry in start)
                {
                    this.size[entry.Key] = 1;
                }
            }
	    }
    }
}
