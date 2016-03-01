using UnityEngine;
using System.Collections;
[System.Serializable]
public class JsonFileKeys
{
	public bool DEBUG = false; 
	public bool HD = false;
	public int FPS = 30;
	public int SCREEN_WIDTH = 2048;
	public int SCREEN_HEIGHT = 1024;
	public float[] SHADER;
	public string BATCH_FILENAME = "ffmpeg.bat";
	public string CAMERA_DEVICES_FILENAME = "CameraDevices.txt";
}
