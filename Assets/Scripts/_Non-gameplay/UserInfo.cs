using UnityEngine;
using System.Collections;

public static class UserInfo {

	static string username;
	static bool loaded;

	const string defaultName = "Anonymous";

	public static string Username {
		get {
			if (!loaded) {
				loaded = true;
				username = PlayerPrefs.GetString("username",defaultName);
			}
			return username;
		}
		set {
			if (string.IsNullOrEmpty(value)) {
				username = defaultName;
			}
			else {
				username = value;
			}

			loaded = true;
			PlayerPrefs.SetString("username",username);
		}
	}

	public static string DeviceID {
		get {
			return SystemInfo.deviceUniqueIdentifier;
		}
	}
	
}
