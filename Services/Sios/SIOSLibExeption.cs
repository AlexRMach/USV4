using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Services.Sios
{
    public class SIOSLibExeption : Exception
    {
        public SIOSLibExeption() : base()
        {

        }

        public SIOSLibExeption(String exeptionMessage) : base(exeptionMessage)
        {

        }


        public static int RetCodeChecker(Func<int> func)
        {
            int retCode = func();
            String err_string = APIWrapper.GetErrorString(retCode);
            if (retCode < 0)
                throw new SIOSLibExeption(String.Format("Method {0} returned status {1} ({2}).",
                    func.Method.Name, Enum.GetName(typeof(SIOSEnums.ErrorsCodes), retCode), err_string));
            else
                return retCode;
        }

    }
}
