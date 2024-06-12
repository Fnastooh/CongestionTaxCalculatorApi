using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion.calculator
{
    public class MonthAndDay
    {
        public int Month { get; set; }
        public int Day { get; set; }

        public static MonthAndDay FromString(string value)
        {
            try
            {
                var result = new MonthAndDay()
                {
                    Month = Convert.ToInt32(value.Split('/')[0]),
                    Day = Convert.ToInt32(value.Split("/")[1])
                };
                return result;
            }
            catch
            {
                return null;
            }
        }

    }
}
