using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Kit.Inventory
{
    /// <summary>
    /// 类《生化危机》的道具
    /// Core: 定义其占用空间
    /// </summary>
    public interface IGridItemDefinition : IItemDefinition
    {
        /// <summary>
        /// 占据的二维空间
        /// 首先是长宽，其次true表示占用这一格，false表示未占用，以便处理不规则图形
        /// </summary>
        bool[,] TakeRoom { get; }
    }

    /// <summary>
    /// 类《生化危机》的格子
    /// Core: 可能和一些格子共同存储同一个道具
    /// </summary>
    public interface IGridItemSlot : IItemSlot
    {
        /// <summary>
        /// 道具占用的所有格子汇总
        /// 引用相同的ItemDefinition
        /// </summary>
        List<IGridItemSlot> SameItemSlots { get; }
    }

    /// <summary>
    /// 类《生化危机》的面向网格的背包系统
    /// </summary>
    public interface IGridInventory : IInventory
    {
        IGridItemSlot[,] ItemSlots { get; }

        bool CanAdd(IGridItemDefinition item);
        bool CanAdd(IGridItemDefinition item, IGridItemSlot slot);
        bool TryAdd(IGridItemDefinition item, int num, out int realAdd);
        bool TryAdd(IGridItemDefinition item, IGridItemSlot slot, int num, out int readAdd);

        bool CanRemove(IGridItemDefinition item);
        bool CanRemove(IGridItemSlot slot);
        bool TryRemove(IGridItemDefinition item);
        bool TryRemove(IGridItemSlot slot, int num, out int readRemove);

        bool Contains(IGridItemDefinition item);
        bool Contains(IGridItemDefinition item, int num);
        bool Contains(IGridItemDefinition item, int num, out int readContains);
    }

    public abstract class GridInventory : IGridInventory
    {
        IGridItemSlot[,] _itemSlots;
        public IGridItemSlot[,] ItemSlots => _itemSlots;

        public GridInventory(int slotLength, int slotWidth)
        {
            _itemSlots = new IGridItemSlot[slotLength, slotWidth];
        }

        public abstract bool CanAdd(IGridItemDefinition item);
        public abstract bool CanAdd(IGridItemDefinition item, IGridItemSlot slot);
        public abstract bool TryAdd(IGridItemDefinition item, IGridItemSlot slot, int num, out int readAdd);
        public abstract bool TryAdd(IGridItemDefinition item, int num, out int realAdd);

        public abstract bool CanRemove(IGridItemDefinition item);
        public abstract bool CanRemove(IGridItemSlot slot);
        public abstract bool TryRemove(IGridItemDefinition item);
        public abstract bool TryRemove(IGridItemSlot slot, int num, out int readRemove);

        public abstract bool Contains(IGridItemDefinition item);
        public abstract bool Contains(IGridItemDefinition item, int num);
        public abstract bool Contains(IGridItemDefinition item, int num, out int readContains);
    }
}