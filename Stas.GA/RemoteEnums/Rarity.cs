// <copyright file="Rarity.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace Stas.GA {
    /// <summary>
    ///     Read Rarity.dat file for Rarity to integer mapping.
    /// </summary>
    public enum Rarity
    {
        /// <summary>
        ///     Normal Item/Monster.
        /// </summary>
        Normal=0,

        /// <summary>
        ///     Magic Item/Monster.
        /// </summary>
        Magic=1,

        /// <summary>
        ///     Rare Item/Monster.
        /// </summary>
        Rare=2,

        /// <summary>
        ///     Unique Item/Monster.
        /// </summary>
        Unique=3,
        Gem = 4, //from dpb
        Currency =5 , 
        Quest =6 , 
        Prophecy =7 //is still valid?

    }
}