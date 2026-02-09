# TestingRepo

## Gameplay state gating
This fishing game only runs while the `GameFlowManager` state is `Running`. To prevent spawns and movement during `Idle` or `Ended`:

1. **Scene setup**
   - Assign the scene's `GameFlowManager` to:
     - `RiverSpawner.gameFlowManager`
     - `RiverDriftMover.gameFlowManager` (on prefabs or instances spawned in the river)
     - `FishingHookController.gameFlowManager`
2. **Start/End**
   - Use the Start UI button (wired in `GameUIController`) to call `GameFlowManager.StartGame()`.
   - Use the End UI button to call `GameFlowManager.EndGame()`.

If any of the `gameFlowManager` references above are left unset, the associated system will remain paused and will not spawn or move objects.

## Leaderboard (best score) setup
The leaderboard HUD shows only the best score. To make it update:

1. **Scene wiring**
   - Assign the scene's `LeaderboardManager` to:
     - `GameFlowManager.leaderboardManager` (so EndGame records the score)
     - `GameUIController.leaderboardManager` (so the UI listens for best score changes)
2. **UI text**
   - Assign a `TextMeshProUGUI` component to `GameUIController.leaderboardText`.
   - The UI will render `Best: {score}` when the best score changes.

If `LeaderboardManager` is not assigned to both the game flow and UI controllers, the best score text will not update.

## Side quest mission system setup
The side quest system spawns a mission every 15 seconds (50% chance) while the game is `Running`. The mission asks the player to catch the same random evidence type three times in a row. On success, the current score is multiplied by 2, the mission text turns green, and it fades out.

1. **Add the manager**
   - Create an empty GameObject (e.g., `SideQuestMissionManager`).
   - Add the `SideQuestMissionManager` component.
2. **Wire references**
   - Assign the scene's `GameFlowManager`.
   - Assign the scene's `EvidenceCounterManager`.
   - Assign the scene's `ScoreManager`.
   - Assign a `TextMeshProUGUI` object for the mission display.
3. **UI text**
   - Ensure the mission text object starts inactive or empty (the script will enable it when a mission starts).
   - Adjust `Mission Check Interval Seconds`, `Mission Chance`, `Required Streak`, and `Success Fade Seconds` as needed in the inspector.

If the game state is not `Running`, the side quest loop pauses and any active mission text is cleared.

## Audio UI setup (slider + mute icon)
The audio UI supports a slider for master volume and a toggle button that swaps between sound-on and sound-off icons.

1. **Audio settings manager**
   - Add `AudioSettingsManager` to a scene object.
   - Assign the volume slider's `OnValueChanged` to `AudioSettingsManager.SetMasterVolume`.
   - With the default `requirePointerForVolumeChange = true`, the slider only applies changes while a pointer (mouse/touch) is pressed.
2. **Sound toggle button**
   - In `GameUIController`, wire:
     - `audioSettingsManager` to the scene's `AudioSettingsManager`.
     - `soundToggleButton` to the UI button.
     - `soundToggleIcon` to the button's `Image`.
     - `soundOnSprite` and `soundOffSprite` to the desired icon assets (normal icon and icon with an X overlay).
   - The button will swap the icon when mute is toggled.
