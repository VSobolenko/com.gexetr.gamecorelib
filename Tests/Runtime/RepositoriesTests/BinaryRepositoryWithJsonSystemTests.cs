﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Game.IO;
using Game.IO.Managers;
using Game.Tests.Runtime.TestingElements;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.Runtime.RepositoriesTests
{
[TestFixture]
internal class BinaryRepositoryWithJsonSystemTests
{
    private const string FileFormat = ".bat";

    private TestClassWithUnityVectorAndQuaternion ReadableClass => new TestClassWithUnityVectorAndQuaternion
        {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
    
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
        var jsonString = new JsonUnitySave().Serialize(obj);
        
        var bf = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            bf.Serialize(stream, jsonString);
            return stream.ToArray();
        }
    }
    
    [Test]
    public void Read_DataFromBinaryFile_ShouldReturnNewClassWithData()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        
        // Act
        var data = saveFile.Read<TestClassWithUnityVectorAndQuaternion>(_pathToReadFile);
        
        // Assert
        data.Should().BeEquivalentTo(ReadableClass);
    }
    
    [Test]
    public void Write_DataToFile_ShouldSaveNewClassDataToFile()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        var testData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var savedData = GetSavedData(testData);
        
        // Act
        saveFile.Write(_pathToWriteFile, testData, FileMode.Create);
        var fileData = File.ReadAllBytes(_pathToWriteFile + FileFormat);
    
        // Assert
        fileData.Should().BeEquivalentTo(savedData);
    }
    
    [Test]
    public void Write_DataToFileWithData_ShouldSaveNewWriteData()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        var firstTestData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var secondTestData = new TestClassWithUnityVectorAndQuaternion
            {id = 8, name = "Enemy", position = Vector3.one, rotation = Quaternion.identity};
        var firstSaveData = GetSavedData(firstTestData);
        var secondSaveData = GetSavedData(secondTestData);
        
        // Act
        saveFile.Write(_pathToWriteFile, firstTestData, FileMode.Create);
        saveFile.Write(_pathToWriteFile, secondTestData, FileMode.Create);
        var fileData = File.ReadAllBytes(_pathToWriteFile + FileFormat);
    
        // Assert
        fileData.Should().BeEquivalentTo(secondSaveData);
        fileData.Should().NotBeEquivalentTo(firstSaveData);
    }
    
    [Test]
    public void Delete_ExistFile_ShouldFolderWithoutFile()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        
        // Act
        saveFile.Delete(_pathToDeleteFile);
    
        // Assert
        Assert.IsFalse(File.Exists(_pathToDeleteFile + FileFormat));
    }
    
    [Test]
    public void Exist_CheckAvailableFile_ShouldReturnTrue()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        
        // Act
        var fileExist = saveFile.IsFileExist(_pathToExistFile);
    
        // Assert
        Assert.IsTrue(fileExist);
    }
    
    [Test]
    public void Exist_CheckNotAvailableFile_ShouldReturnFalse()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        
        // Act
        var fileExist = saveFile.IsFileExist(_pathToNotExistFile);
    
        // Assert
        Assert.IsFalse(fileExist);
    }
    
    [Test]
    public void Serialize_ClassWithData_ShouldReturnSerializeToJsonString()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        var testData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var serializeBytesData = GetSavedData(testData);
        
        // Act
        var serializableData = saveFile.Serialize(testData);
    
        // Assert
        serializableData.Should().BeEquivalentTo(serializeBytesData);
    }
    
    [Test]
    public void Deserialize_JsonString_ShouldReturnNewClassWithData()
    {
        // Arrange
        ISaveFile saveFile = new BinarySaveWithJsonSystem(new JsonUnitySave());
        var testData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var serializeBytesData = GetSavedData(testData);
        
        // Act
        var deserializableClass = saveFile.Deserialize<TestClassWithUnityVectorAndQuaternion>(serializeBytesData);
    
        // Assert
        deserializableClass.Should().BeEquivalentTo(testData);
    }
}
}