using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HDF5DotNet;

namespace ush4.Instruments
{
    public class HDF5
    {

        const string FREQUENCY_FORMAT = "Frequency, {0}";
        const String DISPLACEMENT_FORMAT = "Displacement, {0}";

        public static H5FileId OpenFile(string filename)
        {
            if (!File.Exists(filename))
            {
                var file = H5F.create(filename, H5F.CreateMode.ACC_TRUNC);
                return file;
            }
            else
                return H5F.open(filename, H5F.OpenMode.ACC_RDWR);
        }


        public static H5FileId OpenFile(string filename, String device_name)
        {
            if (!File.Exists(filename))
            {
                var file = H5F.create(filename, H5F.CreateMode.ACC_TRUNC);

                var attr = CreateStringAttribute(file, "Device");
                WriteStringAttribute(attr, device_name);
                Close(attr);
                return file;
            }
            else
                return H5F.open(filename, H5F.OpenMode.ACC_RDWR);
        }

        public static H5GroupId CreateGroup(H5LocId h5FileId, String groupname)
        {
            H5GroupId h5GroupId = H5G.create(h5FileId, groupname);
            return h5GroupId;
        }

        public static H5DataTypeId EnumSpaceType<T>()
        {
            var t = H5T.enumCreate(H5T.H5Type.NATIVE_INT);
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                Console.WriteLine(((T)item).ToString());
                int val = (int)item;
                H5T.enumInsert<int>(t, ((T)item).ToString(), ref val);

            }
            return t;

        }

        public static H5DataTypeId CreateSummaryType()
        {
            H5DataTypeId enumId = EnumSpaceType<Models.Borders.enSetPointType>();

            H5DataTypeId str_type = H5T.copy(H5T.H5Type.C_S1);
            H5T.setVariableSize(str_type);

            H5DataTypeId h5DataTypeId = H5T.create(/*(H5T.CreateClass)H5T.H5TClass.COMPOUND*/ H5T.CreateClass.COMPOUND, HDF5_Structs.SizeOfStruct(typeof(HDF5_Structs.SummaryValue)));
            H5T.insert(h5DataTypeId, "Type of value", HDF5_Structs.FieldOffset(typeof(HDF5_Structs.SummaryValue), "EnSetPointType"), enumId);
            H5T.insert(h5DataTypeId, "Set value", HDF5_Structs.FieldOffset(typeof(HDF5_Structs.SummaryValue), "SetValue"), H5T.H5Type.NATIVE_DOUBLE);
            H5T.insert(h5DataTypeId, "Measured Value", HDF5_Structs.FieldOffset(typeof(HDF5_Structs.SummaryValue), "MeasuredValue"), H5T.H5Type.NATIVE_DOUBLE);
            H5T.insert(h5DataTypeId, "Units", HDF5_Structs.FieldOffset(typeof(HDF5_Structs.SummaryValue), "Units"), str_type);
            return h5DataTypeId;
        }


        public static H5DataTypeId CreateRawType(String disp_units)
        {

            H5DataTypeId h5DataTypeId = H5T.create(H5T.CreateClass.COMPOUND, HDF5_Structs.SizeOfStruct(typeof(HDF5_Structs.Point)));

            H5T.insert(h5DataTypeId, "Time, s", HDF5_Structs.FieldOffset(typeof(HDF5_Structs.Point), "X"), H5T.H5Type.NATIVE_DOUBLE);
            H5T.insert(h5DataTypeId, String.Format(DISPLACEMENT_FORMAT, disp_units)
                , HDF5_Structs.FieldOffset(typeof(HDF5_Structs.Point), "Y"), H5T.H5Type.NATIVE_DOUBLE);
            return h5DataTypeId;
        }


        public static H5DataTypeId CreateFFTType(String disp_units)
        {

            H5DataTypeId h5DataTypeId = H5T.create(H5T.CreateClass.COMPOUND, HDF5_Structs.SizeOfStruct(typeof(HDF5_Structs.Point)));

            H5T.insert(h5DataTypeId, "Frequency, Hz", HDF5_Structs.FieldOffset(typeof(HDF5_Structs.Point), "X"), H5T.H5Type.NATIVE_DOUBLE);
            H5T.insert(h5DataTypeId, String.Format(DISPLACEMENT_FORMAT, disp_units),
                HDF5_Structs.FieldOffset(typeof(HDF5_Structs.Point), "Y"), H5T.H5Type.NATIVE_DOUBLE);
            return h5DataTypeId;
        }

        public static H5DataSpaceId CreateSpace(int l)
        {
            long[] dims = new long[1] { l };
            return H5S.create_simple(1, dims);
        }


        public static H5DataSetId CreateDataset(H5FileOrGroupId h5FileOrGroupId, String dataset_name, H5DataTypeId h5DataTypeId, H5DataSpaceId h5DataSpaceId)
        {
            return H5D.create(h5FileOrGroupId, dataset_name, h5DataTypeId, h5DataSpaceId);
        }


