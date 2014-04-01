using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static void KLine(MarketType _type, MarketPeriod _period)
        {
            string _url = "";
            switch(_type)
            {
                case MarketType.BTC: _url = "https://market.huobi.com/staticmarket/kline{0}.html"; break;
                case MarketType.LTC: _url = "https://market.huobi.com/staticmarket/kline_ltc{0}.html"; break;
            }
            switch(_period)
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


        }
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
            string _sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(_data + "&secret_key="+this.secret, "MD5").ToLower();
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
        #endregion
    }
}
