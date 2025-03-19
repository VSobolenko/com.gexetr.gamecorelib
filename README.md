# Installation (GCL)

- [Shared DLL](https://github.com/VSobolenko/com.gexetr.gamecorelib-shared) (Requires updating)
- Via a git link in the PackageManager 
  ```
  https://github.com/VSobolenko/com.gexetr.gamecorelib.git
  ```
- Editing of `Packages/manifest.json`
  ```
  "com.gexetr.gamecorelib": "https://github.com/VSobolenko/com.gexetr.gamecorelib-shared.git?path=.com.gexetr.gcl",
  ```
- Git Submodule
  ```sh
  git submodule add https://github.com/VSobolenko/com.gexetr.gamecorelib Packages/com.gexetr.gamecorelib
  ```
  
# Dependency

## General 

- Addressables (Resource Managements)
- TextMeshPro (Localizations/Editor)
- Newtonsoft Json (FileIO)
- Dotween (GUI)
- NuGetForUnity (Tests)

## Fast Setup
#### Dotween (GUI)
- [Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676?srsltid=AfmBOooq1yDxnwheWQR_-8s6nq6kAAt4eTU0B3ty3MN30Cj5MoE4V6T7)
- [Official Website](https://dotween.demigiant.com/download.php)

<!--
#### UniTask (ObjectPool)
- [GitHub](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package#:~:text=UPM%20Package)
- (UPM) `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`
-->

#### NuGetForUnity (Tests)
- (UPM) `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity`
    - Moq

---

# Contains

### FSM
| Managers               | States                          | Transitions                     |
|------------------------|---------------------------------|---------------------------------|
| • Lite ST<br>• FSM<br>• Hybrid (WIP) | • DeadState `-i`<br>• QuiteState<br>• State `-io` | • Alive `-oi`<br>• Many2One `-ooi`<br>• Entry `-i`<br>• Dead `-o`<br>• Circle `-circle -i` |


### Managers
- Audio
- Factories
- FileIO
- GoogleSheet
- GUI
- Inputs
- Localizations
- ObjectPool
- Repositories
- Resource Managements

---

# Don't Forget
- TextMeshPro
- [Zenject](https://github.com/modesttree/Zenject?tab=readme-ov-file#installation-) / [VContainer](https://vcontainer.hadashikick.jp/getting-started/installation)
- `.gitignore`, `.gitattributes`, `.gitconfig`, `.editorconfig`
- `.asmdef`
- `AssemblyInfo`
- [NiceVibrations](https://nice-vibrations.moremountains.com/)
- Android Logcat
- URP / HDRP
- [CI / CD](https://serverspace.by/)
- [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes?tab=readme-ov-file#Installation) `https://github.com/dbrizov/NaughtyAttributes.git?path=Assets/NaughtyAttributes`
- [Unity SerializeReferenceExtensions](https://github.com/mackysoft/Unity-SerializeReferenceExtensions)
- [UniTask](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package)
- [R3-General](https://github.com/Cysharp/R3?tab=readme-ov-file#unity) `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity`, [NuGet](https://github.com/GlitchEnzo/NuGetForUnity.git) (R3, ObservableCollections, ObservableCollections.R3)

---

# Project Structure
```
|-- Assets
    |-- _GameName
        |-- Code
            |-- Common/Editor/UI/DI
            |-- Game(Client)
        |-- DynamicAssets
            |-- Prefabs/Resources/Configs
        |-- SandBox
        |-- Scenes
        |-- Shaders
        |-- StaticAssets
            |-- Animations/Sprites
            |-- Textures/Music
            |-- Materials/Models
    |-- Import           // Import assets
    |-- SandBox          // Test files
    |-- StreamingAssets
```

---
<!--   To myself, so as not to forget
# Basic Optimization
- **Enable Physics Layer if needed** (default: all disabled)
    - `Edit -> Project Settings -> Layer Collision Matrix`
- **Optimize Draw Calls:**
    1. Enable Static Batching: `Edit -> Project Settings -> Player -> Other Settings -> Rendering -> Static Batching`
    2. Enable Dynamic Batching: `Edit -> Project Settings -> Player -> Other Settings -> Rendering -> Dynamic Batching`
    3. Enable GPU Instancing in Material: `Material -> Advanced Option -> Enable GPU Instancing`
- **Use Linear Color Space:**
    - `Edit -> Project Settings -> Player -> Other Settings -> Rendering -> Color Space`
- **Use Sprite Atlas:**
    - Enable in `Edit -> Project Settings -> Editor -> Sprite Packer -> Always Enabled`
- **Disable UI Raycast Target** on GameObjects that don't require it
- **Disable Pixel Perfect** in Canvas (if not needed): `Canvas -> Pixel Perfect`
- **Enable Managed Code Stripping**
- **Mark static GameObjects as 'Static'**

---

# Scenes
- If there's only one scene, name it: **Main**
- Test scene should be named: **Test**
- Always use meaningful scene names!
-->

<!--   Быстрое руководство, как работать с git modules
#### //SETUP
- `git submodule add https://github.com/VSobolenko/com.gexetr.gamecorelib` - add submodule to folder root
- add submodule to folder Packages (./GameCor/.git):
```
git submodule add https://github.com/VSobolenko/com.gexetr.gamecorelib Packages/com.gexetr.gamecorelib
```

.. git clone https://github.com/VSobolenko...
- `git submodule init` - run this command after "git clone" to initialize the submodule 
- `git submodule update` - run this command after "git submodule init" to fetch repository data and checkout commit(analog "git clone" but for a submodule)

что бы не делать init и update после git clone, а автоматически сразу инициализировать подмодули, можно клонировани с параметром "--recurse-submodules"
- `git clone --recurse-submodules` https://github.com/VSobolenko

если клонирование было без параметра "--recurse-submodules", то можно выполнить команду ниже, для быстрой инициализации и репозитория
- `git submodule update --init` - объединение команд "git submodule init" и "git submodule update"
- `git submodule update --init --recursive` - объединение команд "git submodule init" и "git submodule update"

#### //UPDATE
для получения изменений, необходимо перейти в папку с подмодулем и выполнить команды "git fetch" и "git merge origin/master"
- `git submodule update --remote [ModuleName]` - автоматическое выполнение команд "git fetch" и "git merge origin/master", до текущего состояния ветки master
- `git submodule update --remote --merge [ModuleName]` - автоматическое выполнение команд "git fetch" и "git merge origin/master", до текущего состояния ветки master
- `git config -f .gitmodules submodule.DbConnector.branch [BranchName]` - установление ветки [BranchName] по умолчанию
- `git push --recurse-submodules=on-demand` - отправить изменения и локальные и всех подмодулей
- `git push --recurse-submodules=check` - отправить локальные изменения и если присутсвует изменения в подмодуле остановить всё выполнение команды

#### //USEFUL
- `git config status.submodulesummary 1` - отображать краткие сведения для сабмодуля при выполнении команды git status
- `git config --global diff.submodule log` -более детальныя информация при команде git diff

#### //DELETE
- удалить сам модуль
- удалить .gitmodule
- удалить .git/modules
- удалить .git/config/submodule
-->
