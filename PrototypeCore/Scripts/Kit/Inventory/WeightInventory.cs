using Prototype.Kit.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Kit.Inventory
{
    /// <summary>
    /// 类《上古卷轴》的道具
    /// Core: 定义其占用空间
    /// </summary>
    public interface IWeightItemDefinition : IItemDefinition
    {
        /// <summary>
        /// 占据的二维空间
        /// 首先是长宽，其次true表示占用这一格，false表示未占用，以便处理不规则图形
        /// </summary>
        int Weight { get; }
    }

    /// <summary>
    /// 类《上古卷轴》的面向重量的背包系统
    /// </summary>
    public interface IWeightInventory : IInventory
    {
        /// <summary>
        /// 负重上限
        /// </summary>
        int MaxWeight { get; }

        /// <summary>
        /// 当前重量
        /// </summary>
        int CurrentWeight { get; }

        /// <summary>
        /// 当前道具栏位
        /// </summary>
        List<IItemSlot> ItemSlots { get; }

        bool CanAdd(IGridItemDefinition item);
        bool CanAdd(IGridItemDefinition item, IItemSlot slot);
        bool TryAdd(IGridItemDefinition item, int num, out int realAdd);
        bool TryAdd(IGridItemDefinition item, IItemSlot slot, int num, out int readAdd);

        bool CanRemove(IGridItemDefinition item);
        bool CanRemove(IItemSlot slot);
        bool TryRemove(IGridItemDefinition item);
        bool TryRemove(IItemSlot slot, int num, out int readRemove);

        bool Contains(IGridItemDefinition item);
        bool Contains(IGridItemDefinition item, int num);
        bool Contains(IGridItemDefinition item, int num, out int readContains);
    }
}