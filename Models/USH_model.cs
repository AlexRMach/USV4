using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ush4.Models
{
    public class USH_model
    {
        public String MainWindowName { get; set; }
        public Borders Borders { get; set; }
        public String PathToListFolder { get; set; }
        public String PathToDataFolder { get; set; }
        public String DisplacementUnits { get; set; }
        public Double MaxFrequencyRelativeSigma { get; set; }

        public int NumberOfPeriods { get; set; }

        public int MaxMeasureTime_minutes { get; set; }

        public Boolean AutoMeasurements { get; set; }

        public int AutoModeWaitTime_seconds { get; set; }
        public USH_model()
        {
            MainWindowName = "Horizontal Seismic Device";
            DisplacementUnits = "m";
            MaxFrequencyRelativeSigma = 5e-3;
            NumberOfPeriods = 10;
            MaxMeasureTime_minutes = 60;
            AutoModeWaitTime_seconds = 20;
            AutoMeasurements = true;
            
            Borders = new Models.Borders();
            Borders.BordersDict = new Dictionary<Models.Borders.enSetPointType, Tuple<double, double>>()
            {
                {Models.Borders.enSetPointType.Frequency, new Tuple<double, double>(0.001, 30 ) },
                {Models.Borders.enSetPointType.Displacement, new Tuple<double, double>(1e-6, 1e-2 ) },
                {Models.Borders.enSetPointType.Velocity, new Tuple<double, double>(1e-7, 3.2e-1 ) },
                {Models.Borders.enSetPointType.Acceleration, new Tuple<double, double>(3e-9, 1e+2 ) },
            };
        }
    }
}
