using congestion.calculator;

namespace CongestionTaxCalculator.Api.Application
{
    public class CalculationHandler : ICalculationHandler
    {
        protected readonly int FreeHourPrice, NormalHourPrice, RushHourPrice, ExtraRushHourPrice;
        protected readonly int[] FreeVehicles, FreeMonths;
        protected readonly List<TimeRange> FreeHourRanges, NormalHourRanges, RushHourRanges, ExtraRushHourRanges;
        protected readonly List<MonthAndDay> Holidays;
        protected readonly bool HasExtraRushHour, IsFreeForDaysBeforeHolidays, PayOnceInAnHour;

        public CalculationHandler(int freeHourPrice, int normalHourPrice, int rushHourPrice, int extraRushHourPrice
            , int[] freeVehicles, int[] freeMonths
            , string[] freeHourRanges, string[] normalHourRanges, string[] rushHourRanges, string[]  extraRushHourRanges
            , bool hasExtraRushHour, bool isFreeForDaysBeforeHolidays, bool payOnceInAnHour
            , string[] holidays)
        {
            FreeHourPrice = freeHourPrice;
            NormalHourPrice = normalHourPrice;
            RushHourPrice = rushHourPrice;
            ExtraRushHourPrice = extraRushHourPrice;
            IsFreeForDaysBeforeHolidays = isFreeForDaysBeforeHolidays;
            FreeVehicles = freeVehicles;
            FreeMonths = freeMonths;
            FreeHourRanges = freeHourRanges.Select(x => TimeRange.FromString(x)).ToList();
            NormalHourRanges = normalHourRanges.Select(x => TimeRange.FromString(x)).ToList();
            RushHourRanges = rushHourRanges.Select(x => TimeRange.FromString(x)).ToList();
            ExtraRushHourRanges = extraRushHourRanges.Select(x => TimeRange.FromString(x)).ToList();
            Holidays = holidays.Select(x => MonthAndDay.FromString(x)).ToList();
        }


        public virtual async Task<VehicleTaxResult[]> GetResults(List<VehicleData> inputData)
        {
            var tasks = new List<Task<VehicleTaxResult>>();
            foreach (var data in inputData)
            {
                if (!FreeVehicles.Contains((int)data.VehicleType))
                    tasks.Add(CalculateVehicle(data));
            }
            var result = await Task.WhenAll(tasks);
            return result;
            
        }


        protected virtual async Task<VehicleTaxResult> CalculateVehicle(VehicleData vehicleData)
        { 
            var Data = vehicleData.PassageDateTimes
                .OrderBy(d => d.DayOfYear).AsQueryable();
            if (FreeMonths.Length > 0)
            {
                foreach (var d in FreeMonths)
                {
                    Data = Data.Where(x => x.Month != d);
                }
            }
            foreach (var holiday in Holidays)
            {
                Data = Data.Where(x => 
                            x.Month != holiday.Month 
                            || x.Day != holiday.Day);

                if (IsFreeForDaysBeforeHolidays)
                    Data = Data.Where(x =>
                        (x.AddDays(1).Month != holiday.Month)
                        || (x.AddDays(1).Day != holiday.Day));
            }
            var newData = Data.ToList();
            var result = await Task.Run<Dictionary<DateTime, ulong>>(() => newData
                            //.ToList()
                            .Select(x => x.Date)
                            .Distinct()
                            .ToDictionary(x => x, x => CalculateDay(newData.Where(d => d.Date == x.Date).ToList()))
                            );
            return new VehicleTaxResult { PlateNo = vehicleData.PlateNo, TaxAmounts = result };
        }


        protected virtual ulong CalculateDay(List<DateTime> passages)
        {
            if (FreeHourPrice == 0)
            { 
                foreach (var range in FreeHourRanges)
                {
                    passages = passages.Where(x => !range.Includes(x.TimeOfDay)).ToList();
                } 
            }
            var pass = passages.ToList();
            var passPrice = pass
                            .Select(x=> new { Time = x.TimeOfDay, Price = CalculateHourPrice(x.TimeOfDay) })
                            .OrderBy(t=>t.Time)
                            .ToList();
            var sum = 0;
            if (passPrice.Count > 0)
            {
                sum = passPrice[0].Price;
                var oneHour = new TimeSpan(0, 1, 0, 0);
                for (var i = 1; i < passPrice.Count; i++)
                {
                    if (PayOnceInAnHour)
                    {
                        if (passPrice[i].Time.Subtract(passPrice[i - 1].Time) <= oneHour)
                        {
                            int max = int.Max(passPrice[i].Price, passPrice[i - 1].Price);
                            sum += max;
                        }
                        else
                            sum += passPrice[i].Price;
                    }
                    else
                        sum += passPrice[i].Price;
                }
            }
            return Convert.ToUInt64(sum);
        }

        protected virtual int CalculateHourPrice(TimeSpan timeOfDay)
        {
            foreach(var range in NormalHourRanges)
                if(range.Includes(timeOfDay))
                    return NormalHourPrice;

            foreach(var range in RushHourRanges)
                if(range.Includes(timeOfDay))
                    return RushHourPrice;

            if (HasExtraRushHour)
                foreach (var range in ExtraRushHourRanges)
                    if (range.Includes(timeOfDay))
                        return ExtraRushHourPrice;

            return 0;
        }
    }
}
