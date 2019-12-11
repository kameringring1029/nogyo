using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;


public class Singer : Unit {

    public GameObject explosionPrefab;

    public override void targetAttack(GameObject targetUnit)
    {
        int damage = getAttackDamage(targetUnit);
        int rand = getRandomFromMapstate(targetUnit);
        if (rand < getAttackCritical(targetUnit)) damage += 10; //critical
        rand = getRandomFromMapstate(targetUnit) + damage % 10;
        if (rand > getAttackHit(targetUnit)) damage = -1; //miss

        int spritevector = (targetUnit.transform.position.x > transform.position.x) ? 1 : -1;
        changeSpriteFlip(spritevector);


        Instantiate(explosionPrefab, targetUnit.transform.position, transform.rotation);
        gameObject.GetComponent<Animator>().SetBool("isAttacking", true);

        deleteReachArea();
    }


    public override void targetReaction(GameObject targetUnit)
    {

        int damage = 0;
        targetUnit.GetComponent<Unit>().beDamaged(damage, gameObject);
        targetUnit.GetComponent<Unit>().restoreActionRight();

        int spritevector = (targetUnit.transform.position.x > transform.position.x) ? 1 : -1;
        changeSpriteFlip(spritevector);
        
        gameObject.GetComponent<Animator>().SetBool("isAttacking", true);

        deleteReachArea();
    }


    public override List<ACTION> getActionableList()
    {
        List<ACTION> actionlist = new List<ACTION>();
        actionlist.Add(ACTION.ATTACK);
        actionlist.Add(ACTION.REACTION);
        actionlist.Add(ACTION.WAIT);

        return actionlist;
    }
}
