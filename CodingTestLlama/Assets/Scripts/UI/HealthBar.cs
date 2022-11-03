using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider = null;

	private float _healthPercentage = 1;

	public float HealthPercentage
	{
		get { return _healthPercentage; }
		set 
		{
			_healthPercentage = Mathf.Clamp01(value);
			UpdateSlider();
		}
	}

	private void Awake()
	{
		if (!_slider)
		{ _slider = GetComponent<Slider>(); }
	}

	private void UpdateSlider()
	{
		if (!_slider)
		{ return; }

		_slider.value = _healthPercentage;	
	}
}
