using UnityEngine;
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
	private bool stopThr = false;
	public Text recvdText;
	
	void Start () {
		init ();
	}
	
	void OnGUI() {
		recvdText.text = lastRcvd;

		Rect rectObj=new Rect(40,10,200,100);
		GUI.Box (rectObj, "rcvd: \n" + lastRcvd);
		
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
		client.Client.Blocking = false; //<---------------------------------
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
		}
	}
}


