namespace Stas.GA;

/// <summary>
///     Points to the item in the game.
///     Item is basically anything that can be put in the inventory/stash.
/// </summary>
public class Item : Entity {
    /// <summary>
    ///     Initializes a new instance of the <see cref="Item" /> class.
    /// </summary>
    /// <param name="address">address of the Entity.</param>
    internal Item(IntPtr address) : base(address) { }

    /// <inheritdoc />
    internal override void Tick(IntPtr ptr, string from=null) {
        if (Address == IntPtr.Zero)
            return;
        // NOTE: ItemStruct is defined in EntityOffsets.cs file.
        var itemData = ui.m.Read<ItemStruct>(Address);

        // this.Id will always be 0x00 because Items don't have
        // Id associated with them.
        eType = eTypes.InventoryItem;
        UpdateComponentData(itemData);
    }
}
