# Vision-Based USV Navigation — Minimal Demo

A small Unity demonstration of the **camera observation → navigation decision → vessel motion → feedback** loop for an unmanned surface vessel (USV).

**Pre-publication release:** this repository reproduces a simplified navigation loop. It does not reproduce the paper's training procedure, learned behavior, or quantitative results. The included controller is a hand-written baseline that uses RGB pixels and auxiliary navigation state.

## Research Overview

This project investigates vision-assisted multi-USV navigation in simulated maritime environments. The framework combines onboard visual observations with auxiliary navigation states and learns continuous control policies through Unity ML-Agents.

The research progresses from reinforcement-learning-based navigation toward perception-aware autonomous agents, incorporating visual scene understanding, dynamic vessel interactions, and goal-directed control in complex port environments.

The reward design considers goal progress, buoy-side navigation, and vessel encounters. These rule-inspired rewards encourage rule-consistent behavior, but do not constitute a formal guarantee of maritime-rule compliance.

This repository provides a lightweight public subset of the full research framework, including demonstration code, procedural environments, an overview of the perception-policy-control architecture, and representative two-USV navigation demos.


### Overall Architecture

The figure below illustrates the overall perception-policy-control pipeline. Visual observations from onboard cameras are combined with auxiliary navigation and environmental information and processed by the learned policy to generate continuous control actions.

<img width="769" height="771" alt="Vision-Guided PPO Framework" src="https://github.com/user-attachments/assets/53e4ac97-de7a-4bb6-9190-4f877933b888" />


### Demo Videos

The following demos show successful two-USV navigation trajectories under different encounter configurations.

- **Blue vessels:** controlled USVs.
- **White vessels:** dynamic ASVs interacting with the controlled agents.
- **Blue spheres:** individual navigation targets for the two USVs.
- **Large left panel:** global view of the simulated port environment.
- **Two right-side panels:** onboard first-person views from the two controlled USVs.

In each scenario, the two controlled USVs approach from opposite directions toward their respective targets while interacting with dynamic vessels and navigating through maritime markers. The three demos illustrate successful trajectories under different initial and encounter conditions.

#### Demo 1 — Two-USV Navigation Case I

https://github.com/user-attachments/assets/b429d164-139b-4677-88aa-6e89281d41bf

#### Demo 2 — Two-USV Navigation Case II

https://github.com/user-attachments/assets/bf461df8-9db0-4a0e-bba5-7c7c08370b3c

#### Demo 3 — Two-USV Navigation Case III

https://github.com/user-attachments/assets/0e266f8e-63d1-4c6d-99f4-723e27ba487a
## Run

1. Add this directory as a project in Unity Hub and open it with **Unity 2022.3.62f1**. This is a Built-in Render Pipeline demo.
2. Select **USV Mini → Create demo scene** in the editor menu.
3. Press **Play**. The scene is generated from primitives; no imported models are needed.
4. Watch the overhead view and the onboard RGB image in the upper left. Use **Restart episode** to repeat the fixed initial condition.

No Python environment, ML-Agents installation or model weights are required. A normal graphics-capable Unity session is needed for image rendering.

## Validation

Checked with Unity 2022.3.62f1 on Linux: script compilation and scene creation passed. A graphics-enabled Play Mode smoke run captured the onboard RGB image and reached the goal in **152 decisions** from the fixed starting condition. This checks the demo's basic operation, not research performance or robustness across scenarios.

## What is reproduced?

| Component | Public demo |
| --- | --- |
| Environment | One vessel, three red/green buoy pairs and a goal |
| Visual observation | Actual Unity camera image, 84 × 84 RGB |
| Auxiliary observation | Goal in the vessel frame, goal distance and forward speed |
| Decision | Pixel color centroids combined with goal heading |
| Action | Continuous yaw and throttle, each in `[-1, 1]` |
| Motion | Simple planar kinematics at a 0.1-second decision interval |
| Feedback | Distance progress, step cost and terminal outcome |
| Termination | Goal, buoy proximity, boundary exit or 600-step timeout |

The baseline extracts red/green pixel centroids and steers toward their image midpoint while tracking the goal. It falls back to goal heading when both colors are not visible. It receives no buoy world coordinates. The environment uses those coordinates for the simplified collision check. Goal information is supplied directly as auxiliary state; this is not camera-only navigation or a learned perception system.

Yaw is mapped to ±45 degrees/second. Throttle is mapped to 0–3 metres/second. These are illustrative demo parameters. Vessel shape, dynamics, collision radius, reward and layout are simplified and are not research settings.

## Code

- [`MinimalUSVDemo.cs`](Assets/Scripts/MinimalUSVDemo.cs): procedural environment, camera capture, `Observe()`, baseline, `Step(action)` and episode feedback.
- [`CreateMinimalScene.cs`](Assets/Editor/CreateMinimalScene.cs): editor command to generate the scene.

`Observe()` returns four auxiliary values and refreshes the RGB texture. `Baseline()` demonstrates how those observations can produce an action. `Step()` advances the environment and records feedback. These methods illustrate the boundary where a learned policy could be connected; this release does not provide a training adapter.

## Scope of this release

The public subset excludes full reward formulas and tuned training configurations, multi-vessel coordination/reset logic, research harbor scenes, ocean/boat asset packages, trained checkpoints, experiment logs and evaluation results. It also excludes the original repository history.

The demonstration score is only feedback for this toy environment. It is not a reported paper metric. Successful navigation in this fixed buoy corridor is not evidence of general obstacle avoidance, multi-vessel capability or maritime-rule compliance.
