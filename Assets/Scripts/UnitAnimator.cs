using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit))]
public class UnitAnimator : MonoBehaviour
{
    Unit unit;
    NavMeshAgent agent;

    // When true, uses unscaled time (for viewer mode while game is paused)
    public bool useUnscaledTime;
    float dt => useUnscaledTime ? Time.unscaledDeltaTime : dt;

    // Body part references (set by Unit after building model)
    [HideInInspector] public Transform head;
    [HideInInspector] public Transform body;
    [HideInInspector] public Transform leftArm;
    [HideInInspector] public Transform rightArm;
    [HideInInspector] public Transform leftLeg;
    [HideInInspector] public Transform rightLeg;
    [HideInInspector] public Transform weapon;       // Sword/Bow/AxeR
    [HideInInspector] public Transform weaponLeft;   // Shield/AxeL/none

    // Original local positions (stored at init for oscillation offsets)
    Vector3 headRest, bodyRest;
    Vector3 leftArmRest, rightArmRest;
    Vector3 leftLegRest, rightLegRest;
    Vector3 weaponRest, weaponLeftRest;

    // Animation state
    public enum AnimState { Idle, Walking, Attacking, Dying, Dead }
    public AnimState currentState = AnimState.Idle;

    float walkCycleTimer;
    float idleTimer;
    float attackAnimTimer;
    float attackAnimDuration = 0.35f;
    float deathTimer;
    bool deathAnimDone;

    // Idle breathing
    float breathSpeed = 1.5f;
    float breathAmount = 0.015f;

    // Walk cycle
    float walkSwingAmount = 25f;  // degrees
    float walkBobAmount = 0.06f;

    // Attack animation
    int attackAnimPhase; // 0=windup, 1=strike, 2=recover
    float attackPhaseTimer;

    // Rage fire particles
    float rageParticleTimer;
    float rageParticleInterval = 0.12f;

    // Shield wall pulse
    float shieldPulseTimer;

    // Footstep dust
    float dustTimer;
    float dustInterval = 0.3f;

