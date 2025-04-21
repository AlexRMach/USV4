using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace ush4.Services.Serialization
{
    [Obsolete]
    public static class SaveAndLoadBySerialization
    {
        public static Object LoadData(String filename, Type object_type)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(object_type);
                return ser.Deserialize(new StreamReader(filename));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error appeared while loading data from file: {0}. {1}", filename, ex.Message));
            }
        }

        public static void SaveData(String filename, Object data)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(data.GetType());
                TextWriter writer = new StreamWriter(filename);
                ser.Serialize(writer, data);
                writer.Close();
                File.SetAttributes(filename, FileAttributes.Normal);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error appeared while saving data to file: {0}. {1}", filename, ex.Message));
            }

        }

        
        public static T CopyObject<T>(this object objSource)
        {
            using (MemoryStream stream = new MemoryStream())

            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objSource);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);

            }

        }
    }


    
}
