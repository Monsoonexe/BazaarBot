
namespace BazaarBot.MarketHistory
{
    class History
    {
	    public readonly HistoryLog Prices;
	    public readonly HistoryLog Asks;
	    public readonly HistoryLog Bids;
	    public readonly HistoryLog Trades;
	    public readonly HistoryLog Profit;

	    public History()
	    {
		    Prices = new HistoryLog(EconNoun.Price);
		    Asks   = new HistoryLog(EconNoun.Ask);
		    Bids   = new HistoryLog(EconNoun.Bid);
		    Trades = new HistoryLog(EconNoun.Trade);
		    Profit = new HistoryLog(EconNoun.Profit);
	    }

	    public void Register(string good)
	    {
		    Prices.Register(good);
		    Asks.Register(good);
		    Bids.Register(good);
		    Trades.Register(good);
		    Profit.Register(good);
	    }
    }
}
