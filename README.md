 <div align="center">

<img src="Docs/ayllu.png" alt="Ayllu — Flying of the Condor" width="700">

# Ayllu — Flying of the Condor

**A story-driven Mixed Reality / Virtual Reality experience featuring redirected walking, portal transitions, animated characters, and immersive storytelling.**

I rebuilt the project from scratch in just two weeks after the previous team failed to deliver a working experience. I implemented the core gameplay systems, animation events, timeline optimization, visual effects, and stable event flow.

</div>

---

## Overview

Ayllu is an immersive MR/VR experience in which players explore a story through interactive environments, magical visual effects, and cinematic sequences.

The project combines Unity gameplay programming with animation systems, spline-driven events, timeline control, and asynchronous task management. These systems work together to deliver smooth transitions and coordinated story events.

### Key Features

- **Redirected Walking** — immersive movement through the experience.
- **Portal Transitions** — seamless transitions between environments.
- **Spline Event System** — triggers actions at precise points along an animation or timeline.
- **Timeline Optimization** — prepares cinematic sequences to reduce frame spikes.
- **Sprite Animation System** — asynchronous flipbook animation with configurable playback.
- **Character Appearance Effects** — dissolve-style visual effects for character appearances.
- **Hand Visual Effects** — animated particles following character movement.
- **Story Event Coordination** — synchronized animations, audio, and gameplay events.

### Technologies

`Unity` · `C#` · `VR/MR` · `UniTask` · `Unity Timeline` · `Unity Splines` · `Shader Graph` · `Visual Effect Graph`

## Project Media

### Story & Gameplay

<div align="center">

<img src="Docs/Condor-Arrival.gif" alt="Condor arrival sequence" width="700">

*Condor arrival and story sequence.*

</div>

| Demonstration | Description |
|---|---|
| [Ayllu Fire Story](Docs/Ayllu-Fire-Story.mp4) | Story-driven cinematic sequence. |
| [Condor Arrival](Docs/Condor-Arrival.mov) | The condor's arrival sequence. |
| [Condor Landing](Docs/Condor-Landing.mp4) | Condor landing animation. |
| [Cross Animation](Docs/Cross-Animation.gif) | Animated cross effect. |
| [Fox Dancing](Docs/Fox-Dancing.gif) | Character animation demonstration. |
| [Llama Appearance](Docs/Llama-Appearance.gif) | Llama dissolve and appearance effect. |
| [Pigeon Animation](Docs/Pigeon-Animation.gif) | Additional character animation. |
| [Hand Visual Effects](Docs/Hand-VFX.gif) | Hand-following particle effects. |

## Code & System Architecture

The following sections showcase the main systems I implemented, including their purpose, technical approach, and supporting code or demonstration media.

### 1. Timer System

**File:** `TimerSystem.cs`

A UniTask-based timer that invokes a Unity event after a specified delay.

The system supports configurable durations, completion callbacks, and cancellation. It uses `CancellationTokenSource` to manage asynchronous operations safely and prevents unfinished tasks from continuing after the component is disabled.

**Implementation highlights**
- Asynchronous countdown using UniTask.
- UnityEvent invocation when the timer finishes.
- Cancellation support through `CancellationTokenSource`.
- Explicit start and stop methods.
- Lifecycle cleanup in `OnDisable()`.

<div align="center">

<img src="Docs/Timer-System-Code.png" alt="Timer System C# implementation" width="600">

<img src="Docs/Timer-System.png" alt="Timer System demonstration" width="600">

</div>

### 2. Sprite Animation System

**File:** `SpriteAnimation.cs`

An asynchronous flipbook animation system that displays a sequence of `Texture2D` frames on a `MeshRenderer`.

The animation updates the displayed frame according to a configurable playback speed, optionally loops, and invokes an event when the final frame is reached.

**Implementation highlights**
- Configurable frame playback speed.
- Optional looping.
- Completion callbacks.
- Automatic animation startup.
- Cancellation and cleanup when disabled.

This system was used to display the Cross animation.

<div align="center">

<img src="Docs/Sprite-Animation-Code.png" alt="Sprite Animation C# implementation" width="600">

<img src="Docs/Cross-Animation.gif" alt="Cross sprite animation" width="600">

</div>

### 3. Llama Appearance System

**File:** `LlamaAppearSystem.cs`

Controls dissolve-style appearance and disappearance effects by animating the shader's `_Disappear` property over time.

The system interpolates shader values with UniTask, allowing the llama to appear and disappear smoothly during key story moments.

