using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetFpsSliderValue : MonoBehaviour
{
	
	[SerializeField] Slider fpsSlider;

	Text fpsValueText;

	void Awake()
	{
		fpsValueText = GetComponent<Text>();
		//fpsSlider.onValueChanged.AddListener(delegate { UpdateValue(); });
	}

	public void UpdateValue()
	{
		fpsValueText.text = "FPS : " + fpsSlider.value.ToString();
    }
}
