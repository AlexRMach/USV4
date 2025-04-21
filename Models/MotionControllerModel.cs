using ush4.Models.ELMO;
using ElmoMotionControlComponents.Drive.EASComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Models
{
    public class MotionControllerModel : DriveModel
    {
        public override void Connect()
        {
            base.Connect();
            Commands = new ElmoCommandHandler(driveCommunication);
        }

        public String UserProgramName { get; set; }
        public Double CountsPerUnit { get; set; }
        public int CenterOfOscillations { get; set; }


        public MotionControllerModel() : base()
        {
            UserProgramName = "homing"; // oscillate
            CountsPerUnit = 100e6;
        }
    }
}
