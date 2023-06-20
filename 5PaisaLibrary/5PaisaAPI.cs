using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace _5PaisaLibrary
{
    public class _5PaisaAPI
    {
        private string _root = "https://Openapi.5paisa.com/VendorsAPI/Service1.svc/";
        private string _apiKey;
        private string EncryptionKey;
        private string encryptUserId;
        Token Token { get; set; }
        public _5PaisaAPI(string apiKey, string encryptionKey, string encryptUserId,string Root = null)
        {

            _apiKey = apiKey;
            EncryptionKey = encryptionKey;
            this.encryptUserId = encryptUserId;
           
        }
        /* Makes a POST request */
        private string POSTWebRequest(Token agr, string URL, string Data)
        {
            try
            {
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                HttpWebRequest httpWebRequest = null;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                if (agr != null)
                    httpWebRequest.Headers.Add("Authorization", "Bearer " + agr.AccessToken);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";

                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                httpWebRequest.ContentLength = byteArray.Length;
                string Json = "";

                Stream dataStream = httpWebRequest.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = httpWebRequest.GetResponse();

                using (dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    Json = reader.ReadToEnd();
                }
                return Json;
            }
            catch (Exception ex)
            {
                return "PostError:" + ex.Message;
            }
        }

        #region OuthLogin
        public OutputBaseClass GetOuthLogin(string RequestToken)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                TokenResponse agr = new TokenResponse();
                string URL = _root + "GetAccessToken";
                var dataStringSession = JsonConvert.SerializeObject(new
                {
                    head = new { Key = _apiKey },
                    //body = new { ClientCode= ClientCode, JWTToken = Token, Key= VendorKey, AllowMap = Allowmap }
                    body = new { RequestToken = RequestToken, EncryKey = EncryptionKey, UserId = encryptUserId }

                });
                var json = POSTWebRequest(null, URL, dataStringSession);
                agr = JsonConvert.DeserializeObject<TokenResponse>(json);
                if (agr.body.Status == "0")
                {

                    res.TokenResponse = agr.body;
                    res.status = agr.body.Status;
                    res.http_error = agr.body.Message;
                    res.http_code = agr.errorcode;
                    this.Token = agr.body;
                }
                else
                {

                    res.status = agr.body.Status;
                    res.http_error = agr.body.Message;
                }
            }
            catch (Exception ex)
            {

                res.http_error = ex.Message.ToString();
            }
            return res;
        }
        #endregion

        #region TOTPLogin
        public OutputBaseClass TOTPLogin(string _EmailId,string _TOTP,string _Pin)

        {
            OutputBaseClass res = new OutputBaseClass();


            try
            {
                TokenResponse agr = new TokenResponse();
                string URL = _root + "TOTPLogin";
                var dataStringSession = JsonConvert.SerializeObject(new
                {
                    head = new { Key = _apiKey },
                    body = new { Email_ID = _EmailId, TOTP = _TOTP, PIN = _Pin }

                });
                var json = POSTWebRequest(null, URL, dataStringSession);
                agr = JsonConvert.DeserializeObject<TokenResponse>(json);
                if (agr.body.Status == "0")
                {
                    res.TokenResponse = agr.body;
                    res.status = agr.body.Status;
                    res.http_error = agr.body.Message;
                    this.Token = agr.body;
                }
                else
                {
                    res.status = agr.body.Status;
                    res.http_error = agr.body.Message;

                }
            }
            catch (Exception ex)
            {
                //res.status = false;

                res.http_error = ex.Message;
            }
            return res;
        }
        #endregion

        #region PlaceOrder/ModifyOrder/CancelOrder

        public OutputBaseClass placeOrder(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();
            
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V1/PlaceOrderRequest";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                Exchange = order.Exchange,
                                ExchangeType = order.ExchangeType,
                                ScripCode = order.ScripCode,
                                ScripData = order.ScripData,
                                Price = order.Price,
                                OrderType = order.OrderType,
                                Qty = order.Qty,
                                DisQty = order.DisQty,
                                StopLossPrice = order.StopLossPrice,
                                IsIntraday = order.IsIntraday,
                                iOrderValidity = order.iOrderValidity,
                                AppSource = order.AppSource,
                                RemoteOrderID = order.RemoteOrderID

                            }

                        });

                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                        if (pres.body.Status=="0")
                        {
                           // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.PlaceOrderResponse = pres;
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                           
                        }
                        else
                        {
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status ="-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                //res.status = false;
                //res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }

        public OutputBaseClass ModifyOrder(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V1/ModifyOrderRequest";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                
                                Price = order.Price==null?0: order.Price,
                                Qty = order.Qty==null?0:order.Qty,
                                StopLossPrice = order.StopLossPrice==null?0:order.StopLossPrice,
                                ExchOrderID=order.ExchOrderID
                            }

                        });

                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                        if (pres.body.Status == "0")
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.PlaceOrderResponse = pres;
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;

                        }
                        else
                        {
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "false";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                res.status = "-1";
                res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }

        public OutputBaseClass CancelOrder(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V1/CancelOrderRequest";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                ExchOrderID = order.ExchOrderID,

                            }

                        });

                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                        if (pres.body.Status == "0")
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.PlaceOrderResponse = pres;
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                        }
                        else
                        {
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                //res.status = false;
                res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }

        #endregion

        #region TradeBook
        public OutputBaseClass TradeBook(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V1/TradeBook";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                ClientCode = order.ClientCode,

                            }
                        });

                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        TradeBookResponse pres = JsonConvert.DeserializeObject<TradeBookResponse>(Json);
                        if (pres.body.status == "0")
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.TradeBook = pres;
                            res.status = pres.body.status;
                            res.http_error = pres.body.message;

                        }
                        else
                        {
                            res.status = pres.body.status;
                            res.http_error = pres.body.message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                //res.status = false;
                res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }

        #endregion

        #region TradeHistory
        public OutputBaseClass TradeHistory(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V1/TradeHistory";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                ClientCode = order.ClientCode,
                                ExchOrderIDs = order.ExchOrderList

                            }

                        });

                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        TradeHistoryResponse pres = JsonConvert.DeserializeObject<TradeHistoryResponse>(Json);
                        if (pres.body.Status == "0")
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.TradeHistory = pres;
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;

                        }
                        else
                        {
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                res.http_error = ex.Message;
            }
            return res;

        }
        #endregion

        #region OrderBook  
        public OutputBaseClass OrderBook(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V3/OrderBook";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                ClientCode = order.ClientCode,
                            }

                        });
                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        OrderBookResponse pres = JsonConvert.DeserializeObject<OrderBookResponse>(Json);
                        if (pres.body.Status == "0")
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.OrderBook = pres;
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;

                        }
                        else
                        {
                            res.status = pres.body.Status;
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                //res.status = false;
               // res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }

        #endregion

        #region MarketFeed
        public OutputBaseClass MarketFeed(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V1/MarketFeed";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                MarketFeedData = order.MarketFeedData,

                                LastRequestTime = order.LastRequestTime,
                                RefreshRate = order.RefreshRate
                            }

                        }); ;
                    
                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        MarketFeedResponse pres = JsonConvert.DeserializeObject<MarketFeedResponse>(Json);
                        if (pres.body.Status == 0)
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.MarketFeed = pres;
                            res.status = Convert.ToString(pres.body.Status);
                            res.http_error = pres.body.Message;

                        }
                        else
                        {
                            res.status = Convert.ToString(pres.body.Status);
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                //res.status = false;
                // res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }
        #endregion

        #region NetPositionNetWise
        public OutputBaseClass NetPositionNetWise(OrderInfo order)
        {
            OutputBaseClass res = new OutputBaseClass();

            res.http_code = "200";
            try
            {
                Token Token = this.Token;
                if (Token != null)
                {
                    if (ValidateToken(Token))
                    {
                        string URL = _root + "V2/NetPositionNetWise";
                        var dataStringSession = JsonConvert.SerializeObject(new
                        {
                            head = new { key = _apiKey },
                            body = new
                            {
                                ClientCode = order.ClientCode
                    }

                        }); 

                        string Json = POSTWebRequest(Token, URL, dataStringSession);
                        NetPositionNetWiseRes pres = JsonConvert.DeserializeObject<NetPositionNetWiseRes>(Json);
                        if (pres.body.Status == 0)
                        {
                            // OrderResponse pres = JsonConvert.DeserializeObject<OrderResponse>(Json);
                            res.NetPositionNetWise = pres;
                            res.status = Convert.ToString(pres.body.Status);
                            res.http_error = pres.body.Message;

                        }
                        else
                        {
                            res.status = Convert.ToString(pres.body.Status);
                            res.http_error = pres.body.Message;
                            //res.http_error = Json.Replace("PostError:", "");
                        }
                    }
                    else
                    {
                        res.status = "-1";
                        res.http_error = "Token not exist";
                    }
                }
                else
                {
                    res.status = "-1";
                    res.http_error = "Token not exist";
                }
            }
            catch (Exception ex)
            {
                //res.status = false;
                // res.http_code = "404";
                res.http_error = ex.Message;
            }
            return res;

        }

        #endregion

        #region ValidateToken
        private bool ValidateToken(Token token)
        {
            bool result = false;
            if (token != null)
            {
                if (token.AccessToken != "")
                {
                    result = true;
                }
            }
            else
                result = false;

            return result;
        }
        #endregion
    }
}



