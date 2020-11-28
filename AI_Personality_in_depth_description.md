# AI Personality in-depth description


## Overview 

With the Unofficial Crusader Patch, the inner workings of the Stronghold Crusader AI have been made accessible to you. Editing the AIC (AI Character) parameters will modify the AI's behavior to a certain degree: 

Among a number of other things, it is possible to change army composition for the AI, as well as how the AI manages its stockpile, how it uses different production chains and how fast it sets up, balancing army and economy. 

If you want to script your very own AI for the game you love and would like to change AI behaviour any further than what the AIC parameters currently allow, you are very much welcome to join our awesome community and contribute to the UCP project, making more and more mechanics available for modding.


## Economy

There are many aspects of the AI's economy that can be configured, including how it manages its popularity, gathers and manages different resources and generally goes about setting up its castle.

### Popularity and taxes
Popularity affects how fast peasants spawn (or despawn), for the AI just as for human players, and there are three main parameters available for regulating the AI's popularity: The AI will normally let its popularity fluctuate between `LowestPopularity` and `HighestPopularity` periodically. When anything unexpected (e.g. destroyed granary) causes the popularity to drop below the `CriticalPopularity` threshold, the AI will see this as an emergency case, causing it to take drastic measures to gather gold and regain popularity. Such measures include selling most of their stored goods, buying food and setting negative taxes (a.k.a. donations). 

Taxes are set by the ingame tax tiers, ranging in impact from +7 popularity to -24 popularity.

__TODO:__ 
- Hint: influence the recruitment speed of the AI by regulating its popularity. 
- mention how taxes are set (i.e. what AIC parameters are used, what values correspond to what tax tier, etc.) and also does not connect taxes to the popularity management. 


### Farms and other resource buildings

To regulate the AI's economy, you can set a maximum amount for each type of resource building (using the `Max[ResourceBuildingName]` parameters), as well as how fast the AI will build them relative to its other production facilities. The latter is achieved using the `PopulationPer[ResourceBuildingName]` parameters that control at what amounts of (total) population another resource building of the corresponding type will be built. 

