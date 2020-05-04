using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public static class IPMethods
    {
        public static string GetLocalPCName()
        {
            return Environment.MachineName;
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
                
            }
            return "Could not find";
        }
        public static string GetLocalMac()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                    return AddressBytesToString(nic.GetPhysicalAddress().GetAddressBytes());
            }

            return "Could not find";
        }
        private static string AddressBytesToString(byte[] addressBytes)
        {
            return string.Join(":", (from b in addressBytes
                                     select b.ToString("X2")).ToArray());
        }
        public static string CreateRegistryId()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user

            string unique_pc_id = CreateClientId();

            key.SetValue("pc_id", unique_pc_id);
            key.Close();

            return unique_pc_id;
        }
        private static string CreateClientId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[10];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        public static string GetRegistryId()
        {
            // opening the subkey
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user
            string unique_pc_id = "not set";

            if (keyread != null)
            {
                if(keyread.GetValue("pc_id") != null)
                unique_pc_id = keyread.GetValue("pc_id").ToString();
                keyread.Close();
            }
            return unique_pc_id;
        }
    }
}
