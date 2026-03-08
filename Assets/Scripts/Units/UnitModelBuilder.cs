using UnityEngine;

public static class UnitModelBuilder
{
    static readonly Color DefaultMetal = new(0.55f, 0.55f, 0.58f);
    static readonly Color DefaultLeather = new(0.4f, 0.28f, 0.15f);
    static readonly Color DefaultWood = new(0.5f, 0.33f, 0.15f);
    static readonly Color DefaultGold = new(0.85f, 0.7f, 0.2f);
    static readonly Color DefaultBone = new(0.9f, 0.88f, 0.82f);
    static readonly Color DefaultChainmail = new(0.48f, 0.5f, 0.52f);
    static readonly Color DefaultSteel = new(0.7f, 0.72f, 0.75f);

    public static void BuildModel(Unit unit, UnitVisualConfig config, Color factionPrimary, Color factionSecondary)
    {
        BuildSkeleton(unit, config);
        BuildBody(unit, config, config.skinTint);
        BuildArmor(unit, config, factionPrimary);
        BuildHelmet(unit, config, config.armorTint);
        BuildWeapon(unit, config);
        BuildSecondaryWeapon(unit, config);
        BuildAccessories(unit, config, factionPrimary);
    }

    public static void BuildSkeleton(Unit unit, UnitVisualConfig config)
    {
        float s = config.bodyScale;
        float sw = config.shoulderWidth;
        float hw = config.hipWidth;

        unit.pivotHips = CreatePivot("Pivot_Hips", unit.transform, new Vector3(0, 0.65f * s, 0));
        unit.pivotWaist = CreatePivot("Pivot_Waist", unit.pivotHips, new Vector3(0, 0.35f * s, 0));
        unit.pivotNeck = CreatePivot("Pivot_Neck", unit.pivotWaist, new Vector3(0, 0.55f * s, 0));
        unit.pivotLeftShoulder = CreatePivot("Pivot_LShoulder", unit.pivotWaist, new Vector3(-sw, 0.45f * s, 0));
        unit.pivotRightShoulder = CreatePivot("Pivot_RShoulder", unit.pivotWaist, new Vector3(sw, 0.45f * s, 0));
        unit.pivotLeftElbow = CreatePivot("Pivot_LElbow", unit.pivotLeftShoulder, new Vector3(0, -0.32f * s, 0));
        unit.pivotRightElbow = CreatePivot("Pivot_RElbow", unit.pivotRightShoulder, new Vector3(0, -0.32f * s, 0));
        unit.pivotLeftHand = CreatePivot("Pivot_LHand", unit.pivotLeftElbow, new Vector3(0, -0.28f * s, 0));
        unit.pivotRightHand = CreatePivot("Pivot_RHand", unit.pivotRightElbow, new Vector3(0, -0.28f * s, 0));
        unit.pivotLeftHip = CreatePivot("Pivot_LHip", unit.pivotHips, new Vector3(-hw, 0, 0));
        unit.pivotRightHip = CreatePivot("Pivot_RHip", unit.pivotHips, new Vector3(hw, 0, 0));
        unit.pivotLeftKnee = CreatePivot("Pivot_LKnee", unit.pivotLeftHip, new Vector3(0, -0.38f * s, 0));
        unit.pivotRightKnee = CreatePivot("Pivot_RKnee", unit.pivotRightHip, new Vector3(0, -0.38f * s, 0));
        unit.pivotCape = CreatePivot("Pivot_Cape", unit.pivotWaist, new Vector3(0, 0.4f * s, -0.18f * s));
    }

