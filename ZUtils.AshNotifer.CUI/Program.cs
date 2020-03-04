using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ZUtils.AshNotifer.Extensions;

namespace ZUtils.AshNotifer.CUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(LanInfo.GetLanInfo());
        }
    }    
}
