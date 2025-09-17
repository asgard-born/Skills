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
Represents a single skill with properties:

- `IsLearned` – whether the skill is currently learned.
- `IsBase` – whether the skill is a base skill.
- `Neighbors` – skills connected to this skill in the tree.

The **view (`SkillView`) updates automatically** when the `SkillViewModel` changes. The service never interacts with the view directly, only through the view model contracts.

---

## Articulation Points Algorithm

The service calculates **articulation points** in the learned skill graph:

- Uses **Depth-First Search (DFS)** starting from the base skill.
- Tracks discovery times and low-link values to detect skills whose removal would disconnect the graph.
- These critical skills cannot be forgotten, ensuring all remaining skills stay connected to the base.

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

## Why DFS is Used

DFS efficiently explores the skill tree and calculates low-link values for articulation points.  
This ensures no learned skill becomes disconnected from the base skill when forgetting skills.

---

## Notes

- Only skills connected to a base skill (directly or through a chain of learned skills) can be forgotten.
- The service **never updates views directly**; it only updates view models.
- UI reacts automatically to changes in the view model, maintaining **weak coupling** and separation of concerns.