- **Note:** 
    Setting these parameters to 0 will result in the AI not placing any resource buildings of that type, same as setting the max amount parameter to 0 would do. 

    (Alternatively, setting the parameters to a value higher than the maximum population available to the AI would also achieve the same, but an AI's maximum population is not defined explicitly anywhere and depends on the amount of workers required for its economy (as set up in the AIC file), as well as available houses (in all corresponding AIV files), making this rather unreliable.) 

The `ResourceRebuildDelay` parameter defines how fast the AI rebuilds destroyed resource buildings. 

All farms "share" their maximum and population parameters, but you can additionally specify which types of farms the AI will build, by populating the values for parameters `Farm1` to `Farm8`. The AI will generally try to build farms in the specified order and ratio (i.e. starting at `Farm1`, going through the list until `Farm8`, then back to `Farm1`, and so on), although it will skip farms it cannot build at the time. 

- **Note:** 
    This list of farms is a bit tricky, as hop farms currently don't count towards the maximum amount of farm buildings despite being set up using the farm parameters. Therefore, setting up hop farms in the first spot might result in the AI spamming hop farms on any space that's available, while not building any other farms and never reaching its maximum farm limit. 

__TODO:__
- consider putting a general explanation of list behaviour in one place (farms, various troops, siege engines, etc)


### Build speed

The `BuildInterval` parameter plays a very important role as it determines the build speed for an AI: The lower the interval is set, the faster the AI will build its castle, with 1 resulting in the fastest speed. 

- **Note:** 
    Any AI with 5k gold or more will temporarily override this value with 1 automatically, until its gold depletes below 5k. Also, the `BuildInterval` parameter only affects the build steps set up in AIV files, but does not affect placement of resource buildings. 
- **Note:** 
    The AI will only use the recruitment behavior (see below) defined by the relevant AIC values after its AIV build steps have been completed (or skipped due to a lack of resources); before that, no attack troops will be recruited. Thus, the value of the `BuildInterval` parameter will impact how soon the AI is able to start recruiting attack troops.

### Resource management

Setting the maximum stocked amounts of different resources and weapons is useful to manage the limited space for stockpile, granary and armoury space the AI has (depending on the AIVs used). The AI will sell excess resources when their amount surpasses the corresponding maximum parameter value plus `MaxResourceVariance`. 

- **Note:** 
    Keep in mind stocks of a resource may sometimes get split between several stockpile spots, even when the set maximum amount would fit in fewer stockpile spots. This should be considered when planning out stockpile space usage. 
- **Note:** 
    Additionally, for stone (and possibly wood or iron), the AI may sometimes stock up more than the usual maximum amounts as defined above, depending on the cost of its most expensive building. For example, if an AI's AIV contains a big round tower (costing 40 stone), the AI may try to stock up that amount of resources on top of its already maximum defined stock. Sometimes the AI will also keep those extra stocks even after all buildings are constructed and intact, and sometimes it may not keep such extra stocks at all. Either way, building costs should be considered when setting maximum resource values. 
- **Note:** 
    If an AIV building cannot be placed due to the location being blocked, the AI may still try to keep the required resources stocked up indefinitely, as long as it does not exceed the assigned maximum resource values. 

For the AI's food management in particular, you can adjust the `MaxFood` parameter, which defines the maximum amount of food (per food type!) to be stored at once, and set minimum values for those food types your AI should keep in stock at all times. With the `DoubleRationsThreshold` parameter you can control at which amount of stocked food (total of all food types!) the AI will increase their food rations. 

The AI will sell excess food, and buy food to maintain the minimum amount set for each food type. The amount of food resources purchased at once can be set with `TradeAmountFood`. 

- **Note:** 
    Despite not being actual food, wheat is still treated like food regarding `MaxFood` and `TradeAmountFood` parameters. Moreover, hops will also be bought in margins of `TradeAmountFood`, but its maximum stock is subject to the `MaxResourceOther` parameter, same as iron, flour and pitch. 

Generally, the `TradeAmount[ResourceName]` parameters control how many units of a resource the AI will purchase at once, if it is running low on stocks for certain resources. The AI will periodically buy goods it needs, if it has enough gold to afford them. The AI's gold budget to buy goods is shared with the budget to recruit troops. Both will be initiated usually when the AI surpasses the `RecruitGoldThreshold` value in gold, but also sometimes below that. 

So far, we know that the AI prioritizes buying food (including wheat and hops) over everything else. After that it prioritizes buying weapons and recruiting troops, then buying resources to build up its missing AIV buildings. Next it would proceed to buy wood, iron, flour and beer for consumption/processing by corresponding production buildings (weapon workshops, bakers, inns) if there is still gold left and the stocks for those resources are currently empty. 

- **Note:** 
    Sortie units, raid units, defensive siege engines and resource buildings are handled outside of this procedure, so the AI will always buy/recruit those even when below its `RecruitGoldThreshold`. 
- **Note:** 
    Even though the AI will "cheat" for pitch ditches and walls (essentially, building them for free), it seems the AI still needs one unit of the corresponding resource in stock, or enough gold so it would be able to buy it. It is possible, though, that the relevant criterion preventing the AI from building (free) walls is actually "being under pressure", instead of "being broke". At least, the AI has been observed to stop building/repairing walls mostly when completely broke and under pressure, while on some occasion still repairing walls when broke but not pressured. 

__TODO:__
- Consider renaming "pressure" to "fear" or explain the wording in more detail somewhere
- Maybe explain trade ticks in more detail somewhere?

A fast way to let the AI sell resources which it doesn't use in any of its production chains, freeing up space, is to add these resources to the `SellResource[Number]` parameters. The resources in that list will be sold instantly (or, more precisely, as soon as the AI's market trading routine has nothing of higher priority to do for a moment) when they come into stock. This means any resources the AI needs to construct buildings or produce other goods should not be in that list. If they were, the AI would constantly buy and sell these resources, loosing gold and occupying trade ticks  in the process. 

Using the parameters for blacksmith, poleturner and fletcher settings, one can control what weapon types the AI will produce with the corresponding workshops. Setting their value to "Both" will cause the AI to alternate between default and non-default weapon for the corresponding workshop type, i.e. setting every second workshop of that type to produce the non-default weapon. For example, an AI with 5 fletchers/blacksmiths/poleturners would set the 2nd and 4th fletcher/blacksmith/poleturner to produce crossbows/maces/pikes, and the 1st, 3rd and 5th to produce bows/swords/spears respectively, if the workshop settings were set to "Both". 


