using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Color List", menuName = "Scriptable Objecs/Color List")]
public class ColorList : ScriptableObject
{
    public List<Material> Materials;
}
