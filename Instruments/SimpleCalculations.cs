using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Instruments
{
    public static class SimpleCalculations
    {
        const Double _2PI = 2.0 * Math.PI;

        public static Double DerivationCoeffOfSinusoidal(Double Hz)
        {

            return _2PI * Hz;
        }

        public static Double DerivationCoeffOfSinusoidal(Double Hz, int derivation_pow)
        {
            if (Hz == 0)
                return 0;

            Double to_ret = Math.Pow(DerivationCoeffOfSinusoidal(Hz), derivation_pow);
            return to_ret;

        }

        public static Double MeasurementDuration_seconds(Double frequency, int number_of_periods, int max_duration_minutes)
        {
            Double duration = number_of_periods / frequency;
            Double max_duration_seconds = max_duration_minutes * 60.0;
            if (duration > max_duration_seconds)
            {
                int max_num_of_periods = (int)Math.Floor(max_duration_seconds * frequency);
                duration = max_num_of_periods / frequency;
            }
            return duration;
        }
    }
}
