using UnityEngine;
using System.Collections;

public class DynamicCrosshair : MonoBehaviour {
    public RectTransform[] crosshair;

    private static WeaponInventory inventory;
    public static void SetInventory(WeaponInventory value) {
        inventory = value;
    }
	// Update is called once per frame
	void Update () {
        if (!UIManager.IsCurrentMenuWindow(Menu.PlayerHUD)) return;
        if(inventory == null) return;
        MoveCrosshair(CalculatePixelMove(inventory.currentWeapon.shotSpread));
	}
    float CalculatePixelMove(float currentSpread) {
        float vertFov = Camera.main.fieldOfView;
        float angleToCentre = vertFov / 2;
        float moveFraction = currentSpread / angleToCentre;
        return Screen.height/2f * moveFraction;
    }
    void MoveCrosshair(float screenDistance) {
        // Right, Up, Left, Down
        crosshair[0].anchoredPosition = new Vector2(screenDistance, 0);
        crosshair[1].anchoredPosition = new Vector2(0, screenDistance);
        crosshair[2].anchoredPosition = new Vector2(-screenDistance, 0);
        crosshair[3].anchoredPosition = new Vector2(0, -screenDistance);

    }
}