**Implementation highlights**
- Smooth shader-property interpolation.
- Asynchronous animation using UniTask.
- Configurable appearance and disappearance transitions.
- Material property updates during animation.
- Unity Editor context-menu helpers for testing.

<div align="center">

<img src="Docs/Llama-Appearance-Code.png" alt="Llama Appearance System C# implementation" width="600">

<img src="Docs/Llama-Appearance.gif" alt="Llama appearance effect" width="600">

</div>

### 4. Spline Events Invoker

**File:** `SplineEventsInvoker.cs`

A lightweight event system that connects Unity Splines and animation progress to gameplay events.

It evaluates the current spline position and triggers configured UnityEvents when the animation reaches specific timestamps or spline milestones.

**Implementation highlights**
- Spline-driven event timing.
- Configurable event timestamps.
- Integration with character animations.
- Support for audio, transitions, and gameplay actions.
- Minimal coupling between animation logic and event handling.

This system was used to synchronize llama animations and story events with specific spline moments.

<div align="center">

<img src="Docs/Spline-Event-System-Code.png" alt="Spline Events Invoker C# implementation" width="600">

<img src="Docs/Spline-Event-System.png" alt="Spline Event System demonstration" width="600">

</div>

### 5. Timeline Prewarm Controller

**File:** `TimelinePrewarmController.cs`

A performance optimization system that prepares Unity Timeline sequences before their actual playback.

Instead of playing a timeline for the first time during a critical story moment, the controller rebuilds and evaluates the playable graph ahead of time, then pauses it until playback is required.

**Implementation highlights**
- Prepares the playable graph before playback.
- Evaluates the timeline to initialize required resources.
- Pauses the timeline after preparation.
- Supports pausing, unpausing, and restarting.
- Helps reduce first-playback stutters during cinematic sequences.

This was particularly important for smooth story sequences such as the condor flight and cave events.

<div align="center">

<img src="Docs/Timeline-Prewarm-Code.png" alt="Timeline Prewarm Controller C# implementation" width="600">

<img src="Docs/Timeline.png" alt="Unity Timeline sequence" width="600">

</div>

### 6. Hand VFX Graph

**File:** `HandVFXGraph.cs`

A visual effects system that generates particles across a skinned mesh and updates their positions as the character moves.

The system samples vertex positions, applies randomized offsets and size variations, and updates particle positions to follow the character's hands.

**Implementation highlights**
- Vertex sampling from a `SkinnedMeshRenderer`.
- Particle placement across the animated mesh.
- Randomized particle sizes and offsets.
- Continuous particle updates.
- Integration with Unity Visual Effect Graph.

The result creates magical visual effects around the llama's hands during important story moments.

<div align="center">

<img src="Docs/Hand-VFX-Code.png" alt="Hand VFX Graph C# implementation" width="600">

<img src="Docs/Hand-VFX.gif" alt="Hand visual effects demonstration" width="600">

</div>

## Additional Media

More visual demonstrations of the project's animation and gameplay systems are available below.

<div align="center">

| Animation | Preview |
|---|---|
| Fox Dancing | [Watch GIF](Docs/Fox-Dancing.gif) |
| Pigeon Animation | [Watch GIF](Docs/Pigeon-Animation.gif) |
| Cross Animation | [Watch GIF](Docs/Cross-Animation.gif) |
| Llama Appearance | [Watch GIF](Docs/Llama-Appearance.gif) |
| Hand VFX | [Watch GIF](Docs/Hand-VFX.gif) |

</div>

## My Responsibilities

- Rebuilt the project from scratch within two weeks.
- Implemented the core gameplay and event systems in C#.
- Developed asynchronous systems using UniTask.
- Created spline-based event triggering and animation coordination.
- Optimized Unity Timeline playback to reduce frame spikes.
- Implemented shader-driven character appearance effects.
- Developed mesh-based particle effects for animated characters.
- Integrated redirected walking and portal transitions into the experience.

## Technical Highlights

| Area | Implementation |
|---|---|
| Asynchronous programming | UniTask and cancellation tokens |
| Event-driven architecture | UnityEvents and spline-triggered events |
| Animation | Sprite flipbooks and synchronized character animations |
| Rendering | Shader properties and Visual Effect Graph |
| Performance | Timeline prewarming and playable graph initialization |
| Lifecycle management | Cancellation and component cleanup |

---

<div align="center">

**Ayllu — Flying of the Condor**

Unity · C# · VR/MR · Gameplay Systems · Performance Optimization

</div>