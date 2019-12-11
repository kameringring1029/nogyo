
/*
 * 収穫物とかのステータス用クラス
 */
[System.Serializable]
public class NogyoItemStatus
{
    public enum STSVALUE { SIZE, SWEET, BITTER, SOUR, SALTY, UMAMI}

    public int size;
    public int sweet;
    public int bitter;
    public int sour;
    public int salty;
    public int umami;

    public NogyoItemStatus(int sz = 1, int sw =0, int bi =0, int so =0, int sa =0, int um =0)
    {
        size = sz;
        sweet = sw;
        bitter = bi;
        sour = so;
        salty = sa;
        umami = um;
    }


    /*
     * int値から値取得
     */
    public int getValueByNum(STSVALUE num)
    {
        switch (num)
        {
            case STSVALUE.SIZE:
                return size;
            case STSVALUE.SWEET:
                return sweet;
            case STSVALUE.BITTER:
                return sour;
            case STSVALUE.SALTY:
                return salty;
            case STSVALUE.UMAMI:
                return umami;
        }

        return size;
    }
    public int getValueByNum(int num)
    {
        switch (num)
        {
            case (int)STSVALUE.SIZE:
                return size;
            case (int)STSVALUE.SWEET:
                return sweet;
            case (int)STSVALUE.BITTER:
                return sour;
            case (int)STSVALUE.SALTY:
                return salty;
            case (int)STSVALUE.UMAMI:
                return umami;
        }

        return size;
    }
}
