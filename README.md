# Card Matching Game Example

### Summary

This is a simple, responsive, and smooth card-matching game built from scratch using **Unity 2021.3.45f1**, with no third-party frameworks or purchased assets. The project supports both PC and mobile platforms and uses Unity's **New Input System** for input handling.

The game includes a basic **menu** with `Start` and `Load` buttons. If the user had previously started a game, they can continue from where they left off using the load feature. Gameplay is fluid — players can flip cards continuously without waiting for the match result of the previous pair, allowing for fast-paced interaction. All animations, card layout systems, save/load mechanisms, and effects were implemented using native C# and Unity features.

---

## 🔧 Project Setup

- **Engine**: Unity 2021.3.45f1 LTS  
- **Platform**: Desktop (PC/Mac) + Mobile (Android/iOS)  
- **Input**: Unity New Input System  
- **No external assets or frameworks used**

---

## ✅ Feature Checklist (Based on Requirements)

### 1. Develop from Scratch  
✔ Built entirely from scratch using Unity 2021.3.45f1 — no third-party or purchased assets were used.

### 2. Smooth Gameplay with Card Flip Animations  
✔ Flip animations are smooth and handled with scale-based animation.  
✔ Users are allowed to continue flipping cards without waiting for the previous match animation or comparison to complete.

### 3. Dynamic Card Layouts (2x2, 2x3, 5x6, etc.)  
✔ Implemented using a flexible layout system inside `CardManager`.  
✔ Cards dynamically scale and reposition themselves based on the screen size and camera position.

### 4. Save/Load System  
✔ Implemented a modular save system using `SaveLoadManager` and `PlayerPrefSaveLoadDal` (Data Access Layer).  
✔ Allows future switching to cloud saves by simply swapping the DAL instance, keeping the core game logic untouched.

### 5. Scoring System  
✔ Score increases upon correct matches.  
✔ A combo system rewards players with bonus points for consecutive successful matches.

### 6. Sound Effects  
✔ Integrated basic sound effects for:
- Card flipping  
- Successful match  
- Mismatch  
- Game over  

### 7. Git Usage & Version Control  
✔ Git was used throughout the development.  
✔ Two branches were maintained:  
  - `main` – production-ready version  
  - `development` – active development  
✔ Frequent, meaningful commit messages were written as if working in a collaborative team.  
✔ The **first commit** contains an empty Unity project, marking the beginning of the work.

---

## 🧪 Scope & Stability

- 💡 Focused solely on gameplay mechanics (menu system included but minimal).  
- 💯 Game runs with **zero errors, warnings, or crashes** in both desktop and mobile builds.  
- ✔ Fully tested on Android and desktop platforms.  
- 🧩 The codebase is modular, scalable, and easy to extend (e.g., with cloud save support or new layouts).

---

## 🎮 How to Play

- Flip cards to find matching pairs.  
- Matched cards stay flipped; mismatched cards flip back after a short delay.  
- Try to match all pairs with the fewest attempts and the highest combo!

---

## 🚀 Build Targets

- ✅ Windows/Mac  
- ✅ Android *(iOS compatible in theory, but not tested due to submission scope)*

---

## 📌 Final Notes

- Clean, readable code.  
- Strong focus on architecture and separation of concerns.  
- Game flow is designed to be extendable — from UI expansion to analytics or advanced save mechanisms.
