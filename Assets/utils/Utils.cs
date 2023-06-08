using System;
using UnityEngine;

public static class Utils
{
    public static GameObject GenerateGameObject(string namePrefix)
    {
        GameObject toCreate = new();
        Guid guid = Guid.NewGuid();
        toCreate.name = $"{namePrefix}-" + guid.ToString();
        return toCreate;
    }
}