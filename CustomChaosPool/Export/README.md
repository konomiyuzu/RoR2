# CustomChaosPool

### ever wanted to have Glowing Meteorite and Spinel Tonic in your bottle chaos pool?  
### no? or maybe you want to remove volcanic egg?  
### well this mod is for you!


Allows for full customization of the Bottled Chaos equipment pool via config file

>Only the host needs the mod? (untested)

## Config guide
### Input
The config accepts a string which has to be devided by commas.

> **for example** : Equipment1, Equipment2, Equipment3

The Config can accept the name, internal name or the index of the equipment  
you can find the names, internal names and indexes of all equipments [here](https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names)  
>**for example** : 0, Disposable Missle Launcher, DroneBackup  
>this would select the Preon Accumulator, the Disposable Missile Launcher and the Backup from the bottled chaos pool  

The Config is also case and space insensitive
> **for example** : "vOLc aNic eGG " or "eccentricvase" are both valid inputs	
### Config Options


#### Remove Equipment

> Removes any selected equipment from the pool  
> **for example** : "Jade Elephant, Sawmerang"  
> this will remove jade elephant and sawmerang from the equipment pool

#### Enable Experimental settings
> Enables experimental settings which might break things  
> Such as allowing you to add elite aspects or trophy hunter to the equipment pool

#### Add Equipment
> Adds any selected equipment to the pool  
> **for example** : "Glowing Meteorite, Spinel Tonic"  
> this will add glowing meteorite and spinel tonic to the equipment pool  
> Overides Remove/Add Equipment

#### Use Exact Pool & Pool
> Lets you set an exact pool  
> **for example** : "Radar Scanner, Royal Capacitor"  
> this would set the equipment pool to only contain radar scanner and royal capacitator

#### Auto Cleanup non default equipment
> Automatically removes any non default equipment from the pool  
> **for example** : "Radar Scanner, Trophy Hunter's Tricorn"  
> if Auto cleanup is enabled trophy hunter would be automatically removed  
> this is meant to prevent mistakes

## Known Issues
#### If Trophy hunter is added to the pool and is rolled, it will replace your equipment with the consumed version of trophy hunter regardless of your equipment

I might fix this if i ever feel motivated to (persumably by having a run ruined by it)
## Contact
You can find me in the modding discord **konomiyu#3611**, feel free to message me regarding any issues or suggestions for the mod
## Changelog

**1.0.0**

* Inital Release.
