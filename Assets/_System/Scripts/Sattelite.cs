using System.Linq;
using UnityEngine;

public class Sattelite : MonoBehaviour
{
    float Ma; //SemiMajor
    float Mi; //SemiMinor
    Vector3 focus1; //The main focal point
    Vector3 focus2; //The empty focus
    Vector3 fociOffset; //Offset from center, same for 1 and 2
    Vector3 center; //Kinda the same as fociOffset
    float Mu; //mass * G
    public Vector3 v; // velocity
    public Object SOI; //What it is orbiting
    Object current; //Used for mass
    float e; //eccentricity
    Vector3 eccentricityVector; //For once I named it as it is
    float hyperbolaAngleRange;
    int travelDirection = -1; //1 or -1
    [SerializeField] float hyperbolaRenderRange;
    [SerializeField] float trueAnomaly;
    [SerializeField] int orbitPoints; //Precision of hyperbola/ellipse

    LineRenderer lr; //Orbit renderer
    Vector3[] orbit; //Orbit verticies
    [SerializeField] Material trailMat; //Orbit material

    float angle1; //Ascending node rotation
    float angle2; //Inclination rotation
    float angle3; //Argument of periapsis rotation (rotation around it's own axis)

    private void Start()
    {
        GameObject orbit = new GameObject("Orbit");
        lr = orbit.AddComponent<LineRenderer>();

        CalcProperties();
        Visualize(lr);
        trueAnomaly = CalcTrueAnomaly();
        travelDirection = CalcOrbitDirection();
    }

