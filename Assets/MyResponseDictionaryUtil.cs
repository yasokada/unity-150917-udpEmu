﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for Dictionary / List

using System; // for DateTime

// TODO: AllClear()
// TODO: Remove()
// TODO: keep maximum response count for each command

/*
 * v0.1 2015/09/19
 *   - Add()
 *   - Init()
 *   - DisplayElementsWithKey()
 *   - FindRandomly()
 *   - Clear()
 *   - fakeRandome()
 */

namespace NS_MyResponseDictionaryUtil
{
	public static class MyResponseDictionaryUtil {
		static Dictionary <string, List<string>> myDic = null;

		// ------ private ------
		private static bool hasKey(string searchKey) {
			List<string> resList;

// KeyNotFoundException when searchKey is not registered
//			resList = myDic[searchKey];

			bool res;
			res = myDic.TryGetValue (searchKey, out resList);
			if (res == false) {
				return false;
			}
			return (resList.Count > 0);
		}

		private static long get_msec() {
			System.DateTime now = System.DateTime.Now;
			System.DateTime nowMsec0 = new DateTime (now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			long TotalMsec = (long)((DateTime.Now - nowMsec0).TotalMilliseconds);
			
			return TotalMsec;
		}

		private static int fakeRandom(int maxval) {
			long msec = get_msec ();
			return (int)(msec % (maxval+1));
		}

		// ------ public ------

		public static void Init() {
			if (myDic == null) {
				myDic = new Dictionary<string, List<string>> ();
			}
		}

		public static void Clear() {
			if (myDic == null) {
				return;
			}
			myDic.Clear ();
		}

		public static void Add(string commandStr, string responseStr) {
			if (myDic == null) {
				return; // fail
			}

			List<string> responseList;
			if (hasKey (commandStr)) { // add
				responseList = myDic[commandStr];
				myDic.Remove (commandStr);
			} else { // new
				responseList = new List<string>();
			}
			responseList.Add(responseStr);
			myDic.Add(commandStr, responseList);
		}

		public static void Debug_DisplayAllElements() {
			foreach(KeyValuePair<string, List<string>> pair in myDic) {
				Debug.Log("cmd:" + pair.Key.ToString());
				foreach(var element in pair.Value) {
					Debug.Log("res:" + element);
				}
			}
		}

		public static void DisplayElementsWithKey(string searchKey) {
			if (hasKey (searchKey) == false) {
				return; // fail
			}
			List<string> resList;
			resList = myDic [searchKey];
			Debug.Log ("cmd:" + searchKey);
			foreach (var element in resList) {
				Debug.Log("res:" + element);
			}
		}

		public static bool FindRandomly(string searchKey, out string resStr) {
			if (hasKey (searchKey) == false) {
				resStr = ""; 
				return false;
			}
			List<string> resList;
			resList = myDic [searchKey];
			int pos;
//			pos = UnityEngine.Random.Range (0, resList.Count); // cannot be used except for Start() / Update()
			pos = fakeRandom (resList.Count - 1); 
			resStr = resList [pos];
			return true;
		}

		public static void Test_main() {
			Init ();
			Add ("hello", "hello, Mike");
			Add ("hello", "hello, Suzuki sann");
			Add ("hello", "hello, Francheska");
			Add ("sleep", "OK. sleeping");
			Add ("time", "2015/09/18 06:20");
			Add ("time", "2015/09/18 06:25");
			Add ("time", "2015/09/18 06:30");

//			Clear ();
			DisplayElementsWithKey ("hello");

			bool isOk;
			string keyStr, resStr;

			keyStr = "time";
			for (int loop=0; loop<10; loop++) {
				isOk = FindRandomly(keyStr, out resStr);
				if (isOk) {
					Debug.Log(keyStr + " >> " + resStr);
				} else {
					Debug.Log(keyStr + " : response not found");
				}
			}
		}

	}
}

