using System;
using UnityEngine;

// My naive attempt at making cheating slightly more difficult. Not savvy enough to know if this is even remotely helpful or not. Oh well...

public struct SecureInt : IEquatable<SecureInt> {

	int encryptedValue;
	const int key = 31415;

	// encrypt/decrypt value
	static int TransformValue(int value) {
		return value ^ key;
	}

	/*
	 * operators and equality checking:
	*/

	public static implicit operator int(SecureInt secureInt)
	{
		return TransformValue (secureInt.encryptedValue);
	}

	public static implicit operator SecureInt(int intValue)
	{
		SecureInt secureInt = new SecureInt ();
		secureInt.encryptedValue = TransformValue (intValue);
		return secureInt;
	}

	public static SecureInt operator++(SecureInt secureInt) {
		int newIntValue = TransformValue (secureInt.encryptedValue) + 1;
		secureInt.encryptedValue = TransformValue (newIntValue);

		return secureInt;
	}

	public static SecureInt operator--(SecureInt secureInt) {
		int newValue = TransformValue (secureInt.encryptedValue) - 1;
		secureInt.encryptedValue = TransformValue (newValue);

		return secureInt;
	}

	public bool Equals(SecureInt other)
	{
		return encryptedValue == other.encryptedValue;
	}
}
