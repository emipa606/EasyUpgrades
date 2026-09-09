# Easy Upgrades (Continued) Copilot Instructions

## Mod Overview and Purpose
The "Easy Upgrades (Continued)" mod for RimWorld is designed to streamline and enhance the upgrading process of various in-game structures and items. This mod allows players to upgrade or downgrade their buildings without the need to deconstruct them, retaining resource costs only for the upgrades or downgrades differences. It is particularly useful for players who want more control over their colony's buildings and want to integrate seamlessly with other mods enhancing the building or upgrading processes.

## Key Features and Systems
- **Dynamic Upgrading:** Upgrade buildings directly without deconstruction. Only the difference in resource costs is needed for upgrading or refunded for downgrading.
- **Upgrade Designation:** Provide an Orders designator so players can queue upgrades on multiple upgradeable things at once.
- **Multiple Mod Integration:** Includes supports for workbenches from VGP Garden Gourmet and VGP Garden Fabrics, and several Vanilla Furniture Expanded mods.
- **Complex Research Requirements:** Incorporate multiple research requirements for specific upgrades.
- **Quality Designation:** Introduce a quality-designator system to designate multiple items for quality upgrades at once.
- **Configurable Settings:** The maximum quality for upgrades and success probabilities can be modified in mod settings.
- **Contextual Upgrading:** Upgrade options are available depending on the item type and current state (e.g., a sculpture needs to be uninstalled before upgrading its quality).
- **Material-Sensitive Costs:** Resource costs for non-stuff items are calculated based on tech-level and item value, allowing more fair and logical resource management.

## Coding Patterns and Conventions
- Follow standard C# conventions, including meaningful class and method names.
- Use `PascalCase` for class and method names, and `camelCase` for local variables.
- Keep code documentation and comments up-to-date to aid future maintenance or updates.
- Adhere to high modularization standards, separating different functionalities into well-defined classes and methods.

## XML Integration
- Utilize XML definitions for integrating new components and designators, such as `DesignationDefs`, `JobDefs`, and `WorkGiverDefs`.
- Maintain clean separation of logic within XML files for easy identification and modification.
- Ensure XML files have correctly defined hierarchy and references to prevent run-time errors.

## Harmony Patching
- Use Harmony to patch existing game methods for customized behavior, ensuring compatibility with other mods.
- Implement patches with the goal of minimizing performance overhead.
- Apply conditional checks within patch methods to cater to specific scenarios such as resource calculations or interface updates.

## Suggestions for Copilot
- **Enhanced Suggestions:** Use Copilot to generate method stubs or XML structures, prompting consistency across similar operations involved in upgrading systems.
- **Code Completion Plugins:** Encourage using code completion for developing routine actions like logging, exception handling, or data calculations.
- **Automated Refactoring:** Leverage Copilot to provide refactoring suggestions for optimizing existing code or identifying redundant parts.
- **Documentation Updates:** Assist in generating consistent code documentation or updating class/file summaries according to changes.
- **Debugging Enhancements:** Provide debugging assistance, especially related to intricate mod compatibility issues or error logging improvements.

## Known Issues
- Occasionally, resources might not get deducted when increasing the quality of an item. Monitor player reports for insights into possible fixes.
- Encourage users to isolate the mod during error occurrence tests and report problems through the appropriate channels, primarily the Discord-error-reporting channel.

Thank you for using Easy Upgrades, and feel free to contribute solutions or improvements to the GitHub repository.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

