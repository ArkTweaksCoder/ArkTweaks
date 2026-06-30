# 🏗 Ark Tweaks Architecture

## Overview

Ark Tweaks follows a modular architecture separating UI, system logic, and tweak execution.

---

## 🔷 Layers

### 1. UI Layer (WPF)
- Views (XAML pages)
- ViewModels (MVVM logic)
- Styles (themes, colors, layout)

Responsibilities:
- Display system information
- Handle user interaction
- Send commands to core engine

---

### 2. Core Engine

Responsible for all logic:

- OptimizationEngine
- SafetyValidator
- TweakExecutor
- RestoreManager

Flow:
User Action → Engine → Validation → Execution → Logging

---

### 3. System Layer

Handles Windows interaction:

- RegistryService
- PowerPlanService
- StartupService
- TempCleanerService

Uses:
- Windows APIs
- PowerShell
- CMD where necessary

---

### 4. Tweaks Layer

Each tweak is modular:

- defined as a class
- has Apply() and Revert()
- includes metadata (risk, description)

---

## 🔁 Execution Flow

1. Scan system
2. Recommend tweaks
3. User selects
4. Validate safety
5. Create restore point
6. Apply changes
7. Log results

---

## 🧠 Design Goal

Keep system modular so new tweaks can be added without modifying core engine.
