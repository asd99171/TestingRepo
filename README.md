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

## Catch counter setup
To show how many Fish, Evidence, and Corpse items were caught:

1. **Scene wiring**
   - Assign the scene's `CatchCounterManager` to:
     - `GameFlowManager.catchCounterManager` (so counts reset and enable with the round)
     - `FishingHookController.catchCounterManager` (so catches are recorded)
     - `GameUIController.catchCounterManager` (so the UI listens for count changes)
2. **UI text**
   - Assign `TextMeshProUGUI` components to:
     - `GameUIController.fishCountText`
     - `GameUIController.evidenceCountText`
     - `GameUIController.corpseCountText`

If any of these references are missing, the catch count labels will not update.
