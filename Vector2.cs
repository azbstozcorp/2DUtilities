class Vector2 {
    public float x, y;

    public float Length2 => x * x + y * y;
    public float Length => (float)System.Math.Sqrt(Length2);

    public Vector2 Normal() {
        Vector2 result = new Vector2();

        if (Length > 0.0f) {
            result.x = x / Length;
            result.y = y / Length;
        }

        return result;
    }

    public static float DotProduct(Vector2 va, Vector2 vb) =>
        va.x * vb.x + va.y * vb.y;

    public static bool operator ==(Vector2 va, Vector2 vb) =>
        va != null && vb != null &&
        va.x == vb.x &&
        va.y == vb.y;

    public static bool operator !=(Vector2 va, Vector2 vb) =>
        !(va == vb);

    public static Vector2 operator +(Vector2 va, Vector2 vb) =>
        new Vector2(va.x + vb.x, va.y + vb.y);
    public static Vector2 operator -(Vector2 va, Vector2 vb) =>
        new Vector2(va.x - vb.x, va.y - vb.y);

    public static Vector2 operator *(Vector2 va, Vector2 vb) =>
        new Vector2(va.x * vb.x, va.y * vb.y);
    public static Vector2 operator /(Vector2 va, Vector2 vb) =>
        new Vector2(va.x / vb.x, va.y / vb.y);

    public static Vector2 operator *(Vector2 v, float f) =>
        new Vector2(v.x * f, v.y * f);
    public static Vector2 operator /(Vector2 v, float f) =>
        new Vector2(v.x / f, v.y / f);

    public Vector2() { }
    public Vector2(Vector2 v) {
        x = v.x;
        y = v.y;
    }
    public Vector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public override int GetHashCode() =>
        base.GetHashCode();

    public override bool Equals(object obj) =>
        obj is Vector2 v && v == this;
}
