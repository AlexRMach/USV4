using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Models
{
    public class Borders
    {
        public enum enSetPointType
        {
            Frequency,
            Displacement,
            Velocity,
            Acceleration
        }

        public Dictionary<enSetPointType, Tuple<Double, Double>> BordersDict { get; set; }
    }
}
