using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class ChangeMaterialsBuilding : BuildingDependingOnStability
{
    [FormerlySerializedAs("_M_Grey")]
    [Header("MATERIAL FOR INACTIVE BUILDING")]
    [SerializeField] private Material mGrey;

    private readonly List<Material[]> _originalMaterials = new();
        
    public override void Init()
    {
        base.Init();

        SaveOriginalMaterials();
    }

    private void SaveOriginalMaterials()
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
            for (var i = 0; i < renderer.materials.Length; i++)
            {
                greyMaterials[i] = mGrey;
            }
            renderer.materials = greyMaterials;
        }
    }

    protected void RestoreOriginalMaterials()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();

        for (var i = 0; i < renderers.Length; i++)
        {
            if (i < _originalMaterials.Count)
            {
                renderers[i].materials = _originalMaterials[i];
            }
        }
    }
    
    protected string[] GetCurrentMaterialPaths()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        List<string> names = new();

        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                names.Add(mat.name.Replace(" (Instance)", ""));
            }
        }
        return names.ToArray();
    }

    protected void SetMaterialsFromPaths(string[] materialNames)
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        int index = 0;

        foreach (var renderer in renderers)
        {
            Material[] mats = new Material[renderer.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                if (index < materialNames.Length)
                {
                    var matName = materialNames[index++];
                    mats[i] = Resources.Load<Material>($"Materials/{matName}");
                }
            }
            renderer.materials = mats;
        }
    }
}
