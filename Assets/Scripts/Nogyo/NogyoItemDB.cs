using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * アイテムのデータベース
 * シングルトンにしました
 */

public sealed class NogyoItemDB{

    // 唯一のインスタンス
    private static NogyoItemDB instance = new NogyoItemDB(); 

   public Dictionary<string, NogyoItem> db { get; }

    // インスタンスのゲッタ
    public static NogyoItemDB getinstance()
    {
        return instance;
    }
    
    // コンストラクタ
    private NogyoItemDB()
    {
        db = new Dictionary<string, NogyoItem>();

        db.Add("Seed_GMary", 
            new NogyoItem("Seed_GMary", "ゴールドマリーのたね","日当たりさえよければとっても育てやすい。",NogyoItem.NogyoItemGroup.Seed, 50, -1, new NogyoItemStatus(1,0,1,0,2,0), Produce.PRODUCE_TYPE.GMary));
        db.Add("Seed_WClover", 
            new NogyoItem("Seed_WClover", "シログメクサのたね", "シャジクソウ属の多年草。原産地はヨーロッパらしい。わりとどこでも育つ。", NogyoItem.NogyoItemGroup.Seed, 50, -1, new NogyoItemStatus(1, 0,1,0,2,0), Produce.PRODUCE_TYPE.WClover));
        db.Add("Seed_Carrot",
            new NogyoItem("Seed_Carrot", "にんよんのたね", "アフガニスタン原産とのこと。深めの土がいいんだけどベランダだとあんまり盛れないのが残念。", NogyoItem.NogyoItemGroup.Seed, 100, -1, new NogyoItemStatus(1, 1, 1, 3, 2, 3), Produce.PRODUCE_TYPE.Carrot));
        db.Add("Harv_GMary", 
            new NogyoItem("Harv_GMary", "ゴールドマリー","力強く鮮やかな花。今となっては花を愛でる余裕の無い人か、花を愛でる余裕しか無い人しかいない。麦わら帽子。『悲しみ』『変わらぬ愛』",NogyoItem.NogyoItemGroup.Flower, 300, -1, new NogyoItemStatus(1, 1,2,1,3,1), Produce.PRODUCE_TYPE.GMary));
        db.Add("Harv_WClover", 
            new NogyoItem("Harv_WClover", "シログメクサ","いろんな雑草で友だちと冠作るのが好きだった。あの子もいなくなっちゃったなぁ。『幸運』『約束』",NogyoItem.NogyoItemGroup.Flower, 300, -1, new NogyoItemStatus(1, 3,1,1,1,2), Produce.PRODUCE_TYPE.WClover));
        db.Add("Harv_Carrot",
            new NogyoItem("Harv_Carrot", "にんよん", "子どもの頃苦手だったけど親にも言えなかった。今は煮物がおいしい。『幼い夢』", NogyoItem.NogyoItemGroup.Vegi, 500, -1, new NogyoItemStatus(1, 1, 1, 3, 2, 3), Produce.PRODUCE_TYPE.Carrot));
        db.Add("Harv_Mikan",
            new NogyoItem("Harv_Mikan", "ニカン", "常緑低木に実るやつ。鉢植えでも育つしムシも付きづらいしで意外と手軽なんだよね。皮も色々使えてべんりだよ。かんかん。『純潔』『親愛』", NogyoItem.NogyoItemGroup.Fruit, 500, -1, new NogyoItemStatus(2, 5, 1, 4, 1, 2), Produce.PRODUCE_TYPE.Mikan));
        db.Add("Harv_SMuscat",
            new NogyoItem("Harv_SMuscat", "シャイニームスカット", "ドリルっぽいよね。ジベレリン交配のたまもの。種があるVerが好きな変...もとい嗜好家も多いとか。『陶酔』『忘却』", NogyoItem.NogyoItemGroup.Fruit, 500, -1, new NogyoItemStatus(2, 4, 1, 3, 1, 2), Produce.PRODUCE_TYPE.SMuscat));
        db.Add("Harv_SBerry",
            new NogyoItem("Harv_SBerry", "ストローベリー", "かわいい代表。ただルルには敵わないよね。『愛情』『完全なる善』", NogyoItem.NogyoItemGroup.Fruit, 500, -1, new NogyoItemStatus(1, 3, 1, 5, 1, 2), Produce.PRODUCE_TYPE.SBerry));
        db.Add("Harv_Apple",
            new NogyoItem("Harv_Apple", "ナップル", "アルも大好き、一世を風靡した某社ロゴデザインのやつ。『誘惑』『後悔』", NogyoItem.NogyoItemGroup.Fruit, 500, -1, new NogyoItemStatus(2, 3, 1, 4, 1, 2), Produce.PRODUCE_TYPE.Apple));
        db.Add("Harv_Negi",
            new NogyoItem("Harv_Negi", "おねぎ", "背負われたり振り回されたり。どうしてバーチャルアイドルのほうが人気になっちゃったんだ。度し難い。『愛嬌』『笑顔』", NogyoItem.NogyoItemGroup.Vegi, 500, -1, new NogyoItemStatus(3, 1, 2, 3, 2, 2), Produce.PRODUCE_TYPE.Negi));
        db.Add("Harv_Grape",
            new NogyoItem("Harv_Grape", "ブドー", "ドリルっぽいよね。わたしが生まれる前くらいまではこっちの種ありブドーが主流だったみたい。『陶酔』『忘却』", NogyoItem.NogyoItemGroup.Fruit, 3000, 10000, new NogyoItemStatus(2, 4, 1, 3, 1, 3), Produce.PRODUCE_TYPE.Grape));
        db.Add("Water_Normal",
            new NogyoItem("Water_Normal", "お水", "どこの水でしょう", NogyoItem.NogyoItemGroup.Water, 100, -1, new NogyoItemStatus(1, 0,0,0,0,0)));
        db.Add("Chemi_Normal",
            new NogyoItem("Chemi_Normal", "特製肥料", "わたし特製シンプルなケミカル肥料。", NogyoItem.NogyoItemGroup.Chemi, 100, -1, new NogyoItemStatus(1, 1,1,1,1,1)));

    }

    /*
     * ProduceTypeからアイテム情報を取得
     */
    public NogyoItem getItemFromPType(NogyoItem.NogyoItemGroup group, Produce.PRODUCE_TYPE ptype)
    {
        foreach(KeyValuePair<string, NogyoItem> pair in db)
        {
            if (pair.Value.group == group && pair.Value.producetype == ptype)
                return db[pair.Key];
        }

        return null;
    }
    
    //
    public static NogyoItem.NogyoItemGroup getGroupFromType(Produce.PRODUCE_TYPE type)
    {
        switch (type)
        {
            case Produce.PRODUCE_TYPE.GMary:
                return NogyoItem.NogyoItemGroup.Flower;
            case Produce.PRODUCE_TYPE.WClover:
                return NogyoItem.NogyoItemGroup.Flower;
            case Produce.PRODUCE_TYPE.Carrot:
                return NogyoItem.NogyoItemGroup.Vegi;

            default:
                return NogyoItem.NogyoItemGroup.Null;

        }

    }

    /* idからitemのインスタンスを取得 */
    public NogyoItem getItemById(string id)
    {
        if (db.ContainsKey(id)) return db[id];

        return null;
    }


    /*
    public NogyoItem.NogyoItemGroup getGroupFromProdState(Produce prod)
    {
        switch (prod.status)
        {
            case Produce.PRODUCE_STATE.Harvest:
                if()
                return NogyoItem.NogyoItemGroup.Harvest;

            default:
                return NogyoItem.NogyoItemGroup.Seed;
        }
    }
    */
}
