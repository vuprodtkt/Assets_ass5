using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

enum Behavior
{
    move,
    attack,
    dead,
    None
}
enum State
{
    waiting,
    success,
    failed
}
enum Type
{
    AND,
    OR,
    LEAF,
    ROOT
}
class Node
{
    public Type type;
    public State state;
    public Behavior behavior;
    public Node parent;
    public List<Node> lstChild;
    public bool isAttack;
    public bool isDead;

    public Node(Type type, State state, Behavior behavior, Node parent = null, bool isattack = false, bool isdead = false)
    {
        this.type = type;
        this.state = state;
        this.behavior = behavior;
        this.parent = null;
        this.parent = parent;
        this.lstChild = new List<Node>();
        this.isAttack = isattack;
        this.isDead = isdead;
    }

}
class BehaviorTree
{
    public Node root;
    public bool isAttack;
    public bool isDead;

    public BehaviorTree(Node root)
    {
        this.root = root;
        this.isAttack = false;
        this.isDead = false;
    }

    public void updateTree()
    {
        this.root = updateNode(this.root);
    }

    private Node updateNode(Node node)
    {
        if (node.type == Type.LEAF)
        {
            if (node.behavior == Behavior.dead)
            {
                if (this.isDead == node.isDead)
                {
                    node.state = State.success;
                    return node;
                }
                else
                {
                    node.state = State.failed;
                    return node;
                }
            }
            else if (node.behavior == Behavior.attack)
            {
                if (this.isAttack == node.isAttack)
                {
                    node.state = State.success;
                    return node;
                }
                else
                {
                    node.state = State.failed;
                    return node;
                }
            }
            else
            {
                node.state = State.success;
                return node;
            }
        }
        else if (node.type == Type.AND)
        {
            foreach (Node e in node.lstChild)
            {
                if (e.state == State.waiting) updateNode(e);

                if (e.state == State.failed)
                {
                    node.state = State.failed;
                    return node;
                }
            }
            node.state = State.success;
            return node;
        }
        else
        {
            foreach (Node e in node.lstChild)
            {
                if (e.state == State.waiting) updateNode(e);

                if (e.state == State.success)
                {
                    node.state = State.success;
                    node.behavior = e.behavior;
                    return node;
                }
            }
            node.state = State.failed;
            return node;
        }
        return node;
    }

    public Behavior getBehavior()
    {
        return root.behavior;
    }

    public void resetTree()
    {
        reset(this.root);
    }

    private void reset(Node node)
    {
        if (node.type == Type.LEAF)
        {
            node.state = State.waiting;
        }
        else if(node.type == Type.AND)
        {
            node.state = State.waiting;
            foreach (Node e in node.lstChild)
            {
                reset(e);
            }
        }
        else
        {
            node.state = State.waiting;
            node.behavior = Behavior.None;

            foreach(Node e in node.lstChild)
            {
                reset(e);
            }
        }
    }
}
