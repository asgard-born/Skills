# SkillsService & Articulation Points Algorithm

## Overview

`SkillsService` manages a **skill tree** (UI presentation) built on a **skill graph** (data structure) with these features:

- Tracks learned and base skills
- Determines learnable/forgettable skills
- Ensures all learned skills remain connected to a base skill
- Updates UI **reactively** via `SkillViewModel` with **loose coupling** between service and view

---

## Key Concepts

### Core Principles
- **Base skill permanence**: Base skills cannot be forgotten
- **Connectivity requirement**: All skills must remain connected to base
- **Point economy**: Learning costs points, forgetting refunds them
- **UI consistency**: View models always reflect current game state

### SkillViewModel

Acts as a **data contract** between service and view, enabling **reactive updates** with **loose coupling**.

**Properties:**
- `Type` – skill identifier
- `Neighbors` – connected skills
- `Cost` – point cost to learn the skill
- `IsBase` – whether this is a base skill (cannot be forgotten)
- `IsLearned`
- `CanBeLearned`
- `CanBeForgotten`

Views subscribe to `SkillViewModel` changes via ReactiveProperties/ReactiveCommands for automatic UI updates.

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