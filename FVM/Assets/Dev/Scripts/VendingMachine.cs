using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

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
    
    private TestGameManager gm;
    
    private Rigidbody rb;

    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        gm = TestGameManager.Instance;
        
        vmKg = gm.GetKg();
        
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
        //if (Input.GetKey(KeyCode.W))
        //{
        //    Thrust();
        //}

        if (Input.GetKey(KeyCode.Q))
        {
            Torque(ScreenInputTest.ScreenTouchType.LEFT);
        }

        if (Input.GetKey(KeyCode.E))
        {
            Torque(ScreenInputTest.ScreenTouchType.RIGHT);
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

        curFuel -= Time.deltaTime;
    }

    public void Torque(ScreenInputTest.ScreenTouchType touchType)
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

        gm.UpdateUI(LabelText.labelType.CUR_KG, (int)curKg);
        gm.UpdateUI(LabelText.labelType.CUR_FUEL, (int)curFuel);
        gm.UpdateUI(LabelText.labelType.CUR_ANGLE, (int)transform.eulerAngles.z);
        gm.UpdateUI(LabelText.labelType.CUR_HEIGHT, (int)curHeight);
        gm.UpdateUI(LabelText.labelType.MAX_HEIGHT, (int)maxHeight);
        gm.UpdateUI(LabelText.labelType.CUR_THRUST, curThrust > 0 ? (int)curThrust : 0);
        gm.UpdateUI(LabelText.labelType.CUR_VELOCITY, rb.velocity.y > 0 ? (int)rb.velocity.y : 0);
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
                
                Camera.main.GetComponent<CameraFollow>().Shake(0.5f, 0.75f);
                
                Destroy(this.gameObject);
            }
        }
    }

    

    // TODO : 프로토타입은 AddForce 로 진행 (질량을 고려한 움직임)
    //float accel = 0f;
    //void MoveB()
    //{
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        accel += thrust * Time.deltaTime;
    //    }
    //    else
    //    {
    //        accel = 0;
    //    }

    //    if (Input.GetKey(KeyCode.Q))
    //    {
    //        rb.AddTorque(-transform.forward * (torque * Time.deltaTime), ForceMode.Acceleration);
    //    }

    //    if (Input.GetKey(KeyCode.E))
    //    {
    //        rb.AddTorque(transform.forward * (torque * Time.deltaTime), ForceMode.Acceleration);
    //    }

    //    if(accel > 0f)
    //    {
    //        rb.velocity = transform.up * accel;
    //    }
    //}

    //float IncrementTowards(float n, float target, float a)
    //{
    //    if (n == target) {
    //        return n;
    //    }
    //    else {
    //        float dir = Mathf.Sign(target = n);
    //        n += a * Time.deltaTime;
    //        return (dir == Mathf.Sign(target - n)) ? n : target;
    //    }
    //}
}
