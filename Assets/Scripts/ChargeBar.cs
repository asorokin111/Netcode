using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    [SerializeField]
    private Gradient _colors;
    private Image _chargeBar;
    private float _fill = 0;

    private void Awake()
    {
        _chargeBar = GetComponent<Image>();
    }

    private void OnEnable()
    {
        GolfClub.OnHitForceChanged += UpdateChargeBar;
    }

    private void OnDisable()
    {
        GolfClub.OnHitForceChanged -= UpdateChargeBar;
    }

    private void UpdateChargeBar(float maxForce, float currentForce, float initialForce)
    {
        _fill = Mathf.Clamp(currentForce / maxForce, 0.0f, 1.0f);
        if (_fill <= initialForce)
        {
            _chargeBar.fillAmount = 0.0f;
            return;
        }
        _chargeBar.fillAmount = _fill;
        SetColor();
    }

    private void SetColor()
    {
        _chargeBar.color = _colors.Evaluate(_fill - 1);
    }
}