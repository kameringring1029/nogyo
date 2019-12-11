
using General;


/*
 * Map上のBlock情報の定義
 */

namespace Information
{

    public class BlockInfo
    {
        public virtual string type() { return ""; }
        public virtual string effect() { return "なし"; }
        public virtual GROUNDTYPE groundtype() { return GROUNDTYPE.NORMAL; }
    }



    public class Kusa : BlockInfo
    {
        public override string type() { return "通常"; }
        public override string effect() { return "なし"; }
        public override GROUNDTYPE groundtype() { return GROUNDTYPE.NORMAL; }

    }

    public class Sea : BlockInfo
    {
        public override string type() { return "海"; }
        public override string effect() { return "泳げます"; }
        public override GROUNDTYPE groundtype() { return GROUNDTYPE.SEA; }

    }

    public class High : BlockInfo
    {
        public override string type() { return "段差"; }
        public override string effect() { return "高低差あり"; }
        public override GROUNDTYPE groundtype() { return GROUNDTYPE.HIGH; }

    }

    public class Unmovable : BlockInfo
    {
        public override string type() { return "障害物"; }
        public override string effect() { return "通り抜けできません"; }
        public override GROUNDTYPE groundtype() { return GROUNDTYPE.UNMOVABLE; }

    }
}
