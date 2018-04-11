using System.Collections.Generic;

namespace MAVNetTest
{
    static class CompletedFlags
    {
        /// <summary>
        /// 用于存储完成标志的列表
        /// A list is used for saving the Completed Flag 
        /// </summary>
        private static List<bool> _completedBools = new List<bool>();

        /// <summary>
        /// 初始化列表
        /// Initialize the list
        /// </summary>
        /// <param name="amount">Amount of flags</param>
        public static void Initialization(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                lock (_completedBools)
                {
                    _completedBools.Add(false);
                }
            }
        }

        /// <summary>
        /// 任务完成，更改标志
        /// While misstion completed, change the flag.
        /// </summary>
        public static void Completed()
        {
            lock (_completedBools)
            {
                for (var i = 0; i < _completedBools.Count; i++)
                {
                    if (_completedBools[i] == false)
                    {
                        _completedBools[i] = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否所有线程任务均已完成
        /// Judge whether all missions are completed
        /// </summary>
        /// <returns></returns>
        public static bool IsAllCompleted()
        {
            lock (_completedBools)
            {
                foreach (var flag in _completedBools)
                {
                    if (flag == false)
                        return false;
                }
                return true;
            }
        }
    }
}
