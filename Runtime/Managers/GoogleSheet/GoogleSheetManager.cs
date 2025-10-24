using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Managers.GoogleSheet
{
public sealed class GoogleSheetManager
{
    private readonly string _url;

    public GoogleSheetManager(string url)
    {
        _url = url;
    }

    public bool TryLoadUnityJSON<T>(int row, int column, out T instance, bool keepSpaces = false)
    {
        instance = default;

        if (TryLoadPage(out var settingsTable) == false) return false;

        instance = JsonUtility.FromJson<T>(GetCleanJSON(settingsTable[row][column], keepSpaces));

        return true;
    }

    public bool TryLoadNewtonsoftJSON<T>(int row, int column, out T instance, bool keepSpaces = false)
    {
        instance = default;

        if (TryLoadPage(out var settingsTable) == false) return false;

        instance = JsonConvert.DeserializeObject<T>(GetCleanJSON(settingsTable[row][column], keepSpaces));

        return true;
    }

    /*
      private static readonly Regex CsvMultilineFixRegex = new Regex("\"([^\"]|\"\"|\\n)*\"");
      ...
      {
         data = CsvMultilineFixRegex.Replace(data, m => m.Value.Replace("\n", "\\n"));
        // table
        var table = new List<string[]>();

        var rows = data.Split('\n');
        foreach (var row in rows)
            table.Add(Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"));

        return table;
       }
     */
    public bool TryLoadPage(out List<string[]> page)
    {
        page = default;

        if (TryGetCSV(out var data) == false)
            return false;
        page = new Regex("\"([^\"]|\"\"|\\n)*\"").Replace(data, m => m.Value.Replace("\n", "\\n")).Split('\n')
                                                 .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                                 .ToList();

        return true;
    }

    private bool TryGetCSV(out string rawCSV)
    {
        rawCSV = string.Empty;
        try
        {
            using (var www = new WebClient())
            {
                rawCSV = www.DownloadString(_url.Replace("/edit?", "/export?format=csv&"));
            }
        }
        catch (Exception e)
        {
            Log.Error($"Can't get csv {e.Message}");

            return false;
        }

        return true;
    }

    private static string GetCleanJSON(string json, bool keepSpaces = false)
    {
        json = json.Replace("\"\"", "\"")
                   .Replace("\"{", "{")
                   .Replace("}\"", "}")
                   .Replace("\\n", "")
                   .Replace("\r", "")
                   .Replace("FALSE", "false")
                   .Replace("TRUE", "true");

        if (!keepSpaces)
        {
            json = json.Replace(" ", "");
        }

        return json;
    }
}
}