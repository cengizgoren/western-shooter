using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position + new Vector3(0, 2f, 0), Camera.main.transform.rotation);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }

    private TextMeshPro textMesh;
    private float timeUntilFadeAway;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        timeUntilFadeAway = 1f;
    }

    private void Update()
    {
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;

        timeUntilFadeAway -= Time.deltaTime;
        if (timeUntilFadeAway < 0)
        {
            float fadeAwaySpeed = 3f;
            textColor.a -= fadeAwaySpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
