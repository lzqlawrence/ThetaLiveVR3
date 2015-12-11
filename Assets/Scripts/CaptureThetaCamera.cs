using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using System.Security.Permissions;

using System.Runtime.InteropServices;

public class CaptureThetaCamera : MonoBehaviour
{
	//-------------------------------
	// DLL読み込み.
	//-------------------------------
	[DllImport ("NamedPipeUtils")]
	private static extern int Test();

	[DllImport ("NamedPipeUtils")]
	private static extern int InitNamedPipe();

	[DllImport ("NamedPipeUtils")]
	private static extern int Connect();

	[DllImport ("NamedPipeUtils")]
	private static extern int DisConnect();  

	[DllImport ("NamedPipeUtils")]
	private static extern int WritePipe(IntPtr data, int data_size);
	

	//-------------------------------
	// 内部用定数(iniから変更可能).
	//-------------------------------
	[SerializeField]
	JsonFileKeys jsonfileKeys;
	const int SCREEN_WIDTH = 2048;
	const int SCREEN_HEIGHT = 1024;

	//-------------------------------
	// インスペクタから設定.
	//-------------------------------
	[SerializeField]
	RenderTexture renderTex;

	[SerializeField]
	Camera		mainCamera;

	[SerializeField]
	Text		label;

	[SerializeField]
	Image		liveIconImage;

	//-------------------------------
	// キャッシュ.
	//-------------------------------
	IniFile ini;
	Texture2D tex;
	Rect rect;

	WebCamTexture webcamTexture = null;
	GCHandle cameraTexturePixelsHandle_;
	IntPtr cameraTexturePixelsPtr_;
	
	static System.Diagnostics.Process exProcess;

	bool			isInit		= false;
	bool			isWrite		= false;
	string			initResult	= string.Empty;
	static string	currentPath	= string.Empty;
	static bool		isFinish	= false;
	static bool		isFinishThread = false;

	Thread thread2;

	void Start ()
	{
		//カレントフォルダを取得.
		currentPath = Directory.GetCurrentDirectory();
		currentPath = currentPath.Replace("/", "\\\\");
		Debug.Log(currentPath);


		//renderTex.width = SCREEN_WIDTH;
		//renderTex.height = SCREEN_HEIGHT;
		//Screen.SetResolution(SCREEN_WIDTH, SCREEN_HEIGHT, false);


		System.IO.StreamReader reader = new System.IO.StreamReader(currentPath + "\\" + "Setting.json", System.Text.Encoding.GetEncoding("utf-8"));
		string textData = reader.ReadToEnd();
		reader.Close();
		Debug.Log(textData);

		jsonfileKeys = new JsonFileKeys();
		jsonfileKeys = JsonUtility.FromJson<JsonFileKeys>(textData);

		//RenderTextureと画面解像度を指定サイズに合わせる.
		renderTex.width = jsonfileKeys.SCREEN_WIDTH;
		renderTex.height = jsonfileKeys.SCREEN_HEIGHT;
		Screen.SetResolution(jsonfileKeys.SCREEN_WIDTH, jsonfileKeys.SCREEN_HEIGHT, false);
		liveIconImage.gameObject.SetActive(false);
		
		//外部通信用のパイプを作成.
		int result = InitNamedPipe();
		initResult = "init():" + ((result == 1) ? "OK" : "NG");

		Debug.Log(initResult);

		//初期化処理(パイプを作成できなければアプリを閉じる).
		if (result == 1)
		{
			StartCoroutine(InitializeRoutine());
		}
		else
		{
			label.text = initResult + "\nパイプの作成に失敗しました。\n3秒後にアプリを終了します。\n\nまた、UnityEditerの方も含めて、\n多重起動はしないで下さい。";
			StartCoroutine(EndRoutine(3f));
		}
			
	}

	/// <summary>
	/// カメラ初期化.
	/// </summary>
	IEnumerator InitializeRoutine()
	{
		List<WebCamDevice> device = new List<WebCamDevice>(WebCamTexture.devices);

		//(テスト)接続されているデバイス名をログに取る.
		foreach (WebCamDevice d in device)
		{
			Debug.Log("接続されているカメラ名 : " + d.name);
		}

		//指定したカメラの番号を取得.
		int deviceNumber = GetCameraIndexFromTextfile(ref device);

		if (deviceNumber >= 0)
		{
			//初期化フラグ.
			isInit = true;

			// オリジナルのままカメラ情報を取得.
			if (webcamTexture == null)
				webcamTexture = new WebCamTexture(device[deviceNumber].name, UnityEngine.Screen.width, UnityEngine.Screen.height,jsonfileKeys.FPS);

			//WebCameraを起動.
			if (webcamTexture != null)
				webcamTexture.Play();
			
			//サイズが取得されるまで待つ(webcamTexture.Playの後でないとwidth,heightが取れないので注意).
			while (webcamTexture.width == webcamTexture.height)
			{
				yield return null;
			}

			//カメラアスペクト比を2:1とする.
			float webcamAspect = 2f / 1f;

			float height = 2f * mainCamera.orthographicSize;
			float width = height * webcamAspect;

			//四角形ポリゴンのサイズをカメラアスペクト比に合わせる.
			transform.localScale = new Vector3(-width, height, 1.0f);

			//カメラ映像をテクスチャに反映.
			Renderer renderer = gameObject.GetComponent<Renderer>();
			renderer.material.mainTexture = webcamTexture;

			if(renderTex != null)
				RenderTexture.active = renderTex;

			label.text ="Webカメラが接続されました\n(Connectを開始しました。)";
			
		}
		else
		{
			label.text = "指定されたWebカメラが見つかりません\n(Rキーで再接続チェック)";
			Debug.Log("指定されたWebカメラが検出できませんでした");
			yield break;
		}

		yield return new WaitForSecondsRealTime(0.38f);
		
		yield return StartCoroutine(SendTexture());
	}

