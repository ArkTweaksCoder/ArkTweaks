You are working inside an existing GitHub repository called "ArkTweaks".

YOU MUST START BY READING THE FULL REPOSITORY:

Read in this exact order:
- README.md
- ARCHITECTURE.md
- UI_GUIDE.md
- TWEAK_DATABASE.md
- SAFETY_MODEL.md
- ROADMAP.md
- API_REFERENCE.md

Do NOT start coding until you fully understand the system design.

---

# 🎯 CURRENT GOAL

Build a WORKING MVP of Ark Optimizer (WPF .NET 8).

This is NOT a full product yet.

This is ONLY a functional desktop application with:
- UI shell
- navigation system
- license system
- basic safe system tools

No advanced features yet.

No future systems yet.

---

# 🧱 MVP SCOPE (STRICT)

ONLY implement:

## 1. UI SYSTEM
- MainWindow with modern dark UI
- Left sidebar navigation
- Page switching system (IMPORTANT FIX: must work properly)
- Pages:
  - Dashboard
  - Optimize
  - Cleanup
  - Startup
  - Gaming
  - Performance
  - Restore
  - Settings
  - About

NAVIGATION MUST BE CENTRALIZED:
- Create NavigationService
- Create PageType enum
- Pages must switch through NavigationService ONLY

DO NOT create disconnected UI pages.

---

## 2. LICENSE SYSTEM (FULL IMPLEMENTATION REQUIRED)

Implement full license system:

### License tiers:
- Free
- Standard
- Pro
- Ultimate

### License key format:
ARK-XXXX-XXXX-XXXX-TIER

Where TIER = STD | PRO | ULT

---

### REQUIRED FILES:
- LicenseTier enum
- LicenseInfo model
- LicenseValidator
- LicenseStorage (AppData/ArkTweaks/license.json)
- LicenseService (central manager)

---

### RULES:
- Free is default if no key exists
- Invalid key = Free
- License is loaded on app startup
- License must be globally accessible via LicenseService

---

### FEATURE GATING (CRITICAL)

All features must use:

LicenseService.IsFeatureEnabled(feature)

DO NOT use direct tier comparisons anywhere.

Example:
❌ if (tier == Pro)
✔ if (IsFeatureEnabled(ProFeature.X))

---

## 3. SAFETY SYSTEM (UPDATED ENFORCEMENT)

You MUST enforce SAFETY_MODEL.md in code.

### Add:
- SafetyValidator must run BEFORE any system action
- No tweak or system action can bypass validation

### Required pipeline:
UI Action → License Check → Safety Check → Execution

If safety fails:
- block action
- show UI warning

---

## 4. ARCHITECTURE RULES

Follow MVVM strictly:
- UI never directly calls system services
- ViewModels communicate with Services only

Core layers:
- UI
- Core
- Services

DO NOT over-engineer beyond this.

---

## 5. SIMPLIFICATION RULE (IMPORTANT FIX)

You must reduce complexity:

### ONLY IMPLEMENT ACTIVE SYSTEMS:

Active Features:
- System dashboard (basic info only)
- Temp cleanup
- Startup manager (basic toggle)
- Restore point creation
- Power plan switch

---

### EVERYTHING ELSE MUST BE STUBBED:
- Advanced tweaks
- diagnostics
- automation
- network tools
- gaming profiles

These should exist ONLY as placeholders in UI.

---

## 6. NAVIGATION FIX (IMPORTANT CHANGE)

Fix navigation system properly:

- Sidebar click → NavigationService → Page switch
- No direct page instantiation inside UI
- Must support clean switching without memory leaks

---

## 7. OUTPUT EXPECTATION

After completion:

- App compiles successfully
- App launches
- Sidebar navigation works
- Pages switch correctly
- License system works
- Free tier works by default
- License key unlock works instantly
- Locked features show upgrade prompt
- Safety system blocks unsafe actions

---

## 8. STOP CONDITION

Stop after MVP is complete.

DO NOT:
- build extra features
- expand tweak database
- add future products
- implement ping tool
- implement keyboard tool

ONLY MVP.

Wait for next instruction.
