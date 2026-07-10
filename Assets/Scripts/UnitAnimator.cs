using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit))]
public class UnitAnimator : MonoBehaviour
{
    Unit unit;
    NavMeshAgent agent;

    public bool useUnscaledTime;
    float dt => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    // Pivot references (set by spawner after Unit builds model)
    [HideInInspector] public Transform pivotHips;
    [HideInInspector] public Transform pivotWaist;
    [HideInInspector] public Transform pivotNeck;
    [HideInInspector] public Transform pivotLeftShoulder;
    [HideInInspector] public Transform pivotRightShoulder;
    [HideInInspector] public Transform pivotLeftElbow;
    [HideInInspector] public Transform pivotRightElbow;
    [HideInInspector] public Transform pivotLeftHand;
    [HideInInspector] public Transform pivotRightHand;
    [HideInInspector] public Transform pivotLeftHip;
    [HideInInspector] public Transform pivotRightHip;
    [HideInInspector] public Transform pivotLeftKnee;
    [HideInInspector] public Transform pivotRightKnee;
    [HideInInspector] public Transform pivotCape;

    // Legacy compat references
    [HideInInspector] public Transform head;
    [HideInInspector] public Transform body;
    [HideInInspector] public Transform leftArm;
    [HideInInspector] public Transform rightArm;
    [HideInInspector] public Transform leftLeg;
    [HideInInspector] public Transform rightLeg;
    [HideInInspector] public Transform weapon;
    [HideInInspector] public Transform weaponLeft;

    // Rest rotations for pivots
    Quaternion[] pivotRests;
    Transform[] allPivots;

    // Animation state
    public enum AnimState { Idle, Walking, Attacking, Dying, Dead }
    public AnimState currentState = AnimState.Idle;

    float walkCycleTimer;
    float idleTimer;
    float attackAnimTimer;
    float attackAnimDuration = 0.5f;
    float deathTimer;
    bool deathAnimDone;

    // Secondary motion (cape, hair)
    Vector3 capeVelocity;
    float capeAngleX;
    float capeAngleZ;

    // Rage fire particles
    float rageParticleTimer;
    float rageParticleInterval = 0.08f;

    // Shield wall pulse
    float shieldPulseTimer;

    // Footstep dust
    float dustTimer;
    float dustInterval = 0.28f;

    // Berserker idle twitch
    float twitchTimer;

    // Weapon drop on death
    bool weaponDropped;

    // Data-driven animation profile (set via SetAnimationProfile for typeDefinition units)
    UnitCategory? animCategory;
    WeaponStyle animWeapon = WeaponStyle.None;

    void Start()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void InitializeRests()
    {
        CachePivots();
        StorePivotRests();
    }

    void CachePivots()
    {
        if (unit == null) unit = GetComponent<Unit>();
        pivotHips = unit.pivotHips;
        pivotWaist = unit.pivotWaist;
        pivotNeck = unit.pivotNeck;
        pivotLeftShoulder = unit.pivotLeftShoulder;
        pivotRightShoulder = unit.pivotRightShoulder;
        pivotLeftElbow = unit.pivotLeftElbow;
        pivotRightElbow = unit.pivotRightElbow;
        pivotLeftHand = unit.pivotLeftHand;
        pivotRightHand = unit.pivotRightHand;
        pivotLeftHip = unit.pivotLeftHip;
        pivotRightHip = unit.pivotRightHip;
        pivotLeftKnee = unit.pivotLeftKnee;
        pivotRightKnee = unit.pivotRightKnee;
        pivotCape = unit.pivotCape;

        allPivots = new Transform[] {
            pivotHips, pivotWaist, pivotNeck,
            pivotLeftShoulder, pivotRightShoulder,
            pivotLeftElbow, pivotRightElbow,
            pivotLeftHand, pivotRightHand,
            pivotLeftHip, pivotRightHip,
            pivotLeftKnee, pivotRightKnee,
            pivotCape
        };
    }

    void StorePivotRests()
    {
        if (allPivots == null) return;
        pivotRests = new Quaternion[allPivots.Length];
        for (int i = 0; i < allPivots.Length; i++)
        {
            pivotRests[i] = allPivots[i] != null ? allPivots[i].localRotation : Quaternion.identity;
        }
    }

    void ResetAllPivots()
    {
        if (allPivots == null || pivotRests == null) return;
        for (int i = 0; i < allPivots.Length; i++)
        {
            if (allPivots[i] != null)
                allPivots[i].localRotation = pivotRests[i];
        }
    }

    // ========== EASING CURVES ==========

    static float EaseInOut(float t) => t * t * (3f - 2f * t);

    static float EaseOutBack(float t)
    {
        float s = 1.7f;
        t -= 1f;
        return 1f + t * t * ((s + 1f) * t + s);
    }