    public static void BuildBody(Unit unit, UnitVisualConfig config, Color skinTint)
    {
        float s = config.bodyScale;
        var skin = ShaderHelper.SkinMaterial(skinTint);
        var cloth = ShaderHelper.ClothMaterial(config.clothTint);

        unit.partHead = CreateSphere("Head", unit.pivotNeck, new Vector3(0, 0.18f * s, 0),
            new Vector3(0.38f, 0.4f, 0.38f) * s, skin, unit).transform;
        AddEyes(unit.pivotNeck, s, unit);

        unit.partBody = CreatePart(PrimitiveType.Cube, "Torso", unit.pivotWaist,
            new Vector3(0, 0.2f * s, 0), new Vector3(0.5f, 0.42f, 0.28f) * s, cloth, unit).transform;
        CreatePart(PrimitiveType.Cube, "Belly", unit.pivotWaist,
            new Vector3(0, -0.05f * s, 0), new Vector3(0.46f, 0.3f, 0.26f) * s, cloth, unit);
        CreatePart(PrimitiveType.Cube, "Tunic", unit.pivotHips,
            new Vector3(0, 0.15f * s, 0), new Vector3(0.48f, 0.3f, 0.28f) * s, cloth, unit);

        unit.partLeftArm = CreateCylinder("LUpperArm", unit.pivotLeftShoulder,
            new Vector3(0, -0.15f * s, 0), new Vector3(0.13f, 0.16f, 0.13f) * s, skin, unit).transform;
        CreateCylinder("LForearm", unit.pivotLeftElbow, new Vector3(0, -0.12f * s, 0),
            new Vector3(0.11f, 0.14f, 0.11f) * s, skin, unit);
        CreateSphere("LHand", unit.pivotLeftHand, Vector3.zero, new Vector3(0.09f, 0.09f, 0.09f) * s, skin, unit);

        unit.partRightArm = CreateCylinder("RUpperArm", unit.pivotRightShoulder,
            new Vector3(0, -0.15f * s, 0), new Vector3(0.13f, 0.16f, 0.13f) * s, skin, unit).transform;
        CreateCylinder("RForearm", unit.pivotRightElbow, new Vector3(0, -0.12f * s, 0),
            new Vector3(0.11f, 0.14f, 0.11f) * s, skin, unit);
        CreateSphere("RHand", unit.pivotRightHand, Vector3.zero, new Vector3(0.09f, 0.09f, 0.09f) * s, skin, unit);

        unit.partLeftLeg = CreateCylinder("LUpperLeg", unit.pivotLeftHip,
            new Vector3(0, -0.18f * s, 0), new Vector3(0.14f, 0.19f, 0.14f) * s, cloth, unit).transform;
        CreateCylinder("LLowerLeg", unit.pivotLeftKnee, new Vector3(0, -0.16f * s, 0),
            new Vector3(0.12f, 0.17f, 0.12f) * s, cloth, unit);
        CreatePart(PrimitiveType.Cube, "LBoot", unit.pivotLeftKnee,
            new Vector3(0, -0.34f * s, 0.02f), new Vector3(0.15f, 0.1f, 0.2f) * s,
            ShaderHelper.LeatherMaterial(DefaultLeather), unit);

        unit.partRightLeg = CreateCylinder("RUpperLeg", unit.pivotRightHip,
            new Vector3(0, -0.18f * s, 0), new Vector3(0.14f, 0.19f, 0.14f) * s, cloth, unit).transform;
        CreateCylinder("RLowerLeg", unit.pivotRightKnee, new Vector3(0, -0.16f * s, 0),
            new Vector3(0.12f, 0.17f, 0.12f) * s, cloth, unit);
        CreatePart(PrimitiveType.Cube, "RBoot", unit.pivotRightKnee,
            new Vector3(0, -0.34f * s, 0.02f), new Vector3(0.15f, 0.1f, 0.2f) * s,
            ShaderHelper.LeatherMaterial(DefaultLeather), unit);
    }

