using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class MyCarCamera : MonoBehaviour
{
    Transform rootNode;
    Transform car;
    Rigidbody carPhysics;
    private GameObject PlayerCar;
    public float posX, posY, posZ;
    public float rotX;

    [Tooltip("If car speed is below this value, then the camera will default to looking forwards.")]
    public float rotationThreshold = 1f;

    [Tooltip("How closely the camera follows the car's position. The lower the value, the more the camera will lag behind.")]
    public float cameraStickiness = 10.0f;

    [Tooltip("How closely the camera matches the car's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
    public float cameraRotationSpeed = 5.0f;

    private Camera m_camera;
    private void Start()
    {
        // PlayerCar = GameObject.FindGameObjectWithTag("PlayerCar");
    }
    public void OnStartFollowing(GameObject go)
    {
        // PlayerCar = go.GetComponentInChildren<CarController>().gameObject;
        // transform.parent.SetParent(go.transform,false);
        // transform.parent.localPosition = Vector3.zero;
        // transform.parent.localRotation= Quaternion.identity;
        
        PlayerCar = go.GetComponentInChildren<CarController>().gameObject;
        m_camera = this.GetComponent<Camera>();
        rootNode = transform;
    }

    void LateUpdate()
    {
        if (PlayerCar == null)
        {
            return;
        }
        m_camera.transform.Translate(posX, posY, posZ);
        // float rotZ = m_camera.transform.eulerAngles.z;
        // m_camera.transform.Rotate(rotX, 0, -rotZ);
        car = PlayerCar.GetComponent<Transform>();
        carPhysics = PlayerCar.GetComponent<Rigidbody>();
        Quaternion look;

        // Moves the camera to match the car's position.
        rootNode.position = Vector3.Lerp(rootNode.position, car.position, cameraStickiness * Time.fixedDeltaTime);

        // // If the car isn't moving, default to looking forwards. Prevents camera from freaking out with a zero velocity getting put into a Quaternion.LookRotation
        // if (carPhysics.velocity.magnitude < rotationThreshold)
        //     look = Quaternion.LookRotation(car.forward);
        // else
        //     look = Quaternion.LookRotation(carPhysics.velocity.normalized);
        //
        // Rotate the camera towards the velocity vector.
        look = Quaternion.LookRotation(car.forward);
        look = Quaternion.Slerp(rootNode.rotation, look, cameraRotationSpeed * Time.fixedDeltaTime);
        rootNode.rotation = look;
    }
}