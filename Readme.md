# SkillsService & Articulation Points Algorithm

## Overview

`SkillsService` manages a skill tree system with these features:

- Tracks learned and base skills.
- Determines which skills can be learned or forgotten.
- Ensures all learned skills remain connected to a base skill.
- Updates the UI **reactively** via `SkillViewModel`, keeping a **loose coupling** between service and view.

---

## Key Concepts

### SkillViewModel

Represents a single skill in the tree.  
It acts as a contract between the service and the view, ensuring **reactive updates** with **loose coupling**.

**Properties:**

- `Type` – unique identifier of the skill.
- `Neighbors` – list of connected skills (graph edges).
- `Cost` – how many scores are required to learn the skill.
- `IsBase` – whether this is a base skill (cannot be forgotten).
- `IsLearned` – whether the skill is currently learned.
- `CanBeLearned` – whether the skill can currently be learned.
- `CanBeForgotten` – whether the skill can currently be forgotten.

The view (`SkillView`) **subscribes to `SkillViewModel` changes**.  
When the service updates the model, the UI refreshes automatically without any direct call to the view.

---

## Articulation Points Algorithm

The service calculates **articulation points** in the learned skill graph:

- Uses **Depth-First Search (DFS)** starting from the base skill.
- Tracks discovery times and low-link values to detect skills whose removal would disconnect the graph.
- These critical skills cannot be forgotten, ensuring all remaining skills stay connected to the base.

DFS efficiently explores the skill tree and calculates low-link values for articulation points.  
This ensures no learned skill becomes disconnected from the base skill when forgetting skills.
---

## How the Service Works

1. **Skill Selection**
    - Updates the selected skill’s status via the view model.

2. **Learning a Skill**
    - Checks if the skill can be learned.
    - Marks the skill as learned via the view model.

3. **Forgetting a Skill**
    - Checks if the skill can be forgotten.
    - Marks the skill as forgotten via the view model.

4. **Forgetting All Skills**
    - Iteratively removes all forgettable skills while maintaining base connectivity.
    - Updates each skill via the view model contract.
- Recalculates articulation points to ensure remaining skills are still connected.
---

## Notes

- Only skills connected to a base skill (directly or through a chain of learned skills) can be forgotten.
- The service **never updates views directly**; it only updates view models.
- UI reacts automatically to changes in the view model, maintaining **weak coupling** and separation of concerns.
