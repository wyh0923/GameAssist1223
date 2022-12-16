﻿using System;
using ImGuiNET;

namespace Stas.GA {
    /// <summary>
    ///     Contains the static data for calculating the terrain height.
    /// </summary>
    public class TerrainHeightHelper : RemoteObjectBase {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TerrainHeightHelper" /> class.
        /// </summary>
        /// <param name="address">address of the remote memory object.</param>
        /// <param name="size">expected size of this remote memory object.</param>
        internal TerrainHeightHelper(IntPtr address, int size) : base(address) {
            this.Values = new byte[size];
        }
        internal override void Tick(IntPtr ptr, string from=null) {
            Address = ptr;
            if (Address == IntPtr.Zero)
                return;
            this.Values = ui.m.ReadMemoryArray<byte>(this.Address, this.Values.Length);
        }
        protected override void Clear() {
            for (var i = 0; i < this.Values.Length; i++) {
                this.Values[i] = 0;
            }
        }
        /// <summary>
        ///     Gets the values associated with this class.
        /// </summary>
        public byte[] Values { get; private set; }

        /// <inheritdoc />
        internal override void ToImGui() {
            base.ToImGui();
            ImGui.Text(string.Join(' ', this.Values));
        }

      
       

      
       
    }
}