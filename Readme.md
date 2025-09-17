# SkillsService & Articulation Points Algorithm

## Overview

`SkillsService` manages a **skill tree** (UI presentation) built on a **skill graph** (data structure) with these features:

- Tracks learned and base skills
- Determines which skills can be learned or forgotten
- Ensures all learned skills remain connected to a base skill
- Updates the UI **reactively** via `SkillViewModel`, maintaining **loose coupling** between service and view

---

## Key Concepts

- **Base skill permanence**: Base skills can never be forgotten
- **Connectivity requirement**: All learned skills must remain connected to base
- **Point economy**: Learning costs points, forgetting refunds points
- **UI consistency**: View models always reflect current game state

### SkillViewModel

Represents a single skill in the tree.  
It acts as a **data contract** between the service and the view, ensuring **reactive updates** with **loose coupling**.

**Properties:**

- `Type` – unique identifier of the skill
- `Neighbors` – neighboring skills (connected vertices in the graph)
- `Cost` – how many points are required to learn the skill
- `IsBase` – whether this is a base skill (cannot be forgotten)
- `IsLearned` – whether the skill is currently learned
- `CanBeLearned` – whether the skill can currently be learned
- `CanBeForgotten` – whether the skill can currently be forgotten

The view (`SkillView`) **subscribes to `SkillViewModel` changes** via ReactiveProperties/ReactiveCommands.  
When the service updates the model, the UI refreshes automatically without any direct reference to the view.

---

## Articulation Points Algorithm

The service uses **Tarjan's algorithm** for finding articulation points:

- Uses **Depth-First Search (DFS)** starting from the base skill
- Based on proven computer science principles
- Tracks discovery times (`disc`) and low-link values (`low`) to detect critical skills
- Skills whose removal would disconnect the graph from the base cannot be forgotten
- Maintains connectivity ensuring all remaining skills stay linked to the base

The algorithm efficiently (O(V+E)) identifies articulation points in the learned skill graph.

---
## Performance Efficiency
- **O(V + E)** complexity - fastest possible for graph traversal
- Executes in milliseconds even for large skill trees
- Only runs when skill tree structure changes
- View model updates are optimized to only affect changed skills
- Reactive architecture minimizes unnecessary UI updates

---
## How the Service Works

### 1. Initialization
- Sets up reactive subscriptions to score changes and user actions
- Initializes all skill view models with their initial status

### 2. Skill Selection
- Updates the selected skill's status via the view model
- Recalculates articulation points to ensure UI reflects current state

### 3. Learning a Skill
- Checks prerequisites: sufficient points and adjacent learned skill
- Marks skill as learned and updates view model
- Recalculates articulation points for the modified graph

### 4. Forgetting a Skill
- Checks if skill is not base and not an articulation point
- Marks skill as forgotten and updates view model
- Returns skill points and recalculates articulation points

### 5. Forgetting All Skills
- Iteratively removes all forgettable skills while maintaining connectivity
- Processes skills in optimal order to maximize forgettable skills
- Updates each skill status via view model contract
- Recalculates articulation points after each change