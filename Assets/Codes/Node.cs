using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public interface ILogicNode
{
    void Attach(ILogicNode node);
    void Detach(ILogicNode node);
    void OnAttach(ILogicNode parent);
    void OnDetach(ILogicNode parent);

    void UpdateLogic(float deltaTime);

    void OnUpdateLogic(float deltaTime);

    ILogicNode Parent { get; set; }
    List<ILogicNode> Children { get; set; }
}


public class MonoLogicNode : MonoBehaviour, ILogicNode
{
    public ILogicNode Parent { get; set; }
    public List<ILogicNode> Children { get; set; }

    public void Attach(ILogicNode node)
    {
        if (node == null)
        {
            Debug.LogError("attachNode node is null");
            return;
        }

        node.Parent = this;
        Children.Add(node);
        node.OnAttach(this);
    }

    public void Detach(ILogicNode node)
    {
        if (node == null)
        {
            Debug.LogError("DetachNode node is null");
            return;
        }

        Children.Remove(node);
        node.OnDetach(this);
        node.Parent = null;

    }

    public virtual void OnAttach(ILogicNode parent)
    {

    }

    public virtual void OnDetach(ILogicNode parent)
    {

    }

    public virtual void OnUpdateLogic(float deltaTime)
    {

    }

    public void UpdateLogic(float deltaTime)
    {
        OnUpdateLogic(deltaTime);
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].UpdateLogic(deltaTime);
        }
    }


}



public class LogicNode : ILogicNode
{
    public ILogicNode Parent { get; set; }
    public List<ILogicNode> Children { get; set; }

    public void Attach(ILogicNode node)
    {
        if (node == null)
        {
            Debug.LogError("attachNode node is null");//TODO: 这里用在非主线程要改打印日志
            return;
        }

        node.Parent = this;
        Children.Add(node);
        node.OnAttach(this);
    }

    public void Detach(ILogicNode node)
    {
        if (node == null)
        {
            Debug.LogError("DetachNode node is null");
            return;
        }

        Children.Remove(node);
        node.OnDetach(this);
        node.Parent = null;

    }

    public virtual void OnAttach(ILogicNode parent)
    {

    }

    public virtual void OnDetach(ILogicNode parent)
    {

    }

    public virtual void OnUpdateLogic(float deltaTime)
    {

    }

    public void UpdateLogic(float deltaTime)
    {
        OnUpdateLogic(deltaTime);
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].UpdateLogic(deltaTime);
        }
    }
}




