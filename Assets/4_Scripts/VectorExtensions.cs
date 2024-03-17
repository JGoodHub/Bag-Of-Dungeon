using UnityEngine;

public static class VectorUtils
{

    public static Vector2 Vec3ToVec2_XZ(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 Vec2ToVec3_XZ(Vector2 vector, float y = 0)
    {
        return new Vector3(vector.x, y, vector.y);
    }

    public static Vector3 Random(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        return new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), UnityEngine.Random.Range(minZ, maxZ));
    }

    public static Vector3 Random(float x, float y, float z)
    {
        return new Vector3(UnityEngine.Random.Range(-x, x), UnityEngine.Random.Range(-y, y), UnityEngine.Random.Range(-z, z));
    }

}