	IEnumerator SendTexture()
	{
		//Connect用のスレッドを立てる.
		Thread thread = new Thread(new ThreadStart(ConnectThread));
		thread.IsBackground = true;
		thread.Start();

		yield return new WaitForSecondsRealTime(0.1f);

		thread2 = new Thread(new ThreadStart(CallBatThread));
		thread2.IsBackground = true;
		thread2.Start();

		//繋がるまで待機.
		while (!isFinish)
		{
			yield return null;
		}

		label.text = "Connect:OK -> 撮影中\n(Qキーで停止します)";
		isWrite = true;
		liveIconImage.gameObject.SetActive(true);

		tex = new Texture2D(renderTex.width, renderTex.height);
		rect = new Rect(0, 0, renderTex.width, renderTex.height);

		while (true)
		{
			try
			{
				if (!isInit) break;
				RenderTexture.active = renderTex;
				mainCamera.Render();
				tex.ReadPixels(rect, 0, 0);
				tex.Apply();
				
				WriteTexture();
			}
			catch (IOException e)
			{
				Debug.Log(e.Message);
				yield break;
			}
			
			yield return null;
		}
		label.text = "撮影を停止しました\n(Rキーで再接続)";
	}

	static void ConnectThread()
	{
		int result = Connect();
		Debug.Log("Connect() = " + result);
		isFinish = true;
	}

	static void CallBatThread()
	{
		try
		{
			string targetDir = currentPath;
			exProcess = new System.Diagnostics.Process();
			exProcess.StartInfo.WorkingDirectory = targetDir;
			exProcess.StartInfo.FileName = JsonFileKeys.BATCH_FILENAME;
			exProcess.StartInfo.CreateNoWindow = true;
			//exProcess.
            exProcess.Start();
			exProcess.WaitForExit();
		}
		catch (Exception ex)
		{
			Debug.Log("Exception Occurred: " + ex.Message + ","+ ex.StackTrace.ToString());
			//exProcess.Dispose();
			exProcess.Close();
		}

		exProcess = null;
		OnFinishThread();
    }

	void WriteTexture()
	{
		Color32[] color32 = tex.GetPixels32();
		cameraTexturePixelsHandle_ = GCHandle.Alloc(color32, GCHandleType.Pinned);
		cameraTexturePixelsPtr_ = cameraTexturePixelsHandle_.AddrOfPinnedObject();

		WritePipe(cameraTexturePixelsPtr_, jsonfileKeys.SCREEN_WIDTH * jsonfileKeys.SCREEN_HEIGHT * 4);

		cameraTexturePixelsHandle_.Free();
	}

	void Update()
	{
		if (!isInit)
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				StartCoroutine(InitializeRoutine());
			}
		}
		if(isWrite)
		{
			if (Input.GetKeyDown(KeyCode.Q) && isFinish)
			{
				if(!exProcess.CloseMainWindow())
					exProcess.Kill();
				else
					exProcess.Close();

				exProcess = null;
				isFinish = false;
			}
			if (isFinishThread)
			{
				isInit = false;
				isWrite = false;
				isFinish = false;
				isFinishThread = false;
                webcamTexture.Stop();
				int result = DisConnect();
				Debug.Log("DisConnect() = " + (result == 1 ? "OK" : "NG"));
				liveIconImage.gameObject.SetActive(false);
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(exProcess != null)
			{
				if (!exProcess.CloseMainWindow())
					exProcess.Kill();
				else
					exProcess.Close();

				exProcess = null;
			}
			UnityEngine.Application.Quit();
		}
	}

	static void OnFinishThread()
	{
		isFinishThread = true;
		Debug.Log("Thread Finish");
	}

	/// <summary>
	/// テキストファイルで指定された接続カメラの番号を取得.
	/// </summary>
	/// <param name="device"></param>
	/// <returns></returns>
	static int GetCameraIndexFromTextfile(ref List<WebCamDevice> device)
	{
		//テキストファイルから対応デバイス名を取得.
		List<string> deviceNameList = new List<string>();
		StreamReader file = new StreamReader(currentPath + "\\" + JsonFileKeys.CAMERA_DEVICES_FILENAME);

		while (file.Peek() > -1)
		{
			//1行ずつ読み込む.
			string lineStr = file.ReadLine().Trim();
			deviceNameList.Add(lineStr);
			Debug.Log("指定されているカメラ名 : " + lineStr);
		}

		int deviceNumber = -1;
		foreach (string name in deviceNameList)
		{
			deviceNumber = device.FindIndex(d => d.name == name);
			if (deviceNumber != -1) break;
		}
		return deviceNumber;
	}

	/// <summary>
	/// 指定した秒後に終了する.
	/// </summary>
	IEnumerator EndRoutine(float t)
	{
		yield return new WaitForSecondsRealTime(t);

		UnityEngine.Application.Quit();
		yield return 0;
	}
	
}

/*
 * 書き出しffmpeg設定のメモ
 * C:\ffmpeg-20150521-git-0b9d636-win64-static\bin>ffmpeg -y -f rawvideo -r 20 -vcodec rawvideo -pix_fmt rgba -s 2048x1024 -i \\.\pipe\ThetaLiveVR -vcodec libx264 -pix_fmt yuv420p -s 2048x1024 -b:v 3800k -threads 0 -f mp4 theta_test3.mp4
*/

