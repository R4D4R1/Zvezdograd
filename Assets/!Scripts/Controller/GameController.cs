using UnityEngine;

public class GameController : MonoBehaviour
{
    [field: Range(0f, 1f)]
    [field: SerializeField] public float GameStartDelay { get; private set; }

    [field: Range(0f, 1f)]
    [field: SerializeField] public float GameAfterLoadDelay { get; private set; }

}
