using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image CurrentHP;
    [SerializeField]
    private TMP_Text Text;

    public GameObject Unit;

    private int maxHP = -1;

    private void Start()
    {
        if(Unit.TryGetComponent(out IHPBarReader component))
        {
            maxHP = component.MaxHP;
            Text.text = $"{maxHP}/{maxHP}";
            CurrentHP.fillAmount = 1;
        }
        else
        {
            Debug.LogError($"Не удаётся получить значени HP [{Unit.name}]");
            return;
        }
    }

    public void OnCurrentHPChange(int value)
    {
        if (Unit.TryGetComponent(out IHPBarReader component))
        {
            maxHP = component.MaxHP;
            Text.text = $"{maxHP}/{maxHP}";
            CurrentHP.fillAmount = 1;
        }
        else
        {
            Debug.LogError($"Не удаётся получить значени HP [{Unit.name}]");
            return;
        }

        Text.text = $"{value}/{maxHP}";
        CurrentHP.fillAmount = (float)value/(float)maxHP;

    }
}
