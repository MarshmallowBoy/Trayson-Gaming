using UnityEngine;

public class FollowStations : MonoBehaviour
{
    public Transform[] Stations;
    public float Speed = 1;
    public float RotationalSpeed = 1;
    public int ActiveStation = 0;
    float RotationProgress = 0;
    void FixedUpdate()
    {
        if (Stations.Length == ActiveStation)
        {
            ActiveStation = 0;
        }
        transform.position += (Stations[ActiveStation].position - transform.position).normalized * Speed;
        //transform.localEulerAngles += new Vector3(0, (Stations[ActiveStation].transform.eulerAngles.y - transform.eulerAngles.y) + 90, 0) * (RotationalSpeed/10);
        transform.localRotation = Quaternion.Slerp(transform.rotation, Stations[ActiveStation].rotation, RotationProgress++ * (RotationalSpeed / 100) * 0.02f);
        //transform.Rotate((Stations[ActiveStation].rotation.eulerAngles - transform.rotation.eulerAngles).normalized * RotationalSpeed);
        if (Vector3.Distance(transform.position, Stations[ActiveStation].position) < 1f)
        {
            ActiveStation++;
            RotationProgress = 0;
        }
    }

    public Quaternion LerpRotation(Quaternion a, Quaternion b, float t)
    {
        float W = Mathf.LerpAngle(a.w, b.w, t);
        float X = Mathf.LerpAngle(a.x, b.x, t);
        float Y = Mathf.LerpAngle(a.y, b.y, t);
        float Z = Mathf.LerpAngle(a.z, b.z, t);
        Quaternion Rotation = new Quaternion();
        Rotation.Set(X, Y, Z, W);
        return Rotation;
    }
}
