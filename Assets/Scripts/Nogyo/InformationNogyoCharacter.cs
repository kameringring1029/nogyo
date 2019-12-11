
/*
 * Nogyo情報の定義
 */

using UnityEngine;

namespace Information
{
    // 実装クラス軍
    public class Ako : NogyoProfile
    {
        public override string id() { return "A"; }
        public override string name() { return "スズリ"; }
        public override string description() { return "しゅじんこーdayo"; }
        public override Color color() { return Color.green; }

    }
    public class Bko : NogyoProfile
    {
        public override string id() { return "B"; }
        public override string name() { return "アル"; }
        public override string description() { return "B子dayo"; }
        public override Color color() { return Color.blue; }

    }

    // 親クラス
    public class NogyoProfile
    {
        public virtual string id() { return ""; }
        public virtual string name() { return ""; }

        public virtual string description() { return ""; }
        public virtual Color color() { return Color.black; }

    }

    // ユーティリティ
    public class NogyoProfileUtil
    {
        public static NogyoProfile getProfile(int id)
        {
            switch (id)
            {
                case 0:
                    return new Ako();
                case 1:
                    return new Bko();
                default:
                    return new Ako();
            }
        }
    }

}