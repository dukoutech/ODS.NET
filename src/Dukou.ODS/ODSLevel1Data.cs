using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dukou.ODS
{
    public class ODSLevel1Data : ODSData
    {
        private static DateTime UTC1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public string Status
        {
            get
            {
                return Get("5");
            }
        }

        public string Symbol
        {
            get
            {
                return Get("10");
            }
        }

        /// <summary>
        /// 交易所或报价机构名字 
        /// </summary>
        public string ExChange
        {
            get
            {
                return Get("11");
            }
        }

        private decimal? askPrice;

        /// <summary>
        /// 最低卖方叫价
        /// </summary>
        public decimal? AskPrice
        {
            get
            {
                if (Get("20") == null)
                {
                    return null;
                }
                if (askPrice == null)
                {
                    askPrice = decimal.Parse(Get("20"));
                }
                return askPrice;
            }
        }

        private DateTime? askTime;

        /// <summary>
        /// 卖方叫价时间 
        /// </summary>
        public DateTime? AskTime
        {
            get
            {
                if (Get("21") == null)
                {
                    return null;
                }

                if (askTime == null)
                {
                    askTime = UTC1970.AddMilliseconds(double.Parse(Get("21")) * 1000);
                }

                return askTime.Value.ToLocalTime();
            }
        }

        private decimal? bidPrice;

        /// <summary>
        /// 最高买方叫价 
        /// </summary>
        public decimal? BidPrice
        {
            get
            {
                if (Get("22") == null)
                {
                    return null;
                }
                if (bidPrice == null)
                {
                    bidPrice = decimal.Parse(Get("22"));
                }
                return bidPrice;
            }
        }

        private DateTime? bidTime;

        /// <summary>
        /// 买方叫价时间 
        /// </summary>
        public DateTime? BidTime
        {
            get
            {
                if (Get("23") == null)
                {
                    return null;
                }
                if (bidTime == null)
                {
                    bidTime = UTC1970.AddMilliseconds(double.Parse(Get("23")) * 1000);
                }
                return bidTime.Value.ToLocalTime();
            }
        }

        private decimal? tradePrice;

        /// <summary>
        /// 最后成交价 
        /// </summary>
        public decimal? TradePrice
        {
            get
            {
                if (Get("24") == null)
                {
                    return null;
                }
                if (tradePrice == null)
                {
                    tradePrice = decimal.Parse(Get("24"));
                }
                return tradePrice;
            }
        }

        private DateTime? tradeTime;

        /// <summary>
        /// 最后成交时间
        /// </summary>
        public DateTime? TradeTime
        {
            get
            {
                if (Get("25") == null)
                {
                    return null;
                }
                if (tradeTime == null)
                {
                    tradeTime = UTC1970.AddMilliseconds(double.Parse(Get("25")) * 1000);
                }
                return tradeTime.Value.ToLocalTime();
            }
        }

        public override string ToString()
        {
            return $"Status={Status}|Symbol={Symbol}|ExChange={ExChange}|AskPrice={AskPrice?.ToString()}|AskTime={AskTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}|BidPrice={BidPrice?.ToString()}|BidTime={BidTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}|TradePrice={TradePrice?.ToString()}|TradeTime={TradeTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
        }
    }
}
