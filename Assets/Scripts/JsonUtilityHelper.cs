using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class JsonUtilityHelper{
    
    public static T[] MapFromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.maps;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] maps;
    }

}