### Ally Interactions

Human players can interact with allied AI players using the ally menu (trumpet icon). Available actions are help/defense requests, attack requests, as well as requesting or sending goods. Whether or not the AI will accept one of your requests depends on two parameters: 

For requesting goods, the AI checks if the requested amount of resources plus the value of the AI's `MinimumGoodsRequiredAfterTrade` parameter is lower than its current stock of that resource. 

For commands to attack or defend, the AI will check if the current size of its recruited main attack force exceeds the `AttForceSupportAllyThreshold` value. 

When the AI itself tries to buy resources from the market but realizes it doesn't have sufficient gold to do so, it will subsequently request twice that amount of goods from human allies instead. 

__TODO:__
- Does this fit in the economy section or should it be put elsewhere? (It has both military and economy aspects) 


## Military

__TODO:__ add an overview / summary sentence here (similar to the one for the "Economy" section)

### Recruitment probabilities

The AI groups its troops into four main categories: Sortie units, defensive units, raiding units and attack force units. Moreover, there are three power states in which the AI can be: Default, Weak and Strong. For each of these states, one can set recruiting probabilities for defensive, raiding and attack force units. Thereby, the three probability values for a state need to add up to 100. 

The AI will dynamically switch its recruitment activities between those troop categories during a game (e.g. when reaching `RecruitGoldThreshold`), at which point the probabilities come into play: The higher one of the probability values, the more likely the AI is to switch to recruiting troops for the corresponding category. For instance, setting `RecruitProbDefDefault` to 0, `RecruitProbRaidDefault` to 80 and `RecruitProbAttackDefault` to 20 would cause the AI – when being in the default power state – to switch to recruiting raiding troops with about 80% probability, and to attack troops with about 20% probability, whenever the recruitment category switch is triggered. 

- **Note:** 
    The AI will always prioritize recruiting sortie units, regardless of what recruitment probabilities are set in the AIC parameters for the other three troop categories. Moreover, the AI's recruitment behavior differs while it is still setting up its castle, being more defensive overall and not recruiting attack troops. Something similar can also be noticed when the AI feels pressured: When this happens, it sends a message, calls its allies for help, recruits defensive units faster than usual and also sells more goods in order to do so. 
- **Note:** 
    While it is possible to set recruiting probability values for a power state to not add up to 100, in that case the resulting behavior ingame may happen to not match the provided values. 
- **Note:** 
    The AI will recruit sortie units, raid units and defensive siege engines even when below its `RecruitGoldThreshold`. 

### Recruitment speed

The AI's recruitment speed is mainly handled by intervals between subsequent recruiting actions. For each power state, there is a separate recruitment interval parameter which defines the game ticks before the AI will recruit another unit. The highest recruitment speed will be reached by setting the recruitment interval to 1. If the AI doesn't spend gold (by recruiting troops) as fast as it gains gold from its economy, it will stack up gold in its treasury. 

- **Note:** 
    Actual recruitment speed can also be affected/limited by the speed at which new peasants are spawning (which in turn depends on current popularity) as well as a lack of available peasants due to insufficient housing in AIVs. Also, the AI will generally stop recruiting new attack force units while it is already performing an attack. 

### Sortie troops

Sortie units are sent out by the AI when one of its workers gets killed by enemy units. `SortieUnitMelee` will run into the attacking enemies, while `SortieUnitRanged` will move towards the spot where the worker died, shooting at enemy units on the way there, and guard that location until another dead worker causes them to move to a different location. 

Constantly recruiting (and losing) lots of sortie units can eventually cause the AI to "bleed out" economically in cases where it has set up resource buildings close to enemy defenses, as those defenses will usually be stronger than the sortie units sent there, often resulting in very unfavorable exchanges with high losses for the sortie troops but without noticeable losses for the enemy. 

The AI prioritizes recruiting sortie units over any other recruitments and stocks up its sortie units at the keep to reach at least the `SortieUnitRangedMin` and `SortieUnitMeleeMin` values before sending them out. The AI will recruit additional sortie unit ranged troops when it has enough gold to afford them, which can result in about 20 more than the defined minimum. 

- **Note:** 
    While it is possible to assign ranged units as `SortieUnitMelee` and melee units as `SortieUnitRanged`, this is generally not recommended due to the different behavior of the two types of sortie troops – sending ranged units to run into close combat is rarely useful. 

