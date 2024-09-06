using UnityEngine;

public class SpecialEnemy : EnemyAI
{

    public float specialAbilityCooldown = 5f;
    private float nextAbilityTime = 5f;
    [SerializeField] float BuffRange = 30;
    [SerializeField] bool HealthBuffer;
    [SerializeField] bool SpeedBuffer;
    [SerializeField] bool damageBUffer;
    [SerializeField] bool SpecailCaster;

    [Header("-----Ability Stats-----")]
    [SerializeField] int DamageBuff;
    [SerializeField] int SpeedBuff;
    [SerializeField] int HealthBuff;
   


    [Header("-----Projectile Type-----")]
    [SerializeField] GameObject NoramalProjectilePrefab;
    [SerializeField] GameObject SpecialProjectile;
    [SerializeField] GameObject BossProjectilePrefab;


    [SerializeField] AudioClip ZombieBuff;
    [SerializeField, Range(0f, 1f)] float ZombieBuffVol;

    protected override void Update()
    {
        base.Update();

        if (Time.time >= nextAbilityTime)
        {
            UseSpecialAbility();
            PlayAudio(ZombieBuff, ZombieBuffVol);
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

                    if (HealthBuffer)
                    {
                        int totalHealthBuff = HealthBuff + round;
                        FellowZombie.AddHP(totalHealthBuff);
                    }

                    if (damageBUffer)
                    {
                        int totalDamageBuff = DamageBuff * gameManager.gameInstance.GetGameRound();
                        FellowZombie.AddDamage(totalDamageBuff);
                    }

                    if (SpeedBuffer)
                    {
                        int totalSpeedBuff = SpeedBuff * gameManager.gameInstance.GetGameRound();
                        FellowZombie.AddSpeed(totalSpeedBuff);
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
