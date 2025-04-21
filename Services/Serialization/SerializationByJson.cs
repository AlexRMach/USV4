using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Services.Serialization
{
    public static class SerializationByJson
    {

        public static void Serialize(String filename, Object data)
        {
            try
            {
                string output = JsonConvert.SerializeObject(data, Formatting.Indented);
                TextWriter writer = new StreamWriter(filename);
                writer.Write(output);
                writer.Close();
                File.SetAttributes(filename, FileAttributes.Normal);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error appeared while serialize data to file: {0}. {1}", filename, ex.Message));
            }

        }


        public static T Deserialize<T>(String filename)
        {
            try
            {
                StreamReader streamReader = new StreamReader(filename);
                T obj = JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
                streamReader.Close();
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error appeared while deserialize data from file: {0}. {1}", filename, ex.Message));
            }
        }
    }
}