### Defense troops

The troop types the AI will recruit to defend its castle can be specified by populating the values for parameters `DefUnit1` to `DefUnit8`. The AI will generally try to recruit defense troops, up to the total amount specified for `DefTotal`, in the specified order and ratio (i.e. starting at `DefUnit1`, going through the list until `DefUnit8`, then back to `DefUnit1`, and so on), although it will skip troop types it cannot build at the time. Regarding how those defensive units are put to use, there are two main sub-categories, "wall defense" troops stationed at troop spots set in the AI's AIV files, and "outer defense" troops stationed outside the castle. 

Recruited defense troops (and starting troops), up to the amount set for `DefWalls`, make up the "wall defense" and are sent to the troop spots defined in the chosen AIV file (or to the campfire-keep area, if no matching troop spots are present in the AIV). While most of those wall defense units will simply stay at their defined spots, there are some exceptions: Some unit types (currently including macemen, spearmen and horse archers) will instead patrol between all corresponding troop spots defined in the used AIV. Those patrolling wall defense units are split into a number of groups as specified by the `DefWallPatrolGroups` parameter. The delay before they move from one troop spot to the next is specified by `DefWallPatrolRallyTime`. Here, a value of 4 corresponds to approximately one month of ingame time. 

Additional defense troops (i.e. at most `DefTotal` minus `DefWalls` units) are distributed outside the castle to guard resource buildings, split up into a number of groups as specified by the `OuterPatrolGroupsCount` parameter. The `OuterPatrolGroupsMove` parameter can be set to True in order to make those groups actually patrol between different buildings; otherwise they will stay at their position indefinitely. When set to patrol, the groups will move to a new location periodically, according to the value set in `OuterPatrolRallyDelay`; again, a value of 4 corresponds to approximately one month of ingame time. 

- **Note:** 
    The AI can build more than the defined amount of `DefTotal` as defensive units, if the `OuterPatrolGroupsCount` is not set to 0, which would prevent the AI to recruit any more than the `DefWalls` amount as defensive units. The overrecruiting only happens in cases that the AI has a lot of gold.  
__TODO:__ clarify that hint text!

For digging their own moat (and that of allies), the AI will recruit additional troops of the type specified as `DefDiggingUnit`. Those do not count towards the `DefTotal` but instead have a separate `DefDiggingUnitMax` parameter that limits their amount. 

- **Note:** 
    Only unit types that are able to dig moats can be set as `DefDiggingUnit`. 
- **Note:** 
    The AI will not recruit defensive digging troops when its AIV does not contain any moat. 

__TODO:__ even if allies have moats?

You can also set how fast the AI should rebuild defensive siege engines defined in the AIV file. The `DefSiegeEngineBuildDelay` parameter controls how many game ticks the AI will wait after a defensive siege engine was destroyed, before building a new one in its place. The `DefSiegeEngineGoldThreshold` parameter defines how far the AI can go into debt (i.e. below zero gold) to rebuild those siege engines. In other words, the AI will always stay above *-1 x `DefSiegeEngineGoldThreshold`* when rebuilding them. 

- **Note:** 
    This also means setting the parameter to a negative value (e.g. -500) will cause the AI to only rebuild defensive siege engines if it will stay above +500 gold afterwards, which allows limiting how easily the AI spends gold on those siege engines. 

### AI target

__TODO:__ maybe reorder the sections and put AI target elsewhere? 

At any point in time, the AI will explicitly target exactly one enemy player with raids and siege attacks. An AI's current target enemy may change during a game, but the criteria by which the current target is chosen stay the same. Those targeting criteria can be set for each AI character by assigning one of the possible values for its `TargetChoice` parameter. This is quite an important choice, as it decides how the AI makes its attack decisions based on the current state of the game. 

Setting this parameter's value to `Player` will result in the AI always attacking the closest human opponent on the map (or the closest AI opponent if no human opponents are present). This also means the AI will only change its attack target once the current target is killed. 

- **Note:** 
    Distances are calculated by adding up horizontal and vertical distance between the keeps of the AI and its potential target. 
    __TODO:__ still true if keeps were moved in the AIV?
    
Setting this parameter's value to `Closest` will make the AI always target its closest enemy (independent of whether it is a human or AI player). This also means the AI will only change its attack target once the current target is killed. 

