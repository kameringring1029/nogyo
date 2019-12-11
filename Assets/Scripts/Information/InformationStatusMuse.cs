
using General;

/*
 * μ`sユニットのステータス定義
 * 継承元はInformationStatusParent.cs
 */

namespace Information
{

    public class Kotori_KY : statusTable
    {
        public override string graphic_id() { return "Kotori-KY"; }
        public override string job_id() { return "Sage"; }
        public override string name() { return "コトリ"; }
        public override string job() { return "賢者"; }
        public override string subname() { return "トワイライトタイガー"; }
        public override int level() { return 28; }
        public override int movable() { return 3; }
        public override int reach() { return 2; }
        public override int hp() { return 18; }
        public override int attack_phy() { return 5; }
        public override int guard_phy() { return 8; }
        public override int attack_magic() { return 24; }
        public override int guard_magic() { return 28; }
        public override int agility() { return 18; }
        public override int luck() { return 15; }

    }



    public class Eli_DS : statusTable
    {
        public override string graphic_id() { return "Eli-DS"; }
        public override string job_id() { return "Pirates"; }
        public override string name() { return "エリ"; }
        public override string job() { return "海賊"; }
        public override string subname() { return "Dancing Stars on Me"; }
        public override int level() { return 23; }
        public override int movable() { return 6; }
        public override int reach() { return 1; }
        public override int hp() { return 32; }
        public override int attack_phy() { return 15; }
        public override int guard_phy() { return 8; }
        public override int attack_magic() { return 8; }
        public override int guard_magic() { return 7; }
        public override int agility() { return 8; }
        public override int luck() { return 12; }


        public override MOVETYPE movetype() { return MOVETYPE.SWIM; }


        public override int group_number() { return 0; }
        public override int unitingroup_number() { return 3; }
        public override int member_number() { return 2; }
    }


    public class Umi_DG : statusTable
    {
        public override string graphic_id() { return "Umi-DG"; }
        public override string job_id() { return "Archer"; }
        public override string name() { return "ウミ"; }
        public override string job() { return "アーチャー"; }
        public override string subname() { return "道着"; }
        public override int level() { return 22; }
        public override int movable() { return 5; }
        public override int reach() { return 2; }
        public override int hp() { return 28; }
        public override int attack_phy() { return 19; }
        public override int guard_phy() { return 5; }
        public override int attack_magic() { return 7; }
        public override int guard_magic() { return 6; }
        public override int agility() { return 6; }
        public override int luck() { return 3; }

        public override int group_number() { return 0; }
        public override int unitingroup_number() { return 2; }
        public override int member_number() { return 4; }

    }


    public class Rin_HN : statusTable
    {
        public override string graphic_id() { return "Rin-HN"; }
        public override string job_id() { return "Pirates"; }
        public override string name() { return "リン"; }
        public override string job() { return "ニンジャ"; }
        public override string subname() { return "星空忍法"; }
        public override int level() { return 21; }
        public override int movable() { return 7; }
        public override int reach() { return 2; }
        public override int hp() { return 34; }
        public override int attack_phy() { return 9; }
        public override int guard_phy() { return 6; }
        public override int attack_magic() { return 4; }
        public override int guard_magic() { return 4; }
        public override int agility() { return 15; }
        public override int luck() { return 14; }

        public override int group_number() { return 0; }
        public override int unitingroup_number() { return 2; }
        public override int member_number() { return 5; }
    }

    public class Rin_LB : statusTable
    {
        public override string graphic_id() { return "Rin-LB"; }
        public override string job_id() { return "Sage"; }
        public override string name() { return "リン"; }
        public override string job() { return "？"; }
        public override string subname() { return "Love wing bell"; }
        public override int level() { return 1; }
        public override int movable() { return 4; }
        public override int reach() { return 5; }
        public override int hp() { return 4; }
        public override int attack_phy() { return 0; }
        public override int guard_phy() { return 3; }
        public override int attack_magic() { return 26; }
        public override int guard_magic() { return 8; }
        public override int agility() { return 9; }
        public override int luck() { return 50; }

    }

    public class Hanayo_LB : statusTable
    {
        public override string graphic_id() { return "Hanayo-LB"; }
        public override string job_id() { return "Singer"; }
        public override string name() { return "ハナヨ"; }
        public override string job() { return "？"; }
        public override string subname() { return "Love wing bell"; }
        public override int level() { return 21; }
        public override int movable() { return 4; }
        public override int reach() { return 1; }
        public override int hp() { return 22; }
        public override int attack_phy() { return 1; }
        public override int guard_phy() { return 7; }
        public override int attack_magic() { return 12; }
        public override int guard_magic() { return 9; }
        public override int agility() { return 4; }
        public override int luck() { return 50; }

        public override int group_number() { return 0; }
        public override int unitingroup_number() { return 1; }
        public override int member_number() { return 8; }
    }


}

