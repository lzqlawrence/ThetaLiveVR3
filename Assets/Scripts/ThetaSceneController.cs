using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

using System;
using System.Collections.Generic;
using System.IO;
public class ThetaSceneController : MonoBehaviour
{

	//void Start()
	//{
	//	//Processオブジェクトを作成
	//	System.Diagnostics.Process p = new System.Diagnostics.Process();

	//	//ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定.
	//	p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
	//	//出力を読み取れるようにする.
	//	p.StartInfo.UseShellExecute = false;
	//	p.StartInfo.RedirectStandardOutput = true;
	//	p.StartInfo.RedirectStandardInput = false;
	//	//ウィンドウを表示しないようにする.
	//	p.StartInfo.CreateNoWindow = true;

	//	const string folderName = @"ffmpeg";

	//	string filePath = Application.persistentDataPath + "\\" + folderName;
	//	List<string>  strCmds = new List<string>();

	//	//文字列テスト.
	//	strCmds.Add(@"/c dir  /w");

	//	//パス移動
	//	strCmds.Add(@"/c cd " + filePath );
	//	strCmds.Add(@"/" + filePath + " cd " + folderName );
	//	//ディレクトリ表示
	//	strCmds.Add("/" + filePath + "/" + folderName + @" dir  /w");

	//	//ffmpeg
	//	strCmds.Add(@"ffmpeg -y -f rawvideo -r 10 -vcodec rawvideo -pix_fmt rgba -s 2048x1024 -i \\.\pipe\ThetaLiveVR -vcodec libx264 -pix_fmt yuv420p -s 2048x1024 -b:v 1000k -threads 0 -f mp4 theta_test2.mp4");

	//	for (int i = 0; i<strCmds.Count; i++)
	//	{
	//		p.StartInfo.Arguments = strCmds[i];
	//		System.Diagnostics.Process pro = System.Diagnostics.Process.Start(p.StartInfo);

	//		string results = pro.StandardOutput.ReadToEnd();
	//		p.WaitForExit();

	//		Debug.Log(results);
	//	}
	//	p.Close();

	//	Debug.Log(filePath);
	//	//出力された結果を表示
		
	//}

	//public int CallBatch()
	//{
	//	System.Diagnostics.Process p = new System.Diagnostics.Process();
	//	p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
	//	p.StartInfo.CreateNoWindow = true;
	//	p.StartInfo.UseShellExecute = false;
	//	p.StartInfo.Arguments = string.Format(@"/" + Application.persistentDataPath +"/"+ "ffmpeg.bat");
	//	System.Diagnostics.Process pro = System.Diagnostics.Process.Start(p.StartInfo);
	//	pro.WaitForExit();
	//	return pro.ExitCode;
	//}


	//void Update ()
	//{
	//	if(Input.GetKeyDown(KeyCode.Escape))
 //       {
	//		Application.Quit();
	//	}
	//}

}