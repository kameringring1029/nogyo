using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;

public class Sage : Unit {

    public GameObject explosionPrefab;

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

    public override int getAttackDamage(GameObject targetUnit)
    {
        int damage = unitInfo.attack_magic[1]
        - targetUnit.GetComponent<Unit>().unitInfo.guard_magic[1];
        if(damage<0) damage  =0;
        return damage;
    }
}
