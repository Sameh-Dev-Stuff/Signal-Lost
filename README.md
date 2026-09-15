
# SIGNAL LOST

**A compact top-down survival shooter, built solo in Unity.**

*Stranded on a hostile alien world. Find the parts. Activate the beacon. Survive until rescue arrives.*

<br>

<img width="1543" height="864" alt="1" src="https://github.com/user-attachments/assets/3ebd78d3-819f-4534-ae5a-ba12f6153dc7" />
<img width="1534" height="855" alt="2" src="https://github.com/user-attachments/assets/6e7c8595-5f9e-4fe9-8a29-64025a11cf58" />
<img width="1920" height="927" alt="3" src="https://github.com/user-attachments/assets/9a8b9758-d194-4803-ac25-0ee5074d1a24" />

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

**Not built yet:** enemy attack/death states, the two remaining enemy types, the full 3-mode weapon system, pickups, the beacon objective, and UI.

<br>

## Technical Note — Enemy Movement

> [!TIP]
> Enemy motion runs through `Rigidbody.MovePosition` in `FixedUpdate`, not through the `NavMeshAgent` moving the transform directly. The player and Cinemachine both update on the physics step, so letting the agent move in `Update` caused visible jitter against the player.
>
> The agent is kept as a pure pathfinding solver (`updatePosition = false`), and `agent.velocity` — which already has acceleration and braking applied — is read each physics step and applied to the rigidbody. `agent.nextPosition` is synced back afterward so pathfinding stays aligned with where the enemy actually ends up after collisions. Rotation is currently left to the agent (`updateRotation = true`), since it stays stable even at low speeds.

<br>

## Built With

Unity 6.3 (URP) · New Input System · Cinemachine · NavMesh

<br>

---

<div align="center">
<sub>A solo portfolio project — built to be finished, not endless.</sub>
</div>
