using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poco
{
    public class UnityDumper : AbstractDumper
    {
        public override AbstractNode getRoot()
        {
            return new RootNode();
        }
    }

    public class RootNode : AbstractNode
    {
        private List<AbstractNode> children = null;

        public RootNode()
        {
            // Console.WriteLine("RootNode initialized");
            children = new List<AbstractNode>();

            //var go = Transform.FindObjectsOfType(Il2CppType.From(typeof(GameObject)));
            //Debug.Log("ok?");
            // var c = Transform.FindObjectsOfType(Il2CppType.From(typeof(GameObject))).Count;
            // Console.WriteLine("FindObjectsOfType : " + c);

            foreach (var o in Transform.FindObjectsOfType(Il2CppType.From(typeof(GameObject))))
            {
                GameObject? obj = o.TryCast<GameObject>();
                if (obj == null)
                {
                    Console.WriteLine("o.TryCast null!");
                    continue;
                }
                if (obj.transform.parent == null)
                {
                    children.Add(new UnityNode(obj));
                }
            }
        }

        public override List<AbstractNode> getChildren() //<Modified> 
        {
            return children;
        }
        
        public override bool ContainNodeImpl(string text)
        {
            foreach (var c in children)
            {
                if (c.ContainNodeImpl(text))
                    return true;
            }
            return false;
        }
		
        public override bool SetFieldNameImpl(string text)
        {
            foreach (var c in children)
            {
                if (c.SetFieldNameImpl(text))
                    return true;
            }

            return false;
        }

        public override AbstractNode? GetFieldNameNode()
        {
            foreach (var c in children)
            {
                var n = c.GetFieldNameNode();
                if (n != null)
                    return n;
            }

            return null;
        }
    }
}
