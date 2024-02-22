using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
    private StreamWriter logger;

    public void WriteLog(string logPath, List<string> logs)
    {
        logger = new StreamWriter(logPath, false, Encoding.UTF8);
        foreach (string logData in logs)
        {
            logger.Write(logData);
        }
        
        logger.Close();
    }
    
    
}
