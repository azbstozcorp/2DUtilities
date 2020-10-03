class Quadtree {
    Node root;

    public bool Insert(Agent agent) {
        agent.world = this;
        return root.Insert(agent);
    }

    public bool Remove(Agent agent) {
        agent.world = null;
        return root.Remove(agent);
    }

    public IEnumerable<Agent> Query(Vector2 topLeft, Vector2 bottomRight) =>
        root.Query(topLeft, bottomRight);

    public IEnumerable<Agent> Query(Vector2 middle, float radius) {
        IEnumerable<Agent> rectQuery = Query(middle - radius, middle + radius);

        foreach (Agent agent in rectQuery)
            if ((agent.pos - middle).Length <= radius)
                yield return agent;
    }

    public IEnumerable<Agent> Contents =>
        root.GetAllChildren();

    class Node {
        public static int agentsPer = 100;

        #region bounds
            public Vector2 topLeft;
            public Vector2 bottomRight;

            Vector2 middle;
            Vector2 topMid;
            Vector2 leftMid;
            Vector2 rightMid;
            Vector2 bottomMid;
            #endregion

        Node tl, tr;
        Node bl, br;

        System.Collections.Generic.List<Agent> agents;

        public bool Vector2Intersects(Vector2 v) =>
            v.x >= topLeft.x && v.y >= topLeft.y && v.x <= bottomRight.x && v.y <= bottomRight.y;
        public bool RectIntersects(Vector2 topL, Vector2 botR) =>
            topL.x < bottomRight.x && topL.y < bottomRight.y && botR.x > topLeft.x && botR.y > topL.y;

        void Subdivide() {
            if (tl == null) tl = new Node(topLeft, middle);
            if (tr == null) tr = new Node(topMid, rightMid);
            if (bl == null) bl = new Node(leftMid, bottomMid);
            if (br == null) br = new Node(middle, bottomRight);
        }

        void TryCollapse() {
            if (tl?.agents.Count == 0 &&
                tr?.agents.Count == 0 &&
                bl?.agents.Count == 0 &&
                br?.agents.Count == 0) {
                tl = null;
                tr = null;
                bl = null;
                br = null;
            }
        }

        public bool Insert(Agent agent) {
            if (!Vector2Intersects(agent.pos))
                return false;

            TryCollapse();

            if (agents.Count < agentsPer) {
                agents.Add(agent);
                return true;

            } else {
                Subdivide();
                if (tl.Insert(agent)) return true;
                if (tr.Insert(agent)) return true;
                if (bl.Insert(agent)) return true;
                if (br.Insert(agent)) return true;
            }

            return false;
        }

        public bool Remove(Agent agent) {
            if (!Vector2Intersects(agent.pos))
                return false;

            if (agents.Contains(agent)) {
                agents.Remove(agent);
                return true;
            } else {
                if (tl != null && tl.Remove(agent)) { return true; }
                if (tr != null && tr.Remove(agent)) { return true; }
                if (bl != null && bl.Remove(agent)) { return true; }
                if (br != null && br.Remove(agent)) { return true; }
            }
            return false;
        }

        public IEnumerable<Agent> Query(Vector2 topLeft, Vector2 bottomRight) {
            System.Collections.Generic.List<Agent> result = new System.Collections.Generic.List<Agent>();

            if (RectIntersects(topLeft, bottomRight))
                foreach (Agent agent in agents)
                    if (agent != null &&
                        agent.pos.x > topLeft.x && agent.pos.y > topLeft.y && agent.pos.x <= bottomRight.x && agent.pos.y <= bottomRight.y)
                        yield return agent;

            if (tl != null) result.AddRange(tl.Query(topLeft, bottomRight));
            if (tr != null) result.AddRange(tr.Query(topLeft, bottomRight));
            if (bl != null) result.AddRange(bl.Query(topLeft, bottomRight));
            if (br != null) result.AddRange(br.Query(topLeft, bottomRight));

            foreach (Agent agent in result)
                yield return agent;
        }

        public IEnumerable<Agent> GetAllChildren() {
            System.Collections.Generic.List<Agent> result = new System.Collections.Generic.List<Agent>();

            result.AddRange(agents);

            if (tl != null) result.AddRange(tl.GetAllChildren());
            if (tr != null) result.AddRange(tr.GetAllChildren());
            if (bl != null) result.AddRange(bl.GetAllChildren());
            if (br != null) result.AddRange(br.GetAllChildren());

            foreach (Agent agent in result)
                yield return agent;
        }

        public Node(Vector2 topLeft, Vector2 bottomRight) {
            agents = new System.Collections.Generic.List<Agent>(4);
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;

            middle = topLeft + (bottomRight - topLeft) / 2;
            topMid = new Vector2(middle.x, topLeft.y);
            leftMid = new Vector2(topLeft.x, middle.y);
            rightMid = new Vector2(bottomRight.x, middle.y);
            bottomMid = new Vector2(middle.x, bottomRight.y);
        }
    }

    public Quadtree(Vector2 topLeft, Vector2 bottomRight) {
        root = new Node(topLeft, bottomRight);
    }
}

class Agent {
    public Quadtree world;
    public Vector2 pos;

    public void Move(Vector2 v) =>
        pos += v;
}
