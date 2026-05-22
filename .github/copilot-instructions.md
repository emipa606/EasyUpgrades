# Copilot Instructions for Easy Upgrades (Continued)

## Mod Overview and Purpose

Easy Upgrades (Continued) is a RimWorld mod that allows players to upgrade or downgrade buildings, items, clothing, and sculptures in their colonies without needing to deconstruct them first. The mod figures out the cost based on the resource difference between the old and new objects. This streamlined process reduces resource waste and adjusts the mod compatibility to enhance gameplay for more complex modded environments.

## Key Features and Systems

- **Upgrade/Downgrade Functionality**: Enables players to upgrade certain buildings like stoves and generators with only the difference in resources required.
- **Quality Manipulation**: Allows players to attempt to increase the quality of items, weapons, apparel, and sculptures, depending on pawn skills.
- **Mod Compatibility**: Provides support for multiple popular mods like VGP Garden series and Vanilla Furniture Expanded.
- **Dynamic Settings**: Features mod settings that let players choose max quality levels and modify upgrade chance values.
- **Refund System**: Includes logic to refund resources appropriately, including fuel, during upgrades or downgrades.

## Coding Patterns and Conventions

The mod uses several key patterns to ensure maintainability and integration with RimWorld:

- **Component-Based Design**: Utilizes `ThingComp` to extend functionality of in-game objects without invasive changes.
- **Defining Extensions**: Employs definition classes like `EasyUpgradesDesignationDefOf` for custom designator identification.
- **Mod Settings Management**: Implements `ModSettings` through `EasyUpgradesSettings` for customizable player options.
- **Job Drivers and WorkGivers**: Leverages custom job drivers and work givers for organizing upgrade tasks and other interactions.

## XML Integration

XML definitions play a substantial role in organizing mod elements:

- XML is used for defining `ThingDefs` and integrating new functionalities with pre-existing or new objects in the game.
- `CompProperties` are harnessed to connect C# logic with XML definitions for easy customization and extension.

## Harmony Patching

The mod uses Harmony for runtime patching:

- **Patch Methods**: Patching methods involve replacing parts of existing game logic to add or modify functionality without altering the base game code.
- Ensure that patches are conditionally applied to reduce performance overhead and maximize compatibility.

## Suggestions for Copilot

When developing with GitHub Copilot in this project, consider the following guidelines:

1. **Component Integration**: Suggest proper usage of `ThingComp` to non-invasively add behavior to in-game objects.
2. **Job Management**: Offer auto-completions or refactoring suggestions around job handlers like `JobDriver_ModifyThing`.
3. **Resource Calculation**: Aid in formulating logic for difference calculations between resources during upgrades.
4. **Error Handling**: Prompt best practices for try-catch blocks, especially around quality upgrade failures or resources refunds.
5. **Modular Function Design**: Encourage patterns where behaviors are split into modular functions for reuse and clarity.
6. **XML-C# Sync**: Ensure integration between XML values and C# logic is transparent and maintainable.
7. **Documentation Hints**: Include suggestions for inline comments to maintain code clarity and aid future development.

By adhering to the above guidelines, Copilot can both assist in development and help ensure that any new modifications remain consistent with the existing codebase's structure and design ethos.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
