using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for Dictionary / List

namespace NS_MyCommandEmuUtil
{
	public static class MyCommandEmuUtil {
		static Dictionary <string, List<string>> myDic = null;

		public static void Init() {
			if (myDic == null) {
				myDic = new Dictionary<string, List<string>> ();
			}
		}

		public static void Add(string command, string response) {

		}
	}
}

