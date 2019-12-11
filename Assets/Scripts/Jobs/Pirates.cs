using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;


public class Pirates : Unit {

    public GameObject explosionPrefab;
    private Vector2 targetPosition;

    public override void targetAttack(GameObject targetUnit)
    {
        int damage = getAttackDamage(targetUnit);
        int rand = getRandomFromMapstate(targetUnit);
        if (rand < getAttackCritical(targetUnit)) damage += 10; //critical
        rand = getRandomFromMapstate(targetUnit) + damage % 10;
        if (rand > getAttackHit(targetUnit)) damage = -1; //miss


        targetUnit.GetComponent<Unit>().beDamaged(damage, gameObject);

        Instantiate(explosionPrefab, targetUnit.transform.position, transform.rotation);


        int spritevector = (targetUnit.transform.position.x > transform.position.x) ? 1 : -1;
        changeSpriteFlip(spritevector);
        deleteReachArea();

        gameObject.GetComponent<Animator>().SetBool("isAttacking", true);
    }


    private void instantiateAttackEffect()
    {
        Instantiate(explosionPrefab, targetPosition, transform.rotation);
    }
}
