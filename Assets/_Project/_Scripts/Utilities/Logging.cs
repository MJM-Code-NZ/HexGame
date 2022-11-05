using UnityEngine;

namespace MJM.HG
{
    public static class Logging
    {
        private static Logger _generalLogger;
        public static Logger GeneralLogger { get { return _generalLogger; } }

        private static Logger _sceneLogger;
        public static Logger SceneLogger { get { return _sceneLogger; } }
        public static void LoadLoggers()
        {
            // Call this function when the game starts
            _generalLogger = new Logger(Debug.unityLogger.logHandler); 
            _sceneLogger = new Logger(Debug.unityLogger.logHandler);

            _generalLogger.logEnabled = false;
            _sceneLogger.logEnabled = false;
        }
    }
}
