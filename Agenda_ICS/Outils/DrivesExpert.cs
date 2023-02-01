using System.Management;

namespace NOutils
{
    public class DrivesExpert
    {
        public static string GetHddDatas(string driveLetter)
        {
            try
            {
                using (var partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + driveLetter +
                                                    "'} WHERE ResultClass=Win32_DiskPartition"))
                {
                    foreach (var partition in partitions.Get())
                    {
                        using (var drives = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
                                                                partition["DeviceID"] +
                                                                "'} WHERE ResultClass=Win32_DiskDrive"))
                        {
                            foreach (var drive in drives.Get())
                            {
                                return (string)drive["SerialNumber"];
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return "<unknown>";
        }

        public static string ShaCodeOfDrive(string pathToDatasFile)
        {
            var driveLetter = AdministrationDatasExpert.ReadLine(pathToDatasFile, 0).Substring(0, 1);

            var idOfDrive = GetHddDatas(driveLetter);
            var sha = ShaExpert.ComputeSHA256(idOfDrive);
            return sha;
        }
    }
}
