using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackWrapper : MonoBehaviour
{
    private AbstractAttack _main;
    private SpriteRenderer _sr;

    public int AttackWeight { get {  return _main.AttackWeight; } }
    public int Damage {  get { return _main.Damage; } }


    private void Init(AbstractAttack attack)
    {
        _main = attack;
        _sr = gameObject.AddComponent<SpriteRenderer>();
        _sr.sprite = _main.Sprite;
    }

    public async UniTask<bool> TryRelease(List<Transform> targets, int AdditionalDamage = 0)
    {
        bool temp = Random.Range(0, 100) <= _main.ReleaseChance;

        if (temp)
        {
            await ReleaseAttack(targets, AdditionalDamage);
        }

        return temp;
    }

    public async UniTask ReleaseAttack(List<Transform> targets, int AdditionalDamage = 0)
    {
        await _main.ReleaseAttack(targets, AdditionalDamage,  async (x) => { await MoveTo(x, AdditionalDamage); });
    }

    private async UniTask MoveTo(Transform target, int AdditionalDamage = 0)
    {
        gameObject.SetActive(true);
        while (Vector3.Distance(target.position, transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, _main.Speed * Time.deltaTime);
            await UniTask.WaitForFixedUpdate();
        }
        transform.position = target.position;
        if(target.TryGetComponent(out IDamagable unit))
        {
            unit.TakeDamage(Damage + AdditionalDamage);
        }
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
        await UniTask.WaitForSeconds(0.25f);
    }


    public static AttackWrapper Create(Transform parent, AbstractAttack attack)
    {
        var wrapper = new GameObject().AddComponent<AttackWrapper>();
        wrapper.Init(attack);
        wrapper.transform.localScale = new Vector3(2, 2, 1);
        wrapper.transform.SetParent(parent);
        wrapper.transform.localPosition = Vector3.zero;
        wrapper.gameObject.SetActive(false);
        wrapper.name = attack.GetType().Name + " Wrapper";
        return wrapper;
    }
}