        //public static H5AttributeId CreateAttribute(H5ObjectWithAttributes h5ObjectWithAttributes, string attr_name, 
        //    H5DataTypeId h5DataTypeId, H5DataSpaceId h5DataSpaceId)
        //{
        //    return H5A.create(h5ObjectWithAttributes,  attr_name, h5DataTypeId, h5DataSpaceId) ;
        //}

        //public static H5DataTypeId CreateAttributeType()
        //{
        //    H5DataTypeId str_type = H5T.copy(H5T.H5Type.C_S1);// new H5DataTypeId(H5T.H5Type.C_S1));
        //    H5T.setVariableSize(str_type);
        //    return str_type;
        //}

        public static H5AttributeId CreateStringAttribute(H5ObjectWithAttributes h5ObjectWithAttributes, string attr_name)
        {
            var typeId = H5T.create(H5T.CreateClass.STRING, 256);
            var spaceId = H5S.create(H5S.H5SClass.SCALAR);
            return H5A.create(h5ObjectWithAttributes, attr_name, typeId, spaceId);
        }

        public static void WriteStringAttribute(H5AttributeId h5AttributeId, String attribute_str)
        {
            var typeId = H5T.create(H5T.CreateClass.STRING, 256);
            byte[] ascii_str = ASCIIEncoding.ASCII.GetBytes(attribute_str);
            H5A.write<byte>(h5AttributeId, typeId, new H5Array<byte>(ascii_str));
        }


        //public static H5DataSpaceId CreateAttributeSpaceId()
        //{

        //}

        public static void WriteDataset<T>(H5DataSetId h5DataSetId, H5DataTypeId typeId, T[] data)
        {
            H5D.write<T>(h5DataSetId, typeId, new H5Array<T>(data));
        }

        public static void Close(Object obj)
        {
            Type type = obj.GetType();
            if (typeof(H5FileId).Equals(type))
                H5F.close((H5FileId)obj);
            else if (typeof(H5GroupId).Equals(type))
                H5G.close((H5GroupId)obj);
            else if (typeof(H5DataSetId).Equals(type))
                H5D.close((H5DataSetId)obj);
            else if (typeof(H5DataSpaceId).Equals(type))
                H5S.close((H5DataSpaceId)obj);
            else if (typeof(H5AttributeId).Equals(type))
                H5A.close((H5AttributeId)obj);

        }

        private static Double GetValueFromSummary(HDF5_Structs.SummaryValue[] summaries, Models.Borders.enSetPointType enSetPointType)
        {
            foreach (var item in summaries)
            {
                if (item.EnSetPointType == enSetPointType)
                    return item.SetValue;
            }
            throw new Exception("The required type was not found!");
        }

        private static unsafe String GetUnitsFromSummary(HDF5_Structs.SummaryValue[] summaries, Models.Borders.enSetPointType enSetPointType)
        {
            foreach (var item in summaries)
            {
                if (item.EnSetPointType == enSetPointType)
                    return Marshal.PtrToStringAnsi((IntPtr)item.Units);
            }
            throw new Exception("The required type was not found!");
        }


        public static void SaveResultToHDF5(String file_name, String groupname,
            HDF5_Structs.SummaryValue[] summaries, HDF5_Structs.Point[] Raw, HDF5_Structs.Point[] FFT, String device_name)
        {
            Double frequency = GetValueFromSummary(summaries, Models.Borders.enSetPointType.Frequency);

            String units = GetUnitsFromSummary(summaries, Models.Borders.enSetPointType.Displacement);
            //String groupname = String.Format("{0}   Frequency: {1} Hz", DateTime.Now.ToLongTimeString(), frequency);

            var file = OpenFile(file_name, device_name);
            var group = CreateGroup(file, groupname);

            //String test = "test atttribute";


            //var attribute = CreateStringAttribute(file, "Device");
            //WriteStringAttribute(attribute, test);

            var fft_type = CreateFFTType(units);
            var raw_type = CreateRawType(units);
            var summary_type = CreateSummaryType();

            var fft_space = CreateSpace(FFT.Length);
            var raw_space = CreateSpace(Raw.Length);
            var summary_space = CreateSpace(summaries.Length);

            var fft_dataset = CreateDataset(group, "FFT", fft_type, fft_space);
            var raw_dataset = CreateDataset(group, "Raw displacement data", raw_type, raw_space);
            var summary_dataset = CreateDataset(group, "Summary", summary_type, summary_space);

            WriteDataset<HDF5_Structs.Point>(fft_dataset, fft_type, FFT);
            WriteDataset<HDF5_Structs.Point>(raw_dataset, raw_type, Raw);
            WriteDataset<HDF5_Structs.SummaryValue>(summary_dataset, summary_type, summaries);

            Close(fft_space);
            Close(raw_space);
            Close(summary_space);

            Close(fft_dataset);
            Close(raw_dataset);
            Close(summary_dataset);

            Close(group);
            Close(file);

        }

    }
}
