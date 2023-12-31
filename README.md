# 5Paisa Connect .Net library
5Paisa Connect is a set of REST-like APIs that expose many capabilities required to build a complete investment and trading platform. Execute orders in real time, manage user portfolio, stream live market data (WebSockets), and more, with the simple HTTP API collection.
# Install Client Library

Execute in Tools » NuGet Package Manager » Package Manager Console

dotnet add package 5PaisaDotNetLibrary 1.1

Download 5Paisaapi-dotnet.dll file and add reference to your desktop/web application
# API Usage

            string APIKey = "";
            string EncryptionKey = "";
            string EncryptUserId = "";
            string RequestToken = "";
            string ClientCode = "";

            OutputBaseClass obj = new OutputBaseClass();
            connect = new _5PaisaAPI(APIKey, EncryptionKey, EncryptUserId);
            Token agr = new Token();

           // GenerateAccessToken
            obj = connect.GetOuthLogin(RequestToken);
            agr = obj.TokenResponse;

            //GetTOTP Login
            obj = connect.TOTPLogin(ClientCode, TOTP, Pin);
            if (obj.status == "0")
            {
                string RequestTokenOTP = obj.TokenResponse.RequestToken;
                obj = connect.GetOuthLogin(RequestTokenOTP);
                agr = obj.TokenResponse;
            }

            //Marketfeed

            OrderInfo MarketFeed = new OrderInfo()
            {
                MarketFeedData = new List<MarketFeedDataListReq>()
            };
            string[][] arr = new string[2][];

            // Initialize the elements.
            arr[0] = new string[4] { "N", "C", "0", "RELIANCE_EQ" };
            arr[1] = new string[4] { "N", "C", "0", "RELIANCE_EQ" };
            for (int i = 0; i < arr.Length; i++)
            {
                MarketFeedDataListReq a1 = new MarketFeedDataListReq();
                a1.Exch = arr[i][0];
                a1.ExchType = arr[i][1];
                a1.ScripCode  = Convert.ToInt32(arr[i][2]);
                a1.ScripData = arr[i][3];
                MarketFeed.MarketFeedData.Add(a1);
            }

            MarketFeed.LastRequestTime = DateTime.Today;
            MarketFeed.RefreshRate = "";
            obj = connect.MarketFeed(MarketFeed);
            MarketFeedResponse resMarketFeed = obj.MarketFeed;
            //End 

            //OrderBook
            OrderInfo orderBook = new OrderInfo();
            orderBook.ClientCode = ClientCode;
            obj = connect.OrderBook(orderBook);
            OrderBookResponse res = obj.OrderBook;

            //TradeBook
            OrderInfo TradeBook = new OrderInfo();
            TradeBook.ClientCode = ClientCode;
            obj = connect.TradeBook(TradeBook);
            TradeBookResponse resTradeBook = obj.TradeBook;

            //TradeHistory
            OrderInfo TradeHistory = new OrderInfo()
            {
                ExchOrderList = new List<ExchOrderIDList>()
            };
            var ExOrderId = new string[] { "1000000009723498", "1000000009723499", "1000000009723420" };
            
            foreach (var item in ExOrderId)
            {
                ExchOrderIDList ExchOrderId = new ExchOrderIDList();
                ExchOrderId.ExchOrderID = item;
                TradeHistory.ExchOrderList.Add(ExchOrderId);
            }
            TradeHistory.ClientCode = ClientCode;
            obj = connect.TradeHistory(TradeHistory);
            TradeHistoryResponse resTradeHistory = obj.TradeHistory;

            //PlaceOrder
            OrderInfo order = new OrderInfo();
            order.Exchange = '';
            order.ExchangeType = '';
            order.ScripCode = 0;
            order.ScripData = "";
            order.Price = 236;
            order.OrderType = "";
            order.Qty = 1;
            order.DisQty = 0;
            order.StopLossPrice = 0;
            order.IsIntraday = false;
            order.iOrderValidity = 0;
            order.AppSource = 10345;
            order.RemoteOrderID = "";
            obj = connect.placeOrder(order);
            OrderResponse resOrderbook = obj.PlaceOrderResponse;

            //ModifyOrder
            OrderInfo Modifyorder = new OrderInfo();
            Modifyorder.Qty = 2;
            //Modifyorder.StopLossPrice = "";
            //Modifyorder.Price = "";
            Modifyorder.ExchOrderID = "";
            obj = connect.ModifyOrder(Modifyorder);
            OrderResponse Modifyres = obj.PlaceOrderResponse;

            //CancelOrder
            OrderInfo Cancelorder = new OrderInfo();
            Modifyorder.ExchOrderID = ClientCode;
            obj = connect.CancelOrder(order);
            OrderResponse Cancelres = obj.PlaceOrderResponse;

       
