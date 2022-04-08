# CustomChaosPool

### ever wanted to have Glowing Meteorite and Spinel Tonic in your bottle chaos pool?  
### no? or maybe you want to remove volcanic egg?  
### well this mod is for you!


Allows for full customization of the Bottled Chaos equipment pool via config file  
should be compatible with mods as long as you can get their name/internal name/equipment index

``Only the host needs the mod? (untested)``

## Config guide
### Input
The config accepts a string which has to be devided by commas.

> **for example** : Equipment1, Equipment2, Equipment3

The Config can accept the name, internal name or the index of the equipment  
you can find the names, internal names and indexes of all equipments [here](https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names)  
>**for example** : 0, Disposable Missle Launcher, DroneBackup  
>this would select the Preon Accumulator(0), the Disposable Missile Launcher(Disposable Missle Launcher) and the Backup(DroneBackup)
#### **please note that if you have modded equipment indexes would be different**

The Config also has options for case and space insensitivity
> **for example** : "vOLc aNic eGG " or "eccentricvase" can both be valid inputs	
### Config Options

### Selection

#### Capitalization Insensitivity

> Enables Capitilization insensitivity  
> **for example** : "Eccentric Vase" and "eccentric vase" will both work with this enabled  
> Some modded equipment might have the same internal name with different capitilizations so try disabling this if that's the case

#### Space Insensitivity

> Enables Space insensitivity  
> **for example** : "Eccentric Vase" and "Ecc entri c Vase " will both work with this enabled  
> Some modded equipment might have the same name with different Spacing so try disabling this if that's the case

#### Special Character Insensitivity

> Enables Special Character insensitivity  
> **for example** : "Trophy Hunter's Tricorn" and "Trophy Hunters Tricorn" will both work with this enabled  
> Some modded equipment might have the foreign character names which will get discarded by this option

### Equipment

#### Remove Equipment

> Removes any selected equipment from the pool  
> **for example** : "Jade Elephant, Sawmerang"  
> this will remove jade elephant and sawmerang from the equipment pool

### Experimental

#### Enable Experimental settings
> Enables experimental settings which might break things  
> Such as allowing you to add elite aspects or trophy hunter to the equipment pool  
> Required for all other options in experimental

#### Add Equipment
> Adds any selected equipment to the pool  
> **for example** : "Glowing Meteorite, Spinel Tonic"  
> this will add glowing meteorite and spinel tonic to the equipment pool  

#### Use Exact Pool & Pool
> Lets you set an exact pool  
> **for example** : "Radar Scanner, Royal Capacitor"  
> this would set the equipment pool to only contain radar scanner and royal capacitator  
> Overides Remove/Add Equipment

#### Auto Cleanup non default equipment
> Automatically removes any non default equipment from the pool  
> **for example** : "Radar Scanner, Trophy Hunter's Tricorn"  
> if Auto cleanup is enabled trophy hunter would be automatically removed  
> this is meant to prevent mistakes

#### Trophy Hunter's Tricorn patch
> Enables a patch for trophy hunter's tricorn  
> This patch fixes an issue where if trophy hunter's tricorn is added to the pool and is rolled it will replace your equipment with the consumed version of trophy hunter's tricorn regardless of your current equipment  
> Might break other mods that modifies the trophy hunter's tricorn's code

### Miscellaneous

#### Log Debug
> Logs extra info for debugging purposes

#### Give Equipment Info
> Logs the Indexes, Names and Internal names of all equipments to the console  
> this is meant to be used to find the internal name of modded equipments

## Mod Compatibility
as said above this mod should be compatible with other mods as long as you can get the name/internal name/equipment index

you can find those by enabling "Give Equipment Info" then looking at the log file.

### Step by step guide

#### Method 1 Console

1. Enable Give Equipment Info  
![Step 1](https://i.imgur.com/SM13YZ4.png)
2. Launch the game, Then in BepInEx Enable Developer Tools in Settings, you might also want to disable Close this window when the game is loaded  
![Step 2](https://i.imgur.com/iOv1Zcy.png)
3. In Console Search for "CustomChaosPool"  
![Step 3](https://i.imgur.com/rtDy7WD.png)
4. Wait for "Equipment Info" to pop up  
![Step 4](https://i.imgur.com/aL2UMd5.png)
5. Double click on "Equipment Info" and search for what ever equipment you want there  
![Step 5](https://i.imgur.com/ZUm9C7B.png)

#### Method 2 Log
1. Enable Give Equipment Info  
![Step 1](https://i.imgur.com/SM13YZ4.png)
2. Launch the game until the main menu, then close the game  
![Step 2](https://i.imgur.com/x0eaXPn.png)
3. Locate your BepInEx folder, the image below assumes your using thunderstore  
![Step 3](https://i.imgur.com/QHuDGbG.png)
4. Open "LogOutput.log"  
![Step 4](https://i.imgur.com/VaNTX2v.png)
5. Search for "Equipment Info" (try Ctrl F)  
![Step 5](https://i.imgur.com/Ge7ErJQ.png)

I hope this is easy enough to understand to a non mod developer ^v^

## Known Issues
* ``If Trophy hunter is added to the pool and is rolled, it will replace your equipment with the consumed version of trophy hunter regardless of your equipment``

there is an experimental patch for which may break mods that modifiy the tricorn

* ``If 2 modded equipment somehow have the exact same name, capitalization, spacing and special characters the mod will ignore whichever one loads slower``

though if this were to happen you can still select them via equipment index as that cannot be the same

## Planned features
* ~~Maybe a way to view the equipment name/internal name/index of modded equipments~~ 🗸 implemented 
* ~~Special characters (such as ') insensitivity~~ 🗸 implemented 


## Contact
You can find me in the modding discord **konomiyu#3611**, feel free to message me regarding any issues or suggestions for the mod
## Changelog

**1.1.3**

* Give equipment info now gaves extra info
* Improved step by step guide to be easier to follow

**1.1.2**

* fixed config issues

**1.1.1**

* Uploaded the correct file (version 1.1.0 was accidentally 1.0.2, whoops)

**1.1.0**

* Added Special Character insensitivity (with an option to disable it)
* Added an experimental patch for trophy hunter's tricorn
* Minor internal changes

**1.0.3**

* Added options for disabling space/cap insensitivity
* Give Equipment info now also gives the exact capitalization and spacing

**1.0.2**

* Fixed bug that caused the mod not to work
* Added Debugging stuff
* Added a feature to find the name/internal name/index of modded equipment
* Updated README

**1.0.1**

* Minor Readme changes

**1.0.0**

* Inital Release.
