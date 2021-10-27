using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    Camera camera ;
    [SerializeField] Animator weaponAnimator;
    [SerializeField] Weapon currentWeapon;
    [Header("Shotgun stats")]
    [SerializeField] Vector3[] raycastOffsets;
    [SerializeField] float shotgunDamage;
    [SerializeField] float shotgunRange;
    [SerializeField] float shotgunForce;
    [SerializeField] float shotgunFireRate = 1f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject shotgunMuzzleFlash;
    [SerializeField] Transform shotgunMuzzleFlashTransform;
    [Header("Rocket launcher stats")]
    [SerializeField] float rocketlauncherFireRate = 1f;
    public float maxDamage;
    public float maxExplosionForce;
    public float explosionRadius;
    public float rocketSpeed = 10;
    [Range(0, 1)] public float rocketSelfDamageMultiplier = 0.3f;
    [SerializeField] GameObject rocketProjectile;
    [SerializeField] Transform rocketSpawnTransform;

    [SerializeField] GameObject rocketlauncherMesh;
    [SerializeField] GameObject[] shotgunMesh;
    float shootTimer;

    PlayerSFX playerSFX;
    public enum Weapon
    {
        shotgun, rocketlauncher
    }
    private void Awake()
    {
        camera = Camera.main;
        SetWeaponMeshActive();
        playerSFX = GetComponent<PlayerSFX>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleWeapon(bool toggleWeapon)
    {
        if (toggleWeapon)
        {
            if (currentWeapon == Weapon.rocketlauncher) currentWeapon = Weapon.shotgun;
            else currentWeapon = Weapon.rocketlauncher;
            SetWeaponMeshActive();
        }
    }

    private void SetWeaponMeshActive()
    {
        for (int i = 0; i < shotgunMesh.Length; i++)
        {
            shotgunMesh[i].SetActive(currentWeapon == Weapon.shotgun);
        }
        rocketlauncherMesh.SetActive(currentWeapon == Weapon.rocketlauncher);

    }




    public void Shoot(bool shootInput, float deltaTime)
    {
        shootTimer += deltaTime;
        if (shootInput)
        {

            switch (currentWeapon)
            {
                case Weapon.shotgun:
                    Shotgun();
                    break;
                case Weapon.rocketlauncher:
                    Rocketlauncher();
                    break;
            }
        }

    }

    private void Shotgun()
    {
        if (shootTimer >= shotgunFireRate)
        {
            GameObject newMuzzleFlash = Instantiate(shotgunMuzzleFlash, shotgunMuzzleFlashTransform);
            Destroy(newMuzzleFlash, 0.5f);
            playerSFX.FireShotgunSFX();
            weaponAnimator.SetTrigger("Shoot");
            shootTimer = 0;
            RaycastHit rayHit;


            for (int i = 0; i < raycastOffsets.Length; i++)
            {
                if (Physics.Raycast(camera.transform.position + raycastOffsets[i], camera.transform.forward, out rayHit, shotgunRange, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (rayHit.collider.gameObject != null)
                    {
                        if (rayHit.collider.gameObject.GetComponent<IHealth>() != null)
                        {
                            rayHit.collider.gameObject.GetComponent<IHealth>().Damage(shotgunDamage, (rayHit.point - camera.transform.position) * shotgunForce, rayHit.point);
                            break;
                        }
                        Debug.DrawLine(camera.transform.position + raycastOffsets[i], rayHit.point, Color.green, 3);
                        Debug.Log("Ray hit " + rayHit.collider.gameObject);
                    }
                }
            }
        }
    }

    private void Rocketlauncher()
    {
        if (shootTimer >= rocketlauncherFireRate)
        {
            playerSFX.FireRocketLauncherSFX();
            weaponAnimator.SetTrigger("Shoot");
            shootTimer = 0;
            GameObject newProjectile = Instantiate(rocketProjectile, rocketSpawnTransform.position, rocketSpawnTransform.rotation) as GameObject;
        }
        Debug.Log("ROCKET ROCKET MUAHAHAHAHAHHA");
    }
}





