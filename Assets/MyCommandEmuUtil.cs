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

// KeyNotFoundException
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

		public static void displayAllElementWithKey(string searchKey) {
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

		public static void Test_main() {
			Init ();
			Add ("hello", "hello, Mike");
			Add ("hello", "hello, Suzuki sann");
			Add ("hello", "hello, Francheska");
			displayAllElementWithKey ("hello");

		}

	}
}

