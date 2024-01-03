using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static int g_MaxHp;
    public static int g_CurHp;
    public static int g_MaxSp;
    public static int g_CurSp;
    public static int g_CurGold;
    public static int g_Stage;

    public static Dictionary<int, int> ownCards = new Dictionary<int, int>();
    public static List<int> ownItems = new List<int>();
}
