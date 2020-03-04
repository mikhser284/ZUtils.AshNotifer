using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ZUtils.AshNotifer.Extensions
{
    public class LanInfo
    {
        public IPAddress MyIp { get; private set; }
        
        public IPAddress SubnetMask { get; private set; }

        public IPAddress NetworkAddress { get; private set; }

        public IPAddress BroadcastAddress { get; private set; }

        public IPAddress MinHostAddress { get; private set; }

        public IPAddress MaxHostAddress { get; private set; }

        public Int32 CountOfAddress { get; private set; }

        private LanInfo() { }

        public static LanInfo GetLanInfo()
        {
            LanInfo lanInfo = new LanInfo();
            lanInfo.MyIp = Ext_IPAddress.GetLocalIpAddress();
            lanInfo.SubnetMask = lanInfo.MyIp.GetSubnetMask();
            lanInfo.NetworkAddress = lanInfo.MyIp.GetNetworkAddress(lanInfo.SubnetMask);
            lanInfo.BroadcastAddress = lanInfo.MyIp.GetBroadcastAddress(lanInfo.SubnetMask);
            lanInfo.MinHostAddress = lanInfo.NetworkAddress.Increment();
            lanInfo.MaxHostAddress = lanInfo.BroadcastAddress.Decrement();
            lanInfo.CountOfAddress = Ext_IPAddress.CountOfAddress(lanInfo.MaxHostAddress, lanInfo.MinHostAddress);
            return lanInfo;
        }

        public override String ToString()
        {
            return ""
                + $"My IP:             {MyIp}\n"
                + $"Subnet mask:       {SubnetMask}\n"
                + $"Network address:   {NetworkAddress}\n"
                + $"Broadcast address: {BroadcastAddress}\n"
                + $"Min host address:  {MinHostAddress}\n"
                + $"Max host address:  {MaxHostAddress}\n"
                + $"Count of addreses: {CountOfAddress}";
        }
    }

    public static class Ext_IPAddress
    {
        public static IPAddress Increment(this IPAddress address, uint increment = 1)
        {
            byte[] addressBytes = address.GetAddressBytes().Reverse().ToArray();
            uint ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            var nextAddress = BitConverter.GetBytes(ipAsUint + increment);
            return new IPAddress(nextAddress.Reverse().ToArray());
        }

        public static IPAddress Decrement(this IPAddress address, uint decrement = 1)
        {
            byte[] addressBytes = address.GetAddressBytes().Reverse().ToArray();
            uint ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            var nextAddress = BitConverter.GetBytes(ipAsUint - decrement);
            return new IPAddress(nextAddress.Reverse().ToArray());
        }

        public static Int32 CountOfAddress(IPAddress firstAddress, IPAddress secondAddrress)
        {
            Int32 firstAddressAsInt = BitConverter.ToInt32(firstAddress.GetAddressBytes().Reverse().ToArray(), 0);
            Int32 secondAddressAsInt = BitConverter.ToInt32(secondAddrress.GetAddressBytes().Reverse().ToArray(), 0);
            Int32 countOfAddresses = 1 + Math.Max(firstAddressAsInt, secondAddressAsInt) - Math.Min(firstAddressAsInt, secondAddressAsInt);
            return countOfAddresses;
        }

        public static IPAddress GetLocalIpAddress()
        {
            var res = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(var ip in host.AddressList) if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) return ip;
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static IPAddress GetSubnetMask(this IPAddress address)
        {
            foreach(NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach(UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if(unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if(address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if(ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for(int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if(ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for(int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            IPAddress network1 = address.GetNetworkAddress(subnetMask);
            IPAddress network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }
    }
}
