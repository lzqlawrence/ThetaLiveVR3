using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugObject : MonoBehaviour
{
	[SerializeField] Slider radiusSlider;

	[SerializeField] Slider xSlider;
	[SerializeField] Slider ySlider;
	[SerializeField] Slider zSlider;
	[SerializeField] Slider wSlider;

	[SerializeField] Text radiusText;
	[SerializeField] Text xText;
	[SerializeField] Text yText;
	[SerializeField] Text zText;
	[SerializeField] Text wText;

	[SerializeField] MeshRenderer quad;

	public void SetSlilerValue(float r,float x,float y,float z,float w)
	{
		radiusSlider.value = r;
		xSlider.value = x;
		ySlider.value = y;
		zSlider.value = z;
		wSlider.value = w;
	}

	public void ChangeValueRudius()
	{
		quad.material.SetFloat("_Radius",radiusSlider.value);
		radiusText.text = "radius : " + radiusSlider.value;
    }

	public void ChangeValueOffset(int num)
	{
		quad.material.SetVector("_UVOffset", new Vector4(xSlider.value, ySlider.value, zSlider.value, wSlider.value));
		if(num==1)
			xText.text = "left x : " + xSlider.value;
		else if(num == 2)
			yText.text = "left y : " + ySlider.value;
		else if (num == 3)
			zText.text = "right x : " + zSlider.value;
		else if (num == 4)
			wText.text = "left y : " + wSlider.value;
	}

}
