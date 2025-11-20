using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Kit.Inventory
{
    /// <summary>
    /// 道具栏位接口
    /// 根据游戏类型不同可以有不同的实现方式
    /// 
    /// 类《魔兽世界》：总长度确定（除非背包扩容），可堆叠，可为空
    /// 类《生化危机》：总长度确定（除非背包扩容），物品尺寸不定，可能与其他栏位共用一个物品
    /// 类《上古卷轴》：总长度动态变化，必定绑定一个ItemDefinition
    /// </summary>
    public interface IItemSlot
    {
        IItemDefinition ItemDefinition { get; }
        bool HasItem { get; }
        bool IsEmpty { get; }
        int Quantity { get; }

        bool[,] Size { get; }      //占用格子
        int Weight { get; }        //重量
    }
}