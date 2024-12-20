using System;
using UnityEngine;

namespace Game.PreferencesSaveType
{
public abstract class BaseSavableValue<T> : IDisposable
{
    private string _playerPrefsKey;
    private bool _valueInitialize;
    protected readonly T defaultValue;
    protected T cachedValue;
    private static event Action OnValueChangedOutside;
    
    protected BaseSavableValue(string playerPrefsKey, T defaultValue = default)
    {
        if (string.IsNullOrEmpty(playerPrefsKey))
            throw new Exception("Empty playerPrefsPath in savableValue");
        
        _playerPrefsKey = playerPrefsKey;
        this.defaultValue = defaultValue;
        _valueInitialize = false;
        OnValueChangedOutside += ProcessValueChangedOutside;
    }

    public T Value
    {
        get
        {
            if (_valueInitialize) 
                return cachedValue;
            
            cachedValue = defaultValue;
            if (ContainsStoredValue())
                cachedValue = LoadValue(ref _playerPrefsKey);
            else
                SaveValue(ref _playerPrefsKey);
            _valueInitialize = true;

            return cachedValue;
        }
        set
        {
            if (cachedValue.Equals(value) == false)
                SaveValue(ref _playerPrefsKey);
            cachedValue = value;
            _valueInitialize = true;
        }
    }

    protected abstract T LoadValue(ref string path);
    
    protected abstract void SaveValue(ref string path);

    private bool ContainsStoredValue() => PlayerPrefs.HasKey(_playerPrefsKey);

    public void Reset()
    {
        PlayerPrefs.DeleteKey(_playerPrefsKey);
        _valueInitialize = false;
    }

    private void ProcessValueChangedOutside()
    {
        _valueInitialize = false;
    }
    
    public void Dispose()
    {
        OnValueChangedOutside -= ProcessValueChangedOutside;
        var autoSave = Value;
    }
    
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        OnValueChangedOutside?.Invoke();
    }
    
    public static void SaveAll()
    {
        PlayerPrefs.Save();
        OnValueChangedOutside?.Invoke();
    }
}
}