using UnityEngine;
using System;

//カスタムコルーチン.
public class WaitForSecondsRealTime : CustomYieldInstruction
{
	float waitTime;

	public override bool keepWaiting
	{
		get { return Time.realtimeSinceStartup < waitTime; }
	}

	public WaitForSecondsRealTime(float time)
	{
		waitTime = Time.realtimeSinceStartup + time;
	}
}