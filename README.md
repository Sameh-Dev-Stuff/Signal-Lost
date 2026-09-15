<div align="center">

# SIGNAL LOST

**A compact top-down survival shooter, built solo in Unity.**

*Stranded on a hostile alien world. Find the parts. Activate the beacon. Survive until rescue arrives.*

<br>

![Unity](https://img.shields.io/badge/Unity-6000.0-000000?style=for-the-badge&logo=unity&logoColor=white)
![URP](https://img.shields.io/badge/Render_Pipeline-URP-2A6DB0?style=for-the-badge)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-Prototype-FFB020?style=for-the-badge)

</div>

<br>

---

## About

You're a stranded explorer on an alien planet. Explore, scavenge parts and ammo, fight off aliens, activate a rescue beacon, then survive the final wave until the ship arrives. A full run is meant to take **5–10 minutes**.

> [!NOTE]
> This is a small, deliberately scoped solo portfolio project. The goal is a finished game, not a big one — every system is built only as far as the core loop needs it.

<br>

## Core Loop

```
Explore  →  Scavenge ammo & beacon parts  →  Fight enemies
    ↑                                              ↓
 Survive  ←  Last Stand wave  ←  Activate the rescue beacon
```

<br>

## Current Progress — Prototype Phase

Right now the project has the first slice of the core loop working end to end with a dummy enemy:

- [x] Top-down player controller (New Input System) + Cinemachine camera
- [x] Walk/run Blend Tree driven by movement speed
- [x] Single-fire shooting — raycast, damage, animation trigger
- [x] Reload tied to the reload animation
- [x] Shared `Health` component (used by both player and enemies)
- [x] Dummy enemy (capsule) with NavMesh pathfinding, chasing the player
- [x] Bullet damage switched from `OnCollisionEnter` to `OnTriggerEnter`

**Not built yet:** enemy attack/death states, the two remaining enemy types, the full 3-mode weapon system, pickups, the beacon objective, and UI.

<br>

## Technical Note — Enemy Movement

> [!TIP]
> Enemy motion runs through `Rigidbody.MovePosition` in `FixedUpdate`, not through the `NavMeshAgent` moving the transform directly. The player and Cinemachine both update on the physics step, so letting the agent move in `Update` caused visible jitter against the player.
>
> The agent is kept as a pure pathfinding solver (`updatePosition = false`), and `agent.velocity` — which already has acceleration and braking applied — is read each physics step and applied to the rigidbody. `agent.nextPosition` is synced back afterward so pathfinding stays aligned with where the enemy actually ends up after collisions. Rotation is currently left to the agent (`updateRotation = true`), since it stays stable even at low speeds.

<br>

## Built With

Unity (URP) · New Input System · Cinemachine · NavMesh

<br>

---

<div align="center">
<sub>A solo portfolio project — built to be finished, not endless.</sub>
</div>
