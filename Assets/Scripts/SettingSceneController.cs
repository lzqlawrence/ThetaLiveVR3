using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class SettingSceneController : MonoBehaviour
{
	JsonFileKeys jsonfileKeys;
    string currentPath;

	[SerializeField] Toggle isUsingHDMI;
	[SerializeField] Slider fpsSlider;
	[SerializeField] InputField screenWidthInputField;
	[SerializeField] InputField screenHeightInputField;

	private bool isOnce;

	void Start()
	{
		//カレントフォルダを取得.
		currentPath = Directory.GetCurrentDirectory();
		currentPath = currentPath.Replace("/", "\\\\");
		Debug.Log(currentPath);

		System.IO.StreamReader reader = new System.IO.StreamReader(currentPath + "\\" + "Setting.json", System.Text.Encoding.GetEncoding("utf-8"));
		string textData = reader.ReadToEnd();
		reader.Close();
		Debug.Log(textData);

		jsonfileKeys = new JsonFileKeys();
		jsonfileKeys = JsonUtility.FromJson<JsonFileKeys>(textData);

		isUsingHDMI.isOn = jsonfileKeys.HD;
		fpsSlider.value = jsonfileKeys.FPS;
		screenWidthInputField.text = jsonfileKeys.SCREEN_WIDTH.ToString();
		screenHeightInputField.text = jsonfileKeys.SCREEN_HEIGHT.ToString();
	}

	public void OnClickStatButton()
	{
		if (isOnce) return;
		isOnce = true;
		jsonfileKeys.HD = isUsingHDMI.isOn;
		jsonfileKeys.FPS = (int)fpsSlider.value;
		jsonfileKeys.SCREEN_WIDTH = int.Parse(screenWidthInputField.text);
		jsonfileKeys.SCREEN_HEIGHT = int.Parse(screenHeightInputField.text);
		
        string json = JsonUtility.ToJson(jsonfileKeys,true);
		Debug.Log("write\n" + json);
		File.WriteAllText(currentPath + "\\" + "Setting.json", json);
		
		//  Use this call for wherever a player triggers a custom event
		Analytics.CustomEvent("Setting",new Dictionary<string, object>
		{
			{ "jsonHD", jsonfileKeys.HD },
			{ "jsonFPS", jsonfileKeys.FPS },
			{ "jsonWIDTH", jsonfileKeys.SCREEN_WIDTH },
			{ "jsonHEIGHT", jsonfileKeys.SCREEN_HEIGHT }
  		});
		StartCoroutine(ChangeScene());
		
	}

	IEnumerator ChangeScene()
	{
		yield return new WaitForSecondsRealTime(0.1f);
		SceneManager.LoadScene("ThetaRealtimeEquirectanguler");
	}
}