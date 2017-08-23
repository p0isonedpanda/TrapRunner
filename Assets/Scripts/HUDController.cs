using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    playerController pc;

    [Header("Stamina")]
    public Image staminaBar;
    public Image staminaUsed;

    // Use this for initialization
    void Start ()
    {
        pc = playerController.instance;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Change stamina bar display to represent remaining stamina
        staminaBar.fillAmount = pc.stamina / 100.0f;
        staminaUsed.fillAmount = 1.0f - staminaBar.fillAmount;
    }
}
