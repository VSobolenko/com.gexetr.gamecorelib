### Dependency:
Dotween(GUI)
 1. `https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676?srsltid=AfmBOooq1yDxnwheWQR_-8s6nq6kAAt4eTU0B3ty3MN30Cj5MoE4V6T7`
 2. `https://dotween.demigiant.com/download.php`
    
Addressables (Resource Managements)
 1. `com.unity.addressables` (Unity Registry)
 2. `https://github.com/needle-mirror/com.unity.addressables`

In App Purchasing(Shop)
 1. `com.unity.purchasing` (Unity Registry)
 2. `https://github.com/needle-mirror/com.unity.purchasing`

UniTask (ObjectPool)
 1. `https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package#:~:text=UPM%20Package`
 2. (UPM) `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`

R3(General) + NuGetForUnity-FluentAssertion(Tests)
 1. Description: `https://github.com/Cysharp/R3?tab=readme-ov-file#unity` (R3-general)
 2. (UPM) `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity` (NuGet)
   - R3
   - ObservableCollections
   - ObservableCollections.R3
   - FluentAssertions
 3. (UPM) `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity` (R3-unity)

GameCoreLib (GCL)
 1. (UMP + Git) `https://github.com/VSobolenko/com.gexetr.gamecorelib`
 2. (dll) `https://github.com/VSobolenko/com.gexetr.gamecorelib-shared`

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

FSM
1. Managers
 - Lite ST
 - FSM
2. States
 - DeadState -i
 - QuiteState
 - State -io
3. Transitions
 - Alive -io
 - Circle -circle -i
 - Dead -o
 - Entry -i
 - Many2One -ooi

Managers
1. Ads 
2. Audio
3. Factories
4. FileIO
5. GUI
6. Inputs 
7. Localizations
8. ObjectPool
9. Repositories
10. Resource Managements
11. Shops

Don't forget:
1. TextMeshPro
2. Zenject / VContainer
3. Addressables
4. .gitignore, .gitattributes, .gitconfig, .editorconfig
5. NiceVibrations
6. Andoid Logcat
7. URP / HDRP
8. CI/CD
9. Naughty Attributes
10. .asmdef
11. AssemblyInfo
12. Unity SerializeReferenceExtensions

# Project Structure
    |-- Assets
        |-- _GameName
        	|-- Code
            |-- DynamicAssets
            	|-- Prefabs/Resources
            |-- SandBox
            |-- Scenes
            |-- Shaders
            |-- StaticAssets
            	|-- Animations/Sprites
            	|-- Textures/Music
            	|-- Materials/Models
        |-- Import           				//Import assets
        |-- SandBox          				//Test files
        |-- StreamingAssets

# Optimization
 - Enable if physics layer if needed (default all disable) in `Edit -> Project Settings -> Layer Collision Matrix`
 - Optimize Draw Call: 
    1. Enable Static Batching: `Edit -> Project Settings -> Player -> Other Settings -> Rendering -> Static Batching`
    2. Enable Dynamic Batching: `Edit -> Project Settings -> Player -> Other Settings -> Rendering -> Dynamic Batching`
    3. In material (if necessary) enable: `Material -> Advanced Option -> Enable GPU Instancing`
 - Use Liner gamma: `Edit -> Project Settings -> Player -> Other Settings -> Rendering -> Color Space`
 - Use Sprite Atlas and enable this in Project Settings: `Edit -> Project Settings -> Editor -> Sprite Packer -> Always Enabled`
 - Disable UI `Raycast Target` in gameObject component if he is not responsible for it
 - Disable pixel perfect (if necessary) in canvas: `Canvas - Pixel Perfect`
 - Managed code stripping
 - Check mark "Static" for static GameObjects

 # Scenes
If there is only one scene, name it: **Main** <br>
Name the scene for the test: **Test** <br>
`Use a meaningful name!`

# New Objects
 New objects from the Instantiate () should be named "_Dynamic" / "_dynamic"
