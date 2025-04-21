using ush4.Models.ELMO;
using ElmoMotionControlComponents.Drive.EASComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Models
{
    public class ElmoCommandHandler : ElmoHandler
    {
        public ElmoCommandHandler(IDriveCommunication adapter) : base(adapter)
        {

        }

        public enum EnUserInt
        {
            Amplitude = 1,
            Center = 2,
            UpdateTrigger = 3,
            Program_err = 4,
            PauseOscillations = 5,
            IsReadyForMeasure = 6,
            IsCenter = 7,
            WorkCmd = 8,
            Answer = 9,
            SmState = 10,
            StartOscillations = 11,
            IsSteady = 12,
            PosErr = 13,
            GetVal = 14,
            JpVal = 15,
            ValReady = 16,
        }

        public enum enUserFloat
        {
            Frequency = 1,
            kp2FloatIdx = 2,
            ki2FloatIdx = 3,
            tphFloatIdx = 4,
            uf_6 = 6,
            uf_7 = 7,
            uf_8 = 8,
            uf_9 = 9,
            uf_10 = 10,
            uf_11 = 11,
            uf_12 = 12,
            uf_13 = 13,
            uf_14 = 14,
            uf_15 = 15,
            uf_16 = 16,
            uf_17 = 17,
            uf_18 = 18,
        }

        public void StopOscillations()
        {
            String stop_oscil = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.PauseOscillations, (int)ElmoCommandsEnum.enOnOff.On);
            String rep;

            ResendInErrorCase(stop_oscil, out rep);
        }

        public void StartOscillations()
        {
            String resume_oscil = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.StartOscillations, (int)ElmoCommandsEnum.enOnOff.On);
            String rep;

            ResendInErrorCase(resume_oscil, out rep);
        }

        public void PauseOscillations()
        {
            String stop_oscil = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.PauseOscillations, (int)ElmoCommandsEnum.enOnOff.On);
            String rep;

            ResendInErrorCase(stop_oscil, out rep);
        }

        public void ResumeOscillations()
        {
            String resume_oscil = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.PauseOscillations, (int)ElmoCommandsEnum.enOnOff.Off);
            String rep;

            ResendInErrorCase(resume_oscil, out rep);
        }

        public void SetFrequency(Double frecuency_hz)
        {
            String set_frequency = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.Frequency, frecuency_hz);
            String rep;

            ResendInErrorCase(set_frequency, out rep);
        }

        public void SetOscillationAmplitude(int amplitude_cnt)
        {
            String set_amplitude = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.Amplitude, amplitude_cnt);
            String rep;

            ResendInErrorCase(set_amplitude, out rep);
        }

        public void SetCenterofOscillations(int center_cnt)
        {
            String set_center = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.Center, center_cnt);
            String rep;

            ResendInErrorCase(set_center, out rep);
        }

        public void SetUpdateTrigger()
        {
            String set_updatetrigger = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.UpdateTrigger, (int)ElmoCommandsEnum.enOnOff.On);
            String rep;

            ResendInErrorCase(set_updatetrigger, out rep);
        }

        public void ResetUpdateTrigger()
        {
            String set_updatetrigger = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.UpdateTrigger, (int)ElmoCommandsEnum.enOnOff.On);
            String rep;

            ResendInErrorCase(set_updatetrigger, out rep);
        }

        public void ResetIsReadyForMeasurements()
        {
            String is_ready_measure = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.IsReadyForMeasure, (int)ElmoCommandsEnum.enOnOff.Off);
            String rep;

            ResendInErrorCase(is_ready_measure, out rep);
        }

        public int GetUserProgramError()
        {
            String get_us_error = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.Program_err);
            String rep;

            ResendInErrorCase(get_us_error, out rep);
            return Convert.ToInt32(rep);
        }


        public Boolean GetIsUserProgramPaused()
        {
            String up_paused = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.PauseOscillations);
            String rep;

            ResendInErrorCase(up_paused, out rep);
            return (Convert.ToInt32(rep) != 0);
        }

        public int GetIsReadyForMeasurements()
        {
            String is_ready_measure = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.IsReadyForMeasure);
            String rep;

            ResendInErrorCase(is_ready_measure, out rep);
            return Convert.ToInt32(rep);
        }


        public Double GetCurrentFrequency()
        {
            String get_freq = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.Frequency);
            String rep;

            ResendInErrorCase(get_freq, out rep);
            Double val = Convert.ToDouble(rep, System.Globalization.CultureInfo.InvariantCulture);
            return val;
        }


        public int GetCurrentAmplitude()
        {
            String get_ampl = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.Amplitude);
            String rep;

            ResendInErrorCase(get_ampl, out rep);
            return Convert.ToInt32(rep);        
        }

        public Boolean GetIsCenter()
        {
            String is_center = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.IsCenter);
            String rep;

            ResendInErrorCase(is_center, out rep);
            return (Convert.ToInt32(rep) != 0);
        }

        public void ResetIsCenter()
        {
            String is_center = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.IsCenter, (int)ElmoCommandsEnum.enOnOff.Off);
            String rep;

            ResendInErrorCase(is_center, out rep);
        }

        public int GetAnswer()
        {
            String get_ampl = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.Answer);
            String rep;

            ResendInErrorCase(get_ampl, out rep);
            return Convert.ToInt32(rep);        
        }

        public int GetSmState()
        {
            String get_ampl = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.SmState);
            String rep;

            ResendInErrorCase(get_ampl, out rep);
            return Convert.ToInt32(rep);
        }

        public Boolean GetIsSteady()
        {
            String is_center = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.IsSteady);
            String rep;

            ResendInErrorCase(is_center, out rep);
            return (Convert.ToInt32(rep) != 0);
        }

        public Boolean GetIsSinValReady()
        {
            String is_valReady = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.ValReady);
            String rep;

            ResendInErrorCase(is_valReady, out rep);
            return (Convert.ToInt32(rep) != 0);
        }

        public void SetSinValIndex(int index)
        {
            String set_sin_val_index = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.GetVal, index);
            String rep;

            ResendInErrorCase(set_sin_val_index, out rep);
        }

        public int GetSinValIndex()
        {
            String get_sin_val = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.GetVal);
            String rep;

            ResendInErrorCase(get_sin_val, out rep);

            return Convert.ToInt32(rep);
        }

        public int GetSinJpVal()
        {
            String get_sin_jp_val = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserInt,
                (int)EnUserInt.JpVal);
            String rep;

            ResendInErrorCase(get_sin_jp_val, out rep);

            return Convert.ToInt32(rep);                            
        }

        public Double GetSinTphVal()
        {
            String get_sin_tph_val = ElmoCommandsEnum.ElmoArrayCommands.GetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.tphFloatIdx);
            String rep;

            ResendInErrorCase(get_sin_tph_val, out rep);

            Double val = Convert.ToDouble(rep, System.Globalization.CultureInfo.InvariantCulture);
            return val;
        }

        public void SetUF6(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_6, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF7(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_7, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF8(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_8, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF9(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_9, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF10(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_10, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF11(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_11, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF12(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_12, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF13(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_13, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF14(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_14, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF15(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_15, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF16(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_16, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF17(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_17, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }

        public void SetUF18(Double uf_val)
        {
            String set_uf = ElmoCommandsEnum.ElmoArrayCommands.SetDataRequest(ElmoCommandsEnum.ElmoArrayCommands.UserFloat,
                (int)enUserFloat.uf_18, uf_val);
            String rep;

            ResendInErrorCase(set_uf, out rep);
        }
    }
}
