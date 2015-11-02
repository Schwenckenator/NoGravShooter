using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DynamicCrosshair : MonoBehaviour {
    public RectTransform[] crosshair;
    public Color neutral;
    public Color enemy;
    public Color ally;

    private Image[] crosshairImages;
    private Color currentColour;
    private static WeaponInventory inventory;
    public static void SetInventory(WeaponInventory value) {
        inventory = value;
    }
    public static Collider myActor;

    void Start() {
        crosshairImages = new Image[crosshair.Length];
        for(int i=0; i<crosshair.Length; i++) {
            crosshairImages[i] = crosshair[i].GetComponent<Image>();
        }
    }
	// Update is called once per frame
	void Update () {
        if (!UIManager.IsCurrentMenuWindow(Menu.PlayerHUD)) return;
        if (inventory == null) return;
        if (inventory.currentWeapon == null) return;
        MoveCrosshair(CalculatePixelMove(inventory.currentWeapon.shotSpread));
        ColourCrosshair();
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

    void ColourCrosshair() {
        // Fire a raycast through crosshair
        Color newColour = neutral;
        Transform cam = Camera.main.transform;
        RaycastHit hit;
        
        if (Physics.Raycast(cam.position, cam.forward, out hit)) {
            if (hit.collider.CompareTag("Player") && !hit.collider.Equals(myActor)) {
                if (SettingsManager.singleton.IsTeamGameMode() && NetworkManager.MyPlayer().IsOnTeam(hit.collider.GetComponent<ActorTeam>().GetTeam())){
                    newColour = ally;
                } else {
                    newColour = enemy;
                }
            }
        }

        ChangeCrosshairColour(newColour);
    }

    void ChangeCrosshairColour(Color col) {
        if (currentColour.Equals(col)) return; // Only change if colour is different

        foreach (Image im in crosshairImages) {
            im.color = col;
        }
    }
}
