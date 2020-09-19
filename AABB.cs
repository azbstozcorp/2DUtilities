class AABB {
    public float
        x, y,
        w, h;

    public bool IntersectsX(AABB other) =>
        other != null && 
        x < other.x + other.w && 
        x + w > other.x;

    public bool IntersectsY(AABB other) =>
        other != null && 
        y < other.y + other.h && 
        y + h > other.y;

    public bool Intersects(AABB other) =>
        IntersectsX(other) && 
        IntersectsY(other);

    public AABB() { }
    public AABB(float x, float y, float w, float h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }
}