Setting this parameter's value to `Gold` will make the AI always target whatever enemy currently has the highest amount of gold stocked up. This also means the AI may change its attack target very frequently if multiple opponents have similar amounts of gold, as typically the exact gold amount of a player will fluctuate quite a bit. The switching of attack targets may thus occur pretty much anytime and can even happen whilst attacking and besieging the current/previous target, which will also result in the AI moving away its attack army from the old target in the middle of the siege, and over to the new target. 

- **Note:** 
    When using the "No AI target change during sieges" option inside of the UCP's bug-fix section, the AI will not switch targets after it has initiated a siege – which is usually the point in time when it would construct siege engine tents if it has engineers set up in its attack force to do so – until the siege is over. Even with the option enabled, the AI may still switch targets while the army approaches the current target but before the siege is initiated, though. 

Finally, setting this parameter's value to `Balanced` will make the AI target whatever enemy currently is considered the least powerful player (i.e. has the lowest rank on the "Greatest Lord" screen during the match). 
__TODO:__ outdated info (distance also matters)

### Harassment

The AI has several ways to harass its targeted enemy: It can recruit raid troops to attack and disrupt its target's economy, as well as construct harassment siege engines to attack the target's castle defenses and other buildings on the way there. 

The troop types the AI will recruit to raid its target's economy can be specified by populating the values for parameters `RaidUnit1` to `RaidUnit8`. The AI will generally try to recruit raid troops in the specified order and ratio (i.e. starting at `RaidUnit1`, going through the list until `RaidUnit8`, then back to `RaidUnit1`, and so on), although it will skip troop types it cannot build at the time. 

Recruited raid troops will be gathered and sent to patrol between spots around the AI's own castle as well as own resource buildings. They will then be sent out to attack enemy economy buildings when the minimum number of troops required to start a raid is reached. 

This number is based on the value of the `RaidUnitsBase` parameter, but can vary depending on a few other factors: Regularly, the AI can increase the size of its raiding party by a random number of additional troops in the range between zero and the value of the `RaidUnitsRandom` parameter. 

However if the AI has more than 5000 gold, that `RaidUnitsRandom` parameter value will be doubled; if the AI has less than 1000 gold, that value will be set to zero. If the targeted enemy has less than 500 gold, the `RaidUnitsRandom`  parameter value will even be negated and divided by 2 (i.e. a value of 10 will become -5). 

__TODO:__ btw what happens if RaidUnitsRandom is set to a negative value in the AIC? will the AI behave as one would expect based on the regular formulas for all those circumstances?

- **Note:** 
    The base amount can also get multiplied by 1.25 through some trigger that hasn’t been identified yet. 

---

__TODO:__ all text below needs some general cleanup / proofreading

|

|

v

Besides raids, the AI can build either catapults or fireballistae or both defined in the HarassingSiegeEngine[number] spots to harass its target enemy. It will recruit them by the order of the set siege engines in the list up to a maximum of HarassingSiegeEnginesMax at a time. These siege engines will then periodically move towards the enemy’s castle keep, until they can target an enemy unit or building, in which case they will shoot at said target. The AI requires an engineer’s guild to set these up, as well as an empty spot near the engineer’s guild. It can happen that these engines get set up too fast, thus leading to the AI overbuilding and therefore killing its own siege engines from time to time. If this spot happens to be inside of the castle or behind walls, these siege engines tend to get stuck in one point, at which point the AI will stop recruiting them. Also because of a bug that doesn’t detect the death of the engineer when a siege engine dies, the Ai will build less of these harassing siege engines over time.

### Attacks

#### Basics

The AI recruits its attack force and gathers it around its keep while setting it to aggressive stance, until it reaches the minimum required amount to attack an enemy. This amount is calculated by using the AttForceBase plus a value between 0 and AttForceRandom as a base value. For each consecutive attack the AI will add additional troops to its AttForceBase, depending on the formula chosen inside of the “Increase of the rate of additional attack troops” option inside the AI lords’ section of the Patcher. This additional troops number is set to 5 by default. If an enemy is considered weak, the AI might attack before gathering its full attack force, to a minimum of the AttForceRallyPercentage from the full attack force.

