# GitHub Copilot Instructions for Easy Upgrades (Continued) Mod

## Mod Overview and Purpose
Easy Upgrades (Continued) is a mod for the popular RimWorld game. It is an update of the ToyWalrus mod, designed to provide an easier way to upgrade or downgrade buildings without needing to deconstruct them first. The resource cost remains the same, and the mod also allows players to increase the quality of items, apparel, and sculptures. This mod supports various other mods, including Quality Of Building, VGP Garden Gourmet, VGP Garden Fabrics, Vanilla Furniture Expanded - Production, Vanilla Furniture Expanded - Architect, and Medieval Overhaul.

## Key Features and Systems
- **Building Upgrades and Downgrades**: Players can upgrade or downgrade buildings without deconstructing them, using only the difference in resources.
- **Quality Increase Designator**: Players can attempt to increase the quality of items, apparel, and sculptures. Success, failure, or no change can occur, with customizable chances.
- **Multiple Research Requirements**: Players can have multiple research requirements when upgrading items.
- **Selectable Max Quality**: The maximum quality to upgrade is selectable in the mod settings.
- **Cost Calculation**: Quality upgrades require each original material, including stuff and fixed costs, scaled by the quality multiplier. Items without defined costs use a tech-level and item-value fallback.
- **Quality Upgrade Work**: Work has no upper time cap. Pawns must meet the original building and applicable crafting recipe skill requirements.
- **Support for Multiple Mods**: The mod integrates with various other mods, enhancing compatibility and functionality.

## Coding Patterns and Conventions
- **Designators**: Custom designators like `Designator_Upgrade` and `Designator_IncreaseQuality` are used to handle upgrade and quality increase actions.
- **Comps**: Custom components like `CompUpgrade` and `CompProperties_Upgradable` are used to manage upgrade properties and behaviors.
- **Harmony Patches**: Harmony is extensively used to patch vanilla and third-party code to integrate new features.
- **XML Definitions**: XML files are used to define new designations, jobs, work givers, and patches.

## XML Integration
The mod uses XML files to define new game elements such as designations, jobs, and work givers. Key XML files include:

- **DesignationDefs.xml**: Defines new designations like "Upgrade" and "Increase Quality".
- **JobDefs.xml**: Defines new jobs related to upgrading and quality increasing.
- **WorkGiverDefs.xml**: Defines new work givers for the designated actions.
- **Patches**: XML files that define Harmony patches to modify vanilla behavior.

## Harmony Patching
Harmony is used to patch vanilla and third-party code to integrate new features. Key patches include:

- **AddCompToThingsWithComp.xml**: Adds custom components to things that already have a specific component.
- **AddDesignator.xml**: Adds new designators to the game.

## Suggestions for Copilot
1. **Code Snippets**: Provide code snippets for creating custom designators, components, and Harmony patches.
2. **XML Templates**: Provide XML templates for defining new designations, jobs, and work givers.
3. **Debugging Tips**: Provide tips and tricks for debugging Harmony patches and XML definitions.
4. **Integration Examples**: Provide examples of how to integrate the mod with other mods like Quality Of Building and VGP Garden Gourmet.

## Example C# Code Snippet
Here is an example of a custom designator for increasing quality:

csharp
public class Designator_IncreaseQuality : Designator
{
    public Designator_IncreaseQuality()
    {
        defaultLabel = "Increase Quality";
        defaultDesc = "Attempt to increase the quality of this item.";
        icon = ContentFinder<Texture2D>.Get("Designators/IncreaseQuality");
        soundDrag = SoundDef.Named("DesignateDragStandard");
        useMouseIconWhenDragging = true;
        soundSucceeded = SoundDef.Named("Click");
        hotkey = KeyBindingDefOf.Misc1;
        this.tutorTag = "IncreaseQuality";
    }

    public override bool CanDesignateCell(IntVec3 c)
    {
        return false;
    }

    public override bool CanDesignateThing(Thing t)
    {
        return t is ThingWithComps thingWithComps && thingWithComps.GetComp<CompUpgrade>() != null;
    }

    public override void DesignateThing(Thing t)
    {
        base.DesignateThing(t);
        t.Map.designationManager.AddDesignation(new Designation(t, DesignationDefOf.IncreaseQuality));
    }
}


## Example XML Template
Here is an example of an XML template for a new designation:

xml
<DesignationDef>
    <defName>Upgrade</defName>
    <label>Upgrade</label>
    <description>Upgrade this building.</description>
    <hotKey>M</hotKey>
    <iconPath>Designators/Upgrade</iconPath>
    <workGiverClass>WorkGiver_Upgrade</workGiverClass>
    <jobClass>JobDriver_Upgrade</jobClass>
</DesignationDef>


By following these guidelines and using the provided code snippets and XML templates, you can effectively contribute to the Easy Upgrades (Continued) mod and enhance its functionality.

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
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

