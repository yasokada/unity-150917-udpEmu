using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenSizeKeeper : MonoBehaviour {

	public Button myButton; // set UI>Button whose width is used as standard

	bool isRunningOnAndroid() {
		return (Application.platform == RuntimePlatform.Android);
	}

	void Start () {
		if (isRunningOnAndroid () == false) {
			return;
		}
		float aspect = (float)Screen.height / (float)Screen.width;
		float buttonRatio = 0.5f; // 50%
		int buttonWidth = (int)myButton.GetComponent<RectTransform> ().rect.width;
		float newWidth, newHeight;
		
		newWidth = buttonWidth / buttonRatio;
		newHeight = newWidth * aspect;
		
		Screen.SetResolution ((int)newWidth, (int)newHeight, false);
	}   

}
