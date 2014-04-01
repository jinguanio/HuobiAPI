using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuobiAPI.Lib
{
    public class Market
    {
        public decimal New;
        public decimal Open;
        public decimal Close;
        public decimal High;
        public decimal Low;
        public decimal Level;
        public decimal Volume;
        public decimal Total;
        public MarketTrade[] Sells;
        public MarketTrade[] Buys;
        public MarketTrade5[] TopSells;
        public MarketTrade5[] TopBuys;
        public MarketTraded[] Trades;
        public decimal AMP;
    }

    public class MarketTraded
    {
        public string Time;
        public decimal Price;
        public decimal Volume;
        public string Type;
    }

    public class MarketTrade
    {
        public decimal Price;
        public decimal Level;
        public decimal Volume;
    }
    public class MarketTrade5 : MarketTrade
    {
        public decimal ACCU;
    }
}
