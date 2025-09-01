using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class UnitWrapper : MonoBehaviour, IHPBarReader, IDamagable
{
    private AbstractUnitTemplate _main;
    private SpriteRenderer _sr;
    private List<AttackWrapper> _attacks = new();
    public EventableValue<int> CurrentHP { get; private set; } = new();
    public int MaxHP => IsInitialized ? _main.HP : 0;

    public UnityEvent<int> OnCurrentHPChange;
    public UnityEvent<UnitWrapper> OnUnitDie;

    public bool IsInitialized { get { return _main != null; } }

    public int Reward => IsInitialized ? _main.CoinDrop : 0;

    public Type TemplateType
    {
        get
        {
            if(IsInitialized)
            {
                return _main.GetType();
            }
            else
            {
                return default;
            }
        }
    }


    public async UniTask Init(AbstractUnitTemplate unit)
    {
        if(IsInitialized)
        {
            Debug.LogWarning($"Unit-объект {name} уже инциализирован.");
            return;
        }
        _main = unit;
        CurrentHP.OnValueChange += (x) => OnCurrentHPChange?.Invoke(x);
        CurrentHP.Set(_main.HP);

        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = _main.Sprites[0];
        foreach(var attack in _main.Attacks)
        {
            _attacks.Insert(0, AttackWrapper.Create(transform, attack));
            await UniTask.Yield();
        }
    }

    public void TakeDamage(int damage)
    {
        if(!IsInitialized)
        {
            return;
        }

        Debug.Log($"{name} получает {damage} урона");
        CurrentHP.Set(CurrentHP.Value - damage);
        if (CurrentHP.Value <= 0)
        {
            OnUnitDie?.Invoke(this);
            EnemyPool.Return(this);
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

        int attackCount = _main.AttackSpeed;

        foreach (var attack in _attacks)
        {
            if (await attack.TryRelease(targets, _main.AttackValue))
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
            await _attacks[^1].ReleaseAttack(targets, _main.AttackValue);
            attackCount -= _attacks[^1].AttackWeight;
            AlliveTargetFilters.GetAlliveTargtes(ref targets);
        }
    }

    public void ReturnToContainer(Transform container)
    {
        CurrentHP.Set(_main.HP);
        transform.SetParent(container);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    public async UniTask MoveTo(Transform target)
    {
        while (Vector3.Distance(target.position, transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, 3 * Time.deltaTime);
            await UniTask.WaitForFixedUpdate();
        }
    }
}
