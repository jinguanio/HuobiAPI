using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace HuobiAPI.Lib
{
    public enum MarketType { BTC, LTC }
    public enum MarketPeriod { M1, M5, M15, M30, M60, D, W, M, Y }

    public class Huobi
    {
        private string key, secret;
        private int timeout,timezone;
        private string url = "https://api.huobi.com/api.php";
        public Huobi(string _key, string _secret, int _timeout = 5000, int _timezone = 8)
        {
            this.key = _key;
            this.secret = _secret;
            this.timeout = _timeout;
            this.timezone = _timezone;
        }

        #region 行情API
        #region GetKLine
        public static KLine[] GetKLine(MarketType _type, MarketPeriod _period)
        {
            string _url = "";
            switch (_type)
            {
                case MarketType.BTC: _url = "https://market.huobi.com/staticmarket/kline{0}.html"; break;
                case MarketType.LTC: _url = "https://market.huobi.com/staticmarket/kline_ltc{0}.html"; break;
            }
            switch (_period)
            {
                case MarketPeriod.M1: _url = string.Format(_url, "001"); break;
                case MarketPeriod.M5: _url = string.Format(_url, "005"); break;
                case MarketPeriod.M15: _url = string.Format(_url, "015"); break;
                case MarketPeriod.M30: _url = string.Format(_url, "030"); break;
                case MarketPeriod.M60: _url = string.Format(_url, "060"); break;
                case MarketPeriod.D: _url = string.Format(_url, "100"); break;
                case MarketPeriod.W: _url = string.Format(_url, "200"); break;
                case MarketPeriod.M: _url = string.Format(_url, "300"); break;
                case MarketPeriod.Y: _url = string.Format(_url, "400"); break;
            }

            string _result = Get(_url);
            IList<KLine> _klines = new List<KLine>();
            string[] _lines = _result.Split('\n');
            foreach (string _line in _lines)
            {
                string[] _item = _line.Replace("\r", "").Split(',');
                if (_item.Length != 8) { continue; }
                KLine _kline = new KLine();
                _kline.DateTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    _item[0].Substring(0,4),
                    _item[0].Substring(4,2),
                    _item[0].Substring(6,2),
                    _item[1].Substring(0,2),
                    _item[1].Substring(2,2),
                    _item[1].Substring(4,2)));
                _kline.Open = decimal.Parse(_item[2]);
                _kline.High = decimal.Parse(_item[3]);
                _kline.Low = decimal.Parse(_item[4]);
                _kline.Close = decimal.Parse(_item[5]);
                _kline.Volume = decimal.Parse(_item[6]);
                _kline.Total = decimal.Parse(_item[7]);
                _klines.Add(_kline);
            }
            return _klines.ToArray();
        }
        #endregion

        #region GetLine
        public static Line[] GetLine(MarketType _type, out decimal _middlePrice)
        {
            string _url = "";
            switch (_type)
            {
                case MarketType.BTC: _url = "https://market.huobi.com/staticmarket/td.html"; break;
                case MarketType.LTC: _url = "https://market.huobi.com/staticmarket/td_ltc.html"; break;
            }

            decimal _output = 0M;
            string _result = Get(_url);
            IList<Line> _tlines = new List<Line>();
            string[] _lines = _result.Split('\n');
            foreach (string _line in _lines)
            {
                string[] _item = _line.Replace("\r", "").Split(',');
                if (_item.Length == 1)
                {
                    decimal _current = 0M;
                    if (decimal.TryParse(_item[0], out _current)) { _output = _current; }
                    continue;
                }
                else if (_item.Length != 4)
                {
                    continue;
                }

                Line _tline = new Line();
                _tline.Time = _item[0];
                _tline.Price = decimal.Parse(_item[1]);
                _tline.Volume = decimal.Parse(_item[2]);
                _tline.Total = decimal.Parse(_item[3]);
                _tlines.Add(_tline);
            }

            _middlePrice = _output;
            return _tlines.ToArray();
        }
        #endregion

        #region GetTrades
        public static Market GetTrades(MarketType _type)
        {
            string _url = "";
            switch (_type)
            {
                case MarketType.BTC: _url = "https://market.huobi.com/staticmarket/detail.html"; break;
                case MarketType.LTC: _url = "https://market.huobi.com/staticmarket/detail_ltc.html"; break;
            }

            string _result = "";
            JObject _json;

            while(true)
            {
                _result = Get(_url);
                _result = _result.Replace("view_detail(", "");
                _result = _result.Substring(0, _result.LastIndexOf("}") + 1);
                try
                {
                    _json = JObject.Parse(_result);
                    break;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            Market _market = new Market();
            _market.New = _json["p_new"].Value<decimal>();
            _market.Open = _json["p_open"].Value<decimal>();
            _market.Close = _json["p_last"].Value<decimal>();
            _market.High = _json["p_high"].Value<decimal>();
            _market.Low = _json["p_low"].Value<decimal>();
            _market.Level = _json["level"].Value<decimal>();
            _market.Volume = _json["amount"].Value<decimal>();
            _market.Total = _json["total"].Value<decimal>();
            _market.AMP = _json["amp"].Value<decimal>();

            _market.Buys = new MarketTrade[_json["buys"].Count()];
            for (int i = 0; i < _market.Buys.Length; i++)
            {
                MarketTrade _trade = new MarketTrade();
                _trade.Price = _json["buys"][i]["price"].Value<decimal>();
                _trade.Level = _json["buys"][i]["level"].Value<decimal>();
                _trade.Volume = _json["buys"][i]["amount"].Value<decimal>();
                _market.Buys[i] = _trade;
            }

            _market.Sells = new MarketTrade[_json["sells"].Count()];
            for (int i = 0; i < _market.Sells.Length; i++)
            {
                MarketTrade _trade = new MarketTrade();
                _trade.Price = _json["sells"][i]["price"].Value<decimal>();
                _trade.Level = _json["sells"][i]["level"].Value<decimal>();
                _trade.Volume = _json["sells"][i]["amount"].Value<decimal>();
                _market.Sells[i] = _trade;
            }

            _market.TopBuys = new MarketTrade5[_json["top_buy"].Count()];
            for (int i = 0; i < _market.TopBuys.Length; i++)
            {
                MarketTrade5 _trade = new MarketTrade5();
                _trade.Price = _json["top_buy"][i]["price"].Value<decimal>();
                _trade.Level = _json["top_buy"][i]["level"].Value<decimal>();
                _trade.Volume = _json["top_buy"][i]["amount"].Value<decimal>();
                _trade.ACCU = _json["top_buy"][i]["accu"].Value<decimal>();
                _market.TopBuys[i] = _trade;
            }

            _market.TopSells = new MarketTrade5[_json["top_sell"].Count()];
            for (int i = 0; i < _market.TopSells.Length; i++)
            {
                MarketTrade5 _trade = new MarketTrade5();
                if (_type == MarketType.LTC)
                {
                    _trade.Price = _json["top_sell"][(4 - i).ToString()]["price"].Value<decimal>();
                    _trade.Level = _json["top_sell"][(4 - i).ToString()]["level"].Value<decimal>();
                    _trade.Volume = _json["top_sell"][(4 - i).ToString()]["amount"].Value<decimal>();
                    _trade.ACCU = _json["top_sell"][(4 - i).ToString()]["accu"].Value<decimal>();
                }
                else
                {
                    _trade.Price = _json["top_sell"][i]["price"].Value<decimal>();
                    _trade.Level = _json["top_sell"][i]["level"].Value<decimal>();
                    _trade.Volume = _json["top_sell"][i]["amount"].Value<decimal>();
                    _trade.ACCU = _json["top_sell"][i]["accu"].Value<decimal>();
                }
                _market.TopSells[i] = _trade;
            }

            _market.Trades = new MarketTraded[_json["trades"].Count()];
            for (int i = 0; i < _market.Trades.Length; i++)
            {
                MarketTraded _trade = new MarketTraded();
                _trade.Time = _json["trades"][i]["time"].Value<string>();
                _trade.Price = _json["trades"][i]["price"].Value<decimal>();
                _trade.Volume = _json["trades"][i]["amount"].Value<decimal>();
                _trade.Type = _json["trades"][i]["type"].Value<string>();
                _market.Trades[i] = _trade;
            }

            return _market;
        }
        #endregion
        #endregion

        #region 交易API
        #region GetAccount
        public Account GetAccount()
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("created", this.GetTime().ToString());
            _params.Add("method", "get_account_info");

            JObject _json = this.Post(_params);
            Account _account = new Account();
            _account.Total = _json["total"].Value<decimal>();
            _account.NetAsset = _json["net_asset"].Value<decimal>();
            _account.AvailableCNY = _json["available_cny_display"].Value<decimal>();
            _account.AvailableBTC = _json["available_btc_display"].Value<decimal>();
            _account.FrozenCNY = _json["frozen_cny_display"].Value<decimal>();
            _account.FrozenBTC = _json["frozen_btc_display"].Value<decimal>();
            _account.LoanCNY = _json["loan_cny_display"].Value<decimal>();
            _account.LoanBTC = _json["loan_btc_display"].Value<decimal>();

            return _account;
        }
        #endregion

        #region GetOrders
        public Order[] GetOrders()
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("created", this.GetTime().ToString());
            _params.Add("method", "get_orders");

            JObject _json = this.Post(_params);
            int _count = _json["list"].Count();
            Order[] _orders = new Order[_count];
            for(int i=0;i<_count;i++)
            {
                Order _order = new Order();
                _order.Id = _json["list"][i]["id"].Value<int>();
                _order.Type = (OrderType)_json["list"][i]["type"].Value<int>();
                _order.OrderPrice = _json["list"][i]["order_price"].Value<decimal>();
                _order.OrderAmount = _json["list"][i]["order_amount"].Value<decimal>();
                _order.ProcessedAmount = _json["list"][i]["processed_amount"].Value<decimal>();
                _order.OrderTime = DateTime.Parse("1970-1-1 0:0:0.0").AddSeconds(_json["list"][i]["order_time"].Value<int>()).AddHours(this.timezone);
                _orders[i] = _order;
            }

            return _orders;
        }
        #endregion

        #region GetOrderInfo
        public Order GetOrderInfo(int _id)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("created", this.GetTime().ToString());
            _params.Add("id", _id.ToString());
            _params.Add("method", "order_info");

            JObject _json = this.Post(_params);
            Order _order = new Order();
            _order.Id = _json["id"].Value<int>();
            _order.Type = (OrderType)_json["type"].Value<int>();
            _order.OrderPrice = _json["order_price"].Value<decimal>();
            _order.OrderAmount = _json["order_amount"].Value<decimal>();
            _order.ProcessedPrice = _json["processed_price"].Value<decimal>();
            _order.ProcessedAmount = _json["processed_amount"].Value<decimal>();
            _order.Vot = _json["vot"].Value<decimal>();
            _order.Fee = _json["fee"].Value<decimal>();
            _order.Total = _json["total"].Value<decimal>();
            _order.Status = (OrderStatus)_json["status"].Value<int>();

            return _order;
        }
        #endregion

        #region Buy
        public int Buy(decimal _price, decimal _amount)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("amount", _amount.ToString());
            _params.Add("created", this.GetTime().ToString());
            _params.Add("method", "buy");
            _params.Add("price", _price.ToString());

            JObject _json = this.Post(_params);
            return _json["id"].Value<int>();
        }
        #endregion

        #region Sell
        public int Sell(decimal _price, decimal _amount)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("amount", _amount.ToString());
            _params.Add("created", this.GetTime().ToString());
            _params.Add("method", "sell");
            _params.Add("price", _price.ToString());

            JObject _json = this.Post(_params);
            return _json["id"].Value<int>();
        }
        #endregion

        #region Cancel
        public void Cancel(int _id)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("created", this.GetTime().ToString());
            _params.Add("id", _id.ToString());
            _params.Add("method", "cancel_order");

            this.Post(_params);
        }
        #endregion

        #region Modify
        public int Modify(int _id, decimal _price, decimal _amount)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("amount", _amount.ToString());
            _params.Add("created", this.GetTime().ToString());
            _params.Add("id", _id.ToString());
            _params.Add("method", "modify_order");
            _params.Add("price", _price.ToString());

            JObject _json = this.Post(_params);
            return _json["id"].Value<int>();
        }
        #endregion

        #region GetTime
        private int GetTime()
        {
            return (int)(DateTime.UtcNow - DateTime.Parse("1970-1-1 0:0:0.0")).TotalSeconds;
        }
        #endregion
        #endregion

        #region Get
        private static string Get(string _url, int _timeout = 5000)
        {
            WebClientPlus _webClient = new WebClientPlus(_timeout);
            string _result = _webClient.DownloadString(_url);
            _webClient.Dispose();

            return _result;
        }
        #endregion

        #region Post
        private JObject Post(Dictionary<string, string> _params)
        {
            string _data = "";
            foreach (KeyValuePair<string, string> _param in _params)
            {
                _data += _data == "" ? "" : "&";
                _data += _param.Key + "=" + _param.Value;
            }
            _data = "access_key=" + this.key + "&" + _data;
            string _sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(_data + "&secret_key=" + this.secret, "MD5").ToLower();
            _data += "&sign=" + _sign;

            WebClientPlus _webClient = new WebClientPlus(this.timeout);
            _webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string _result = _webClient.UploadString(this.url, _data);
            _webClient.Dispose();

            if (_result[0] == '[') { _result = "{\"list\":" + _result + "}"; }

            JObject _json = JObject.Parse(_result);
            if (_json.GetValue("code") != null)
            {
                throw new HuobiException(_json.GetValue("code").Value<string>(), _json.GetValue("msg").Value<string>());
            }

            return _json;
        }
        #endregion
    }
}
