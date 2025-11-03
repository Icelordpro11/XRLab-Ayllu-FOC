using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RedirectedWalking : MonoBehaviour
{ 
    public Transform XROrigin;
    
    public Transform centerTransform;
    public Transform playerTransform;
    
    //public Transform realCircleObj;
    //public Transform virtualCircleObj;
    
    public float realRadius = 2.5f;
    public float virtualRadius = 4;

    //public Slider realRSlider;
    //public TMP_Text realSliderText;
    //public Slider virtualRSlider;
    //public TMP_Text virtualSliderText;
    
    private float _distanceReal;
    private float _distanceVirtual;

    /*private void Start()
    {
        AddSliderListeners();
    }*/

    /*private void AddSliderListeners()
    {
        realRSlider.onValueChanged.AddListener(val => {
            realRadius = val;
            realSliderText.text = val.ToString("F2");
            realCircleObj.localScale = new Vector3(val*2, realCircleObj.localScale.y, val*2);
        });

        virtualRSlider.onValueChanged.AddListener(val => {
            virtualRadius = val;
            virtualSliderText.text = val.ToString("F2");
            virtualCircleObj.localScale = new Vector3(val*2, virtualCircleObj.localScale.y, val*2);
        });
    }*/

    private void Update()
    {
        var playerPos = playerTransform.position;
        Vector3 horizontalPlayerPos = new Vector3(playerPos.x, 0, playerPos.z);
        
        var centerPos = centerTransform.position;
        Vector3 horizontalCenterPos = new Vector3(centerPos.x, 0, centerPos.z);
        
        Vector3 direction = (horizontalPlayerPos - horizontalCenterPos).normalized;
        
        _distanceReal = Vector3.Distance(horizontalPlayerPos, horizontalCenterPos);
        _distanceVirtual = _distanceReal * (virtualRadius / realRadius);

        Vector3 offsetPos = direction * (_distanceVirtual - _distanceReal);
        XROrigin.position = new Vector3(offsetPos.x, XROrigin.position.y, offsetPos.z);
    }
}
