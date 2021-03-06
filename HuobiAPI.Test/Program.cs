﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HuobiAPI.Lib;

namespace HuobiAPI.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string _key = "";
            string _secert = "";

            Huobi _huobi = new Huobi(_key, _secert);

            try
            {
                while (true)
                {
                    try
                    {
                        decimal _price = 0M;
                        Line[] _tlines = Huobi.GetLine(MarketType.BTC,out _price);
                        Console.WriteLine(_price + " - " + _tlines.Length);

                        //KLine[] _klines = Huobi.GetKLine(MarketType.BTC, MarketPeriod.M1);
                        //Console.WriteLine(_klines.Length);

                        //Market _market = Huobi.GetTrades(MarketType.BTC);
                        //Console.WriteLine(_market.New);
                        Thread.Sleep(1000);
                    }
                    catch (System.Net.WebException _ex)
                    {
                        Console.WriteLine(_ex.Message);
                    }
                }

                //int _id = _huobi.Buy(3000, 0.01M);
                //Console.WriteLine("Buy：" + _id);

                //int _id = _huobi.Sell(3000, 0.01M);
                //Console.WriteLine("Sell：" + _id);

                //Order[] _orders = _huobi.GetOrders();
                //Console.WriteLine(_orders.Length);
                //for (int i = 0; i < _orders.Length; i++)
                //{
                //    Order _order = _huobi.GetOrderInfo(_orders[i].Id);
                //    Console.WriteLine(_order.Type.ToString() + ":" + _order.Id);
                //}

                //_huobi.Cancel(11629703);
                //Console.WriteLine("Canceled");

                //int _id = _huobi.Modify(11625875, 3111M, 0.01M);
                //Console.WriteLine("Modify：" + _id);

                //Account _account = _huobi.GetAccount();
                //Console.WriteLine(_account.Total);
            }
            catch (HuobiException _ex)
            {
                Console.WriteLine(_ex.Code);
                Console.WriteLine(_ex.Text);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message);
            }


            Console.ReadLine();
        }
    }
}
