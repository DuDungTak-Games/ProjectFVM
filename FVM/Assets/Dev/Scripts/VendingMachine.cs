using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{

    float vmKg = 120; // 자판기 자체의 무게 (Test Value = 120kg)
    float maxFuel = 30; // 자판기의 연료 (Test Value = 30L)

    float fuelFkg = 21; // 자판기 풀 연료 무게 (Test Value = 21kg)
    float fuelEkg = 3; // 자판기 빈 연료 무게 (Test Value = 3kg)

    float currentKg; // 현재 자판기 무게
    float currentFuel; // 현재 자판기 연료

    public float thrust; // 수직 추력 수치 (임시)
    public float torque; // 수평 토크 추력 수치 (임시)

    private Rigidbody rb;

    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentKg = vmKg + (fuelFkg + fuelEkg);
        currentFuel = maxFuel;

        rb.mass = currentKg;
    }

    void Update()
    {
        UpdateVM();

        MoveA();
    }

    void MoveA()
    {
        if (Input.GetKey(KeyCode.W) && currentFuel > 0)
        {
            rb.AddForce(transform.up * (thrust * Time.deltaTime), ForceMode.Acceleration);

            currentFuel -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddTorque(-transform.forward * (torque * Time.deltaTime), ForceMode.Acceleration);
        }

        if (Input.GetKey(KeyCode.E))
        {
            rb.AddTorque(transform.forward * (torque * Time.deltaTime), ForceMode.Acceleration);
        }
    }

    void UpdateVM()
    {
        float fuelKg = ((currentFuel / maxFuel) * fuelFkg) + fuelEkg;

        currentKg = vmKg + fuelKg;

        rb.mass = currentKg;
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
