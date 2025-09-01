using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IHPBarReader, IDamagable
{
    [SerializeField]
    private PlayerPreset Preset;


    public EventableValue<int> ExtraLife { get; private set; } = new();
    public EventableValue<int> CurrentHP { get; private set; } = new();
    public EventableValue<int> AttackSpeed { get; private set; } = new();
    public EventableValue<int> AttackValue { get; private set; } = new();

    public int MaxHP { get; private set; }

    private List<AttackWrapper> _attacks = new();
    private SpriteRenderer _sr;

    public UnityEvent<int> OnCurrentHPChange;
    public UnityEvent<int> OnExtraLifeChange;
    public UnityEvent<int> OnAttackSpeedChange;
    public UnityEvent<int> OnAttackValueChange;

    private bool _isInitialised = false;

    public async UniTask Init()
    {
        if(_isInitialised)
        {
            SetDefault();
            return;
        }

        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = Preset.Sprites[0];
        foreach(var attack in Preset.Attacks)
        {
            _attacks.Insert(0,AttackWrapper.Create(transform, attack));
            await UniTask.Yield();
        }
        ExtraLife.OnValueChange += (x) => OnExtraLifeChange?.Invoke(x);
        CurrentHP.OnValueChange += (x) => OnCurrentHPChange?.Invoke(x);
        AttackSpeed.OnValueChange += (x) => OnAttackSpeedChange?.Invoke(x);
        AttackValue.OnValueChange += (x) => OnAttackValueChange?.Invoke(x);
        SetDefault();
        _isInitialised = true;
        Debug.Log("Player инициализирован");
    }

    private void SetDefault()
    {
        MaxHP = Preset.HP;
        CurrentHP.Set(Preset.HP);
        AttackSpeed.Set(Preset.AttackSpeed);
        AttackValue.Set(Preset.AttackValue);
        gameObject.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        if (!_isInitialised)
        {
            return;
        }

        Debug.Log($"Игрок получает {damage} урона");
        CurrentHP.Set(CurrentHP.Value - damage);
        if (CurrentHP.Value <= 0)
        {
            if (ExtraLife.Value > 0)
            {
                ExtraLife.Set(ExtraLife.Value - 1);
                CurrentHP.Set(MaxHP);
            }
            else
            {
                GameStateMachine.ChangeState(GameStateMachine.Current.EndState);
            }
        }
    }

    public async UniTask Attack(List<Transform> targets)
    {
        if (_attacks.Count == 0 || targets.Count == 0)
            return;

        // Фильтруем мёртвых/неактивных сразу
        AlliveTargetFilters.GetAlliveTargtes(ref targets);

        if (targets.Count == 0)
            return;

        int attackCount = AttackSpeed.Value;

        foreach (var attack in _attacks)
        {
            if (await attack.TryRelease(targets, AttackValue.Value))
            {
                attackCount -= attack.AttackWeight;

                // Обновляем список целей по месту
                AlliveTargetFilters.GetAlliveTargtes(ref targets);

                if (attackCount <= 0 || targets.Count == 0)
                    return;
            }
        }

        // Добиваем оставшимся типом атаки
        while (attackCount > 0 && targets.Count > 0)
        {
            await _attacks[^1].ReleaseAttack(targets, AttackValue.Value);
            attackCount -= _attacks[^1].AttackWeight;
            AlliveTargetFilters.GetAlliveTargtes(ref targets);
        }
    }

    public void UpgradeHP(int amount)
    {
        if (!_isInitialised)
            return;

        MaxHP += amount;
        CurrentHP.Set(CurrentHP.Value + amount);
    }

    public void UpgradeAttackSpeed(int amount)
    {
        if (!_isInitialised)
            return;

        AttackSpeed.Set(AttackSpeed.Value + amount);
    }

    public async UniTask UpgradeAttack(AbstractAttack newAttack)
    {
        if (!_isInitialised || newAttack == null)
            return;

        _attacks.Add(AttackWrapper.Create(transform, newAttack));
        await UniTask.Yield();
    }

    public void UpgradeAttackValue(int amount)
    {
        if (!_isInitialised)
            return;

        AttackValue.Set(AttackValue.Value + amount);
    }

    public void UpgradeExtraLife(int amount)
    {
        if (!_isInitialised)
            return;

        ExtraLife.Set(ExtraLife.Value + amount);
    }
}
