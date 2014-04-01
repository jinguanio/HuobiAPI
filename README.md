Huobi.com 交易和行情API的.Net类库
========
做项目时随手整理的API，缺少注释请多包涵。
========
行情API全使用静态方法调用：
Huobi.GetKLine      分时行情数据接口（K线）
Huobi.GetLine       实时行情数据接口
Huobi.GetTrades     买卖盘 实时成交数据
========
交易API使用动态方法调用，需要先实例化Huobi对象
Huobi.GetAccount    获取个人资产信息
Huobi.GetOrders     获取所有正在进行的委托
Huobi.GetOrderInfo  获取委托详情
Huobi.Buy           买入
Huobi.Sell          卖出
Huobi.Cancel        取消委托单
Huobi.Modify        修改订单
========
行情API文档：
https://www.huobi.com/help/index.php?a=market_help
交易API文档：
https://www.huobi.com/help/index.php?a=api_help
