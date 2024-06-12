using System;

namespace congestion.calculator
{
    public class TimeRange
    {

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public static TimeRange FromString(string value)
        {
            try 
            {
                var result = new TimeRange();
                var times = value.Split('-',':');
                result.Start = new TimeSpan(Convert.ToInt32(times[0]), Convert.ToInt32(times[1]),0);
                result.End = new TimeSpan(Convert.ToInt32(times[2]), Convert.ToInt32(times[3]), 0);
                return result;
            }
            catch 
            {
                return null;
            }
        }

        public bool Includes(TimeSpan time) 
        {
            if(time>Start && time<End) 
                return true;
            return false; 
        }
    }
}
