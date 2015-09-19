using UnityEngine;
using System.Collections;
using UnityEngine.UI; // for Text

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using NS_MyNetUtil; // for MyNetUtil.getMyIPAddress()
using NS_MyResponseDictionaryUtil; // for register command-response dictionary

/* 
 * v0.5 2015/09/19
 *   
 * ----------- UdpEchoServer ==> udpEmu ------------
 * v0.4 2015/08/30
 *   - separate IP address get method to MyNetUtil.cs
 * v0.3 2015/08/30
 *   - show version info
 *   - correct .gitignore file
 * v0.2 2015/08/29
 *   - fix for negative value for delay_msec
 *   - fix for string to int
 *   - fix for android (splash freeze)
 * v0.1 2015/08/29
 *   following features have been implemented.
 *   - delay before echo back
 *   - echo back
 */

public class udpEmu : MonoBehaviour {
	Thread rcvThr;
	UdpClient client;
	public int port = 6000;

	public const string kAppName = "udpEmu";
	public const string kVersion = "v0.5";

	public string lastRcvd;

	public Text myipText; // to show my IP address(port)
	public Text recvdText;
	public InputField delayIF; // to input delay before echo back
	public Text versionText;

	private bool stopThr = false;
	private int delay_msec = 0;

	private enum udpMode {
		ECHO = 0,
		REGISTER, // register responsed dictionary
	}
	private udpMode myUdpMode = udpMode.ECHO;

	int getDelay() { 
		string txt = delayIF.text;
		if (txt.Length == 0) {
			return 0;
		}
		// instead of int.Parse(), Convert.XXX() will return 0 if null
		int res = Convert.ToInt16(delayIF.text);
		if (res < 0) {
			return 0;
		}
		return res;
	}
	
	void Start () {
		versionText.text = kAppName + " " + kVersion;
		myipText.text = MyNetUtil.getMyIPAddress() + " (" + port.ToString () + ")";
		startTread ();
	}

	string getTextMessage(string rcvd)
	{
		if (rcvd.Length == 0) {
			return "";
		}
		string msg = 
			"rx: " + rcvd + System.Environment.NewLine
			+ "tx: " + rcvd;
		return msg;
	}

	void Update() {
		recvdText.text = getTextMessage (lastRcvd);
		delay_msec = getDelay ();
	}
	
	void startTread() {
		Debug.Log ("init udpEmu thread");
		rcvThr = new Thread( new ThreadStart(FuncRcvData));
		rcvThr.Start ();
	}
	
	void OnApplicationQuit() {
		stopThr = true;
		rcvThr.Abort ();
	}

	const string kVer0p1Hash = "dfae271";
	bool isRegisterStartCommand(string rcvd) {
//		return rcvd.Contains ("register,start," + kVer0p1Hash);
		return rcvd.Contains ("SOT," + kVer0p1Hash);
	}
	bool isRegisterExitCommand(string rcvd) {
//		return rcvd.Contains ("register,exit," + kVer0p1Hash);
		return rcvd.Contains ("EOT," + kVer0p1Hash);
	}

	string extractCsvRow_returnWithoutCRLF(string src, int idx)
	{
		// TODO: refactor
		string[] splitted = src.Split(new string[] { System.Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
		string res = "";
		foreach(string each in splitted) {
			string [] elements = each.Split(',');
			res = res + elements[idx]; //  + System.Environment.NewLine;
		}
		return res;
	}

	private string strCommand, strResponse; // to register command-response

	string getSetString(string src, string str2nd) {
		int idx = src.IndexOf (str2nd); // ??? > char or string parameter
		idx += (str2nd + ",").Length; // skip "tx," "rx,"
		return src.Substring (idx);
	}

	bool registerResponseDictionary(string rcvd)
	{
		// 1. check 2nd column (tx | rx)
		string str2nd = extractCsvRow_returnWithoutCRLF (rcvd, /* idx=*/1);
		if (str2nd.Equals ("tx") == false && str2nd.Equals ("rx") == false) {
			Debug.Log("not for registration");
			return false;
		}

		string setstr = getSetString (rcvd, str2nd); // rcvd.Substring (idx);

//		Debug.Log ("register:" + setstr);

		if (str2nd.Equals ("tx")) {
			strCommand = setstr;
		}
		if (str2nd.Equals ("rx")) {
			strResponse = setstr;
			if (strCommand.Length > 0) {
				MyResponseDictionaryUtil.Add(strCommand, strResponse);

				MyResponseDictionaryUtil.Debug_DisplayAllElements(); // TODO: remove // for debug

				return true;
			}
		}

		return false;
	}

	void responseBasedOnUdpMode(ref byte[] data, ref UdpClient client, ref IPEndPoint anyIP) {
		string rcvd = Encoding.ASCII.GetString(data);
		lastRcvd = rcvd;
		string sendmsg; 

		if (lastRcvd.Length == 0) {
			return;
		}

		if (myUdpMode.Equals (udpMode.ECHO)) {
			if (isRegisterStartCommand(lastRcvd)) {
				myUdpMode = udpMode.REGISTER;
				sendmsg = "start register mode" + System.Environment.NewLine;
				data = System.Text.Encoding.ASCII.GetBytes(sendmsg);
				client.Send (data, data.Length, anyIP); // echo
				return;
			}
			// echo
			Thread.Sleep (delay_msec);
			client.Send (data, data.Length, anyIP);
		} else if (myUdpMode.Equals (udpMode.REGISTER)) {
			if (isRegisterExitCommand(lastRcvd)) {
				myUdpMode = udpMode.ECHO;
				sendmsg = "exit register mode" + System.Environment.NewLine;
				data = System.Text.Encoding.ASCII.GetBytes(sendmsg);
				client.Send (data, data.Length, anyIP); // echo
				return;
			}
			registerResponseDictionary(rcvd);

//			data = System.Text.Encoding.ASCII.GetBytes(sendmsg);
//			client.Send (data, data.Length, anyIP); // echo
		}
	}

	private void FuncRcvData()
	{
		MyResponseDictionaryUtil.Init ();

		client = new UdpClient (port);
		client.Client.ReceiveTimeout = 300; // msec
		client.Client.Blocking = false;
		while (stopThr == false) {
			try {
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

				responseBasedOnUdpMode(ref data, ref client, ref anyIP);
			}
			catch (Exception err)
			{
				//              print(err.ToString());
			}

			// without this sleep, on adnroid, the app will not start (freeze at Unity splash)
			Thread.Sleep(20); // 200
		}
		client.Close ();
	}
}


