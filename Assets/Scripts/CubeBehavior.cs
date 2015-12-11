using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class CubeBehavior : MonoBehaviour
{
	[DllImport("FFmpeg")]
	private static extern int CountUp();
	// Use this for initialization
	void Start()
	{
	}
	// Update is called once per frame
	void Update()
	{
		transform.position = new Vector3(0, CountUp() % 10, 0);
	}
}