# 📡 API Reference (Internal)

## Optimization Engine

### ScanSystem()

Returns system state:
- CPU
- RAM
- Storage
- Startup apps

---

### GetRecommendedTweaks()

Returns list of safe tweaks

---

### ApplyTweak(tweakId)

Applies selected tweak

---

### RevertTweak(tweakId)

Reverts changes using stored backup

---

## System Services

### RegistryService
- ReadKey()
- WriteKey()
- BackupKey()

---

### PowerPlanService
- GetPlans()
- SetPlan()

---

### TempCleanerService
- ScanTemp()
- DeleteTemp()

---

## Logging

### LogAction(string message)
Stores system actions locally
