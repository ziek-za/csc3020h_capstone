using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class HUD_AbilityIcon : MonoBehaviour {
	public Image cooldownIcon;

	private float cooldownProgress = 0f;
	private float cooldown = 0f;

	void Update() {
		if (cooldownProgress <= cooldown && cooldownProgress > 0f) {
			Debug.Log ("here " + cooldownProgress + " " + cooldownIcon.fillAmount);
			cooldownProgress -= Time.deltaTime;
			cooldownIcon.fillAmount = Mathf.Clamp01(cooldownProgress/cooldown);
		}
	}

	public void ActivateCooldownGUI(float cooldown) {
		this.cooldown = cooldown;
		cooldownProgress = cooldown;
	}
}
