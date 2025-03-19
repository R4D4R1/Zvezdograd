using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialsBuiliding : BuildingDependingOnStability
{
    [Header("MATERIAL FOR INACTIVE BUILDING")]
    [SerializeField] private Material _M_Grey;

    private List<Material[]> _originalMaterials = new List<Material[]>();

    public override void Init()
    {
        base.Init();

        SaveOriginalMaterials();
    }

    protected void SaveOriginalMaterials()
    {
        _originalMaterials.Clear();
        var renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            _originalMaterials.Add(renderer.materials);
        }
    }

    protected void SetGreyMaterials()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            var greyMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                greyMaterials[i] = _M_Grey;
            }
            renderer.materials = greyMaterials;
        }
    }

    protected void RestoreOriginalMaterials()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if (i < _originalMaterials.Count)
            {
                renderers[i].materials = _originalMaterials[i];
            }
        }
    }
}
