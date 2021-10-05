using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyFileWriter
{
    public string path = "Assets/LogBattles/log.txt";

    public void writeLog(string prefix, List<float> values, bool keepData = true)
    {
        StreamWriter writer = new StreamWriter(path, keepData);

        string toW = prefix;

        foreach(float v in values)
        {
            toW += " " + v;
        }

        writer.WriteLine(toW);
        writer.Close();
    }
}
