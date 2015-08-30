using UnityEngine;
using System.Collections;
using System.Net; // for Dns.XXX()

namespace NS_MyNetUtil
{
	public static class MyNetUtil {
		public static string getMyIPAddress()
		{
			string hostname = Dns.GetHostName ();
			IPAddress[] adrList = Dns.GetHostAddresses (hostname);
			
			foreach (IPAddress adr in adrList) {
				string ipadr = adr.ToString();
				if (ipadr.Contains("192.")) {
					return adr.ToString();
				}
				if (ipadr.Contains("172.20")) {
					return adr.ToString();
				}
			}
			return "IPadr: not found";
		}
	}

}
