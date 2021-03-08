using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[Header("Crosshair")]
	[SerializeField] private Image Crosshair;

	[Header("Ammo Panel")]
	[SerializeField] private GameObject AmmoPanelRoot;
	[SerializeField] private Text ItemName;
	[SerializeField] private Text ItemAmmo;
	[SerializeField] private Text ItemAmmoClip;

	private GBE gbe { get { return GBE.instance; } }

	public static UIController Instance;

	private GameObject _selectedObject = null;

	void Awake()
	{
		ShowAmmoPanel(false, false, false, false);
	}

	void OnEnable()
	{
		Instance = this;
	}
	
	void OnDisable()
	{
		Instance = null;
	}
	
	void Update()
	{
		Crosshair.enabled = true;
	}

	public void ShowAmmoPanel(bool showPanel = true, bool showName = true, bool showAmmo = true, bool showClip = true)
	{
		AmmoPanelRoot.gameObject.SetActive(showPanel);
		ItemName.enabled = showName;
		ItemAmmo.enabled = showAmmo;
		ItemAmmoClip.enabled = showClip;
	}

	public void SetAmmoPanel(string name, int ammo, int clip)
	{
		ItemName.text = name;
		ItemAmmo.text = ammo.ToString();
		ItemAmmoClip.text = clip.ToString();
	}
}