    static float EaseOutElastic(float t)
    {
        if (t <= 0f) return 0f;
        if (t >= 1f) return 1f;
        return Mathf.Sin(-13f * Mathf.PI * 0.5f * (t + 1f)) * Mathf.Pow(2f, -10f * t) + 1f;
    }

    static float EaseInQuad(float t) => t * t;
    static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);

    // ========== MAIN UPDATE ==========

    void Update()
    {
        if (unit == null || deathAnimDone) return;
        if (allPivots == null) CachePivots();

        if (unit.isDead)
        {
            if (currentState != AnimState.Dying && currentState != AnimState.Dead)
                StartDeathAnimation();
        }
        else if (currentState == AnimState.Attacking)
        {
            // Let attack play out
        }
        else if (agent != null && agent.isOnNavMesh && agent.velocity.sqrMagnitude > 0.3f)
        {
            currentState = AnimState.Walking;
        }
        else
        {
            currentState = AnimState.Idle;
        }

        switch (currentState)
        {
            case AnimState.Idle: AnimateIdle(); break;
            case AnimState.Walking: AnimateWalk(); break;
            case AnimState.Attacking: AnimateAttack(); break;
            case AnimState.Dying: AnimateDeath(); break;
        }

        // Secondary motion
        if (!unit.isDead)
        {
            AnimateCape();
            if (unit.isEnraged) AnimateRageFire();
            if (unit.isShieldWalling) AnimateShieldWallPulse();
            if (unit.isMarked) AnimateMarkSpin();
        }
    }

    // ========== IDLE ANIMATION ==========

    void AnimateIdle()
    {
        ResetAllPivots();
        idleTimer += dt * 1.5f;

        float breathX = Mathf.Sin(idleTimer) * 2f;
        float swayZ = Mathf.Sin(idleTimer * 0.37f) * 1f;

        // Breathing: waist rocks forward/back
        if (pivotWaist != null)
            pivotWaist.localRotation = Quaternion.Euler(breathX, 0, 0);

        // Head follows at half rate, counters slightly
        if (pivotNeck != null)
            pivotNeck.localRotation = Quaternion.Euler(-breathX * 0.4f, 0, 0);

        // Hips sway
        if (pivotHips != null)
            pivotHips.localRotation = Quaternion.Euler(0, 0, swayZ);

        // Weapon arm idle sway
        float armSway = Mathf.Sin(idleTimer * 0.8f) * 3f;
        if (pivotRightShoulder != null)
            pivotRightShoulder.localRotation = Quaternion.Euler(armSway, 0, 0);
        if (pivotLeftShoulder != null)
            pivotLeftShoulder.localRotation = Quaternion.Euler(-armSway * 0.5f, 0, 0);

        // Unit-specific idle
        switch (unit.unitType)
        {
            case UnitType.Berserker:
                AnimateBerserkerIdle();
                break;
            case UnitType.Shieldbearer:
                if (pivotLeftShoulder != null)
                    pivotLeftShoulder.localRotation *= Quaternion.Euler(8f, 0, 0);
                break;
            case UnitType.Archer:
                float stringCheck = Mathf.Sin(idleTimer * 0.2f);
                if (stringCheck > 0.95f && pivotRightElbow != null)
                    pivotRightElbow.localRotation = Quaternion.Euler(0, 0, -15f * (stringCheck - 0.95f) * 20f);
                break;
        }
    }

    void AnimateBerserkerIdle()
    {
        twitchTimer -= dt;
        if (twitchTimer <= 0f)
        {
            twitchTimer = Random.Range(2f, 5f);
        }

        float swayAmount = Mathf.Sin(idleTimer * 0.5f) * 1.5f;
        if (pivotHips != null)
            pivotHips.localRotation *= Quaternion.Euler(0, 0, swayAmount);

        // Head twitch
        if (twitchTimer < 0.15f && pivotNeck != null)
        {
            float twitch = Mathf.Sin(twitchTimer * 40f) * 5f;
            pivotNeck.localRotation *= Quaternion.Euler(0, twitch, 0);
        }
    }

    // ========== WALK ANIMATION ==========

    void AnimateWalk()
    {
        ResetAllPivots();

        float speed = agent != null ? agent.velocity.magnitude : 3f;
        float walkFreq = Mathf.Clamp(speed * 1.6f, 3f, 9f);
        walkCycleTimer += dt * walkFreq;

        float phase = Mathf.Sin(walkCycleTimer);
        float phaseOff = Mathf.Cos(walkCycleTimer);

        // Hip swing: forward/back rotation
        float hipSwing = 35f;
        if (pivotLeftHip != null)
            pivotLeftHip.localRotation = Quaternion.Euler(phase * hipSwing, 0, 0);
        if (pivotRightHip != null)
            pivotRightHip.localRotation = Quaternion.Euler(-phase * hipSwing, 0, 0);

        // Knee bend on back-swing (when leg goes backward, knee bends more)
        float leftKneeBend = Mathf.Max(0, phase) * -40f;
        float rightKneeBend = Mathf.Max(0, -phase) * -40f;
        if (pivotLeftKnee != null)
            pivotLeftKnee.localRotation = Quaternion.Euler(leftKneeBend, 0, 0);
        if (pivotRightKnee != null)
            pivotRightKnee.localRotation = Quaternion.Euler(rightKneeBend, 0, 0);

        // Arm counter-swing (opposite to legs)
        float armSwing = 25f;
        if (pivotLeftShoulder != null)
            pivotLeftShoulder.localRotation = Quaternion.Euler(-phase * armSwing, 0, 0);
        if (pivotRightShoulder != null)
            pivotRightShoulder.localRotation = Quaternion.Euler(phase * armSwing, 0, 0);

        // Elbow bend on back-swing
        float leftElbowBend = Mathf.Max(0, -phase) * -20f;
        float rightElbowBend = Mathf.Max(0, phase) * -20f;
        if (pivotLeftElbow != null)
            pivotLeftElbow.localRotation = Quaternion.Euler(leftElbowBend, 0, 0);
        if (pivotRightElbow != null)
            pivotRightElbow.localRotation = Quaternion.Euler(rightElbowBend, 0, 0);

        // Body bob (double frequency)
        float bob = Mathf.Abs(Mathf.Sin(walkCycleTimer * 2f)) * 0.04f;
        float bodyTilt = phase * 2f;
        if (pivotWaist != null)
        {
            Vector3 pos = pivotWaist.localPosition;
            pos.y = 0.35f + bob;
            pivotWaist.localPosition = pos;
            pivotWaist.localRotation = Quaternion.Euler(3f, 0, bodyTilt);
        }

        // Head stabilization (counter body tilt)
        if (pivotNeck != null)
            pivotNeck.localRotation = Quaternion.Euler(-1f, 0, -bodyTilt * 0.5f);

        // Hips slight rotation
        if (pivotHips != null)
        {
            Vector3 hipPos = pivotHips.localPosition;
            hipPos.y = 0.65f + bob * 0.5f;
            pivotHips.localPosition = hipPos;
        }

        // Footstep dust
        dustTimer -= dt;
        if (dustTimer <= 0f && speed > 1f)
        {
            SpawnDust(transform.position);
            dustTimer = dustInterval;
        }
    }

    // ========== ATTACK ANIMATION ==========

    public void SetAnimationProfile(UnitCategory category, WeaponStyle weapon)
    {
        animCategory = category;
        animWeapon = weapon;
    }

    public void PlayAttackAnimation()
    {
        currentState = AnimState.Attacking;
        attackAnimTimer = 0f;

        if (animCategory.HasValue && animWeapon != WeaponStyle.None)
        {
            attackAnimDuration = GetDurationFromWeaponStyle(animWeapon);
        }
        else
        {
            switch (unit.unitType)
            {
                case UnitType.Swordsman: attackAnimDuration = 0.55f; break;
                case UnitType.Archer: attackAnimDuration = 0.65f; break;
                case UnitType.Berserker: attackAnimDuration = 0.45f; break;
                case UnitType.Shieldbearer: attackAnimDuration = 0.6f; break;
            }
        }
    }

    static float GetDurationFromWeaponStyle(WeaponStyle weapon)
    {
        switch (weapon)
        {
            case WeaponStyle.Sword:
            case WeaponStyle.DualSword: return 0.55f;
            case WeaponStyle.Bow:
            case WeaponStyle.Crossbow: return 0.65f;
            case WeaponStyle.Axe:
            case WeaponStyle.DualAxe: return 0.45f;
            case WeaponStyle.Spear: return 0.6f;
            case WeaponStyle.Club:
            case WeaponStyle.Mace: return 0.5f;
            case WeaponStyle.Javelin:
            case WeaponStyle.Sling:
            case WeaponStyle.Atlatl: return 0.55f;
            default: return 0.5f;
        }
    }

    void AnimateAttack()
    {
        ResetAllPivots();
        attackAnimTimer += dt;
        float t = Mathf.Clamp01(attackAnimTimer / attackAnimDuration);

        if (animCategory.HasValue && animWeapon != WeaponStyle.None)
        {
            switch (animWeapon)
            {
                case WeaponStyle.Sword:
                case WeaponStyle.DualSword:
                    AnimateSwordSwing(t);
                    break;
                case WeaponStyle.Axe:
                case WeaponStyle.DualAxe:
                    AnimateAxeSwing(t);
                    break;
                case WeaponStyle.Spear:
                    AnimateSpearThrust(t);
                    break;
                case WeaponStyle.Bow:
                case WeaponStyle.Crossbow:
                    AnimateBowShot(t);
                    break;
                case WeaponStyle.Club:
                case WeaponStyle.Mace:
                    AnimateClubSwing(t);
                    break;
                case WeaponStyle.Javelin:
                case WeaponStyle.Sling:
                case WeaponStyle.Atlatl:
                    AnimateJavelinThrow(t);
                    break;
                default:
                    AnimateSwordSwing(t);
                    break;
            }
        }
        else
        {
            switch (unit.unitType)
            {
                case UnitType.Swordsman: AnimateSwordAttack(t); break;
                case UnitType.Archer: AnimateBowAttack(t); break;
                case UnitType.Berserker: AnimateAxeAttack(t); break;
                case UnitType.Shieldbearer: AnimateSpearAttack(t); break;
            }
        }

        if (t >= 1f)
        {
            currentState = AnimState.Idle;
            ResetAllPivots();
        }
    }

    void AnimateSwordSwing(float t) => AnimateSwordAttack(t);
    void AnimateAxeSwing(float t) => AnimateAxeAttack(t);
    void AnimateSpearThrust(float t) => AnimateSpearAttack(t);
    void AnimateBowShot(float t) => AnimateBowAttack(t);
    void AnimateClubSwing(float t) => AnimateAxeAttack(t);
    void AnimateJavelinThrow(float t) => AnimateSpearAttack(t);

    // --- SWORDSMAN OVERHEAD SLASH ---
    void AnimateSwordAttack(float t)
    {
        if (t < 0.27f)
        {
            // Anticipation: wind up, sword goes high
            float a = EaseInOut(t / 0.27f);
            SetPivotRot(pivotRightShoulder, -60f * a, 0, -10f * a);
            SetPivotRot(pivotRightElbow, -30f * a, 0, 0);
            SetPivotRot(pivotLeftShoulder, 15f * a, 0, 0);
            SetPivotRot(pivotWaist, -5f * a, 0, 0);
            SetPivotRot(pivotHips, 0, 0, 0);
            // Knees bend to load
            SetPivotRot(pivotLeftKnee, -8f * a, 0, 0);
            SetPivotRot(pivotRightKnee, -8f * a, 0, 0);
        }
        else if (t < 0.36f)
        {
            // Hold at apex
            SetPivotRot(pivotRightShoulder, -60f, 0, -10f);
            SetPivotRot(pivotRightElbow, -30f, 0, 0);
            SetPivotRot(pivotLeftShoulder, 15f, 0, 0);
            SetPivotRot(pivotWaist, -5f, 0, 0);
            SetPivotRot(pivotLeftKnee, -8f, 0, 0);
            SetPivotRot(pivotRightKnee, -8f, 0, 0);
        }
        else if (t < 0.64f)
        {
            // Strike: explosive downward swing
            float s = EaseInQuad((t - 0.36f) / 0.28f);
            float shoulderAngle = Mathf.Lerp(-60f, 50f, s);
            float elbowAngle = Mathf.Lerp(-30f, 10f, s);
            SetPivotRot(pivotRightShoulder, shoulderAngle, 0, Mathf.Lerp(-10f, 5f, s));
            SetPivotRot(pivotRightElbow, elbowAngle, 0, 0);
            SetPivotRot(pivotWaist, Mathf.Lerp(-5f, 12f, s), 0, 0);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(15f, 8f, s), 0, 0);
            float kneeStraighten = Mathf.Lerp(-8f, 0f, s);
            SetPivotRot(pivotLeftKnee, kneeStraighten, 0, 0);
            SetPivotRot(pivotRightKnee, kneeStraighten, 0, 0);
        }
        else
        {
            // Follow-through and recovery
            float r = EaseOutBack((t - 0.64f) / 0.36f);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(50f, 0f, r), 0, Mathf.Lerp(5f, 0f, r));
            SetPivotRot(pivotRightElbow, Mathf.Lerp(10f, 0f, r), 0, 0);
            SetPivotRot(pivotWaist, Mathf.Lerp(12f, 0f, r), 0, 0);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(8f, 0f, r), 0, 0);
        }
    }

    // --- ARCHER DRAW AND RELEASE ---
    void AnimateBowAttack(float t)
    {
        if (t < 0.15f)
        {
            // Nock: reach for arrow
            float a = EaseInOut(t / 0.15f);
            SetPivotRot(pivotRightShoulder, -20f * a, -15f * a, 0);
            SetPivotRot(pivotRightElbow, -40f * a, 0, 0);
        }
        else if (t < 0.69f)
        {
            // Draw: extend bow arm, pull string back
            float d = EaseInOut((t - 0.15f) / 0.54f);
            SetPivotRot(pivotLeftShoulder, 40f * d, 0, 0);
            SetPivotRot(pivotLeftElbow, -5f * d, 0, 0);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(-20f, -45f, d), Mathf.Lerp(-15f, 0f, d), 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(-40f, -70f, d), 0, 0);
            SetPivotRot(pivotWaist, -3f * d, 0, 0);
            SetPivotRot(pivotNeck, 5f * d, 0, 0);
        }
        else if (t < 0.77f)
        {
            // Hold with tension tremble
            float tremble = Mathf.Sin(t * 80f) * 1f;
            SetPivotRot(pivotLeftShoulder, 40f, tremble, 0);
            SetPivotRot(pivotLeftElbow, -5f, 0, 0);
            SetPivotRot(pivotRightShoulder, -45f + tremble, 0, 0);
            SetPivotRot(pivotRightElbow, -70f, 0, 0);
            SetPivotRot(pivotWaist, -3f, 0, 0);
            SetPivotRot(pivotNeck, 5f, 0, 0);
        }
        else if (t < 0.85f)
        {
            // Release: snap
            float r = EaseInQuad((t - 0.77f) / 0.08f);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(-45f, 10f, r), 0, 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(-70f, -10f, r), 0, 0);
            SetPivotRot(pivotLeftShoulder, 40f - 5f * r, 0, 0);
            SetPivotRot(pivotWaist, -3f + 2f * r, 0, 0);
            SetPivotRot(pivotNeck, 5f - 3f * r, 0, 0);
        }
        else
        {
            // Recovery
            float rec = EaseOutQuad((t - 0.85f) / 0.15f);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(10f, 0f, rec), 0, 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(-10f, 0f, rec), 0, 0);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(35f, 0f, rec), 0, 0);
            SetPivotRot(pivotWaist, Mathf.Lerp(-1f, 0f, rec), 0, 0);
            SetPivotRot(pivotNeck, Mathf.Lerp(2f, 0f, rec), 0, 0);
        }
    }

    // --- BERSERKER DUAL AXE FLURRY ---
    void AnimateAxeAttack(float t)
    {
        if (t < 0.18f)
        {
            // Right wind-up
            float a = EaseInOut(t / 0.18f);
            SetPivotRot(pivotRightShoulder, -50f * a, -25f * a, 0);
            SetPivotRot(pivotRightElbow, -20f * a, 0, 0);
            SetPivotRot(pivotWaist, 0, -15f * a, 0);
        }
        else if (t < 0.4f)
        {
            // Right slash: explosive cross-body
            float s = EaseInQuad((t - 0.18f) / 0.22f);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(-50f, 60f, s), Mathf.Lerp(-25f, 30f, s), 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(-20f, 5f, s), 0, 0);
            SetPivotRot(pivotWaist, 0, Mathf.Lerp(-15f, 20f, s), 0);
            // Left starts winding up
            float ls = Mathf.Clamp01((t - 0.3f) / 0.1f);
            SetPivotRot(pivotLeftShoulder, -50f * ls, 25f * ls, 0);
            SetPivotRot(pivotLeftElbow, -20f * ls, 0, 0);
        }
        else if (t < 0.56f)
        {
            // Left wind complete
            SetPivotRot(pivotLeftShoulder, -50f, 25f, 0);
            SetPivotRot(pivotLeftElbow, -20f, 0, 0);
            // Right recovering
            float rr = EaseOutQuad((t - 0.4f) / 0.16f);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(60f, 10f, rr), Mathf.Lerp(30f, 5f, rr), 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(5f, 0f, rr), 0, 0);
        }
        else if (t < 0.78f)
        {
            // Left slash: mirror
            float s = EaseInQuad((t - 0.56f) / 0.22f);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(-50f, 60f, s), Mathf.Lerp(25f, -30f, s), 0);
            SetPivotRot(pivotLeftElbow, Mathf.Lerp(-20f, 5f, s), 0, 0);
            SetPivotRot(pivotWaist, 0, Mathf.Lerp(20f, -15f, s), 0);
            // Right resting
            SetPivotRot(pivotRightShoulder, 10f, 5f, 0);
        }
        else
        {
            // Both recover with elastic bounce
            float r = EaseOutElastic((t - 0.78f) / 0.22f);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(60f, 0f, r), Mathf.Lerp(-30f, 0f, r), 0);
            SetPivotRot(pivotLeftElbow, Mathf.Lerp(5f, 0f, r), 0, 0);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(10f, 0f, r), Mathf.Lerp(5f, 0f, r), 0);
            SetPivotRot(pivotWaist, 0, Mathf.Lerp(-15f, 0f, r), 0);
        }

        // Body lunge throughout
        float lunge = Mathf.Sin(t * Mathf.PI) * 5f;
        if (pivotWaist != null)
            pivotWaist.localRotation *= Quaternion.Euler(lunge, 0, 0);
    }

    // --- SHIELDBEARER SPEAR THRUST + SHIELD BASH ---
    void AnimateSpearAttack(float t)
    {
        if (t < 0.17f)
        {
            // Shield guard + crouch
            float a = EaseInOut(t / 0.17f);
            SetPivotRot(pivotLeftShoulder, 20f * a, 0, 0);
            SetPivotRot(pivotLeftElbow, -10f * a, 0, 0);
            SetPivotRot(pivotLeftKnee, -10f * a, 0, 0);
            SetPivotRot(pivotRightKnee, -10f * a, 0, 0);
            if (pivotHips != null)
            {
                Vector3 pos = pivotHips.localPosition;
                pos.y = 0.65f - 0.05f * a;
                pivotHips.localPosition = pos;
            }
        }
        else if (t < 0.42f)
        {
            // Spear pull-back
            float p = EaseInOut((t - 0.17f) / 0.25f);
            SetPivotRot(pivotLeftShoulder, 20f, 0, 0);
            SetPivotRot(pivotRightShoulder, -40f * p, 0, 0);
            SetPivotRot(pivotRightElbow, -35f * p, 0, 0);
            SetPivotRot(pivotLeftKnee, -10f, 0, 0);
            SetPivotRot(pivotRightKnee, -10f, 0, 0);
        }
        else if (t < 0.63f)
        {
            // Thrust forward
            float s = EaseInQuad((t - 0.42f) / 0.21f);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(-40f, 55f, s), 0, 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(-35f, 5f, s), 0, 0);
            SetPivotRot(pivotWaist, 10f * s, 0, 0);
            SetPivotRot(pivotLeftShoulder, 20f, 0, 0);
            // Knees straighten as body lunges
            float knee = Mathf.Lerp(-10f, 0f, s);
            SetPivotRot(pivotLeftKnee, knee, 0, 0);
            SetPivotRot(pivotRightKnee, knee, 0, 0);
        }
        else if (t < 0.8f)
        {
            // Shield bash
            float b = EaseInQuad((t - 0.63f) / 0.17f);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(20f, 50f, b), 0, 0);
            SetPivotRot(pivotLeftElbow, Mathf.Lerp(-10f, 5f, b), 0, 0);
            // Spear holding
            SetPivotRot(pivotRightShoulder, 55f - 15f * b, 0, 0);
            SetPivotRot(pivotRightElbow, 5f, 0, 0);
            SetPivotRot(pivotWaist, 10f + 3f * b, 0, 0);
        }
        else
        {
            // Recovery
            float r = EaseOutBack((t - 0.8f) / 0.2f);
            SetPivotRot(pivotLeftShoulder, Mathf.Lerp(50f, 0f, r), 0, 0);
            SetPivotRot(pivotLeftElbow, Mathf.Lerp(5f, 0f, r), 0, 0);
            SetPivotRot(pivotRightShoulder, Mathf.Lerp(40f, 0f, r), 0, 0);
            SetPivotRot(pivotRightElbow, Mathf.Lerp(5f, 0f, r), 0, 0);
            SetPivotRot(pivotWaist, Mathf.Lerp(13f, 0f, r), 0, 0);
            if (pivotHips != null)
            {
                Vector3 pos = pivotHips.localPosition;
                pos.y = Mathf.Lerp(0.6f, 0.65f, r);
                pivotHips.localPosition = pos;
            }
        }
    }

    // ========== DEATH ANIMATION ==========

    void StartDeathAnimation()
    {
        currentState = AnimState.Dying;
        deathTimer = 0f;
        weaponDropped = false;
    }

    void AnimateDeath()
    {
        deathTimer += dt;
        float totalDuration = 1.5f;
        float t = Mathf.Clamp01(deathTimer / totalDuration);

        if (t < 0.1f)
        {
            // Hit reaction: pitch back, arms fly out
            float a = t / 0.1f;
            SetPivotRot(pivotWaist, -20f * a, 0, 0);
            SetPivotRot(pivotLeftShoulder, -30f * a, -20f * a, 0);
            SetPivotRot(pivotRightShoulder, -30f * a, 20f * a, 0);
        }
        else if (t < 0.33f)
        {
            // Ragdoll fall: body tilts sideways, knees buckle
            float f = (t - 0.1f) / 0.23f;
            float fallDir = (unit.faction == Faction.North) ? 1f : -1f;
            float tiltZ = fallDir * 80f * EaseInQuad(f);
            SetPivotRot(pivotWaist, -20f - 10f * f, 0, tiltZ * 0.3f);
            SetPivotRot(pivotHips, 0, 0, tiltZ * 0.7f);
            SetPivotRot(pivotLeftKnee, -60f * f, 0, 0);
            SetPivotRot(pivotRightKnee, -45f * f, 0, 0);
            SetPivotRot(pivotLeftShoulder, -30f - 40f * f, -20f + 40f * f, 0);
            SetPivotRot(pivotRightShoulder, -30f - 30f * f, 20f - 50f * f, 0);

            // Drop weapon at 20% through fall
            if (!weaponDropped && f > 0.3f)
            {
                weaponDropped = true;
                DropWeapon();
            }
        }
        else if (t < 0.67f)
        {
            // Settle on ground
            float s = (t - 0.33f) / 0.34f;
            float bounceY = Mathf.Sin(s * Mathf.PI) * 0.03f;
            Vector3 pos = transform.position;
            pos.y = Mathf.Max(pos.y - dt * 0.8f, -0.3f);
            transform.position = pos;

            if (pivotHips != null)
            {
                Vector3 hp = pivotHips.localPosition;
                hp.y = Mathf.Lerp(0.65f, 0.1f, EaseOutQuad(s)) + bounceY;
                pivotHips.localPosition = hp;
            }
        }
        else
        {
            // Fade out
            float fade = 1f - ((t - 0.67f) / 0.33f);
            FadeAllParts(fade);

            Vector3 pos = transform.position;
            pos.y -= dt * 0.15f;
            transform.position = pos;
        }

        if (t >= 1f)
        {
            deathAnimDone = true;
            currentState = AnimState.Dead;
        }
    }

    void DropWeapon()
    {
        // Detach the weapon mesh from the hand and give it physics
        Transform weaponParent = unit.partWeapon;
        if (weaponParent != null)
        {
            weaponParent.SetParent(null);
            Rigidbody rb = weaponParent.gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.3f;
            rb.AddForce(Random.insideUnitSphere * 2f + Vector3.up * 1.5f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 8f, ForceMode.Impulse);

            // Re-add a collider so it bounces
            if (weaponParent.GetComponent<Collider>() == null)
            {
                BoxCollider bc = weaponParent.gameObject.AddComponent<BoxCollider>();
                bc.size = Vector3.one;
            }

            Destroy(weaponParent.gameObject, 2.5f);
        }
    }

    void FadeAllParts(float alpha)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            Color c = r.material.color;
            c.a = Mathf.Clamp01(alpha);
            r.material.color = c;
            r.material.renderQueue = 3000;
        }
    }

    // ========== SECONDARY MOTION ==========

    void AnimateCape()
    {
        if (pivotCape == null) return;

        float targetAngleX = 0f;
        float targetAngleZ = 0f;

        if (agent != null && agent.isOnNavMesh)
        {
            Vector3 vel = agent.velocity;
            float speed = vel.magnitude;

            // Cape swings opposite to movement direction
            Vector3 localVel = transform.InverseTransformDirection(vel);
            targetAngleX = localVel.z * 6f;
            targetAngleZ = -localVel.x * 4f;
        }

        // Damped spring
        float springK = 12f;
        float damping = 5f;

        float forceX = (targetAngleX - capeAngleX) * springK;
        capeVelocity.x += forceX * dt;
        capeVelocity.x *= Mathf.Exp(-damping * dt);
        capeAngleX += capeVelocity.x * dt;

        float forceZ = (targetAngleZ - capeAngleZ) * springK;
        capeVelocity.z += forceZ * dt;
        capeVelocity.z *= Mathf.Exp(-damping * dt);
        capeAngleZ += capeVelocity.z * dt;

        // Idle wind sway
        capeAngleX += Mathf.Sin(Time.time * 1.2f + transform.position.x) * 0.3f;
        capeAngleZ += Mathf.Sin(Time.time * 0.8f + transform.position.z) * 0.2f;

        pivotCape.localRotation = Quaternion.Euler(
            Mathf.Clamp(capeAngleX, -30f, 30f),
            0,
            Mathf.Clamp(capeAngleZ, -20f, 20f));
    }

    // ========== ABILITY VFX ==========

    void AnimateRageFire()
    {
        rageParticleTimer -= dt;
        if (rageParticleTimer <= 0f)
        {
            rageParticleTimer = rageParticleInterval;
            SpawnRageParticle();
        }
    }

    void SpawnRageParticle()
    {
        bool useSphere = Random.value > 0.5f;
        PrimitiveType ptype = useSphere ? PrimitiveType.Sphere : PrimitiveType.Cube;
        GameObject p = GameObject.CreatePrimitive(ptype);
        p.name = "RageFlame";
        p.transform.position = transform.position + new Vector3(
            Random.Range(-0.4f, 0.4f), Random.Range(0.1f, 0.5f), Random.Range(-0.3f, 0.3f));
        float size = Random.Range(0.06f, 0.14f);
        p.transform.localScale = Vector3.one * size;
        p.transform.rotation = Random.rotation;

        Renderer rend = p.GetComponent<Renderer>();
        float rr = Random.Range(0.85f, 1f);
        float gg = Random.Range(0.1f, 0.5f);
        Color c = new Color(rr, gg, 0f, 0.8f);
        rend.material = ShaderHelper.CreateMaterial(c, 0, 0.8f, new Color(rr, gg * 0.5f, 0f) * 2f);
        rend.material.renderQueue = 3100;

        Destroy(p.GetComponent<Collider>());

        RageFlameParticle mover = p.AddComponent<RageFlameParticle>();
        mover.lifetime = Random.Range(0.3f, 0.6f);
        mover.riseSpeed = Random.Range(2f, 4f);
    }

    void AnimateShieldWallPulse()
    {
        shieldPulseTimer += dt;

        if (unit.shieldWallEffect != null)
        {
            Renderer r = unit.shieldWallEffect.GetComponent<Renderer>();
            if (r != null)
            {
                float pulse = Mathf.Sin(shieldPulseTimer * 3f) * 0.12f + 0.3f;
                Color c = new Color(0.85f, 0.75f, 0.2f, pulse);
                r.material.color = c;
                ShaderHelper.SetEmission(r.material,
                    new Color(0.85f, 0.7f, 0.15f) * (pulse * 2f));
            }

            float scale = 1f + Mathf.Sin(shieldPulseTimer * 2f) * 0.04f;
            unit.shieldWallEffect.transform.localScale = new Vector3(
                1.4f * scale, 0.01f, 1.4f * scale);
        }
    }

    void AnimateMarkSpin()
    {
        if (unit.markEffect != null)
        {
            unit.markEffect.transform.Rotate(0, 120f * dt, 0);

            Vector3 pos = unit.markEffect.transform.localPosition;
            pos.y = 2.6f + Mathf.Sin(Time.time * 3f) * 0.1f;
            unit.markEffect.transform.localPosition = pos;

            // Pulse scale
            float pulse = 1f + Mathf.Sin(Time.time * 4f) * 0.1f;
            unit.markEffect.transform.localScale = Vector3.one * pulse;
        }
    }

    // ========== FOOTSTEP DUST ==========

    void SpawnDust(Vector3 position)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dust.name = "Dust";
            dust.transform.position = position + new Vector3(
                Random.Range(-0.25f, 0.25f), 0.04f, Random.Range(-0.25f, 0.25f));
            float size = Random.Range(0.06f, 0.12f);
            dust.transform.localScale = Vector3.one * size;

            Renderer rend = dust.GetComponent<Renderer>();
            rend.material = ShaderHelper.CreateMaterial(
                new Color(0.6f, 0.55f, 0.4f, 0.3f), 0, 0.05f);
            rend.material.renderQueue = 3000;

            Destroy(dust.GetComponent<Collider>());

            DustParticle dp = dust.AddComponent<DustParticle>();
            dp.lifetime = Random.Range(0.3f, 0.5f);
        }
    }

    // ========== HELPER ==========

    void SetPivotRot(Transform pivot, float x, float y, float z)
    {
        if (pivot != null)
            pivot.localRotation = Quaternion.Euler(x, y, z);
    }
}

