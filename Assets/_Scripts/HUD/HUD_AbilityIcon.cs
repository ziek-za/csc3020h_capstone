using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class HUD_AbilityIcon : MonoBehaviour {
	public Image cooldownIcon;
	public Slider healthBar;

	// All icons cooldown status
	private float cooldownProgress = 0f;
	private float cooldown = 0f;

	// Buildings health
	private float buildingHealth = 0f;

	void Update() {
		// Used to set cooldown radial fill
		if (cooldownProgress <= cooldown && cooldownProgress > 0f) {
			cooldownProgress -= Time.deltaTime;
			cooldownIcon.fillAmount = Mathf.Clamp01(cooldownProgress/cooldown);
		}
	}

	// Used to activate the cooldown
	public void ActivateCooldownGUI(float cooldown) {
		this.cooldown = cooldown;
		cooldownProgress = cooldown;
	}

	// Used to activate the healthbar above buildings
	public void ActivateBuildingHealth(float currentBuildingHealth) {
		// Used to set health bar slider
		if (currentBuildingHealth > 0f) {
			healthBar.value = Mathf.Clamp01 (currentBuildingHealth/buildingHealth);
		}
	}

	// Used to set the initial buldings health
	public void SetBuildingHealth(float health) {
		this.buildingHealth = health;
	}
}
