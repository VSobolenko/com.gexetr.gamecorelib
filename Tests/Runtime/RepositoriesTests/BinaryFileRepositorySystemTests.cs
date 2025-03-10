﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Game.IO;
using Game.IO.Managers;
using Game.Tests.Runtime.TestingElements;
using NUnit.Framework;

namespace Game.Tests.Runtime.RepositoriesTests
{
[TestFixture]
internal class BinaryFileRepositorySystemTests
{
#if UNITY_EDITOR
    private const string FileFormat = ".txt";
#else
    private const string FileFormat = ".txt";
#endif

    private static SimpleTestClass ReadableClass => new SimpleTestClass {id = 7, name = "Player", };

    private string _testFolderPath;
    private bool _deleteTestFolder = true;
    
    private string _pathToReadFile;
    private string _pathToWriteFile;
    private string _pathToDeleteFile;
    private string _pathToExistFile;
    private string _pathToNotExistFile;
    
    [OneTimeSetUp]
    public void Init()
    {
        var activeExeLocation = Assembly.GetExecutingAssembly().Location;
        _testFolderPath = Path.GetDirectoryName(activeExeLocation) + $@"\{Guid.NewGuid()}\";

        if (Directory.Exists(_testFolderPath) == false)
            Directory.CreateDirectory(_testFolderPath);
        else
        {
            Log.Warning("The folder for temporary files already exists. An empty folder is required!");
            _deleteTestFolder = false;
        }
        
        // Read file
        _pathToReadFile = Path.Combine(_testFolderPath + "ReadFile");
        var savableData = GetSavedData(ReadableClass);
        File.WriteAllBytes(_pathToReadFile + FileFormat, savableData);
        
        // Write file
        _pathToWriteFile = Path.Combine(_testFolderPath + "WriteFile");
        
        // Delete file
        _pathToDeleteFile = Path.Combine(_testFolderPath + "DeleteFile");
        if (File.Exists(_pathToDeleteFile + FileFormat) == false)
            using (File.Create(_pathToDeleteFile + FileFormat))
            { }

        // Exists file
        _pathToExistFile = Path.Combine(_testFolderPath + "ExistFile");
        if (File.Exists(_pathToExistFile + FileFormat) == false)
            using (File.Create(_pathToExistFile + FileFormat))
            { }

        // Not exists file
        _pathToNotExistFile = _testFolderPath + "NotExistFile" + FileFormat;
    }
        
    [OneTimeTearDown]
    public void Revert()
    {
        if (_deleteTestFolder)
            Directory.Delete(_testFolderPath, true);
    }

    private byte[] GetSavedData(object obj)
    {
        var bf = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            bf.Serialize(stream, obj);
            return stream.ToArray();
        }
    }
    
    [Test]
    public void Read_DataFromBinaryFile_ShouldReturnNewClassWithData()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        
        // Act
        var data = saveFile.Read<SimpleTestClass>(_pathToReadFile);
        
        // Assert
        Assert.AreEqual(data, ReadableClass);
    }
    
    [Test]
    public void Write_DataToFile_ShouldSaveNewClassDataToFile()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        var testData = new SimpleTestClass {id = 7, name = "Player",};
        var savedData = GetSavedData(testData);
        
        // Act
        saveFile.Write(_pathToWriteFile, testData, FileMode.Create);
        var fileData = File.ReadAllBytes(_pathToWriteFile + FileFormat);
    
        // Assert
        Assert.AreEqual(fileData, savedData);
    }
    
    [Test]
    public void Write_DataToFileWithData_ShouldSaveNewWriteData()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        var firstTestData = new SimpleTestClass {id = 7, name = "Player", };
        var secondTestData = new SimpleTestClass {id = 8, name = "Enemy", };
        var firstSaveData = GetSavedData(firstTestData);
        var secondSaveData = GetSavedData(secondTestData);
        
        // Act
        saveFile.Write(_pathToWriteFile, firstTestData, FileMode.Create);
        saveFile.Write(_pathToWriteFile, secondTestData, FileMode.Create);
        var fileData = File.ReadAllBytes(_pathToWriteFile + FileFormat);
    
        // Assert
        Assert.AreEqual(fileData, secondSaveData);
        Assert.AreNotEqual(fileData, firstSaveData);
    }
    
    [Test]
    public void Delete_ExistFile_ShouldFolderWithoutFile()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        
        // Act
        saveFile.Delete(_pathToDeleteFile);
    
        // Assert
        Assert.IsFalse(File.Exists(_pathToDeleteFile + FileFormat));
    }
    
    [Test]
    public void Exist_CheckAvailableFile_ShouldReturnTrue()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        
        // Act
        var fileExist = saveFile.IsFileExist(_pathToExistFile);
    
        // Assert
        Assert.IsTrue(fileExist);
    }
    
    [Test]
    public void Exist_CheckNotAvailableFile_ShouldReturnFalse()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        
        // Act
        var fileExist = saveFile.IsFileExist(_pathToNotExistFile);
    
        // Assert
        Assert.IsFalse(fileExist);
    }
    
    [Test]
    public void Serialize_ClassWithData_ShouldReturnSerializeToJsonString()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        var testData = new SimpleTestClass {id = 7, name = "Player", };
        var serializeBytesData = GetSavedData(testData);
        
        // Act
        var serializableData = saveFile.Serialize(testData);
    
        // Assert
        Assert.AreEqual(serializableData, serializeBytesData);
    }
    
    [Test]
    public void Deserialize_JsonString_ShouldReturnNewClassWithData()
    {
        // Arrange
        ISaveFile saveFile = new BinarySave();
        var testData = new SimpleTestClass {id = 7, name = "Player", };
        var serializeBytesData = GetSavedData(testData);
        
        // Act
        var deserializableClass = saveFile.Deserialize<SimpleTestClass>(serializeBytesData);
    
        // Assert
        Assert.AreEqual(deserializableClass, testData);
    }
}
}