public class RageFlameParticle : MonoBehaviour
{
    public float lifetime = 0.5f;
    public float riseSpeed = 2f;
    float timer;
    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float d = Time.unscaledDeltaTime;
        timer += d;
        float t = timer / lifetime;

        transform.position += Vector3.up * riseSpeed * d;

        // Grow then shrink
        float scaleMult = t < 0.3f ? Mathf.Lerp(0.5f, 1.2f, t / 0.3f)
                                    : Mathf.Lerp(1.2f, 0f, (t - 0.3f) / 0.7f);
        transform.localScale = startScale * scaleMult;

        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a = Mathf.Lerp(0.8f, 0f, t);
            r.material.color = c;
        }

        if (timer >= lifetime) Destroy(gameObject);
    }
}

public class DustParticle : MonoBehaviour
{
    public float lifetime = 0.4f;
    float timer;

    void Update()
    {
        float d = Time.unscaledDeltaTime;
        timer += d;
        float t = timer / lifetime;

        transform.position += new Vector3(0, 0.3f * d, 0);
        float grow = 1f + t * 2f;
        transform.localScale = Vector3.one * 0.08f * grow;

        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a = Mathf.Lerp(0.3f, 0f, t);
            r.material.color = c;
        }

        if (timer >= lifetime) Destroy(gameObject);
    }
}
