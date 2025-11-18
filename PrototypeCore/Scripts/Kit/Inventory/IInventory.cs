using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public interface IInventory
    {
        int Capacity { get; }

        int Count { get; }

        IReadOnlyCollection<IItemSlot> Slots { get; }

        bool CanAdd(IItemDefinition item);
        bool TryAdd(IItemDefinition item);

        bool CanRemove(IItemDefinition item);
        bool TryRemove(IItemDefinition item);

        bool Contains(IItemDefinition item);
    }

    /// <summary>
    /// 类《生化危机》的面向网格的背包系统
    /// </summary>
    public interface IGridInventory
    {

    }

    /// <summary>
    /// 类《上古卷轴》的面向重量的背包系统
    /// </summary>
    public interface IWeightInventory
    {

    }

    /// <summary>
    /// 类《魔兽世界》的面向长度的背包系统
    /// </summary>
    public interface IListInventory
    {

    }
}