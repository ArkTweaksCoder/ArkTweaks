# 🛡 Safety Model

## Core Principle

No system change is applied without safety validation.

---

## 🔒 Safety Rules

1. Always create restore point before changes
2. All registry edits must be reversible
3. No disabling security features (Defender, Firewall)
4. No kernel-level modifications
5. No permanent system damage allowed

---

## ⚠ Risk Levels

### Low
- Temporary files
- Startup toggles
- Power plans

### Medium
- Registry changes
- System settings

### High
- Anything affecting core OS behavior (generally avoided)

---

## 🔁 Revert System

Every tweak must include:
- Apply()
- Revert()

---

## 🧠 Philosophy

Stability > performance hype
