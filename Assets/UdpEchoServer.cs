﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI; // for Text

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UdpEchoServer : MonoBehaviour {
	Thread rcvThr;
	UdpClient client;
	public int port = 6000;
	
	public string lastRcvd;

	public Text myipText; // to show my IP address
	public Text portText; // to show UDP port
	public Text recvdText;

	private bool stopThr = false;

	string getMyIPAddress()
	{
		string hostname = Dns.GetHostName ();
		IPAddress[] adrList = Dns.GetHostAddresses (hostname);

		foreach (IPAddress adr in adrList) {
			string ipadr = adr.ToString();
			if (ipadr.Contains("192.")) {
				return adr.ToString();
			}
			if (ipadr.Contains("172.") && ipadr.Contains(".20.")) {
				return adr.ToString();
			}
		}
		return "IPadr: not found";
	}

	void Start () {
		myipText.text = getMyIPAddress ();
		portText.text = port.ToString ();
		init ();
	}

	void Update() {
		recvdText.text = lastRcvd;
	}

	void OnGUI() {
		if (GUI.Button (new Rect (10, 250, 100, 40), "Quit")) {
			Application.Quit ();
		} 
	}
	
	void init() {
		Debug.Log ("init");
		rcvThr = new Thread( new ThreadStart(FuncRcvData));
		rcvThr.Start ();
	}
	
	void OnApplicationQuit() {
		stopThr = true;
		rcvThr.Abort ();
	}
	
	private void FuncRcvData()
	{
		client = new UdpClient (port);
		client.Client.ReceiveTimeout = 300; // msec
		client.Client.Blocking = false;
		while (stopThr == false) {
			try {
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.ASCII.GetString(data);
				lastRcvd = text;

				if (lastRcvd.Length > 0) {
					client.Send(data, data.Length, anyIP); // echo
				}
			}
			catch (Exception err)
			{
				//              print(err.ToString());
			}

			// without this sleep, on adnroid, the app will not start (freeze at Unity splash)
			Thread.Sleep(20); // 200
		}
	}
}


