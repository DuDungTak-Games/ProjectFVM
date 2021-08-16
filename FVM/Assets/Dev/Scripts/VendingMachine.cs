using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{

    [SerializeField] float vmKg = 120; // ���Ǳ� ��ü�� ���� (Test Value = 120kg)
    [SerializeField] float maxFuel = 30; // ���Ǳ��� ���� (Test Value = 30L)

    [SerializeField] float fuelFkg = 21; // ���Ǳ� Ǯ ���� ���� (Test Value = 21kg)
    [SerializeField] float fuelEkg = 3; // ���Ǳ� �� ���� ���� (Test Value = 3kg)

    float curKg; // ���� ���� (����)
    float curFuel; // ���� ����
    float curThrust; // ���� �߷�

    [SerializeField] float maxThrust; // ���� �߷� �ִ� ��ġ (�ӽ�)
    [SerializeField] float torque; // ���� ��ũ �߷� ��ġ (�ӽ�)

    private Rigidbody rb;

    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        vmKg = TestGameManager.Instance.GetKg();
        
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

    void Thrust()
    {
        if (curFuel <= 0 || curThrust <= 0)
            return;

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
    }



    // TODO : ������Ÿ���� AddForce �� ���� (������ ����� ������)
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
