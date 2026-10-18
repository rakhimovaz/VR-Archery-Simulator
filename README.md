# VR-Based Interactive Archery Simulator

## Project Overview
VR-Based Interactive Archery Simulator is a virtual reality project developed in Unity. The goal is to create an interactive archery experience where users can hold a bow, pull the bowstring, and shoot arrows using VR controllers.

**Course:** AR/VR Systems and Applications  
**University:** Central Asian University  
**Project Type:** Team Project

## Problem and Use Case
Traditional archery requires physical equipment and a suitable training area. Our project explores how virtual reality can provide an interactive environment for practicing basic archery actions.

The simulator is designed for VR training, interactive learning, and entertainment.

## Current Features
- VR player interaction using XR Interaction Toolkit
- Bow grabbing with VR controllers
- Bowstring pulling and arrow shooting
- Arrow movement using Unity Rigidbody physics
- Basic target for shooting practice
- XR Device Simulator for desktop testing

## Technologies
- Unity 6
- C#
- XR Interaction Toolkit
- XR Device Simulator
- Meta Quest (target deployment platform)

## How to Run
1. Clone or download this repository.
2. Open the project through Unity Hub using a compatible Unity 6 Editor.
3. Open the main scene (`SampleScene`).
4. Make sure the required XR packages are installed.
5. Press Play to test using XR Device Simulator.
6. For Meta Quest deployment, configure Android build settings and build the application for the headset.

## Team Members and Responsibilities

- **Zuhra Rahimova — XR Interaction & Gameplay Integration:** Unity XR setup, integration and configuration of bow and arrow interactions, gameplay testing and debugging.
- **Fotima Rahimova — Environment & Level Design:** Responsible for the shooting range environment, target placement, materials, and scene layout.
- **Aziza Hamidova — Physics & VR Testing:** Responsible for testing arrow physics, collisions, and Meta Quest deployment.

## Current Status
The project is at the MVP development stage. Basic VR bow interaction and arrow shooting have been implemented. Target scoring and environment improvements are under development.

## Future Development
- Improve the archery target and scoring system
- Add a detailed shooting environment
- Improve arrow spawning and interaction
- Test and optimize gameplay on Meta Quest
- Add gameplay feedback and polish before Week 15

## Testing and Known Issues

### Desktop Testing
- Opened the Unity project through Unity Hub.
- Checked the main scene (SampleScene).
- Reviewed the bow, arrow, and target setup.
- Checked the XR Device Simulator configuration.

### Known Issues
- Target scoring is still under development.
- Some bow and arrow interactions need further testing.
- Meta Quest headset testing has not yet been confirmed.

### Next Steps
- Improve the shooting range environment.
- Test bow and arrow interactions.
- Improve target placement and scene layout.
## Project Architecture

### Main Components
- XR Origin: Manages the VR player's position and controller interactions.
- Bow: Interactive object used for archery.
- Arrow: Uses Rigidbody physics for projectile movement.
- Target: Object used for shooting practice.
- TargetScore: Script intended to handle target scoring.
- XR Device Simulator: Allows desktop testing without a VR headset.

### Project Structure
- Assets/ — Unity scenes, scripts, materials, and game objects.
- Packages/ — Required Unity packages.
- ProjectSettings/ — Unity project configuration.

### Physics and VR Testing
- Review arrow Rigidbody and collider settings.
- Check arrow collisions with the target.
- Prepare for Meta Quest deployment and testing.
