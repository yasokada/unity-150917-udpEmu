using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for Dictionary / List

namespace NS_MyCommandEmuUtil
{
	public static class MyCommandEmuUtil {
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
		// ------ public ------

		public static void Init() {
			if (myDic == null) {
				myDic = new Dictionary<string, List<string>> ();
			}
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

		public static void DisplayAllElementWithKey(string searchKey) {
			foreach(KeyValuePair<string, List<string>> pair in myDic) {
				if (pair.Key != searchKey) {
					continue;
				}
				Debug.Log("cmd:" + pair.Key.ToString());
				foreach(var element in pair.Value) {
					Debug.Log("res:" + element);
				}
			}
		}

		public static bool FindRandomly(string searchKey, out string resStr) {
			if (hasKey (searchKey) == false) {
				resStr = ""; 
				return false;
			}
			List<string> resList;
			resList = myDic [searchKey];
			int pos = Random.Range (0, resList.Count);
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
//			DisplayAllElementWithKey ("hello");

			bool isOk;
			string resStr;
			string keyStr;

			keyStr = "time";
			for (int loop=0; loop<10; loop++) {
				isOk = FindRandomly(keyStr, out resStr);
				if (isOk) {
					Debug.Log(keyStr + " >> " + resStr);
				}
			}
		}

	}
}