    void Visualize(LineRenderer lr)
    {
        if (Ma >= 0)
        {
            VisualizeEllipse(lr);
        }
        else
        {
            VisualizeHyperbola(lr);
        }
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
        lr.widthMultiplier = 0.1f;
        trailMat.color = Random.ColorHSV();
        lr.material = trailMat;
    }
    void VisualizeHyperbola(LineRenderer lr)
    {
        int pointCount = orbitPoints / 2;
        float angleRange = Mathf.Acos((((Ma * (e * e - 1)) / -hyperbolaRenderRange) - 1) / e);
        hyperbolaAngleRange = angleRange * Mathf.Rad2Deg; //Calculation for other purpose, not used for the rendering
        lr.positionCount = pointCount * 2 + 1;
        orbit = new Vector3[pointCount * 2 + 1];
        int index = 0;
        for (int i = -pointCount; i <= pointCount; i++)
        {
            float angle = ((float)i / pointCount) * angleRange;
            float r = GetDistance(angle);
            orbit[index] = new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
            index++;
        }
        orbit = RotateOrbit(orbit);
        var current = orbit;
        for (int i = 0; i < orbit.Length; i++)
        {
            current[i] += SOI.transform.position;
        }
        lr.SetPositions(current);
        lr.widthMultiplier = 0.1f;
        trailMat.color = Random.ColorHSV();
        lr.material = trailMat;
    }
    float GetDistance(float angle)
    {
        return (Ma * (e * e - 1)) / (1 + e * Mathf.Cos(angle));
    }
    Vector3[] RotateOrbit(Vector3[] orbit)
    {
        int direction = focus2.z > 0 ? -1 : 1;
        angle1 = Vector3.Angle(Vector3.right, new Vector3(focus2.x, 0, focus2.z)) * direction;
        orbit = Rotate(orbit, angle1, focus1, Vector3.up);
        direction = focus2.y < 0 ? -1 : 1;
        angle2 = Vector3.Angle(focus2, new Vector3(focus2.x, 0, focus2.z)) * direction;
        orbit = Rotate(orbit, angle2, focus1, Vector3.Cross(new Vector3(focus2.x, 0, focus2.z), Vector3.up));
        Vector3 n1 = Vector3.Cross(focus2, orbit[orbit.Length / 3]);
        Vector3 n2 = Vector3.Cross(focus2, transform.position - SOI.transform.position);
        direction = (int)Mathf.Sign(Vector3.Dot(focus2, Vector3.Cross(n1.normalized, n2.normalized)));
        angle3 = Vector3.Angle(n1, n2) * direction;
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
        focus1 = Vector3.zero;
        Mu = transform.TryGetComponent<Object>(out current) ? Mu = (SOI.mass + current.mass) * StarSystem.singleton.G : Mu = SOI.mass * StarSystem.singleton.G;
        Ma = CalcMajorAxis();
        focus2 = CalcSecondFocus();
        center = (focus1 + focus2) / 2;
        fociOffset = focus2 - center;
        Mi = CalcMinorAxis();
        e = CalcEccentricity();
    }
    float CalcMajorAxis()
    {
        float r = Vector3.Distance(transform.position - SOI.transform.position, focus1);
        float v = this.v.magnitude;
        return 1 / ((2 / r) - (Mathf.Pow(v, 2) / Mu));
    }
    Vector3 CalcSecondFocus()
    {
        if (Ma < 0)
        {
            //Hyperbola
            Vector3 d = transform.position - SOI.transform.position;
            float r = d.magnitude;
            eccentricityVector = (d / r) - (Vector3.Cross(v, Vector3.Cross(d, v)) / Mu);
            return -2 * Ma * eccentricityVector;
        }
        else
        {
            //Ellipse
            Vector3 d = transform.position - SOI.transform.position;
            float r = d.magnitude;
            eccentricityVector = (d / r) - (Vector3.Cross(v, Vector3.Cross(d, v)) / Mu);
            return 2 * Ma * eccentricityVector;
        }
    }
    float CalcMinorAxis()
    {
        float n = Mathf.Pow(fociOffset.magnitude, 2);
        if (Ma < 0)
        {
            return Mathf.Sqrt(n - Mathf.Pow(Ma, 2));
        }
        else
        {
            return Mathf.Sqrt(Mathf.Pow(Ma, 2) - n);
        }
    }
    float CalcEccentricity()
    {
        if (Ma < 0)
        {
            return Mathf.Sqrt(1 + ((Mi * Mi) / (Ma * Ma)));
        }
        else
        {
            return Mathf.Sqrt(1 - ((Mi * Mi) / (Ma * Ma)));
        }
    }
    float CalcTrueAnomaly()
    {
        var hold = orbit.ToList().OrderBy(x => Vector3.Distance(transform.position, x)).FirstOrDefault();
        int index = orbit.ToList().IndexOf(hold);
        float range;
        if (Ma >= 0)
        {
            range = 360;
        }
        else
        {
            range = hyperbolaAngleRange;
        }
        return (float)index / orbitPoints * range;
    }
    int CalcOrbitDirection()
    {
        var angleRange = Ma >= 0 ? 360 : hyperbolaAngleRange;
        float progress = trueAnomaly / angleRange;
        float indexF = orbitPoints * progress;
        int index = Mathf.RoundToInt(indexF);

        Vector3 next = orbit[index] + v.normalized;
        Vector3 plus = orbit[index == orbitPoints ? 0 : index + 1];
        Vector3 minus = orbit[index == 0 ? orbitPoints : index - 1];

        if (Vector3.Distance(next, plus) < Vector3.Distance(next, minus))
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    private void Update()
    {
        float eccentricityParameter;
        float angleRange;

        if (Ma >= 0)
        {
            eccentricityParameter = 1 - (e * e);
            angleRange = 360;
        }
        else
        {
            eccentricityParameter = (e * e) - 1;
            angleRange = hyperbolaAngleRange;
        }

        float anglularMomentum = Mathf.Sqrt(Mu * Mathf.Abs(Ma) * eccentricityParameter);
        float anglularVelocity = anglularMomentum / Mathf.Pow((transform.position - SOI.transform.position).magnitude, 2);
        trueAnomaly += anglularVelocity * Time.deltaTime * StarSystem.singleton.time * travelDirection;
        if (trueAnomaly >= 360)
        {
            if (Ma >= 0)
            {
                trueAnomaly -= 360;
            }
            else
            {
                trueAnomaly = 360f;
            }
        }
        if (trueAnomaly < 0)
        {
            if (Ma >= 0)
            {
                trueAnomaly += 360;
            }
            else
            {
                trueAnomaly = 0f;
            }
        }
        transform.position = GetPos(trueAnomaly, angleRange);
    }

    Vector3 GetPos(float angle, float angleRange)
    {
        float progress = angle / angleRange;
        float indexF = orbitPoints * progress;
        int index = Mathf.RoundToInt(indexF);
        return orbit[index];
    }
}
