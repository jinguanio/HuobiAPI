using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuobiAPI.Lib
{
    public enum OrderType { Buy = 1, Sell = 2 }
    public enum OrderStatus { Untraded = 0, SomeTraded = 1, Traded = 2, Cancelled = 3 }

    public class Order
    {
        public int Id;
        public OrderType Type;
        public decimal OrderPrice;
        public decimal OrderAmount;
        public decimal ProcessedPrice;
        public decimal ProcessedAmount;
        public DateTime OrderTime;
        public decimal Vot;
        public decimal Fee;
        public decimal Total;
        public OrderStatus Status;
    }
}
