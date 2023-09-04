##### TOWERS #####
- Script that rotates the part of the tower it is attached to
    - Has targeting setting
    - The TowerBrain finds all rotate scripts attached to children and controls the targeting of all of them
        - Settings for if they are all controlled at once or individually
        - UI should support a dynamic number of targeting options

- Create TowerBase prefab

- Create TowerShooter prefab

- ~~Automatically find attached guns instead of inserting into list manually~~


##### ENEMIES #####
- Enemy Properties
    - Camo, lead, etc

- Rework how enemies take damage
    - Instead of instantiating a new enemy, "transform" them into the new type
    - Only instantiate if the enemy has the "split" property

- When enemies split into multiple enemies, they should spread out along the track
    - The furthest enemy should not spawn further than where the destroyed enemy died

- ~~Move movement code from enemy to standalone script~~


##### UPGRADES #####
- Tower Upgrades
    - Ideas:
        - Change the projectile prefab
        - Attack speed (Activation Speed)
        - Add guns (Activate guns already attached to the tower)
        - Attack Range
        - Ability to "see" enemies with special properties

- Projectile Modifiers
    - Ideas:
        - Piercing: A single projectile can hit multiple times (Should not be able to hit the same enemy multiple times)
        - Projectile Speed
        - Damage Amount: Destroy multiple "layers" of an enemy
        - Remove special properties from enemies
        - Allow projectiles to spawn other projectiles on death
        - Change sprite of projectile (Needs some sort of priority with conflicting upgrades)
        
    - All projectile modifiers apply to secondary projectiles that might spawn (Projectiles that are spawned from other projectiles)


##### AOE #####
- Create a generic AOE prefab & script that other types derive from
    - AOE could be considered a type of projectile and have similar properties
    - Explosions
    - etc..

- AOE's should only be able to hit a certain number of enemies