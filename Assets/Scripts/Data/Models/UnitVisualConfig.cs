using System;
using UnityEngine;

[Serializable]
public class UnitVisualConfig
{
    public float bodyScale = 1.0f;
    public float shoulderWidth = 0.28f;
    public float hipWidth = 0.12f;
    public ArmorStyle armorStyle = ArmorStyle.Medium;
    public HelmetStyle helmetStyle = HelmetStyle.None;
    public WeaponStyle primaryWeapon = WeaponStyle.Sword;
    public WeaponStyle secondaryWeapon = WeaponStyle.None;
    public ShieldStyle shieldStyle = ShieldStyle.None;
    public bool hasCape;
    public bool hasBackItem;
    public Color armorTint = new(0.48f, 0.5f, 0.52f);
    public Color clothTint = new(0.4f, 0.4f, 0.4f);
    public Color skinTint = new(0.85f, 0.7f, 0.55f);
    public MaterialPreset armorMaterial = MaterialPreset.Chainmail;
    public MaterialPreset weaponMaterial = MaterialPreset.Steel;
}
