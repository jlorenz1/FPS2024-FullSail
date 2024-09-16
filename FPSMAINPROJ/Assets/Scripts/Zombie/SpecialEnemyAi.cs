using UnityEngine;

public class SpecialEnemy : EnemyAI
{
    public float specialAbilityCooldown = 5f;
    private float nextAbilityTime = 5f;
    [SerializeField] float BuffRange = 30;
    [SerializeField] bool maxHealthBuffer;
    [SerializeField] bool AttackSpeedBuffer;
    [SerializeField] bool damageBUffer;
    [SerializeField] bool ArmorBUffer;
    [SerializeField] bool Healer;


    [Header("-----Ability Stats-----")]
    [SerializeField] float AttackSpeedBuff;
    [SerializeField] float AttackDamageBuff;
    [SerializeField] float HealthBuff;
    [SerializeField] float ArmorBuff;
    [SerializeField] float Healing;


    [SerializeField] AudioClip ZombieBuff;
    [SerializeField, Range(0f, 1f)] float ZombieBuffVol;

    protected override void Update()
    {
        base.Update();

        if (Time.time >= nextAbilityTime)
        {
            UseSpecialAbility();
            if (ZombieBuff != null)
            {
               PlayAudio(ZombieBuff, ZombieBuffVol);
            }

            nextAbilityTime = Time.time + specialAbilityCooldown;
        }
    }

    private void UseSpecialAbility()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < BuffRange)
            {

                IEnemyDamage FellowZombie = zombie.GetComponent<IEnemyDamage>();
                if (FellowZombie != null)
                {

                    Debug.Log("A zombie has entered the trigger zone.");

                    if (maxHealthBuffer)
                    {
                        FellowZombie.AddMaxHp(HealthBuff);
                    }

                    if (damageBUffer)
                    {
                      
                        FellowZombie.AddDamage(AttackDamageBuff);
                    }

                    if (AttackSpeedBuffer)
                    {
                        FellowZombie.AddAttackSpeed(AttackSpeedBuff);
                    }

                    if (ArmorBUffer)
                    {
                        FellowZombie.AddArmor(ArmorBuff);
                    }


                    if (Healer)
                    {
                        FellowZombie.AddHP(Healing);
                    }

                   
                }
            }
        }
    }

    protected override void Die()
    {
        // Additional special-specific death logic (if any)
        base.Die();
    }





}
