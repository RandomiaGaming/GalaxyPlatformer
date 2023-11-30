using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(EdgeCollider2D))]
public class Gravitational_Attractor : MonoBehaviour
{
    public static List<Gravitational_Attractor> Gravitational_Attractors = new List<Gravitational_Attractor>();
    public const float Gravitational_Constant = 5;
    public float Temp_Timescale = 1;

    private Rigidbody2D RB;
    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        SpriteShapeController SSC = GetComponent<SpriteShapeController>();
        List<Vector2> Spline_Vertices = new List<Vector2>();
        for (int i = 0; i < SSC.spline.GetPointCount(); i++)
        {
            Spline_Vertices.Add(SSC.spline.GetPosition(i));
        }
        foreach (List<Vector2> Tri in GeometryHelper2D.Triangulate(Spline_Vertices))
        {
            Vector2 Center_Of_Mass = new Vector2((Tri[0].x + Tri[1].x + Tri[2].x) / 3, (Tri[0].y + Tri[1].y + Tri[2].y) / 3);
            Vector2 Bounds_Max = new Vector2(Mathf.Max(new float[] { Tri[0].x, Tri[1].x, Tri[2].x }), Mathf.Max(new float[] { Tri[0].y, Tri[1].y, Tri[2].y }));
            Vector2 Bounds_Min = new Vector2(Mathf.Min(new float[] { Tri[0].x, Tri[1].x, Tri[2].x }), Mathf.Min(new float[] { Tri[0].y, Tri[1].y, Tri[2].y }));
            Vector2 Size = Bounds_Max - Bounds_Min;
            float Area = Size.x * Size.y / 2;
            float Mass = Area * 100;
            Planetary_Tris.Add(new Planitary_Tri(transform, Center_Of_Mass, Mass));
        }
    }
    void Update()
    {
        Time.timeScale = Temp_Timescale;

        foreach (Planitary_Tri Tri in Planet.Planetary_Tris)
        {
            Vector2 Tri_World_Center_Of_Mass = Tri.Center_Of_Mass + (Vector2)Tri.transform.position;
            float Tri_Distance = GeometryHelper2D.Distance(transform.position, Tri_World_Center_Of_Mass);
            float Attraction_Force = (Player_Mass * Tri.Mass) / (Tri_Distance * Tri_Distance * Gravitational_Constant);
            RB.velocity += Attraction_Force * GeometryHelper2D.Vector_Clamp(Tri_World_Center_Of_Mass - (Vector2)transform.position) * Time.deltaTime;
        }
    }
}