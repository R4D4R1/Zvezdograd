using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using UniRx;

public class TerrainTextureSwitcher : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    private const int REGULAR_TERRAIN_TEXTURE_INDEX = 0;
    private const int SNOW_TERRAIN_TEXTURE_INDEX = 4;

    [Inject] private EventController _eventController;

    private void Start()
    {
        _eventController.OnSnowStarted
            .Subscribe(_ => SetTerrainSnowing(true))
            .AddTo(this);

        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.OnSnowChangeState
            .Subscribe(SetTerrainSnowing)
            .AddTo(this);
    }

    /// <summary>
    /// Switches terrain texture between snow and regular based on the weather state.
    /// </summary>
    /// <param name="isSnow">True to switch to snow, false to revert to regular terrain.</param>
    private void SetTerrainSnowing(bool isSnow)
    {
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;
        int width = data.alphamapWidth;
        int height = data.alphamapHeight;

        float[,,] alphas = data.GetAlphamaps(0, 0, width, height);

        int fromIndex = isSnow ? REGULAR_TERRAIN_TEXTURE_INDEX : SNOW_TERRAIN_TEXTURE_INDEX;
        int toIndex = isSnow ? SNOW_TERRAIN_TEXTURE_INDEX : REGULAR_TERRAIN_TEXTURE_INDEX;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float oldValue = alphas[y, x, fromIndex];
                alphas[y, x, toIndex] += oldValue;
                alphas[y, x, fromIndex] = 0f;
            }
        }

        data.SetAlphamaps(0, 0, alphas);
    }
}
