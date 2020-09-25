class Polygon {
    public Vector2 location;
    public float rotation;

    public List<Vector2> points;            // points of the polygon, counterclockwise
    public List<Side> sides;                // sides of the polygon
    public bool Valid => points.Count > 1;  // does this polygon have enough sides

    public void Translate(float x, float y) {
            location.x += x;
            location.y += y;

            foreach (Vector2 point in points) {
                point.x += x;
                point.y += y;
            }
        }
    public void Translate(Vector2 v) =>
            Translate(v.x, v.y);

    public void MoveTo(Vector2 v) =>
            Translate(v - location);
    public void MoveTo(float x, float y) =>
            MoveTo(new Vector2(x, y));

    public void Rotate(float angle) {
            rotation += angle;
            for (int i = 0; i < points.Count; i++) {
                float s = (float)Math.Sin(angle);
                float c = (float)Math.Cos(angle);

                Vector2 p = points[i] - location;
                Vector2 pr = new Vector2(p.x * c - p.y * s, p.x * s + p.y * c);
                points[i] = pr + location;
            }
            RecalculateSides();
        }

    public void Scale(float amount) {
            for (int i = 0; i < points.Count; i++)
                points[i] *= amount;
            RecalculateSides();
        }

    public void AddPoint(Vector2 point) {
            points.Add(point + location);
            RecalculateSides();
        }

    public void RecalculateSides() {
            if (Valid) {
                sides.Clear();
                for (int i = 0; i < points.Count - 1; i++)
                    sides.Add(new Side(points[i], points[i + 1]));
                sides.Add(new Side(points[points.Count - 1], points[0]));
            }
        }

    public void Subdivide(int times) {
            for (; times >= 0; times--) {
                HashSet<Vector2> newPoints = new HashSet<Vector2>();

                foreach (Side side in sides) {
                    newPoints.Add(side.a);
                    newPoints.Add(side.Middle);
                    newPoints.Add(side.b);
                }

                points.Clear();
                foreach (Vector2 v in newPoints)
                    AddPoint(v);
            }
        }

    public static Polygon MakeSquare(float sideLength) {
            Polygon square = new Polygon();

            square.AddPoint(new Vector2(-.5f, -.5f));
            square.AddPoint(new Vector2(+.5f, -.5f));
            square.AddPoint(new Vector2(+.5f, +.5f));
            square.AddPoint(new Vector2(-.5f, +.5f));
            square.Scale(sideLength);

            return square;
        }

    public bool Collision(Polygon other, ref Vector2 resolution) {
            return SAT(other, ref resolution);
        }

    bool SAT(Polygon other, ref Vector2 resolution) {
            float overlap = float.MaxValue;
            Vector2 smallest = new Vector2();

            Vector2[] axesA = GetAxes();
            Vector2[] axesB = other.GetAxes();

            for (int i = 0; i < axesA.Length; i++) {
                Vector2 axis = axesA[i];
                _projection pa = Project(axis);
                _projection pb = other.Project(axis);

                if (!pa.Overlap(pb))
                    return false;

                float o = pa.GetOverlap(pb);
                if (o < overlap) {
                    overlap = o;
                    smallest = axis;
                }
            }

            for (int i = 0; i < axesB.Length; i++) {
                Vector2 axis = axesB[i];
                _projection pa = Project(axis);
                _projection pb = other.Project(axis);

                if (!pa.Overlap(pb))
                    return false;

                float o = pa.GetOverlap(pb);
                if (o < overlap) {
                    overlap = o;
                    smallest = axis;
                }
            }

            if ((other.location - location).DotProduct(smallest) < 0)
                smallest *= -1;

            resolution = smallest * -overlap;
            return true;
        }

    _projection Project(Vector2 onto) {
            float min = onto.DotProduct(points[0]);
            float max = min;

            for (int i = 1; i < points.Count; i++) {
                float p = onto.DotProduct(points[i]);
                if (p < min) min = p;
                else if (p > max) max = p;
            }

            return new _projection(min, max);
        }

    Vector2[] GetAxes() {
            Vector2[] axes = new Vector2[sides.Count];
            for (int i = 0; i < axes.Length; i++)
                axes[i] = sides[i].Normal;
            return axes;
        }

    public Polygon() {
            location = new Vector2(0, 0);
            points = new List<Vector2>();
            sides = new List<Side>();
        }

    class _projection {
            public float min, max;

            public bool Overlap(_projection other) =>
                max > other.min && min < other.max;

            public float GetOverlap(_projection other) =>
                Math.Min(max, other.max) - Math.Max(min, other.min);

            public _projection(float min, float max) {
                if (min < max) {
                    this.min = min;
                    this.max = max;
                } else {
                    this.max = min;
                    this.min = max;
                }
            }
        }

    public class Side {
            public Vector2 a, b;
            public Vector2 Vector => b - a;

            public Vector2 Middle =>
                a + Vector.Normalized() * Vector.Length / 2;

            public Vector2 Normal =>
                Vector.Normal;

            public Side(float ax, float ay, float bx, float by) {
                a = new Vector2(ax, ay);
                b = new Vector2(bx, by);
            }

            public Side(Vector2 a, Vector2 b) {
                this.a = a;
                this.b = b;
            }
        }
}
