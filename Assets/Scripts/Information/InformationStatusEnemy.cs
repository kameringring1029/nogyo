using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 敵ユニットのステータス定義
 * 継承元はInformationStatusParent.cs
 */

namespace Information
{

    public class Enemy1_Smile : statusTable
    {
        public override string graphic_id() { return "Enemy1-Smile"; }
        public override string job_id() { return "Fighter"; }
        public override string name() { return "てき"; }
        public override string job() { return "とり"; }
        public override string subname() { return "スマイル"; }
        public override int level() { return 1; }
        public override int movable() { return 4; }
        public override int reach() { return 1; }
        public override int hp() { return 17; }
        public override int attack_phy() { return 17; }
        public override int guard_phy() { return 7; }
        public override int attack_magic() { return 7; }
        public override int guard_magic() { return 7; }
        public override int agility() { return 10; }
        public override int luck() { return 1; }

    }

    public class Enemy1_Cool : statusTable
    {
        public override string graphic_id() { return "Enemy1-Cool"; }
        public override string job_id() { return "Sage"; }
        public override string name() { return "てき"; }
        public override string job() { return "とり"; }
        public override string subname() { return "クール"; }
        public override int level() { return 1; }
        public override int movable() { return 3; }
        public override int reach() { return 2; }
        public override int hp() { return 17; }
        public override int attack_phy() { return 7; }
        public override int guard_phy() { return 7; }
        public override int attack_magic() { return 17; }
        public override int guard_magic() { return 7; }
        public override int agility() { return 10; }
        public override int luck() { return 7; }

    }


    public class Enemy2_Smile : statusTable
    {
        public override string graphic_id() { return "Enemy2-Smile"; }
        public override string job_id() { return "Fighter"; }
        public override string name() { return "てき"; }
        public override string job() { return "いぬ"; }
        public override string subname() { return "スマイル"; }
        public override int level() { return 1; }
        public override int movable() { return 4; }
        public override int reach() { return 1; }
        public override int hp() { return 17; }
        public override int attack_phy() { return 17; }
        public override int guard_phy() { return 7; }
        public override int attack_magic() { return 7; }
        public override int guard_magic() { return 7; }
        public override int agility() { return 10; }
        public override int luck() { return 1; }

    }

    public class Enemy2_Cool : statusTable
    {
        public override string graphic_id() { return "Enemy2-Cool"; }
        public override string job_id() { return "Sage"; }
        public override string name() { return "てき"; }
        public override string job() { return "いぬ"; }
        public override string subname() { return "クール"; }
        public override int level() { return 1; }
        public override int movable() { return 4; }
        public override int reach() { return 1; }
        public override int hp() { return 17; }
        public override int attack_phy() { return 7; }
        public override int guard_phy() { return 3; }
        public override int attack_magic() { return 4; }
        public override int guard_magic() { return 2; }
        public override int agility() { return 10; }
        public override int luck() { return 1; }

        public override int member_number(){ return 6; }
    }


}