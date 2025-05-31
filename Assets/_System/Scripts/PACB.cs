using UnityEngine;

public class PACB : MonoBehaviour
{
    public float Ma; //SemiMajor
    public float Mi; //SemiMinor
    Vector3 focus1; //The main focal point
    Vector3 focus2; //The empty focus
    float Mu; //mass * G
    public Object SOI; //What it is orbiting
    Object currentObject; //Used for mass
    float e; //eccentricity
    public int travelDirection; //1 or -1
    public float trueAnomaly;
    public int orbitPoints; //Precision of hyperbola/ellipse

    LineRenderer lr; //Orbit renderer
    Vector3[] orbit; //Orbit verticies
    public Color orbitColor;

    public float angle1; //Ascending node rotation
    public float angle2; //Inclination rotation
    public float angle3; //Argument of periapsis rotation (rotation around it's own axis)

    public bool update;

    private void Start()
    {
        GameObject orbit = new GameObject("Orbit");
        orbit.transform.SetParent(transform);
        lr = orbit.AddComponent<LineRenderer>();

        CalcProperties();
        VisualizeEllipse(lr);
        transform.position = GetPos(trueAnomaly);
    }

    void VisualizeEllipse(LineRenderer lr)
    {
        orbit = new Vector3[orbitPoints + 1];
        lr.positionCount = orbitPoints + 1;
        float angle;
        orbit[0] = new Vector3(GetDistance(0), 0, 0);
        for (int i = 1; i < orbitPoints + 1; i++)
        {
            angle = ((float)i / orbitPoints) * 360 * Mathf.Deg2Rad;
            float r = GetDistance(angle);
            orbit[i] = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
        }
        orbit = RotateOrbit(orbit);
        var current = orbit;
        for (int i = 0; i < orbit.Length; i++)
        {
            current[i] += SOI.transform.position;
        }
        lr.SetPositions(current);
        lr.widthMultiplier = 0.02f + .1f * transform.localScale.x;
        orbitColor.a = PlayerPrefs.GetFloat("Opacity");
        lr.SetColors(orbitColor, orbitColor);
        lr.material = StarSystem.singleton.materials[4];
    }
    float GetDistance(float angle)
    {
        return (Ma * (e * e - 1)) / (1 + e * Mathf.Cos(angle));
    }
    Vector3[] RotateOrbit(Vector3[] orbit)
    {
        orbit = Rotate(orbit, angle1, focus1, Vector3.up);
        orbit = Rotate(orbit, angle2, focus1, Vector3.Cross(new Vector3(focus2.x, 0, focus2.z), Vector3.up));
        orbit = Rotate(orbit, angle3, focus1, focus2);

        return orbit;
    }
    Vector3[] Rotate(Vector3[] input, float offset, Vector3 pivot, Vector3 axis)
    {
        var qAngle = Quaternion.AngleAxis(offset, axis);
        var output = new Vector3[input.Length];

        for (var vert = 0; vert < input.Length; vert++)
        {
            // Translate vertex to pivot, rotate, and translate back => not really used in current setup as pivot is always focus1 (0,0,0)
            Vector3 translated = input[vert] - pivot;
            Vector3 rotated = qAngle * translated;
            output[vert] = rotated + pivot;
        }
        return output;
    }
    void CalcProperties()
    {
        Ma *= StarSystem.singleton.AuToUnityUnits;
        Mi *= StarSystem.singleton.AuToUnityUnits;
        focus1 = Vector3.zero;
        Mu = transform.TryGetComponent<Object>(out currentObject) ? Mu = (SOI.mass + currentObject.mass) * StarSystem.singleton.G : Mu = SOI.mass * StarSystem.singleton.G;
        e = CalcEccentricity();
        focus2 = new Vector3(2 * Ma * e, 0, 0);
        if (focus2 == Vector3.zero)
        {
            focus2 = Vector3.right;
        }
        trueAnomaly = Random.value * 360;
    }
    float CalcEccentricity()
    {
        return Mathf.Sqrt(1 - ((Mi * Mi) / (Ma * Ma)));
    }

    private void Update()
    {
        float anglularMomentum = Mathf.Sqrt(Mu * Mathf.Abs(Ma) * (1 - (e * e)));
        float anglularVelocity = anglularMomentum / Mathf.Pow((transform.position - SOI.transform.position).magnitude, 2);
        trueAnomaly += anglularVelocity * Time.deltaTime * StarSystem.singleton.time * travelDirection;
        if (trueAnomaly >= 360)
        {
            trueAnomaly -= 360;
        }
        if (trueAnomaly < 0)
        {
            trueAnomaly += 360;
        }
        transform.position = GetPos(trueAnomaly);

        if (update)
        {
            VisualizeEllipse(lr);
        }
    }

    Vector3 GetPos(float angle)
    {
        angle = angle * Mathf.Deg2Rad;
        float r = GetDistance(angle);
        Vector3 point = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
        point = RotateOrbit(new Vector3[] { point })[0];
        return point + SOI.transform.position;
    }
}
