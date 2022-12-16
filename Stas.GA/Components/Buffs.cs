using System;
using System.Collections.Concurrent;
using ImGuiNET;
namespace Stas.GA; 

/// <summary>
///     The <see cref="Buffs" /> component in the entity.
/// </summary>
public class Buffs : EntComp{
    public Buffs(IntPtr address) : base(address) { 
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<BuffsOffsets>(this.Address);
        this.StatusEffects.Clear();
        var statusEffects = ui.m.ReadStdVector<IntPtr>(data.StatusEffectPtr);
        for (var i = 0; i < statusEffects.Length; i++) {
            var statusEffectData = ui.m.Read<StatusEffectStruct>(statusEffects[i]);
            if (AddressToEffectNameCache.TryGetValue(statusEffectData.BuffDefinationPtr, out var oldEffectname)) {
                // known Effect
                this.StatusEffects.AddOrUpdate(oldEffectname, statusEffectData, (key, oldValue) => {
                    statusEffectData.Charges = ++oldValue.Charges;
                    return statusEffectData;
                });
            }
            else if (this.TryGetNameFromBuffDefination(
                statusEffectData.BuffDefinationPtr,
                out var newEffectName)) {
                // Unknown Effect.
                this.StatusEffects.AddOrUpdate(newEffectName, statusEffectData, (key, oldValue) => {
                    statusEffectData.Charges = ++oldValue.Charges;
                    return statusEffectData;
                });

                AddressToEffectNameCache[statusEffectData.BuffDefinationPtr] = newEffectName;
            }
        }
    }

    /// <summary>
    ///     Stores Key to Effect mapping. This cache saves
    ///     2 x N x M read operations where:
    ///     N = total life components in gamehelper memory,
    ///     M = total number of buff those components has.
    /// </summary>
    private static readonly ConcurrentDictionary<IntPtr, string> AddressToEffectNameCache = new();

  
   
    /// <summary>
    ///     Gets the Buffs/Debuffs associated with the entity.
    ///     This is not updated anymore once entity dies.
    /// </summary>
    public ConcurrentDictionary<string, StatusEffectStruct> StatusEffects { get; } = new();
    /// <inheritdoc />
    
    internal override void ToImGui()
    {
        base.ToImGui();
        if (ImGui.TreeNode("Status Effect (Buff/Debuff) (Click Effect to copy its name)"))
        {
            foreach (var kv in this.StatusEffects)
            {
                ImGuiExt.DisplayTextAndCopyOnClick($"Name: {kv.Key}", kv.Key);
                ImGui.SameLine();
                ImGui.Text($" Details: {kv.Value}");
            }

            ImGui.TreePop();
        }
    }

    /// <inheritdoc />
    protected override void Clear()
    {
        ui.AddToLog("Component Address should never be Zero.", MessType.Warning);
    }

   
    private bool TryGetNameFromBuffDefination(IntPtr addr, out string name)
    {
        var namePtr = ui.m.Read<IntPtr>(addr);
        name = ui.m.ReadUnicodeString(namePtr);
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return true;
    }
}