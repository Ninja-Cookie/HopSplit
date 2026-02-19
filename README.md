# <p align="center">![hop-icon]</p>
# <p align="center">HopSplit - Big Hops LiveSplit Integration Plugin</p>

&nbsp;

## About
**HopSplit** is a **[Big Hops](https://store.steampowered.com/app/1221480/Big_Hops/)** plugin, through **[BepInEx](https://github.com/BepInEx/BepInEx)**, which can easily communicate with **LiveSplit** allowing:
- Loadless Game Time
- Automatic Splitting
- Adjustable Split Settings
- Ability to Sync with Big Hops Built-In Real-Time Speedrun Timer

## Installation
#### (If BepInEx is installed, you can skip to Step 4)
1. Go to **[BepInEx's Releases](https://github.com/BepInEx/BepInEx/releases)** and get the relevent BepInEx ZIP file.
   ##### (This will likely be BepInEx_win_x64, even on Linux, if the game runs through Proton)
2. Locate your games files, usually found in `/Steam/steamapps/common/Big Hops/`
   ##### (`Steam Library > Right-Click Big Hops > Selecting Properties... > Selecting Installed Files > Selecting Browse...`)
3. Open the BepInEx ZIP file, and extract the contents to the games folder.
   ##### (The BepInEx folder and other contents should now be in the same directory as `Big Hops.exe`)
4. Go to **[HopSplit's Releases](https://github.com/Ninja-Cookie/HopSplit/releases)** and get the latest ZIP file containing the plugin.
5. Open the **HopSplit** ZIP file, and extract the contents to inside the `BepInEx` folder, except the provided LiveSplit Any% template file, which you can open using LiveSplit, when extracted elsewhere.

## Usage
- The **HopSplit** Menu can be toggled by pressing `F1`, this is where you can adjust what splits are active or not. All of these settings will be saved automatically on changing them.
- To get LiveSplit to now connect, in LiveSplit, `Right-Click > Control > Start TCP Server` (This will need to be done once, each time LiveSplit is launched). When you start the game, it should now automatically find and connect to LiveSplit, or you can do it manually, by selecting the `Connect LiveSplit` button in the HopSplit Menu, once the TCP Server is running in LiveSplit.

[hop-icon]: https://github.com/user-attachments/assets/605eca72-1ef6-4a07-8d7d-3ea5c34178d1
