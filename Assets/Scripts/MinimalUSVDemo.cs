using System.Collections.Generic;
using UnityEngine;

// Public educational baseline. No research policy, reward shaping or assets.
public sealed class MinimalUSVDemo : MonoBehaviour
{
    const float Dt = 0.1f;
    readonly Vector3 goal = new Vector3(0f, 0f, 18f);
    readonly List<Vector3> buoys = new List<Vector3>();
    readonly List<Material> materials = new List<Material>();
    Transform boat;
    Camera onboard;
    RenderTexture cameraTarget;
    Texture2D rgb;
    float elapsed, speed, episodeReturn;
    int steps;
    string outcome = "Running";

    void Start()
    {
        Shape("Water", PrimitiveType.Cube, new Vector3(0f, -0.3f, 0f),
            new Vector3(24f, 0.2f, 44f), new Color(0.05f, 0.23f, 0.34f));
        for (int z = -8; z <= 8; z += 8)
        {
            AddBuoy(new Vector3(-4f, 0f, z), Color.red);
            AddBuoy(new Vector3(4f, 0f, z), Color.green);
        }
        Shape("Goal", PrimitiveType.Cylinder, goal,
            new Vector3(2f, 0.05f, 2f), Color.yellow);
        boat = Shape("USV", PrimitiveType.Cube, Vector3.zero,
            new Vector3(0.8f, 0.4f, 1.5f), Color.white).transform;
        // Keep the root scale at one so camera placement is in metres.
        var root = new GameObject("USV root").transform;
        root.SetParent(transform);
        boat.SetParent(root);
        boat = root;

        onboard = new GameObject("Onboard RGB camera").AddComponent<Camera>();
        onboard.transform.SetParent(boat, false);
        onboard.transform.localPosition = new Vector3(0f, 1.2f, 0.8f);
        onboard.transform.localRotation = Quaternion.Euler(5f, 0f, 0f);
        onboard.fieldOfView = 90f;
        onboard.nearClipPlane = 0.1f;
        onboard.farClipPlane = 60f;
        onboard.clearFlags = CameraClearFlags.SolidColor;
        onboard.backgroundColor = new Color(0.4f, 0.6f, 0.8f);
        onboard.enabled = false; // Render exactly when collecting an observation.
        cameraTarget = new RenderTexture(84, 84, 16);
        cameraTarget.Create();
        onboard.targetTexture = cameraTarget;
        rgb = new Texture2D(84, 84, TextureFormat.RGB24, false);

        var overview = new GameObject("Overview camera").AddComponent<Camera>();
        overview.transform.SetParent(transform);
        overview.transform.position = new Vector3(0f, 40f, 0f);
        overview.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        overview.orthographic = true;
        overview.orthographicSize = 25f;
        overview.clearFlags = CameraClearFlags.SolidColor;
        overview.backgroundColor = new Color(0.03f, 0.09f, 0.14f);
        ResetEpisode();
    }

    GameObject Shape(string label, PrimitiveType type, Vector3 position,
        Vector3 scale, Color color)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = label;
        obj.transform.SetParent(transform);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        var material = new Material(Shader.Find("Unlit/Color"));
        material.color = color;
        materials.Add(material);
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    void AddBuoy(Vector3 position, Color color)
    {
        buoys.Add(position);
        Shape("Buoy", PrimitiveType.Cylinder, position + Vector3.up * 0.5f,
            new Vector3(1f, 0.7f, 1f), color);
    }

    void ResetEpisode()
    {
        boat.position = new Vector3(-1.5f, 0f, -17f);
        boat.rotation = Quaternion.Euler(0f, -8f, 0f);
        elapsed = speed = episodeReturn = 0f;
        steps = 0;
        outcome = "Running";
        Observe();
    }

    // Four auxiliary states: body-frame goal x/z, distance, forward speed.
    // The camera image is available separately as RGB, 84 x 84.
    public Vector4 Observe()
    {
        var previous = RenderTexture.active;
        try
        {
            onboard.Render();
            RenderTexture.active = cameraTarget;
            rgb.ReadPixels(new Rect(0, 0, 84, 84), 0, 0);
            rgb.Apply(false);
        }
        finally { RenderTexture.active = previous; }
        Vector3 localGoal = boat.InverseTransformDirection(goal - boat.position);
        return new Vector4(localGoal.x / 44f, localGoal.z / 44f,
            localGoal.magnitude / 44f, speed / 3f);
    }

    // Baseline reads image pixels and navigation state; no buoy coordinates.
    Vector2 Baseline(Vector4 state)
    {
        float redX = 0f, greenX = 0f;
        int redCount = 0, greenCount = 0;
        Color32[] pixels = rgb.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color32 p = pixels[i];
            if (p.r > 150 && p.g < 100 && p.b < 100)
            { redX += i % 84; redCount++; }
            if (p.g > 150 && p.r < 100 && p.b < 100)
            { greenX += i % 84; greenCount++; }
        }
        float heading = Mathf.Atan2(state.x, state.y);
        float imageError = redCount > 0 && greenCount > 0
            ? ((redX / redCount + greenX / greenCount) * 0.5f - 41.5f) / 41.5f
            : 0f;
        return new Vector2(Mathf.Clamp(heading * 1.8f + imageError * 0.5f, -1f, 1f), 0.5f);
    }

    // Action: normalized yaw and throttle in [-1, 1]. Toy planar dynamics.
    public void Step(Vector2 action)
    {
        if (outcome != "Running") return;
        float before = Vector3.Distance(boat.position, goal);
        speed = 3f * (Mathf.Clamp(action.y, -1f, 1f) + 1f) * 0.5f;
        boat.Rotate(0f, Mathf.Clamp(action.x, -1f, 1f) * 45f * Dt, 0f);
        boat.position += boat.forward * speed * Dt;
        steps++;
        float after = Vector3.Distance(boat.position, goal);
        episodeReturn += before - after - 0.01f;
        // World geometry is used only by the environment for termination.
        bool collision = buoys.Exists(p => Vector3.Distance(p, boat.position) < 1.1f);
        if (collision) Finish("Collision", -5f);
        else if (Mathf.Abs(boat.position.x) > 11f || Mathf.Abs(boat.position.z) > 21f)
            Finish("Out of bounds", -5f);
        else if (after < 1f) Finish("Goal reached", 5f);
        else if (steps >= 600) Finish("Timeout", 0f);
    }

    void Finish(string result, float reward)
    {
        outcome = result;
        episodeReturn += reward;
        Debug.Log($"Mini USV: {outcome}; steps={steps}; demo return={episodeReturn:F2}");
    }

    void FixedUpdate()
    {
        if (outcome != "Running") return;
        elapsed += Time.fixedDeltaTime;
        if (elapsed < Dt) return;
        elapsed -= Dt;
        Step(Baseline(Observe()));
    }

    void OnGUI()
    {
        if (rgb == null) return;
        GUI.DrawTexture(new Rect(16, 16, 252, 252), rgb);
        GUI.Label(new Rect(16, 275, 500, 25), "Onboard RGB 84 x 84 | image + navigation state baseline");
        GUI.Label(new Rect(16, 300, 500, 25), $"{outcome} | steps {steps} | demo return {episodeReturn:F2}");
        if (GUI.Button(new Rect(16, 330, 160, 30), "Restart episode")) ResetEpisode();
    }

    void OnDestroy()
    {
        if (onboard != null) onboard.targetTexture = null;
        if (cameraTarget != null) { cameraTarget.Release(); Destroy(cameraTarget); }
        if (rgb != null) Destroy(rgb);
        foreach (var material in materials) Destroy(material);
    }
}
