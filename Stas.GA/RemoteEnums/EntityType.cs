// <copyright file="EntityType.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace Stas.GA {
    /// <summary>
    ///     Enum for entity Categorization system.
    /// </summary>
    public enum eTypes {

        /// <summary>
        ///     Unknown entity type i.e. entity isn't categorized yet.
        /// </summary>
        Unidentified,

        /// <summary>
        ///     A special entity type that will make sure that this entity
        ///     components are never updated again from the game memory.
        ///     This is to save the CPU cycles on such entity. All plugins
        ///     are expected to skip this entity.
        ///
        ///     WARNING: if an entity reaches this type it can never go back to not-useless
        ///              unless current area/zone changes.
        /// </summary>
        Useless,
        SelfPlayer,
        OtherPlayer,
        Blockage,//Contains the TriggerableBlockage component.

        Chest,  LargeChest,   Barrel,   DelveChest,  HeistChest,   ExpeditionChest,
        ImportantStrongboxChest, StrongboxChest,  BreachChest,

        Shrine,
        Friendly,
        Monster,
        NPC,
        Misc,
        WorldItem,
        Door,
        Projectile,
        Waypoint,
        Effects,
        MinimapIcon,
        Terrain,
        Stash,
        NeedCheck,
        Exped,
        Portal,
        Pet, Quest, AreaTransition,
        /// <summary>
        ///     Important legion monster or chest when legion isn't opened.
        /// </summary>
        Stage0RewardFIT,
        /// <summary>
        ///     Legion epic chest when legion isn't opened.
        /// </summary>
        Stage0EChestFIT,
        /// <summary>
        ///     Regular legion monster when legion isn't opened.
        /// </summary>
        Stage0FIT,
        /// <summary>
        ///     Important legion monster or chest when legion is opened.
        /// </summary>
        Stage1RewardFIT,
        /// <summary>
        ///     Legion epic chest when legion is opened.
        /// </summary>
        Stage1EChestFIT,
        /// <summary>
        ///     Regular legion monster after legion is opened.
        /// </summary>
        Stage1FIT,
        /// <summary>
        ///     Legion monster after legion is opened and killed by user.
        /// </summary>
        Stage1DeadFIT,

        /// <summary>
        ///     Delirium monster that explode when player steps on it.
        /// </summary>
        DeliriumBomb,
        /// <summary>
        ///     Delirium monster that creates new monster when player steps on it.
        /// </summary>
        DeliriumSpawner,
        InventoryItem,
      
    }
}