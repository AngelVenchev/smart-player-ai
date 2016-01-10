using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.Utility
{
    static class IpV4Provider
    {
        public static string GetLocalIPAddress()
        {
            var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            var ip = addressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
            return ip != null ? ip.ToString() : "http://localhost";
        }
    }
}
