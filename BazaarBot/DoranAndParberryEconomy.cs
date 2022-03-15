﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using BazaarBot.Agents;

namespace BazaarBot
{
    class DoranAndParberryEconomy : Economy
    {

	    public DoranAndParberryEconomy()
	    {
		    var market = new Market("default",this);

            MarketData data = GetMarketData();
            market.init(data); // market.init(MarketData.fromJSON(Json.parse(Assets.getText("assets/settings.json")), getAgent));
		    AddMarket(market);
	    }

        private MarketData GetMarketData()
        {
            List<GoodStack> goods = new List<GoodStack>();
	        List<AgentData>agentTypes = new List<AgentData>();
	        List<AAgent> agents = new List<AAgent>();

            goods.Add(new GoodStack(new Good("food"), 0.5));
            goods.Add(new GoodStack(new Good("wood"), 1.0));
            goods.Add(new GoodStack(new Good("ore"), 1.0));
            goods.Add(new GoodStack(new Good("metal"), 1.0));
            goods.Add(new GoodStack(new Good("tools"), 1.0));
            goods.Add(new GoodStack(new Good("work"), 0.1));

            agentTypes.Add(new AgentData("farmer",100,"farmer"));
            agentTypes.Add(new AgentData("miner",100,"miner"));
            agentTypes.Add(new AgentData("refiner",100,"refiner"));
            agentTypes.Add(new AgentData("woodcutter",100,"woodcutter"));
            agentTypes.Add(new AgentData("blacksmith", 100, "blacksmith"));
            agentTypes.Add(new AgentData("worker", 10, "worker"));

            //InventoryData ii;
            //ii = new InventoryData(20, //farmer
            //    new Dictionary<string,double>{{"food",0},{"tools",1},{"wood",3},{"work",3}},
            //    new Dictionary<string,double>{{"food",1},{"tools",1},{"wood",0},{"work",0}},
            //    null
            //    );
            //agentTypes[0].inventory = ii; 
            //ii = new InventoryData(20, //miner
            //    new Dictionary<string, double> { { "food", 3 }, { "tools", 1 }, { "ore", 0 } },
            //    new Dictionary<string, double> { { "food", 1 }, { "tools", 1 }, { "ore", 0 } },
            //    null
            //    );
            //agentTypes[1].inventory = ii;
            //ii = new InventoryData(20, //refiner
            //    new Dictionary<string, double> { { "food", 3 }, { "tools", 1 }, { "ore", 5 } },
            //    new Dictionary<string, double> { { "food", 1 }, { "tools", 1 }, { "ore", 0 } },
            //    null
            //    );
            //agentTypes[2].inventory = ii;
            //ii = new InventoryData(20, //woodcutter
            //    new Dictionary<string, double> { { "food", 3 }, { "tools", 1 }, { "wood", 0 } },
            //    new Dictionary<string, double> { { "food", 1 }, { "tools", 1 }, { "wood", 0 } },
            //    null
            //    );
            //agentTypes[3].inventory = ii;
            //ii = new InventoryData(20, //blacksmith
            //    new Dictionary<string, double> { { "food", 3 }, { "tools", 1 }, { "metal", 5 }, { "ore", 0 } },
            //    new Dictionary<string, double> { { "food", 1 }, { "tools", 0 }, { "metal", 0 }, { "ore", 0 } },
            //    null
            //    );
            //agentTypes[4].inventory = ii;
            //ii = new InventoryData(20, //worker
            //    new Dictionary<string, double> { { "food", 3 }  },
            //    new Dictionary<string, double> { { "food", 1 } },
            //    null
            //    );
            //agentTypes[5].inventory = ii;


            int idc = 0;
            for (int iagent = 0; iagent < agentTypes.Count; iagent++)
            {
                for (int i = 0; i < 5; i++)
                {
                    agents.Add(getAgent(agentTypes[iagent]));
                    agents[agents.Count - 1].ID = idc++;
                }
            }


            MarketData data = new MarketData(goods, agentTypes, agents);

            return data;

            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "EconomySim.settings.txt";

            //string[] names = assembly.GetManifestResourceNames();


            //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    string result = reader.ReadToEnd();
            //    MarketData data = JsonConvert.DeserializeObject<MarketData>(result);
            //    return data;
            //}
            //return null;
        }


	    public override void signalBankrupt(Market m, AAgent a)
	    {
		    ReplaceAgent(m, a);
	    }

	    private void ReplaceAgent(Market market, AAgent agent)
	    {
		    var bestClass = market.GetMostProfitableAgentClass();

		    //Special case to deal with very high demand-to-supply ratios
		    //This will make them favor entering an underserved market over
		    //Just picking the most profitable class
		    var bestGood = market.GetHottestGood();

		    if (bestGood != null)
		    {
			    var bestGoodClass = GetAgentClassThatMakesMost(bestGood);
			    if (bestGoodClass != null)
			    {
				    bestClass = new Good(bestGoodClass);
			    }
		    }

		    var newAgent = getAgent(market.GetAgentClass(bestClass));
		    market.replaceAgent(agent, newAgent);
	    }


	    /**
	     * Get the average amount of a given good that a given agent class has
	     * @param	className
	     * @param	good
	     * @return
	     */
	    /*
	    public function getAgentClassAverageInventory(className:String, good:String):Float
	    {
		    var list = _agents.filter(function(a:BasicAgent):Bool { return a.className == className; } );
		    var amount:Float = 0;
		    for (agent in list)
		    {
			    amount += agent.queryInventory(good);
		    }
		    amount /= list.length;
		    return amount;
	    }
	    */

	    /**
	     * Find the agent class that produces the most of a given good
	     * @param	good
	     * @return
	     */
	    public String GetAgentClassThatMakesMost(Good good)
	    {
            //TODO - strongly type professions
            string res = "";
		    if (good.ID == "food" )      {res = "farmer";      }
            else if (good.ID == "wood")  { res = "woodcutter"; }
            else if (good.ID == "ore")   { res = "miner"; }
            else if (good.ID == "metal") {res = "refiner"; }
            else if (good.ID == "tools") { res = "blacksmith"; }
            else if (good.ID == "work") { res = "worker"; }
            return res;
	    }

	    /**
	     * Find the agent class that has the most of a given good
	     * @param	good
	     * @return
	     */
	    /*
	    public function getAgentClassWithMost(good:String):String
	    {
		    var amount:Float = 0;
		    var bestAmount:Float = 0;
		    var bestClass:String = "";
		    for (key in _mapAgents.keys())
		    {
			    amount = getAverageInventory(key, good);
			    if (amount > bestAmount)
			    {
				    bestAmount = amount;
				    bestClass = key;
			    }
		    }
		    return bestClass;
	    }
	    */

        //private BasicAgent getAgentScript(AgentData data)
        //{
        //    data.logic = new LogicScript(data.logicName+".hs");
        //    return new Agent(0, data);
        //}

        private AAgent getAgent(AgentData data)
        {
            data.logic = getLogic(data.LogicName);
            return new Agent(0, data);
        }

        private Logic getLogic(String str)
        {
            switch (str)
            {
                case "blacksmith": return new LogicBlacksmith();
                case "farmer": return new LogicFarmer();
                case "miner": return new LogicMiner();
                case "refiner": return new LogicRefiner();
                case "woodcutter": return new LogicWoodcutter();
                case "worker": return new LogicWorker();
            }
            return null;
        }
    }

}