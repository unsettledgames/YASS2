using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviours
{
    public class PathNode
    {
        public Vector3 position;
        public float radius;

        public PathNode(Vector3 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }


    public class Path
    {
        private List<PathNode> nodes;
        private int currIndex;
        private int direction;
        private bool hasReversed;

        public bool loop;
        public bool reverseAtEnd;

        public Path(List<PathNode> nodes, bool loop = false, bool reverseAtEnd = false, int startIndex = 0, int startDirection = 1)
        {
            this.nodes = nodes;
            this.loop = loop;
            this.reverseAtEnd = reverseAtEnd;

            this.currIndex = startIndex;
            this.direction = startDirection;
            this.hasReversed = false;
        }

        public PathNode ReverseNow()
        {
            direction *= -1;
            hasReversed = true;

            return Next();
        }
        
        public PathNode GetCurrentNode()
        {
            return nodes[currIndex];
        }

        public PathNode Next()
        {
            int nextIndex = currIndex + 1 * direction;
            PathNode ret;

            hasReversed = false;

            if (nextIndex < 0)
            {
                if (loop)
                {
                    nextIndex = nodes.Count - 1;
                }
                else if (reverseAtEnd)
                {
                    hasReversed = true;
                    nextIndex = 0;
                    direction *= -1;
                }
                else
                {
                    nextIndex = -1;
                }
            }
            else if (nextIndex >= nodes.Count)
            {
                if (loop)
                {
                    nextIndex = 0;
                }
                else if (reverseAtEnd)
                {
                    hasReversed = true;
                    nextIndex -= 2;
                    direction *= -1;
                }
                else
                {
                    nextIndex = -1;
                }
            }

            if (nextIndex >= 0)
            {
                ret = nodes[nextIndex];
                currIndex = nextIndex;
            }
            else
            {
                ret = null;
            }

            Debug.Log("Curr index: " + currIndex);

            return ret;
        }
        
        public bool Reversed()
        {
            return hasReversed;
        }
    }

}

