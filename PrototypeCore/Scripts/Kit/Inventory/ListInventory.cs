using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Kit.Inventory
{
    /// <summary>
    /// 类《魔兽世界》的面向长度的背包系统
    /// </summary>
    public interface IListInventory : IInventory
    {
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