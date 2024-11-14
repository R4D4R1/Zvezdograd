using UnityEngine;
using UnityEngine.UI;

public class TownController : MonoBehaviour
{
    [SerializeField] private Slider _foodSlider;
    [SerializeField] private Slider _medSlider;
    [SerializeField] private Slider _rawMaterialSlider;
    [SerializeField] private Slider _readyMaterialSlider;
    [SerializeField] private Slider _stabilitySlider;

    public void UpFood()
    {
        _foodSlider.value++;
    }

    public void UpMed()
    {
        _medSlider.value++;
    }

    public void UpRaw()
    {
        _rawMaterialSlider.value++;
    }

    public void UpReady()
    {
        _readyMaterialSlider.value++;
    }

    public void UpStability()
    {
        _stabilitySlider.value+=20;
    }


    public void DownFood()
    {
        _foodSlider.value--;
    }

    public void DownMed()
    {
        _medSlider.value--;
    }

    public void DownRaw()
    {
        _rawMaterialSlider.value--;
    }

    public void DownReady()
    {
        _readyMaterialSlider.value--;
    }

    public void DownStability()
    {
        _stabilitySlider.value-=20;
    }

}
