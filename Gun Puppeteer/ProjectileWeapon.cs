using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float recoilForce = 10, bulletFireDirRot = 0;
    Projectile projectile;
    protected override void Start()
    {
        base.Start();

    }
    protected override void FireWeapon()
    {
        GameObject projectileGameObject = Instantiate(projectilePrefab, barrelEndTransform.position, Quaternion.identity);
        projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.projectileDirection = Quaternion.Euler(0, 0, bulletFireDirRot) * transform.right;
        projectile.Damage = damage;

        // Muzzle Flash
        Instantiate(muzzleFlashVisualPrefab, muzzleFlashTransform.position, Quaternion.identity);

        Recoil();
    }

    private void Recoil()
    {
        if (holderTransform != null)
             holderTransform.GetComponent<Rigidbody2D>().AddForce(-transform.right * recoilForce, ForceMode2D.Impulse);
    }
}
