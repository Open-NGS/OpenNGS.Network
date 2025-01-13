using Neptune.Datas;
using System;
using System.Collections.Generic;

namespace GameData
{
    public class SummonActData
    {   
        /// <summary>
        /// 事件类型
        /// </summary>
        public SummonTodoType SummonTodoType { get; set; }
        /// <summary>
        /// 召唤单位ID
        /// </summary>
        public int ToDoSummonID { get; set; }
        /// <summary>
        /// 不同事件对应不同的ID 如技能ID等
        /// </summary>
        public int SummonToDoParam { get; set; }

    }
}
