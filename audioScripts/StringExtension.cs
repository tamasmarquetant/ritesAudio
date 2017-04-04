using UnityEngine;
using System.Collections;

public static class StringExtension {


	/* function that chops a string by length and return an array of those strings
	 * LINK to source http://answers.unity3d.com/questions/54335/string-split-by-length.html

		calling application:
		string bar = "this is an incredible long string that will be chopped down"; 
		string[] foo = bar.Chop(10);
		foreach(string f in foo) Debug.Log(string.Format("[{0}]", f)); 
	  */ 

	public static string [] Chop (this string value, int length){
		int strLength = value.Length;
		int strCount = (strLength + length - 1) / length;
		string[] result = new string[strCount];
		for (int i = 0; i < strCount; ++i)
		{
			result[i] = value.Substring(i * length, Mathf.Min(length, strLength));
			strLength -= length;
		}
		return result;	
	}


}