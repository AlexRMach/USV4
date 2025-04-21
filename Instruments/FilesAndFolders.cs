using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Instruments
{
    public static class FilesAndFolders
    {
        private const String ALT_FOLDER_NAME = "VNIIM";
        public static String MY_DOCUMENTS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static void CreateFolder(String path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }


        public static String CheckPathToFolderAndReturnSuitable(String path, String program_name, String folder_name_error_case)
        {
            try
            {
                CreateFolder(path);
                return path;
            }
            catch (Exception)
            {
                return AltFolderPath(program_name, folder_name_error_case);

            }
        }

        private static String AltFolderPath(String program_name, string folder_name_error_case)
        {
            try
            {

                String exc_path = Path.Combine(MY_DOCUMENTS_PATH, program_name, folder_name_error_case);
                CreateFolder(exc_path);
                return exc_path;
            }
            catch (Exception ex)
            {
                String alt = Path.Combine(MY_DOCUMENTS_PATH, ALT_FOLDER_NAME, folder_name_error_case);

                CreateFolder(alt);

                return alt;
            }
        }

        public static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(path);
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true,
                                        typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                    continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    writeAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    writeDeny = true;
            }

            return writeAllow && !writeDeny;
        }


    }
}
