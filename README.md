# Unity-Cybersecurity-Game
APU Semester 5 Capstone Project

Play Here!
https://yew-cy.itch.io/penetration-education-cybersecurity-game

## Game Description

Penetration Education Cyber Game is a gamified cybersecurity learning experience built in Unity.
It is designed to teach players both offensive and defensive security skills through two parallel roles:

- Red Team: hack the bank by completing exploitation-based missions.
- Blue Team: modify code to secure vulnerable systems and stop the attacks.

The game combines exploration, NPC dialogue, mission-based progression, and in-game coding challenges to make cybersecurity concepts interactive and practical.

## Core Gameplay Flow

Before each level, the player must talk to an NPC to receive the mission, get a hint, and learn the location of the objective.

For the Red Team levels, a guider bot helps the player at the start. After the mission is completed, the player enters a brain-time mode where they must enter the learned protocol or script correctly to move forward. Once the correct solution is submitted, the wall bursts open and the player can continue to the next section.

## Level Challenges

### Red Team Challenges

1. SQL Injection
2. Broken Access Control
3. Cross-Site Scripting (XSS)
4. Cross-Site Request Forgery (CSRF)
5. JWT Token Exploitation

### Blue Team Challenges

The Blue Team path mirrors the Red Team levels, but the goal is to secure the code instead of exploit it. Players patch unsafe logic and validate safer implementations for:

- SQL injection prevention
- Broken access control prevention
- XSS prevention
- CSRF prevention
- JWT exploitation prevention

## Key Coding Concepts Used

- Unity `MonoBehaviour` scripts for gameplay systems and scene interaction
- `ScriptableObject` data assets for NPC dialogue and mission content
- Interface-based interaction using `IInteractable` and `IChestable`
- Singleton-style managers for shared systems such as missions, save data, music, and levels
- UI state management with panels, canvases, text, and input fields
- Coroutines for timed dialogue, animation, and delayed transitions
- LeanTween for motion, scaling, and UI animation
- Unity Input System for player movement, interaction, and challenge input handling
- Regular expressions for code highlighting and solution validation
- Save/load logic for local and Firebase-backed progress

## Key Functions

### NPC and Mission Flow

- `NPC.Interact()` - starts or advances dialogue when the player talks to an NPC.
- `NPC.StartDialogue()` - opens the dialogue UI and sets the dialogue state based on mission progress.
- `NPC.SyncQuestState()` - checks whether a mission is not started, in progress, or completed.
- `MissionController.AcceptMission()` - adds a mission to the active mission list.
- `MissionController.MissionAlert()` - updates mission alerts and mission count in the UI.
- `MissionController.LoadQuestProgress()` - restores saved quest progress.

### Red Team and Blue Team Challenge Flow

- `bombButton.OnPress()` - closes challenge UI, resets mission panels, and routes the player back into the correct challenge state.
- `bombButton.SQLInjection()` - opens the SQL injection challenge.
- `bombButton.BrokenAccessControl()` - opens the broken access control challenge.
- `bombButton.XSS()` - opens the XSS challenge.
- `bombButton.CSRF()` - opens the CSRF challenge.
- `bombButton.JWT()` - opens the JWT challenge.
- `BlueTeam.Interact()` - opens or closes the Blue Team coding canvas.
- `BlueTeam.StartChallenge()` - starts the coding challenge, timer, and mission music.
- `BlueTeam.CheckCode()` - validates the player’s code against the expected secure solution.
- `BlueTeam.CloseChallenge()` - exits the challenge and restores the previous game state.

### Game State and Support Systems

- `LevelController.UpdateLevelStatus()` - records level completion and time spent.
- `InteractionDetector.InteractButton()` - triggers interaction when the player is in range.
- `BackgroundMusicManager.SwitchToBrainTimeMusic()` - changes music for the coding phase.
- `BackgroundMusicManager.SwitchToMissionMusic()` - changes music for active missions.
- `BackgroundMusicManager.SwitchToDefaultMusic()` - returns to normal gameplay music.
- `savecontroller.OnPlayButtonPressed()` - starts the save/load flow when the game begins.

## Technical Notes

- The project mixes exploration gameplay with embedded security exercises.
- Blue Team challenge input is validated against expected code, with formatting-aware highlighting to help players learn the correct structure.
- Mission progress and level completion are persisted so the player can continue across sessions.

## Summary

This project is meant to help players learn cybersecurity by doing. Instead of only reading about vulnerabilities, the player practices identifying, exploiting, and fixing them through gameplay.

