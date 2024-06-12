using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion.calculator
{
    public class VehicleTaxResult
    {
        public string PlateNo {  get; set; }
        public Dictionary<DateTime,ulong> TaxAmounts { get; set; }

    }
}
