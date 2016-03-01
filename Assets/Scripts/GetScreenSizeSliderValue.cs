using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetScreenSizeSliderValue : MonoBehaviour
{ 
	[SerializeField]
    Slider screenSizeSlider;

	Text screenSizeValueText;

	//void Awake()
	//{
	//	screenSizeValueText = GetComponent<Text>();
	//	//fpsSlider.onValueChanged.AddListener(delegate { UpdateValue(); });
	//}

	//public void UpdateValue()
	//{
	//	int width = (int)screenSizeSlider.value * 2 *32;
	//	int height = (int)screenSizeSlider.value * 32;
	//	screenSizeValueText.text = "Screen Width : " + width;
	//}
}
