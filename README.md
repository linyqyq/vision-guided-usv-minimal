# Vision-Based USV Navigation — Minimal Demo

A lightweight Unity demo of the **camera observation → navigation decision → vessel motion → feedback** loop for autonomous USV navigation.

> **Pre-publication release.** This repository provides a simplified public subset of the research framework. It does not include the original training pipeline, trained policies, tuned configurations, or quantitative research results.

---

## Research Overview

The full research project studies **vision-assisted multi-USV navigation** in simulated port environments, combining onboard visual observations, navigation states, and reinforcement-learning-based continuous control.

The research progresses from state-based navigation toward **perception-aware autonomous agents**, incorporating visual scene understanding, dynamic vessel interactions, maritime markers, and goal-directed control.

This repository includes a lightweight public code demo together with the overall research architecture and representative multi-USV navigation videos.

### Overall Architecture

The framework combines onboard visual observations and auxiliary navigation information with a learned policy to generate continuous vessel-control actions.

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/53e4ac97-de7a-4bb6-9190-4f877933b888"
    width="600"
    alt="Vision-Guided PPO Framework"
  />
</p>

---

## Demo Videos

The following videos show successful **two-USV navigation** under different initial and encounter conditions.

- **Blue vessels:** controlled USVs
- **White vessels:** dynamic ASVs
- **Blue spheres:** individual navigation targets
- **Left panel:** global environment view
- **Right panels:** onboard views from the two controlled USVs

In each case, the two USVs approach from opposite directions toward their respective targets while interacting with surrounding vessels and maritime markers.

<details>
<summary><b>Demo 1 — Two-USV Navigation Case I</b></summary>

<br>

https://github.com/user-attachments/assets/b429d164-139b-4677-88aa-6e89281d41bf

</details>

<details>
<summary><b>Demo 2 — Two-USV Navigation Case II</b></summary>

<br>

https://github.com/user-attachments/assets/bf461df8-9db0-4a0e-bba5-7c7c08370b3c

</details>

<details>
<summary><b>Demo 3 — Two-USV Navigation Case III</b></summary>

<br>

https://github.com/user-attachments/assets/0e266f8e-63d1-4c6d-99f4-723e27ba487a

</details>

---

## Run

1. Open the project with **Unity 2022.3.62f1**.
2. Select **USV Mini → Create demo scene**.
3. Press **Play**.
4. Use **Restart episode** to repeat the fixed initial condition.

No Python environment, ML-Agents installation, or model weights are required.

---

## Public Demo

| Component | Implementation |
| --- | --- |
| Environment | Single USV, buoy corridor, and navigation goal |
| Visual input | 84 × 84 onboard RGB camera |
| Auxiliary state | Goal direction, distance, and forward speed |
| Control | Continuous yaw and throttle |
| Motion | Simplified planar vessel dynamics |
| Feedback | Goal progress, step cost, and terminal outcome |

The included controller is a **hand-written visual baseline**, not a trained policy. It uses image-based buoy cues together with auxiliary goal information to demonstrate the observation–action interface.

The public environment and vessel dynamics are intentionally simplified and should not be interpreted as the full research configuration.

---

## Validation

Tested with **Unity 2022.3.62f1 on Linux**.

The demo successfully compiles, generates the scene, captures onboard RGB observations, and completes the fixed navigation task. This verifies the basic software loop only; it is **not a research-performance or robustness evaluation**.

---

## Code

- [`MinimalUSVDemo.cs`](Assets/Scripts/MinimalUSVDemo.cs)  
  Environment generation, camera observations, baseline control, vessel motion, and episode feedback.

- [`CreateMinimalScene.cs`](Assets/Editor/CreateMinimalScene.cs)  
  Unity editor command for generating the demonstration scene.

---

## Scope

The public release excludes:

- trained checkpoints and original training code
- tuned reward functions and training configurations
- full multi-USV coordination logic
- research-scale harbor environments and asset packages
- experiment logs and quantitative evaluation results

The included demo is intended to illustrate the **software interface and navigation loop**, not to reproduce the full research system or establish maritime-rule compliance.