    void Start()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
        StoreRestPositions();
    }

    void StoreRestPositions()
    {
        if (head != null) headRest = head.localPosition;
        if (body != null) bodyRest = body.localPosition;
        if (leftArm != null) leftArmRest = leftArm.localPosition;
        if (rightArm != null) rightArmRest = rightArm.localPosition;
        if (leftLeg != null) leftLegRest = leftLeg.localPosition;
        if (rightLeg != null) rightLegRest = rightLeg.localPosition;
        if (weapon != null) weaponRest = weapon.localPosition;
        if (weaponLeft != null) weaponLeftRest = weaponLeft.localPosition;
    }

    /// <summary>
    /// Must be called after Unit builds its model and assigns part references.
    /// </summary>
    public void InitializeRests()
    {
        StoreRestPositions();
    }

    void Update()
    {
        if (unit == null || deathAnimDone) return;

        // Determine animation state
        if (unit.isDead)
        {
            if (currentState != AnimState.Dying && currentState != AnimState.Dead)
            {
                StartDeathAnimation();
            }
        }
        else if (currentState == AnimState.Attacking)
        {
            // Let attack anim play out
        }
        else if (agent != null && agent.velocity.sqrMagnitude > 0.3f)
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

        // Ability VFX
        if (!unit.isDead)
        {
            if (unit.isEnraged) AnimateRageFire();
            if (unit.isShieldWalling) AnimateShieldWallPulse();
            if (unit.isMarked) AnimateMarkSpin();
        }
    }

    // --- IDLE ---
    void AnimateIdle()
    {
        idleTimer += dt * breathSpeed;
        float breathOffset = Mathf.Sin(idleTimer) * breathAmount;

        if (body != null)
            body.localPosition = bodyRest + Vector3.up * breathOffset;

        if (head != null)
            head.localPosition = headRest + Vector3.up * breathOffset * 1.2f;

        // Subtle arm sway
        float armSway = Mathf.Sin(idleTimer * 0.7f) * 0.005f;
        if (leftArm != null)
            leftArm.localPosition = leftArmRest + new Vector3(0, armSway, 0);
        if (rightArm != null)
            rightArm.localPosition = rightArmRest + new Vector3(0, -armSway, 0);

        // Reset legs to rest
        ResetLegsToRest();
    }

    // --- WALK ---
    void AnimateWalk()
    {
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        float walkFreq = Mathf.Clamp(speed * 1.8f, 2f, 8f);
        walkCycleTimer += dt * walkFreq;

        float phase = Mathf.Sin(walkCycleTimer);
        float phaseOffset = Mathf.Cos(walkCycleTimer);

        // Leg swing (forward/back via Z offset)
        float legSwing = phase * 0.12f;
        if (leftLeg != null)
            leftLeg.localPosition = leftLegRest + new Vector3(0, Mathf.Abs(phase) * 0.03f, legSwing);
        if (rightLeg != null)
            rightLeg.localPosition = rightLegRest + new Vector3(0, Mathf.Abs(phaseOffset) * 0.03f, -legSwing);

        // Arm counter-swing
        float armSwing = phase * 0.08f;
        if (leftArm != null)
            leftArm.localPosition = leftArmRest + new Vector3(0, 0, -armSwing);
        if (rightArm != null)
            rightArm.localPosition = rightArmRest + new Vector3(0, 0, armSwing);

        // Weapon follows arm
        if (weapon != null)
            weapon.localPosition = weaponRest + new Vector3(0, 0, armSwing * 0.6f);
        if (weaponLeft != null)
            weaponLeft.localPosition = weaponLeftRest + new Vector3(0, 0, -armSwing * 0.6f);

        // Body bob
        float bob = Mathf.Abs(Mathf.Sin(walkCycleTimer * 2f)) * walkBobAmount;
        if (body != null)
            body.localPosition = bodyRest + Vector3.up * bob;
        if (head != null)
            head.localPosition = headRest + Vector3.up * bob;

        // Footstep dust
        dustTimer -= dt;
        if (dustTimer <= 0f && speed > 1f)
        {
            SpawnDust(transform.position);
            dustTimer = dustInterval;
        }
    }

    // --- ATTACK ---
    public void PlayAttackAnimation()
    {
        currentState = AnimState.Attacking;
        attackAnimTimer = 0f;
        attackAnimPhase = 0;
        attackPhaseTimer = 0f;

        switch (unit.unitType)
        {
            case UnitType.Swordsman:
                attackAnimDuration = 0.4f;
                break;
            case UnitType.Archer:
                attackAnimDuration = 0.5f;
                break;
            case UnitType.Berserker:
                attackAnimDuration = 0.35f;
                break;
            case UnitType.Shieldbearer:
                attackAnimDuration = 0.5f;
                break;
        }
    }

    void AnimateAttack()
    {
        attackAnimTimer += dt;
        float t = attackAnimTimer / attackAnimDuration;

        switch (unit.unitType)
        {
            case UnitType.Swordsman: AnimateSwordAttack(t); break;
            case UnitType.Archer: AnimateBowAttack(t); break;
            case UnitType.Berserker: AnimateAxeAttack(t); break;
            case UnitType.Shieldbearer: AnimateSpearAttack(t); break;
        }

        if (t >= 1f)
        {
            currentState = AnimState.Idle;
            ResetToRest();
        }
    }

    void AnimateSwordAttack(float t)
    {
        if (rightArm == null) return;

        if (t < 0.3f)
        {
            // Windup: arm pulls back
            float wind = t / 0.3f;
            rightArm.localPosition = rightArmRest + new Vector3(0.05f, 0.15f * wind, -0.15f * wind);
            if (weapon != null)
                weapon.localPosition = weaponRest + new Vector3(0.05f, 0.2f * wind, -0.1f * wind);
        }
        else if (t < 0.6f)
        {
            // Strike: arm swings forward
            float strike = (t - 0.3f) / 0.3f;
            rightArm.localPosition = rightArmRest + new Vector3(0, 0.15f * (1f - strike), 0.2f * strike);
            if (weapon != null)
                weapon.localPosition = weaponRest + new Vector3(0, 0.2f * (1f - strike), 0.25f * strike);
        }
        else
        {
            // Recover
            float rec = (t - 0.6f) / 0.4f;
            rightArm.localPosition = Vector3.Lerp(
                rightArmRest + new Vector3(0, 0, 0.2f),
                rightArmRest, rec);
            if (weapon != null)
                weapon.localPosition = Vector3.Lerp(
                    weaponRest + new Vector3(0, 0, 0.25f),
                    weaponRest, rec);
        }
    }

    void AnimateBowAttack(float t)
    {
        if (rightArm == null) return;

        if (t < 0.5f)
        {
            // Draw: right arm pulls back (drawing string)
            float draw = t / 0.5f;
            rightArm.localPosition = rightArmRest + new Vector3(0, 0.1f * draw, -0.15f * draw);
            // Slight body lean back
            if (body != null)
                body.localPosition = bodyRest + new Vector3(0, 0, -0.03f * draw);
        }
        else if (t < 0.6f)
        {
            // Release: snap forward
            float release = (t - 0.5f) / 0.1f;
            rightArm.localPosition = rightArmRest + new Vector3(0, 0.1f * (1f - release), 0.05f * release);
        }
        else
        {
            // Recover
            float rec = (t - 0.6f) / 0.4f;
            rightArm.localPosition = Vector3.Lerp(
                rightArmRest + new Vector3(0, 0, 0.05f),
                rightArmRest, rec);
            if (body != null)
                body.localPosition = Vector3.Lerp(bodyRest + new Vector3(0, 0, -0.03f), bodyRest, rec);
        }
    }

    void AnimateAxeAttack(float t)
    {
        // Dual axe: both arms swing alternately
        if (t < 0.25f)
        {
            // Right axe windup
            float w = t / 0.25f;
            if (rightArm != null)
                rightArm.localPosition = rightArmRest + new Vector3(0.08f, 0.2f * w, -0.1f * w);
            if (weapon != null)
                weapon.localPosition = weaponRest + new Vector3(0.08f, 0.25f * w, -0.08f * w);
        }
        else if (t < 0.5f)
        {
            // Right axe strike + left windup
            float s = (t - 0.25f) / 0.25f;
            if (rightArm != null)
                rightArm.localPosition = rightArmRest + new Vector3(0, 0.2f * (1f - s), 0.2f * s);
            if (weapon != null)
                weapon.localPosition = weaponRest + new Vector3(0, 0.25f * (1f - s), 0.25f * s);
            if (leftArm != null)
                leftArm.localPosition = leftArmRest + new Vector3(-0.08f, 0.15f * s, -0.08f * s);
            if (weaponLeft != null)
                weaponLeft.localPosition = weaponLeftRest + new Vector3(-0.08f, 0.2f * s, -0.06f * s);
        }
        else if (t < 0.75f)
        {
            // Left axe strike
            float s = (t - 0.5f) / 0.25f;
            if (leftArm != null)
                leftArm.localPosition = leftArmRest + new Vector3(0, 0.15f * (1f - s), 0.18f * s);
            if (weaponLeft != null)
                weaponLeft.localPosition = weaponLeftRest + new Vector3(0, 0.2f * (1f - s), 0.22f * s);
            // Right recovering
            if (rightArm != null)
                rightArm.localPosition = Vector3.Lerp(rightArmRest + new Vector3(0, 0, 0.2f), rightArmRest, s);
        }
        else
        {
            // Both recover
            float rec = (t - 0.75f) / 0.25f;
            if (leftArm != null)
                leftArm.localPosition = Vector3.Lerp(leftArmRest + new Vector3(0, 0, 0.18f), leftArmRest, rec);
            if (weaponLeft != null)
                weaponLeft.localPosition = Vector3.Lerp(weaponLeftRest + new Vector3(0, 0, 0.22f), weaponLeftRest, rec);
            if (rightArm != null)
                rightArm.localPosition = Vector3.Lerp(rightArmRest, rightArmRest, rec);
        }

        // Body lunge
        if (body != null)
        {
            float lunge = Mathf.Sin(t * Mathf.PI) * 0.06f;
            body.localPosition = bodyRest + new Vector3(0, 0, lunge);
        }
    }

    void AnimateSpearAttack(float t)
    {
        // Spear thrust
        if (rightArm == null) return;

        if (t < 0.35f)
        {
            // Pull spear back
            float w = t / 0.35f;
            rightArm.localPosition = rightArmRest + new Vector3(0, 0.05f * w, -0.2f * w);
            if (weapon != null)
                weapon.localPosition = weaponRest + new Vector3(0, 0, -0.25f * w);
        }
        else if (t < 0.55f)
        {
            // Thrust forward hard
            float s = (t - 0.35f) / 0.2f;
            rightArm.localPosition = rightArmRest + new Vector3(0, 0.05f * (1f - s), 0.25f * s);
            if (weapon != null)
                weapon.localPosition = weaponRest + new Vector3(0, 0, -0.25f + 0.55f * s);
            // Shield pushes forward too
            if (leftArm != null)
                leftArm.localPosition = leftArmRest + new Vector3(0, 0, 0.1f * s);
        }
        else
        {
            // Recover
            float rec = (t - 0.55f) / 0.45f;
            rightArm.localPosition = Vector3.Lerp(rightArmRest + new Vector3(0, 0, 0.25f), rightArmRest, rec);
            if (weapon != null)
                weapon.localPosition = Vector3.Lerp(weaponRest + new Vector3(0, 0, 0.3f), weaponRest, rec);
            if (leftArm != null)
                leftArm.localPosition = Vector3.Lerp(leftArmRest + new Vector3(0, 0, 0.1f), leftArmRest, rec);
        }
    }

    // --- DEATH ---
    void StartDeathAnimation()
    {
        currentState = AnimState.Dying;
        deathTimer = 0f;
    }

    void AnimateDeath()
    {
        deathTimer += dt;
        float t = Mathf.Clamp01(deathTimer / 0.8f);

        // Tilt the whole unit backwards/sideways
        float tiltAngle = t * 90f;
        float direction = (unit.faction == Faction.North) ? 1f : -1f;
        transform.localRotation *= Quaternion.Euler(0, 0, tiltAngle * direction * dt * 3f);

        // Sink into ground
        if (t > 0.3f)
        {
            float sink = (t - 0.3f) / 0.7f;
            Vector3 pos = transform.position;
            pos.y -= sink * 0.8f * dt;
            transform.position = pos;
        }

        // Fade parts
        if (t > 0.5f)
        {
            float fade = 1f - ((t - 0.5f) / 0.5f);
            FadeAllParts(fade);
        }

        if (t >= 1f)
        {
            deathAnimDone = true;
            currentState = AnimState.Dead;
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

    // --- ABILITY VFX ---

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
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p.name = "RageFlame";
        p.transform.position = transform.position + new Vector3(
            Random.Range(-0.4f, 0.4f), Random.Range(0.2f, 1.8f), Random.Range(-0.3f, 0.3f));
        float size = Random.Range(0.06f, 0.14f);
        p.transform.localScale = Vector3.one * size;

        Renderer rend = p.GetComponent<Renderer>();
        float rr = Random.Range(0.85f, 1f);
        float gg = Random.Range(0.1f, 0.5f);
        rend.material = ShaderHelper.CreateMaterial(new Color(rr, gg, 0f, 0.7f));
        rend.material.renderQueue = 3100;

        Destroy(p.GetComponent<Collider>());

        // Animate upward and fade via a simple mover
        RageFlameParticle mover = p.AddComponent<RageFlameParticle>();
        mover.lifetime = Random.Range(0.3f, 0.6f);
        mover.riseSpeed = Random.Range(1.5f, 3f);
    }

    void AnimateShieldWallPulse()
    {
        shieldPulseTimer += dt;

        // Pulse the existing shield wall effect
        if (unit.shieldWallEffect != null)
        {
            Renderer r = unit.shieldWallEffect.GetComponent<Renderer>();
            if (r != null)
            {
                float pulse = Mathf.Sin(shieldPulseTimer * 3f) * 0.1f + 0.25f;
                Color c = new Color(0.8f, 0.75f, 0.3f, pulse);
                r.material.color = c;
            }

            // Gentle size oscillation
            float scale = 1f + Mathf.Sin(shieldPulseTimer * 2f) * 0.05f;
            unit.shieldWallEffect.transform.localScale = new Vector3(
                0.1f * scale, 1.2f * scale, 1.2f * scale);
        }
    }

    void AnimateMarkSpin()
    {
        if (unit.markEffect != null)
        {
            unit.markEffect.transform.Rotate(0, 180f * dt, 45f * dt);

            // Bob up and down
            Vector3 pos = unit.markEffect.transform.localPosition;
            pos.y = 2.5f + Mathf.Sin(Time.time * 3f) * 0.1f;
            unit.markEffect.transform.localPosition = pos;
        }
    }

    // --- FOOTSTEP DUST ---
    void SpawnDust(Vector3 position)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dust.name = "Dust";
            dust.transform.position = position + new Vector3(
                Random.Range(-0.3f, 0.3f), 0.05f, Random.Range(-0.3f, 0.3f));
            float size = Random.Range(0.08f, 0.15f);
            dust.transform.localScale = Vector3.one * size;

            Renderer rend = dust.GetComponent<Renderer>();
            rend.material = ShaderHelper.CreateMaterial(new Color(0.6f, 0.55f, 0.4f, 0.35f));
            rend.material.renderQueue = 3000;

            Destroy(dust.GetComponent<Collider>());

            DustParticle dp = dust.AddComponent<DustParticle>();
            dp.lifetime = Random.Range(0.3f, 0.5f);
        }
    }

    // --- HELPERS ---
    void ResetLegsToRest()
    {
        if (leftLeg != null) leftLeg.localPosition = leftLegRest;
        if (rightLeg != null) rightLeg.localPosition = rightLegRest;
    }

    void ResetToRest()
    {
        if (head != null) head.localPosition = headRest;
        if (body != null) body.localPosition = bodyRest;
        if (leftArm != null) leftArm.localPosition = leftArmRest;
        if (rightArm != null) rightArm.localPosition = rightArmRest;
        if (leftLeg != null) leftLeg.localPosition = leftLegRest;
        if (rightLeg != null) rightLeg.localPosition = rightLegRest;
        if (weapon != null) weapon.localPosition = weaponRest;
        if (weaponLeft != null) weaponLeft.localPosition = weaponLeftRest;
    }
}

/// <summary>
/// Simple upward-moving fire particle that fades and dies.
/// Uses unscaledDeltaTime so it works while paused (in unit viewer).
/// </summary>
public class RageFlameParticle : MonoBehaviour
{
    public float lifetime = 0.5f;
    public float riseSpeed = 2f;
    float timer;

    void Update()
    {
        float d = Time.unscaledDeltaTime;
        timer += d;
        float t = timer / lifetime;

        transform.position += Vector3.up * riseSpeed * d;
        transform.localScale *= (1f - d * 2f);

        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a = Mathf.Lerp(0.7f, 0f, t);
            r.material.color = c;
        }

        if (timer >= lifetime) Destroy(gameObject);
    }
}

/// <summary>
/// Dust particle that rises slightly, expands, and fades.
/// Uses unscaledDeltaTime so it works while paused (in unit viewer).
/// </summary>
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
        float grow = 1f + t * 1.5f;
        transform.localScale = Vector3.one * 0.1f * grow;

        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a = Mathf.Lerp(0.35f, 0f, t);
            r.material.color = c;
        }

        if (timer >= lifetime) Destroy(gameObject);
    }
}
