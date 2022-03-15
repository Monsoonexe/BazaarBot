using System;

namespace BazaarBot.Agents
{
    class Logic
    {
	    private bool init = false;

        //public function new(?data:Dynamic)
        //{
        //    //no implemenation -- provide your own in a subclass
        //}

	    /**
	     * Perform this logic on the given agent
	     * @param	agent
	     */

	    public virtual void perform(AAgent agent, Market market)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

        protected void Produce(AAgent agent, Good commodity, double amount, double chance = 1.0)
	    {
		    if (chance >= 1.0 || Quick.rnd.NextDouble() < chance)
		    {
			    agent.ProduceInventory(commodity, amount);
		    }
	    }

	    protected void Consume(AAgent agent, Good commodity, double amount, double chance = 1.0)
	    {
            if (chance >= 1.0 || Quick.rnd.NextDouble() < chance)
		    {
                //if (commodity == "money")
                //{
                //    agent.changeInventory(comm
                //    agent.money -= amount;
                //}
                //else
                //{
				    agent.ConsumeInventory(commodity, -amount);
                //}
		    }
	    }

    }

    //TODO - strongly type Goods, their logic, and their professions

    class LogicBlacksmith : Logic
    {
        override public void perform(AAgent agent, Market market)
        {
            var food = agent.QueryQuantity(new Good("food"));
            var metal = agent.QueryQuantity(new Good("metal"));
            var tools = agent.QueryQuantity(new Good("tools"));
            var need_tools = tools < 4;

            var has_food = food >= 1;
            var has_metal = metal >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, new Good("food"), 1);//cost of living

            if (has_food && has_metal & need_tools)
            {
                //convert all metal into tools
                Consume(agent, new Good("metal"), metal);
                Produce(agent, new Good("tools"), metal);
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent, "food", 2); stub todo needed?
                }
            }
        }
    }

    class LogicFarmer : Logic
    {
        override public void perform(AAgent agent, Market market)
        {
            var wood = agent.QueryQuantity(new Good("wood"));
            var tools = agent.QueryQuantity(new Good("tools"));
            var food = agent.QueryQuantity(new Good("food"));
            var need_food = food < 10;
            var work = agent.QueryQuantity(new Good("work"));

            var has_wood = wood >= 1;
            var has_tools = tools >= 1;
            var has_work = work >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business

            if (need_food)
            {
                if (has_wood && has_tools && has_work)
                {
                    //produce 4 food, consume 1 wood, break tools with 10% chance
                    Consume(agent, new Good("wood"), 1, 1);
                    Consume(agent, new Good("tools"), 1, 0.1);
                    Consume(agent, new Good("work"), 1, 1);
                    Produce(agent, new Good("food"), 6, 1);
                }
                else if (has_wood && !has_tools && has_work)
                {
                    //produce 2 food, consume 1 wood
                    Consume(agent, new Good("wood"), 1, 1);
                    Consume(agent, new Good("work"), 1, 1);
                    Produce(agent, new Good("food"), 3, 1);
                }
                else //no wood
                {
                    //produce 1 food, 
                    Produce(agent, new Good("food"), 1, 1);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
            }
        }
    }


    class LogicMiner : Logic
    {
        override public void perform(AAgent agent, Market market)
        {
            var food = agent.QueryQuantity(new Good("food"));
            var tools = agent.QueryQuantity(new Good("tools"));
            var ore = agent.QueryQuantity(new Good("ore"));
            var need_ore = ore < 4;

            var has_food = food >= 1;
            var has_tools = tools >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, new Good("food"), 1);//cost of living

            if (has_food && need_ore)
            {
                if (has_tools)
                {
                    //produce 4 ore, consume 1 food, break tools with 10% chance
                    Consume(agent, new Good("food"), 1);
                    Consume(agent, new Good("tools"), 1, 0.1);
                    Produce(agent, new Good("ore"), 4);
                }
                else
                {
                    //produce 2 ore, consume 1 food
                    Consume(agent, new Good("food"), 1);
                    Produce(agent, new Good("ore"), 2);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent,"food",2);
                }
            }
        }
    }

    class LogicRefiner : Logic
    {
        override public void perform(AAgent agent, Market market)
        {
            var food = agent.QueryQuantity(new Good("food"));
            var tools = agent.QueryQuantity(new Good("tools"));
            var ore = agent.QueryQuantity(new Good("ore"));
            if (ore > 4) ore = 4;
            var metal = agent.QueryQuantity(new Good("metal"));
            var need_metal = metal < 4;

            var has_food = food >= 1;
            var has_tools = tools >= 1;
            var has_ore = ore >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, new Good("food"), 1);//cost of living

            if (has_food && has_ore && need_metal)
            {
                if (has_tools)
                {
                    //convert all ore into metal, consume 1 food, break tools with 10% chance
                    Consume(agent, new Good("ore"), ore);
                    Consume(agent, new Good("food"), 1);
                    Consume(agent, new Good("tools"), 1, 0.1);
                    Produce(agent, new Good("metal"), ore);
                }
                else
                {
                    //convert up to 2 ore into metal, consume 1 food
                    var max = agent.QueryQuantity(new Good("ore"));
                    if (max > 2) { max = 2; }
                    Consume(agent, new Good("ore"), max);
                    Consume(agent, new Good("food"), 1);
                    Produce(agent, new Good("metal"), max);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent, "food", 2);
                }
            }
        }
    }

    class LogicWoodcutter : Logic
    {
        override public void perform(AAgent agent, Market market)
        {
            var food = agent.QueryQuantity(new Good("food"));
            var tools = agent.QueryQuantity(new Good("tools"));
            var wood = agent.QueryQuantity(new Good("wood"));
            var need_wood = wood < 4;

            var has_food = food >= 1;
            var has_tools = tools >= 1;

            //_consume(agent, "money", 0.5);//cost of living/business
            Consume(agent, new Good("food"), 1);//cost of living

            if (has_food && need_wood)
            {
                if (has_tools)
                {
                    //produce 2 wood, consume 1 food, break tools with 10% chance
                    Consume(agent, new Good("food"), 1);
                    Consume(agent, new Good("tools"), 1, 0.1);
                    Produce(agent, new Good("wood"), 2);
                }
                else
                {
                    //produce 1 wood, consume 1 food
                    Consume(agent, new Good("food"), 1);
                    Produce(agent, new Good("wood"), 1);
                }
            }
            else
            {
                //fined $2 for being idle
                //_consume(agent, "money", 2);
                if (!has_food && agent.get_inventoryFull())
                {
                    //make_room_for(agent, "food", 2);
                }
            }
        }
    }

    class LogicWorker : Logic
    {
        override public void perform(AAgent agent, Market market)
        {
            var food = agent.QueryQuantity(new Good("food"));
            var has_food = food >= 1;
            var work = agent.QueryQuantity(new Good("work"));
            var need_work = work < 1;

            Consume(agent, new Good("food"), 1);
            //TODO - _consume(agent, "money", 0.5);//cost of living/business

            if (need_work)
            {
                Produce(agent, new Good("work"), 1);
            }
        }
    }


}