    public static void BuildArmor(Unit unit, UnitVisualConfig config, Color factionPrimary)
    {
        var metal = MaterialFor(config.armorMaterial, config.armorTint);
        var cloth = ShaderHelper.ClothMaterial(factionPrimary);
        float s = config.bodyScale;

        switch (config.armorStyle)
        {
            case ArmorStyle.None:
                break;
            case ArmorStyle.Light:
                CreatePart(PrimitiveType.Cube, "ChestGuard", unit.pivotWaist,
                    new Vector3(0, 0.28f * s, 0), new Vector3(0.44f, 0.2f, 0.24f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "LBracer", unit.pivotLeftElbow,
                    new Vector3(0, -0.06f * s, 0), new Vector3(0.12f, 0.15f, 0.12f) * s,
                    ShaderHelper.LeatherMaterial(DefaultLeather), unit);
                CreatePart(PrimitiveType.Cube, "RBracer", unit.pivotRightElbow,
                    new Vector3(0, -0.06f * s, 0), new Vector3(0.12f, 0.15f, 0.12f) * s,
                    ShaderHelper.LeatherMaterial(DefaultLeather), unit);
                break;
            case ArmorStyle.Medium:
                CreatePart(PrimitiveType.Cube, "ChestPlate", unit.pivotWaist,
                    new Vector3(0, 0.2f * s, 0), new Vector3(0.5f, 0.4f, 0.28f) * s, metal, unit);
                CreateCylinder("LArmGuard", unit.pivotLeftShoulder,
                    new Vector3(0, -0.15f * s, 0), new Vector3(0.13f, 0.15f, 0.13f) * s, metal, unit);
                CreateCylinder("RArmGuard", unit.pivotRightShoulder,
                    new Vector3(0, -0.15f * s, 0), new Vector3(0.13f, 0.15f, 0.13f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "Surcoat", unit.pivotHips,
                    new Vector3(0, 0.2f * s, 0), new Vector3(0.52f, 0.36f, 0.3f) * s, cloth, unit);
                break;
            case ArmorStyle.Heavy:
                CreatePart(PrimitiveType.Cube, "ChestPlate", unit.pivotWaist,
                    new Vector3(0, 0.2f * s, 0), new Vector3(0.54f, 0.44f, 0.32f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "LPauldron", unit.pivotLeftShoulder,
                    new Vector3(0, -0.02f * s, 0), new Vector3(0.18f, 0.1f, 0.18f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "RPauldron", unit.pivotRightShoulder,
                    new Vector3(0, -0.02f * s, 0), new Vector3(0.18f, 0.1f, 0.18f) * s, metal, unit);
                CreateCylinder("LVambrace", unit.pivotLeftElbow,
                    new Vector3(0, -0.12f * s, 0), new Vector3(0.12f, 0.14f, 0.12f) * s, metal, unit);
                CreateCylinder("RVambrace", unit.pivotRightElbow,
                    new Vector3(0, -0.12f * s, 0), new Vector3(0.12f, 0.14f, 0.12f) * s, metal, unit);
                CreateCylinder("LCuisse", unit.pivotLeftHip,
                    new Vector3(0, -0.18f * s, 0), new Vector3(0.15f, 0.19f, 0.15f) * s, metal, unit);
                CreateCylinder("RCuisse", unit.pivotRightHip,
                    new Vector3(0, -0.18f * s, 0), new Vector3(0.15f, 0.19f, 0.15f) * s, metal, unit);
                CreateCylinder("LGreave", unit.pivotLeftKnee,
                    new Vector3(0, -0.16f * s, 0), new Vector3(0.13f, 0.17f, 0.13f) * s, metal, unit);
                CreateCylinder("RGreave", unit.pivotRightKnee,
                    new Vector3(0, -0.16f * s, 0), new Vector3(0.13f, 0.17f, 0.13f) * s, metal, unit);
                break;
            case ArmorStyle.Robes:
                CreatePart(PrimitiveType.Cube, "Robe", unit.pivotWaist,
                    new Vector3(0, 0.1f * s, 0), new Vector3(0.52f, 0.6f, 0.32f) * s, cloth, unit);
                CreatePart(PrimitiveType.Cube, "RobeSleeveL", unit.pivotLeftShoulder,
                    new Vector3(0, -0.1f * s, 0), new Vector3(0.2f, 0.35f, 0.2f) * s, cloth, unit);
                CreatePart(PrimitiveType.Cube, "RobeSleeveR", unit.pivotRightShoulder,
                    new Vector3(0, -0.1f * s, 0), new Vector3(0.2f, 0.35f, 0.2f) * s, cloth, unit);
                break;
            case ArmorStyle.Fur:
                CreatePart(PrimitiveType.Cube, "FurCloak", unit.pivotWaist,
                    new Vector3(0, 0.28f * s, -0.08f), new Vector3(0.58f, 0.35f, 0.28f) * s,
                    ShaderHelper.FurMaterial(new Color(0.45f, 0.35f, 0.25f)), unit);
                CreatePart(PrimitiveType.Cube, "FurCollar", unit.pivotWaist,
                    new Vector3(0, 0.48f * s, 0), new Vector3(0.56f, 0.12f, 0.3f) * s,
                    ShaderHelper.FurMaterial(new Color(0.3f, 0.22f, 0.15f)), unit);
                break;
        }
    }

    public static void BuildHelmet(Unit unit, UnitVisualConfig config, Color metalColor)
    {
        float s = config.bodyScale;
        var metal = ShaderHelper.SteelMaterial(metalColor);
        var cloth = ShaderHelper.ClothMaterial(config.clothTint);

        switch (config.helmetStyle)
        {
            case HelmetStyle.None:
                break;
            case HelmetStyle.Hood:
                CreateSphere("Hood", unit.pivotNeck, new Vector3(0, 0.26f * s, -0.04f),
                    new Vector3(0.42f, 0.32f, 0.42f) * s, cloth, unit);
                CreatePart(PrimitiveType.Cube, "HoodBrim", unit.pivotNeck,
                    new Vector3(0, 0.13f * s, 0.13f), new Vector3(0.4f, 0.06f, 0.08f) * s, cloth, unit);
                break;
            case HelmetStyle.Conical:
                CreatePart(PrimitiveType.Cube, "Helmet", unit.pivotNeck,
                    new Vector3(0, 0.3f * s, 0), new Vector3(0.42f, 0.28f, 0.42f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "HelmetRim", unit.pivotNeck,
                    new Vector3(0, 0.2f * s, 0), new Vector3(0.44f, 0.05f, 0.44f) * s, metal, unit);
                break;
            case HelmetStyle.Nasal:
                CreateSphere("Helmet", unit.pivotNeck, new Vector3(0, 0.32f * s, 0),
                    new Vector3(0.42f, 0.22f, 0.42f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "HelmetRim", unit.pivotNeck,
                    new Vector3(0, 0.22f * s, 0), new Vector3(0.44f, 0.05f, 0.44f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "NasalGuard", unit.pivotNeck,
                    new Vector3(0, 0.2f * s, 0.17f), new Vector3(0.05f, 0.25f, 0.05f) * s, metal, unit);
                break;
            case HelmetStyle.Spectacle:
                CreateSphere("Helmet", unit.pivotNeck, new Vector3(0, 0.3f * s, 0),
                    new Vector3(0.44f, 0.24f, 0.44f) * s, metal, unit);
                CreatePart(PrimitiveType.Cube, "FaceGuard", unit.pivotNeck,
                    new Vector3(0, 0.2f * s, 0.18f), new Vector3(0.35f, 0.08f, 0.04f) * s, metal, unit);
                break;
            case HelmetStyle.Turban:
                CreatePart(PrimitiveType.Cube, "Turban", unit.pivotNeck,
                    new Vector3(0, 0.28f * s, 0), new Vector3(0.48f, 0.2f, 0.48f) * s, cloth, unit);
                CreatePart(PrimitiveType.Cube, "TurbanWrap", unit.pivotNeck,
                    new Vector3(0, 0.35f * s, 0), new Vector3(0.4f, 0.08f, 0.4f) * s, cloth, unit);
                break;
            case HelmetStyle.Straw:
                CreatePart(PrimitiveType.Cube, "StrawHat", unit.pivotNeck,
                    new Vector3(0, 0.35f * s, 0), new Vector3(0.5f, 0.06f, 0.5f) * s,
                    ShaderHelper.LeatherMaterial(new Color(0.6f, 0.5f, 0.3f)), unit);
                CreatePart(PrimitiveType.Cylinder, "StrawCrown", unit.pivotNeck,
                    new Vector3(0, 0.28f * s, 0), new Vector3(0.35f, 0.12f, 0.35f) * s,
                    ShaderHelper.LeatherMaterial(new Color(0.55f, 0.45f, 0.25f)), unit);
                break;
            case HelmetStyle.Feathered:
                CreatePart(PrimitiveType.Cube, "Headband", unit.pivotNeck,
                    new Vector3(0, 0.3f * s, 0), new Vector3(0.4f, 0.06f, 0.4f) * s, cloth, unit);
                CreatePart(PrimitiveType.Cube, "Feather", unit.pivotNeck,
                    new Vector3(0, 0.5f * s, 0), new Vector3(0.04f, 0.25f, 0.08f) * s, cloth, unit);
                break;
            case HelmetStyle.Crown:
                CreatePart(PrimitiveType.Cube, "CrownBase", unit.pivotNeck,
                    new Vector3(0, 0.28f * s, 0), new Vector3(0.42f, 0.08f, 0.42f) * s,
                    ShaderHelper.GoldMaterial(DefaultGold), unit);
                CreatePart(PrimitiveType.Cube, "CrownSpike", unit.pivotNeck,
                    new Vector3(0, 0.42f * s, 0), new Vector3(0.05f, 0.12f, 0.05f) * s,
                    ShaderHelper.GoldMaterial(DefaultGold), unit);
                break;
            case HelmetStyle.Wrapped:
                CreatePart(PrimitiveType.Cube, "HeadWrap", unit.pivotNeck,
                    new Vector3(0, 0.26f * s, 0), new Vector3(0.44f, 0.22f, 0.44f) * s, cloth, unit);
                break;
        }
    }

    public static void BuildWeapon(Unit unit, UnitVisualConfig config)
    {
        var wood = ShaderHelper.WoodMaterial(DefaultWood);
        var metal = MaterialFor(config.weaponMaterial, config.armorTint);
        var leather = ShaderHelper.LeatherMaterial(DefaultLeather);
        float s = config.bodyScale;

        switch (config.primaryWeapon)
        {
            case WeaponStyle.None:
            case WeaponStyle.Elephant:
                break;
            case WeaponStyle.Sword:
                CreatePart(PrimitiveType.Cube, "SwordGrip", unit.pivotRightHand,
                    new Vector3(0, -0.02f * s, 0), new Vector3(0.04f, 0.12f, 0.04f) * s, leather, unit);
                CreateSphere("SwordPommel", unit.pivotRightHand,
                    new Vector3(0, -0.09f * s, 0), new Vector3(0.06f, 0.06f, 0.06f) * s,
                    ShaderHelper.GoldMaterial(DefaultGold), unit);
                CreatePart(PrimitiveType.Cube, "SwordGuard", unit.pivotRightHand,
                    new Vector3(0, 0.04f * s, 0), new Vector3(0.16f, 0.035f, 0.05f) * s,
                    ShaderHelper.GoldMaterial(DefaultGold), unit);
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "SwordBlade", unit.pivotRightHand,
                    new Vector3(0, 0.04f * s, 0.32f), new Vector3(0.05f, 0.1f, 0.5f) * s, metal, unit).transform;
                unit.weaponTip = CreatePivot("WeaponTip", unit.pivotRightHand, new Vector3(0, 0.04f * s, 0.55f));
                break;
            case WeaponStyle.Axe:
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "AxeHandle", unit.pivotRightHand,
                    new Vector3(0, 0, 0.28f), new Vector3(0.04f, 0.04f, 0.48f) * s, wood, unit).transform;
                CreatePart(PrimitiveType.Cube, "AxeHead", unit.pivotRightHand,
                    new Vector3(0, 0.08f * s, 0.48f), new Vector3(0.03f, 0.22f, 0.12f) * s, metal, unit);
                unit.weaponTip = CreatePivot("AxeTip", unit.pivotRightHand, new Vector3(0, 0.2f * s, 0.54f));
                break;
            case WeaponStyle.Spear:
                unit.partWeapon = CreateCylinder("SpearShaft", unit.pivotRightHand,
                    new Vector3(0, 0, 0.5f), new Vector3(0.03f, 0.03f, 0.55f) * s, wood, unit).transform;
                unit.partWeapon.localRotation = Quaternion.Euler(90, 0, 0);
                CreatePart(PrimitiveType.Cube, "SpearHead", unit.pivotRightHand,
                    new Vector3(0, 0, 0.95f), new Vector3(0.03f, 0.07f, 0.12f) * s, metal, unit);
                unit.weaponTip = CreatePivot("SpearTip", unit.pivotRightHand, new Vector3(0, 0, 1.05f));
                break;
            case WeaponStyle.Bow:
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "BowUpper", unit.pivotRightHand,
                    new Vector3(0, 0.22f, 0.1f), new Vector3(0.03f, 0.28f, 0.07f) * s, wood, unit).transform;
                CreatePart(PrimitiveType.Cube, "BowLower", unit.pivotRightHand,
                    new Vector3(0, -0.2f, 0.1f), new Vector3(0.03f, 0.26f, 0.06f) * s, wood, unit);
                CreatePart(PrimitiveType.Cube, "BowString", unit.pivotRightHand,
                    new Vector3(0, 0, 0.04f), new Vector3(0.015f, 0.5f, 0.015f) * s,
                    ShaderHelper.BoneMaterial(DefaultBone), unit);
                unit.weaponTip = CreatePivot("BowTip", unit.pivotRightHand, new Vector3(0, 0.35f, 0.1f));
                break;
            case WeaponStyle.Crossbow:
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "CrossbowStock", unit.pivotRightHand,
                    new Vector3(0, 0, 0.2f), new Vector3(0.06f, 0.12f, 0.25f) * s, wood, unit).transform;
                CreatePart(PrimitiveType.Cube, "CrossbowProd", unit.pivotRightHand,
                    new Vector3(0, 0.08f, 0.25f), new Vector3(0.02f, 0.02f, 0.2f) * s, wood, unit);
                CreatePart(PrimitiveType.Cube, "CrossbowBolt", unit.pivotRightHand,
                    new Vector3(0, 0.08f, 0.4f), new Vector3(0.015f, 0.015f, 0.15f) * s, metal, unit);
                unit.weaponTip = CreatePivot("CrossbowTip", unit.pivotRightHand, new Vector3(0, 0.08f, 0.5f));
                break;
            case WeaponStyle.Club:
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "Club", unit.pivotRightHand,
                    new Vector3(0, 0, 0.25f), new Vector3(0.08f, 0.08f, 0.4f) * s, wood, unit).transform;
                unit.weaponTip = CreatePivot("ClubTip", unit.pivotRightHand, new Vector3(0, 0, 0.45f));
                break;
            case WeaponStyle.Mace:
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "MaceHandle", unit.pivotRightHand,
                    new Vector3(0, 0, 0.2f), new Vector3(0.04f, 0.04f, 0.35f) * s, wood, unit).transform;
                CreateSphere("MaceHead", unit.pivotRightHand,
                    new Vector3(0, 0, 0.4f), new Vector3(0.1f, 0.1f, 0.1f) * s, metal, unit);
                unit.weaponTip = CreatePivot("MaceTip", unit.pivotRightHand, new Vector3(0, 0, 0.5f));
                break;
            case WeaponStyle.Javelin:
                unit.partWeapon = CreateCylinder("JavelinShaft", unit.pivotRightHand,
                    new Vector3(0, 0, 0.35f), new Vector3(0.025f, 0.025f, 0.4f) * s, wood, unit).transform;
                unit.partWeapon.localRotation = Quaternion.Euler(90, 0, 0);
                CreatePart(PrimitiveType.Cube, "JavelinHead", unit.pivotRightHand,
                    new Vector3(0, 0, 0.7f), new Vector3(0.02f, 0.05f, 0.08f) * s, metal, unit);
                unit.weaponTip = CreatePivot("JavelinTip", unit.pivotRightHand, new Vector3(0, 0, 0.78f));
                break;
            case WeaponStyle.Sling:
                CreatePart(PrimitiveType.Cube, "SlingPouch", unit.pivotRightHand,
                    new Vector3(0, 0, 0.05f), new Vector3(0.06f, 0.04f, 0.08f) * s, leather, unit);
                CreatePart(PrimitiveType.Cube, "SlingCord", unit.pivotRightHand,
                    new Vector3(0, 0, 0.1f), new Vector3(0.01f, 0.01f, 0.15f) * s, leather, unit);
                unit.partWeapon = unit.pivotRightHand;
                unit.weaponTip = CreatePivot("SlingTip", unit.pivotRightHand, new Vector3(0, 0, 0.2f));
                break;
            case WeaponStyle.DualAxe:
            {
                // Build primary-hand axe inline to avoid infinite recursion
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "AxeHandle", unit.pivotRightHand,
                    new Vector3(0, 0, 0.28f), new Vector3(0.04f, 0.04f, 0.48f) * s, wood, unit).transform;
                CreatePart(PrimitiveType.Cube, "AxeHead", unit.pivotRightHand,
                    new Vector3(0, 0.08f * s, 0.48f), new Vector3(0.03f, 0.22f, 0.12f) * s, metal, unit);
                unit.weaponTip = CreatePivot("AxeTip", unit.pivotRightHand, new Vector3(0, 0.2f * s, 0.54f));
                break;
            }
            case WeaponStyle.DualSword:
            {
                // Build primary-hand sword inline to avoid infinite recursion
                CreatePart(PrimitiveType.Cube, "SwordGrip", unit.pivotRightHand,
                    new Vector3(0, -0.02f * s, 0), new Vector3(0.04f, 0.12f, 0.04f) * s, leather, unit);
                CreateSphere("SwordPommel", unit.pivotRightHand,
                    new Vector3(0, -0.09f * s, 0), new Vector3(0.06f, 0.06f, 0.06f) * s,
                    ShaderHelper.GoldMaterial(DefaultGold), unit);
                CreatePart(PrimitiveType.Cube, "SwordGuard", unit.pivotRightHand,
                    new Vector3(0, 0.04f * s, 0), new Vector3(0.16f, 0.035f, 0.05f) * s,
                    ShaderHelper.GoldMaterial(DefaultGold), unit);
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "SwordBlade", unit.pivotRightHand,
                    new Vector3(0, 0.04f * s, 0.32f), new Vector3(0.05f, 0.1f, 0.5f) * s, metal, unit).transform;
                unit.weaponTip = CreatePivot("WeaponTip", unit.pivotRightHand, new Vector3(0, 0.04f * s, 0.55f));
                break;
            }
            case WeaponStyle.Atlatl:
                unit.partWeapon = CreatePart(PrimitiveType.Cube, "Atlatl", unit.pivotRightHand,
                    new Vector3(0, 0, 0.15f), new Vector3(0.03f, 0.08f, 0.25f) * s, wood, unit).transform;
                unit.weaponTip = CreatePivot("AtlatlTip", unit.pivotRightHand, new Vector3(0, 0, 0.35f));
                break;
        }
    }

    public static void BuildSecondaryWeapon(Unit unit, UnitVisualConfig config)
    {
        if (config.shieldStyle != ShieldStyle.None)
        {
            BuildShield(unit, config);
            return;
        }
        var wood = ShaderHelper.WoodMaterial(DefaultWood);
        var metal = MaterialFor(config.weaponMaterial, config.armorTint);
        float s = config.bodyScale;

        var sec = config.secondaryWeapon;
        if (config.primaryWeapon == WeaponStyle.DualAxe) sec = WeaponStyle.Axe;
        else if (config.primaryWeapon == WeaponStyle.DualSword) sec = WeaponStyle.Sword;
        if (sec == WeaponStyle.None) return;

        switch (sec)
        {
            case WeaponStyle.Sword:
                unit.partWeaponLeft = CreatePart(PrimitiveType.Cube, "OffhandSword", unit.pivotLeftHand,
                    new Vector3(0, 0, 0.2f), new Vector3(0.04f, 0.1f, 0.35f) * s, metal, unit).transform;
                unit.weaponLeftTip = CreatePivot("OffhandTip", unit.pivotLeftHand, new Vector3(0, 0, 0.4f));
                break;
            case WeaponStyle.Axe:
                unit.partWeaponLeft = CreatePart(PrimitiveType.Cube, "OffhandAxeHandle", unit.pivotLeftHand,
                    new Vector3(0, 0, 0.25f), new Vector3(0.04f, 0.04f, 0.45f) * s, wood, unit).transform;
                CreatePart(PrimitiveType.Cube, "OffhandAxeHead", unit.pivotLeftHand,
                    new Vector3(0, 0.08f * s, 0.45f), new Vector3(0.03f, 0.2f, 0.1f) * s, metal, unit);
                unit.weaponLeftTip = CreatePivot("OffhandAxeTip", unit.pivotLeftHand, new Vector3(0, 0.18f * s, 0.5f));
                break;
        }
    }

    static void BuildShield(Unit unit, UnitVisualConfig config)
    {
        var wood = ShaderHelper.WoodMaterial(DefaultWood);
        var metal = ShaderHelper.SteelMaterial(config.armorTint);
        var cloth = ShaderHelper.ClothMaterial(config.clothTint);
        float s = config.bodyScale;

        switch (config.shieldStyle)
        {
            case ShieldStyle.Round:
                unit.partWeaponLeft = CreateCylinder("Shield", unit.pivotLeftHand,
                    new Vector3(-0.12f * s, 0.05f, 0.05f), new Vector3(0.45f, 0.025f, 0.45f) * s, wood, unit).transform;
                unit.partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
                CreateCylinder("ShieldBoss", unit.pivotLeftHand,
                    new Vector3(-0.16f * s, 0.05f, 0.05f), new Vector3(0.1f, 0.04f, 0.1f) * s, metal, unit);
                unit.weaponLeftTip = null;
                break;
            case ShieldStyle.Kite:
                unit.partWeaponLeft = CreatePart(PrimitiveType.Cube, "KiteShield", unit.pivotLeftHand,
                    new Vector3(-0.15f * s, 0.1f, 0.05f), new Vector3(0.04f, 0.5f, 0.4f) * s, wood, unit).transform;
                unit.partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
                CreatePart(PrimitiveType.Cube, "ShieldBoss", unit.pivotLeftHand,
                    new Vector3(-0.2f * s, 0.1f, 0.05f), new Vector3(0.06f, 0.12f, 0.08f) * s, metal, unit);
                unit.weaponLeftTip = null;
                break;
            case ShieldStyle.Tower:
                unit.partWeaponLeft = CreatePart(PrimitiveType.Cube, "TowerShield", unit.pivotLeftHand,
                    new Vector3(-0.2f * s, 0.15f, 0.08f), new Vector3(0.05f, 0.65f, 0.5f) * s, wood, unit).transform;
                unit.partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
                CreatePart(PrimitiveType.Cube, "ShieldBoss", unit.pivotLeftHand,
                    new Vector3(-0.25f * s, 0.15f, 0.08f), new Vector3(0.08f, 0.15f, 0.1f) * s, metal, unit);
                unit.weaponLeftTip = null;
                break;
            case ShieldStyle.Buckler:
                unit.partWeaponLeft = CreateCylinder("Buckler", unit.pivotLeftHand,
                    new Vector3(-0.08f * s, 0.02f, 0.02f), new Vector3(0.25f, 0.03f, 0.25f) * s, wood, unit).transform;
                unit.partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
                CreateCylinder("BucklerBoss", unit.pivotLeftHand,
                    new Vector3(-0.1f * s, 0.02f, 0.02f), new Vector3(0.06f, 0.035f, 0.06f) * s, metal, unit);
                unit.weaponLeftTip = null;
                break;
            case ShieldStyle.None:
                break;
        }
    }

    public static void BuildAccessories(Unit unit, UnitVisualConfig config, Color factionPrimary)
    {
        float s = config.bodyScale;
        var cloth = ShaderHelper.ClothMaterial(factionPrimary * 0.65f);
        var leather = ShaderHelper.LeatherMaterial(DefaultLeather);

        CreatePart(PrimitiveType.Cube, "Belt", unit.pivotHips,
            new Vector3(0, 0.3f * s, 0), new Vector3(0.5f, 0.06f, 0.3f) * s, leather, unit);
        CreatePart(PrimitiveType.Cube, "BeltBuckle", unit.pivotHips,
            new Vector3(0, 0.3f * s, 0.16f), new Vector3(0.06f, 0.05f, 0.025f) * s,
            ShaderHelper.GoldMaterial(DefaultGold), unit);

        if (config.hasCape)
        {
            CreatePart(PrimitiveType.Cube, "Cape", unit.pivotCape,
                new Vector3(0, -0.35f * s, -0.02f), new Vector3(0.44f, 0.6f, 0.035f) * s, cloth, unit);
        }

        if (config.hasBackItem)
        {
            CreatePart(PrimitiveType.Cube, "Quiver", unit.pivotWaist,
                new Vector3(0.12f * s, 0.2f, -0.15f), new Vector3(0.1f, 0.4f, 0.1f) * s, leather, unit);
            CreatePart(PrimitiveType.Cube, "Arrows", unit.pivotWaist,
                new Vector3(0.12f * s, 0.42f, -0.15f), new Vector3(0.05f, 0.08f, 0.05f) * s,
                ShaderHelper.WoodMaterial(DefaultWood), unit);
        }
    }

    public static void BuildLegacyViking(Unit unit, string legacyType, Color primary, Color secondary)
    {
        var config = new UnitVisualConfig { bodyScale = 1f, shoulderWidth = 0.28f, hipWidth = 0.12f };
        BuildSkeleton(unit, config);

        switch (legacyType)
        {
            case "Huscarl":
            case "Swordsman":
                unit.BuildLegacySwordsmanModel(primary, secondary);
                break;
            case "Hunter":
            case "Archer":
                unit.BuildLegacyArcherModel(primary, secondary);
                break;
            case "Berserker":
                unit.BuildLegacyBerserkerModel(primary, secondary);
                break;
            case "Shieldbearer":
                unit.BuildLegacyShieldbearerModel(primary, secondary);
                break;
        }
    }

    static Transform CreatePivot(string name, Transform parent, Vector3 localPos)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj.transform;
    }

    static GameObject CreatePart(PrimitiveType type, string name, Transform parent,
        Vector3 localPos, Vector3 scale, Material mat, Unit unit)
    {
        var part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent);
        part.transform.localPosition = localPos;
        part.transform.localScale = scale;
        part.transform.localRotation = Quaternion.identity;
        part.GetComponent<Renderer>().material = mat;
        if (part.TryGetComponent<Collider>(out var col)) Object.Destroy(col);
        part.layer = unit.gameObject.layer;
        return part;
    }

    static GameObject CreateSphere(string name, Transform parent, Vector3 localPos,
        Vector3 scale, Material mat, Unit unit) =>
        CreatePart(PrimitiveType.Sphere, name, parent, localPos, scale, mat, unit);

    static GameObject CreateCylinder(string name, Transform parent, Vector3 localPos,
        Vector3 scale, Material mat, Unit unit) =>
        CreatePart(PrimitiveType.Cylinder, name, parent, localPos, scale, mat, unit);

    static void AddEyes(Transform neck, float s, Unit unit)
    {
        var boneWhite = ShaderHelper.CreateMaterial(DefaultBone);
        var pupil = ShaderHelper.CreateMaterial(new Color(0.2f, 0.3f, 0.5f));
        float eyeX = 0.08f * s;
        CreatePart(PrimitiveType.Cube, "LeftEye", neck,
            new Vector3(-eyeX, 0.2f * s, 0.15f), new Vector3(0.055f, 0.04f, 0.03f) * s, boneWhite, unit);
        CreatePart(PrimitiveType.Cube, "LeftPupil", neck,
            new Vector3(-eyeX, 0.2f * s, 0.165f), new Vector3(0.03f, 0.03f, 0.02f) * s, pupil, unit);
        CreatePart(PrimitiveType.Cube, "RightEye", neck,
            new Vector3(eyeX, 0.2f * s, 0.15f), new Vector3(0.055f, 0.04f, 0.03f) * s, boneWhite, unit);
        CreatePart(PrimitiveType.Cube, "RightPupil", neck,
            new Vector3(eyeX, 0.2f * s, 0.165f), new Vector3(0.03f, 0.03f, 0.02f) * s, pupil, unit);
    }

    static Material MaterialFor(MaterialPreset preset, Color color)
    {
        return preset switch
        {
            MaterialPreset.Skin => ShaderHelper.SkinMaterial(color),
            MaterialPreset.Chainmail => ShaderHelper.ChainmailMaterial(color),
            MaterialPreset.Steel => ShaderHelper.SteelMaterial(color),
            MaterialPreset.Gold => ShaderHelper.GoldMaterial(color),
            MaterialPreset.Leather => ShaderHelper.LeatherMaterial(color),
            MaterialPreset.Fur => ShaderHelper.FurMaterial(color),
            MaterialPreset.Wood => ShaderHelper.WoodMaterial(color),
            MaterialPreset.Bone => ShaderHelper.BoneMaterial(color),
            MaterialPreset.Cloth or MaterialPreset.Silk or MaterialPreset.Cotton => ShaderHelper.ClothMaterial(color),
            _ => ShaderHelper.CreateMaterial(color)
        };
    }
}
