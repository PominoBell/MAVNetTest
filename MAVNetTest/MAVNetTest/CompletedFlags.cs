using System.Collections.Generic;

namespace MAVNetTest
{
    static class CompletedFlags
    {
        private static List<bool> _completedBools = new List<bool>();

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
