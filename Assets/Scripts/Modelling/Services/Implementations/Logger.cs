using UnityEngine;

namespace Modelling.Services
{
    public class Logger : ILogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
        }
    }
}