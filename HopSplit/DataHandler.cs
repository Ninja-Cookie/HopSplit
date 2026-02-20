using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace HopSplit
{
    internal static class DataHandler
    {
        private     readonly static string  PATH_Dir                = Paths.ConfigPath;
        private     const string            PATH_FolderName         = "HopSplit";
        private     const string            PATH_FileName_Splits    = "splits";
        private     const string            PATH_FileName_Settings  = "settings";
        private     const string            PATH_FileType           = "frog";
        internal    const int               VERSION                 = 1;

        internal readonly static string     PATH_FullDir            = Path.Combine(PATH_Dir, PATH_FolderName);
        internal readonly static string     PATH_FullPath_Splits    = Path.Combine(PATH_FullDir, $"{PATH_FileName_Splits}.{PATH_FileType}");
        internal readonly static string     PATH_FullPath_Settings  = Path.Combine(PATH_FullDir, $"{PATH_FileName_Settings}.{PATH_FileType}");

        private readonly static Dictionary<Type, DataContractSerializer> Serializers = new Dictionary<Type, DataContractSerializer>()
        {
            { typeof(Data_Splits),      new DataContractSerializer(typeof(Data_Splits))     },
            { typeof(Data_Settings),    new DataContractSerializer(typeof(Data_Settings))   }
        };

        [DataContract]
        private class Wrapper_Split
        {
            [DataMember]
            internal string Split;

            [DataMember]
            internal bool State;

            internal Wrapper_Split(string split, bool state)
            {
                Split = split;
                State = state;
            }
        }

        [DataContract]
        private class Data_Splits
        {
            [DataMember]
            internal int Version = VERSION;

            [DataMember]
            internal Wrapper_Split[] Splits = Array.Empty<Wrapper_Split>();

            internal Data_Splits(Wrapper_Split[] splits)
            {
                Splits = splits;
            }
        }

        [DataContract]
        private class Data_Settings
        {
            [DataMember]
            internal int Version = VERSION;

            [DataMember]
            internal bool ForceSyncTime = false;

            [DataMember]
            internal bool DisplayFPS = true;

            internal Data_Settings(bool forceSyncTime, bool displayFPS)
            {
                ForceSyncTime   = forceSyncTime;
                DisplayFPS      = displayFPS;
            }
        }

        internal static void Save()
        {
            SaveSettings();
            SaveSplits();
        }

        private static void SaveData<T>(T data, string path) where T : class
        {
            if (!Serializers.TryGetValue(typeof(T), out var serializer))
                return;

            try
            {
                if (!Directory.Exists(PATH_FullDir))
                    Directory.CreateDirectory(PATH_FullDir);

                if (Directory.Exists(PATH_FullDir))
                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                        serializer.WriteObject(stream, data);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static void SaveSplits()
        {
            List<Wrapper_Split> splits = new List<Wrapper_Split>();
            foreach (var split in ConfigHandler.ActiveSplits)
                splits.Add(new Wrapper_Split(split.Key, split.Value));

            SaveData(new Data_Splits(splits.ToArray()), PATH_FullPath_Splits);
        }

        private static void SaveSettings()
        {
            SaveData(new Data_Settings(ConfigHandler.ForceSyncTime, ConfigHandler.DisplayFPS), PATH_FullPath_Settings);
        }

        internal static void LoadAll()
        {
            LoadSettings();
            LoadSplits();
        }

        private static T LoadData<T>(string path) where T : class
        {
            if (!File.Exists(path) || !Serializers.TryGetValue(typeof(T), out var serializer))
                return null;

            try
            {
                T data = null;
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    data = (T)serializer.ReadObject(stream);

                return data;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        internal static void LoadSplits()
        {
            var splitData = LoadData<Data_Splits>(PATH_FullPath_Splits);
            if (splitData == null || splitData.Version != VERSION)
                return;

            foreach (var split in splitData.Splits)
            {
                if (ConfigHandler.ActiveSplits.TryGetValue(split.Split, out _))
                    ConfigHandler.ActiveSplits[split.Split] = split.State;
            }
        }

        internal static void LoadSettings()
        {
            var settingsData = LoadData<Data_Settings>(PATH_FullPath_Settings);
            if (settingsData == null || settingsData.Version != VERSION)
                return;

            ConfigHandler.ForceSyncTime = settingsData.ForceSyncTime;
            ConfigHandler.DisplayFPS    = settingsData.DisplayFPS;
        }
    }
}
