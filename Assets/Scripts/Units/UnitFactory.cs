using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class UnitFactory
{
    public static Unit CreateUnit(UnitTypeDefinition typeDef, FactionDefinition faction, Faction side,
        Vector3 position, Quaternion rotation)
    {
        GameObject unitObj = new GameObject(typeDef.displayName);
        unitObj.transform.position = position;
        unitObj.transform.rotation = rotation;

        CapsuleCollider col = unitObj.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1.1f, 0);
        col.radius = 0.45f;
        col.height = 2.2f;

        NavMeshAgent agent = unitObj.AddComponent<NavMeshAgent>();
        agent.radius = col.radius;
        agent.height = 2.0f;
        agent.baseOffset = 0f;

        Unit unit = unitObj.AddComponent<Unit>();
        unit.faction = side;
        unit.typeDefinition = typeDef;
        unit.factionDefinition = faction;

        ApplyStats(unit, typeDef);
        unit.currentHealth = unit.maxHealth;

        UnitModelBuilder.BuildModel(unit, typeDef.visualConfig, faction.primaryColor, faction.secondaryColor);

        AttachComponents(unitObj, typeDef);
        UnitAnimator animator = unitObj.GetComponent<UnitAnimator>();
        WireAnimator(unit, animator);
        animator.InitializeRests();

        agent.speed = unit.moveSpeed;

        unit.EnsureSelectionRingAndShadow();

        int layer = LayerMask.NameToLayer(side == Faction.Attacker ? "Attacker" : "Defender");
        if (layer >= 0) unitObj.layer = layer;

        return unit;
    }

    private static void AttachComponents(GameObject unitObj, UnitTypeDefinition typeDef)
    {
        unitObj.AddComponent<UnitMovement>();
        unitObj.AddComponent<UnitCombat>();
        unitObj.AddComponent<UnitAnimator>();
        unitObj.AddComponent<HealthBar>();
    }

    private static void WireAnimator(Unit unit, UnitAnimator animator)
    {
        if (animator == null) return;

        animator.pivotHips = unit.pivotHips;
        animator.pivotWaist = unit.pivotWaist;
        animator.pivotNeck = unit.pivotNeck;
        animator.pivotLeftShoulder = unit.pivotLeftShoulder;
        animator.pivotRightShoulder = unit.pivotRightShoulder;
        animator.pivotLeftElbow = unit.pivotLeftElbow;
        animator.pivotRightElbow = unit.pivotRightElbow;
        animator.pivotLeftHand = unit.pivotLeftHand;
        animator.pivotRightHand = unit.pivotRightHand;
        animator.pivotLeftHip = unit.pivotLeftHip;
        animator.pivotRightHip = unit.pivotRightHip;
        animator.pivotLeftKnee = unit.pivotLeftKnee;
        animator.pivotRightKnee = unit.pivotRightKnee;
        animator.pivotCape = unit.pivotCape;

        animator.head = unit.partHead;
        animator.body = unit.partBody;
        animator.leftArm = unit.partLeftArm;
        animator.rightArm = unit.partRightArm;
        animator.leftLeg = unit.partLeftLeg;
        animator.rightLeg = unit.partRightLeg;
        animator.weapon = unit.partWeapon;
        animator.weaponLeft = unit.partWeaponLeft;
    }

    private static void ApplyStats(Unit unit, UnitTypeDefinition typeDef)
    {
        unit.maxHealth = typeDef.maxHP;
        unit.attackDamage = typeDef.attackDamage;
        unit.armor = typeDef.armor;
        unit.moveSpeed = typeDef.moveSpeed;
        unit.attackRange = typeDef.attackRange;
        unit.attackCooldown = typeDef.attackCooldown;
    }
}
