using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AttackType : ScriptableObject
{
    public string attackId;
    public string attackName;
    public float range;
    public DamageType damageType;
    public float attackSpeed;
    public int healthDamage;
    public int shieldDamage;
    public string attackSoundPath;
    public bool continuousSound;

    public AudioClip GetAudioClip() {
        return Resources.Load<AudioClip>(attackSoundPath.Replace("Assets/Resources/", "").Split('.')[0]);
    }
}
