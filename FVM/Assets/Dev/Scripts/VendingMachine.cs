using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TouchType = VMInputScreen.ScreenTouchType;

public class VendingMachine : MonoBehaviour
{

    [SerializeField] float vmKg = 120; // 자판기 자체의 무게 (Test Value = 120kg)
    [SerializeField] float maxFuel = 30; // 자판기의 연료 (Test Value = 30L)

    [SerializeField] float fuelFkg = 21; // 자판기 풀 연료 무게 (Test Value = 21kg)
    [SerializeField] float fuelEkg = 3; // 자판기 빈 연료 무게 (Test Value = 3kg)

    float curKg; // 현재 무게 (총합)
    float curFuel; // 현재 연료
    float curThrust; // 현재 추력
    float curHeight, maxHeight; // 현재 고도, 최고 고도

    float lastVelocityY = 0;
    
    [SerializeField] float maxThrust; // 수직 추력 최대 수치 (임시)
    [SerializeField] float torque; // 수평 토크 추력 수치 (임시)

    [SerializeField] GameObject deadEffect_Prefab;
    [SerializeField] GameObject[] fireEffect, smokeEffect;

    public VMInputScreen vmInput;
    
    private CameraShake cameraShake;
    
    // private TestGameManager gm;
    
    private Rigidbody rb;

    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();

        Camera.main.TryGetComponent(out cameraShake);

        vmInput.onTouch.AddListener(Torque);
    }

    void Start()
    {   
        // gm = TestGameManager.Instance;
        
        // TODO : 여기도 수정해야함
        // vmKg = gm.GetKg();
        
        curKg = vmKg + (fuelFkg + fuelEkg);
        curFuel = maxFuel;
        curThrust = 0;

        rb.mass = curKg;
    }

    void Update()
    {
        UpdateVM();
        Thrust();

        LocalInput();
    }

    private void LateUpdate()
    {
        lastVelocityY = rb.velocity.y;
    }

    void LocalInput()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Torque(TouchType.LEFT);
        }

        if (Input.GetKey(KeyCode.E))
        {
            Torque(TouchType.RIGHT);
        }
    }

    public void UpdateThrust(float sliderValue)
    {
        curThrust = (maxThrust * sliderValue);
    }

    void SetEffect(bool isActive)
    {
        fireEffect[0].gameObject.SetActive(isActive);
        smokeEffect[0].gameObject.SetActive(isActive);
        
        fireEffect[1].gameObject.SetActive(isActive);
        smokeEffect[1].gameObject.SetActive(isActive);
    }

    void Thrust()
    {
        if (curFuel <= 0 || curThrust <= 0)
        {
            SetEffect(false);
            return;
        }

        SetEffect(true);

        rb.AddForce(transform.up * (curThrust * Time.deltaTime), ForceMode.Force);

        curFuel -= ((curThrust/maxThrust) * 1) * Time.deltaTime;
    }

    void Torque(TouchType touchType)
    {
        rb.AddTorque((transform.forward * (int)touchType) * (torque * Time.deltaTime), ForceMode.Force);
    }

    void UpdateVM()
    {
        float fuelKg = ((curFuel / maxFuel) * fuelFkg) + fuelEkg;

        curKg = vmKg + fuelKg;

        rb.mass = curKg;

        curHeight = transform.position.y;
        if (maxHeight < curHeight)
        {
            maxHeight = curHeight;
        }

        CheckFalling(rb.velocity.y);

        // TODO : 추후 수정
        //gm.UpdateUI(LabelText.labelType.CUR_KG, (int)curKg);
        //gm.UpdateUI(LabelText.labelType.CUR_FUEL, (int)curFuel);
        //gm.UpdateUI(LabelText.labelType.CUR_ANGLE, (int)transform.eulerAngles.z);
        //gm.UpdateUI(LabelText.labelType.CUR_HEIGHT, (int)curHeight);
        //gm.UpdateUI(LabelText.labelType.MAX_HEIGHT, (int)maxHeight);
        //gm.UpdateUI(LabelText.labelType.CUR_THRUST, curThrust > 0 ? (int)curThrust : 0);
        //gm.UpdateUI(LabelText.labelType.CUR_VELOCITY, rb.velocity.y > 0 ? (int)rb.velocity.y : 0);
    }

    void CheckFalling(float velocityY)
    {
        float maxShakeVelocity = 1000;
        float maxShakeAmount = 3;
        
        if (velocityY < -10)
        {
            float t = Mathf.Lerp(0, maxShakeAmount, Mathf.Abs(velocityY) / maxShakeVelocity);
            
            cameraShake?.ShakeLoop(t);
        }
        else
        {
            cameraShake?.ShakeLoop(0);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject)
        {
            if (transform.position.y < 100 && lastVelocityY < -50)
            {
                Vector3 spawnPos = transform.position;
                spawnPos.y = 1;

                Instantiate(deadEffect_Prefab, spawnPos, Quaternion.identity);
                
                cameraShake?.Shake(0.5f, 0.75f);

                GameManager.Instance.ActionGameEvent(GameState.END);

                Destroy(this.gameObject, 0.01f);
            }
        }
    }
}