An attack of the AI will start by its troops moving off its keep, gathering in some place closer to the target enemy’s castle walls. It will start sieging the enemy’s castle when all of its remaining troops from its attack force or at least a percentage of AttForceRallyPercentage has gathered at the rally point. Then it will start by building its siege tends and splitting off the army in the predefined groups, which will act according to their roles. The siege engines it constructs have to be defined in the SiegeEngine[number] spots. The attack force army consists of two main groups. The AttUnitMain[number] units and all extra behavior defined units. The ratio of the siege engines built per attack, depends on the number of engineers send with this attack, but will be a maximum of 8. Catapults and trebuchets will throw with cows every CowThrowInterval shot.

All predefined roles of units will be recruited equally, if the AI has enough resources and gold to afford them, otherwise it will priorities units that are available. This means for example, if you define 10 engineers for 5 catapults to be built, set AttMaxAssassins to 10 and set AttUnitMain1 to 4 to Monk, while the minimum amount of troops required for its next attack is only 10, it will get probably 3 engineers, 3 assassins and 4 monks, as monks are cheaper and easier available to it. However, how exactly this composition works and how the proportions of each attacking unit that can be defined are truly set, hasn’t yet been discovered.

Attacking units can get stuck on the field after a failed siege. The siege will then only end if the siege times out, which is usually after a long time ingame, or if the remaining troops get killed off somehow. The AI can also end its siege by being pressured. Pressure might also result in a retreat of the AI. Even if a siege is almost won, the AI will then send all its units back and rally them in front of its castle, then moving them back up on his keep.

As soon as a free path is opened towards the enemy keep, every unit that can move up on the keep will be rallied onto the keep automatically. Horse archers and knights can not climb the keep, which leads to them hanging around the keep in aggressive stance.

#### Attack unit types

AttDiggingUnit can be set to any unit type that can dig up moats and will only be recruited if the enemy has a moat to dig through. AttUnit2 is a unit that gets sent in first, prior to the main attack force and attacks structures that are close by and castle walls. To open gates and crush towers, the AI relies on AttMaxAssassins and AttMaxLaddermen, which are the defined amounts of each troop type. The laddermen can get stuck and not move towards the enemy walls, if there is no clear path from the walls to the keep, which can be fixed by the bugfix option “Fix AI not using laddermen if they are enclosed “ inside the patcher. The AI will also use tunnelers to target the closest tile of wall or the closest tower to its siege force, setting them up at maximum range.

The AttUnitPatrol will patrol around the enemy castle when attacking, with ranged units stopping to shoot, every time an enemy unit is in their range. An exception to that stopping behavior are horse archers. These units will get issued commands to go to a new patrol point periodically after AttUnitPatrolRecommandDelay. The Ai can only control up to one group out of AttUnitPatrolGroupsCount, which leads extra groups to be stuck at the attack force’s initial rally point. Patrol units can also get stuck, if the rally point is too far away from the enemy castle, or the direct path is blocked by a cliff. They can also get stuck if the rally point is inside of an ally castle.

Another commonly used type for ranged troops is the AttUnitBackup type. These troops will guide the rallying troops to their rally point. They might also stay at that rally point or advance further as the main army moves to attack the enemy castle. However, they usually stay at the rally point, if there is no direct open path to the target enemies keep. These can also be split into multiple groups, which will then split up when the siege starts. Each of these groups will be supported by one shield if those are set up in the SiegeEngine[number] spots, but this behavior is not consistent.

AttUnitEngage troops will engage the biggest group of units, that the enemy has outside of his castle walls. This group is also well suited for ranged units, that will then not get stuck as often as other groups.

AttUnitSiegeDef units can also be split up in groups. These will go to the biggest group of attacking or harassing siege engines the AI currently has and guard them until they die.

#### Main force

A small percentage of the main army force will roam around and target attack buildings of the AI’s economy, like farms and stone quarries out side of the enemy’s castle. The main army will wait for a delay of Unknown130 in case the AI has set up siege engines with its attack. If all the siege engines are destroyed, the delay will be ignored, and the army will march in immediately. If the enemy castle is enclosed, only as many troops as attackable wall tiles, will march in at a time. This can be bypassed by activating the “Improved attack waves” option in the Patcher. The main army will be scaled up indefinitely for further attacks. It is the only army that still grows, when the other troop types are at their maximum defined values, with each attack. 

__TODO__ replace remaining “’” with "'"



