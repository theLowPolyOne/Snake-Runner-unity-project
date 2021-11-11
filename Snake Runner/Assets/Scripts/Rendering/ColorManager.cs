using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private List <GameObject> targets;
    [SerializeField] private ColorList colorList;
    private Material colorMaterial => colorList.Materials.Find(x => x.name == CurrentMaterial);
    public static List<string> TMPList;
    [HideInInspector] public List<string> PopupList;
    [ListToPopup(typeof(ColorManager), "TMPList")] public string CurrentMaterial;

    private void Awake()
    {
        SetColor();
    }

    private void OnValidate()
    {
        SetColor();
    }

    public void SetColor()
    {
        List<IRecolourible> renderers = GetAllRenderers();
        if (colorMaterial != null)
        {
            foreach (IRecolourible renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }
                renderer.SetColorMaterial(colorMaterial);
            }
        }
    }

    public List<IRecolourible> GetAllRenderers()
    {
        List<IRecolourible> renderers = new List<IRecolourible>();

        for (int i = 0; i < targets.Count; i++)
        {
            IRecolourible renderer = targets[i].GetComponent<IRecolourible>();
            renderers.Add(renderer);
        }

        return renderers;
    }

    public List<string> GetMaterialsNames()
    {
        List<string> AllMaterials = new List<string>();
        
        for (int i = 0; i < colorList.Materials.Count; i++)
        {
            string MaterialName = colorList.Materials[i].name;
            AllMaterials.Add(MaterialName);
        }

        return AllMaterials;
    }

    public void OnBeforeSerialize()
    {
        PopupList = GetMaterialsNames();
        TMPList = PopupList;
    }

    public void OnAfterDeserialize() { }
}
