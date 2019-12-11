using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;


/*
 * ユニットのステータス定義のスーパークラス
 */

namespace Information
{

    public class UnitStatus {

        public string graphic_id = "";

        public string name = "";
        public string job = "";
        public string subname = "";

        public int group_number = 0;
        public int unitingroup_number = 0;
        public int member_number = 0;
        public Color color;

        // ステータスは{基のステ, 修正後のステ}
        public int[] level = new int[2];
        public int[] reach = new int[2];
        public int[] hp = new int[2];
        public int[] attack_phy = new int[2];
        public int[] guard_phy = new int[2];
        public int[] attack_magic = new int[2];
        public int[] guard_magic = new int[2];
        public int[] agility = new int[2];
        public int[] luck = new int[2];


        public int[] movable = new int[2];
        public MOVETYPE movetype;


        public UnitStatus(statusTable status)
        {
            this.graphic_id = status.graphic_id();

            this.name = status.name();
            this.job = status.job();
            this.subname = status.subname();

            this.group_number = status.group_number();
            this.unitingroup_number = status.unitingroup_number();
            this.member_number = status.member_number();
            this.color = status.color();

            // ステータスは{基のステ, 修正後のステ}
            for (int i = 0; i < 2; i++)
            {
                this.level[i] = status.level();
                this.reach[i] = status.reach();
                this.hp[i] = status.hp();
                this.attack_phy[i] = status.attack_phy();
                this.guard_phy[i] = status.guard_phy();
                this.attack_magic[i] = status.attack_magic();
                this.guard_magic[i] = status.guard_magic();
                this.agility[i] = status.agility();
                this.luck[i] = status.luck();

                this.movable[i] = status.movable();
            }

            movetype = status.movetype();
        }

        // ゲーム中のユニットの現在の情報を出力
        // ステータスは{基のステ, 修正後のステ}
        public string outputInfo()
        {

            string hpF = string.Format("<color=yellow>HP</color> {0,3}", hp[1], hp[0]);
            string movableF = string.Format("<color=yellow>MOV</color>{0,3}", movable[1]);
            string reachF = string.Format("<color=yellow>RNG</color>{0,3}", reach[1]);
            string attack_phyF = string.Format("<color=yellow>ATK</color>{0,3}", attack_phy[1]);
            string guard_phyF = string.Format("<color=yellow>DEF</color>{0,3}", guard_phy[1]);
            string attack_magicF = string.Format("<color=yellow>MAT</color>{0,3}", attack_magic[1]);
            string guard_magicF = string.Format("<color=yellow>MDF</color>{0,3}", guard_magic[1]);
            string agilityF = string.Format("<color=yellow>AGL</color>{0,3}", agility[1]);
            string luckF = string.Format("<color=yellow>LCK</color>{0,3}", luck[1]);

            string outinfo =
                "" + name + "" + " <size=10>(" + subname + ")" + "\n" +
                " 【" + job + "  Lv: " + level[1] + "】</size>\n\n" +
                "" +
                hpF + "\n"+
               movableF + "  " + reachF + "\n" +
               attack_phyF + "  " + guard_phyF + "  " + agilityF + "\n" +
               attack_magicF + "  " + guard_magicF + "  " + luckF +
               "";
//            " <sprite=\"Aqours\" index=0> " + " <sprite=\"CYR\" index=0>";

            return outinfo;
        }
    }


    public class statusTable {
        public virtual string graphic_id() { return ""; }
        public virtual string job_id() { return ""; }
        public virtual string name() { return ""; }
        public virtual string job() { return ""; }
        public virtual string subname() { return ""; }
        public virtual int level() { return 0; }
        public virtual int movable() { return 0; }
        public virtual int reach() { return 0; }
        public virtual int hp() { return 0; }
        public virtual int attack_phy() { return 0; }
        public virtual int guard_phy() { return 0; }
        public virtual int attack_magic() { return 0; }
        public virtual int guard_magic() { return 0; }
        public virtual int agility() { return 0; }
        public virtual int luck() { return 0; }
        public virtual MOVETYPE movetype() { return MOVETYPE.WALK; }

        public virtual string description() { return ""; }
        public virtual string status_description() { return ""; }
        public virtual Color color() { return Color.black; }
        public virtual int group_number() { return 0; }
        public virtual int unitingroup_number() { return 0; }
        public virtual int member_number() { return 0; }

    }




}
