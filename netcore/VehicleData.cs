using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion.calculator
{
    public class VehicleData
    {
        public VehicleType VehicleType { get; set; }
        public string PlateNo { get; set; }
        public List<DateTime> PassageDateTimes { get; set; }
        
    }
}
