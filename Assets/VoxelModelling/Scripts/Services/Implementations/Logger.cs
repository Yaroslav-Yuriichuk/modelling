using UnityEngine;

namespace VoxelModelling.Services
{
    public class Logger : ILogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
        }
    }
}