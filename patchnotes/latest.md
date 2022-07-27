## Patch Notes - v0.0.46
----

New Content:
- Draft Mode: Now allows players to construct any Squad they like

Balance Changes:
- Rebalance: All Hero base HP reduced by 24%
- Rebalance: Boost Shields and Super Charge now scale with Base Power instead of current Power
- Rebalance: Buffed Omnislash Scaling
- Rebalance: Draft fights and curves
- Rebalance: Draft Level Up Odds of Equipment Rarity has been tuned
- Rebalance: Enemies got some power back
- Rebalance: Enemies who flee from battle still grant you reward XP and Creds
- Rebalance: Major Encounter Builder Rework
- Rebalance: Rifle - Buffed Bullet Barrage and Covering Fire
- Rebalance: The power curve is a little more linear but gets higher
- Rebalance: Tuned some Stealth and Scheme cards that caused circular boost engines unintentionally
- Rebalance: Veda's Starting Magic Decreased by 1

Player Aids:
- Design: Added Implants to the Tutorial Clinic
- Tutorial: Added explanation panels for Clinic/Shop/Deckbuilder

Card Improvements:
- Card Function: Improved Roomsweeper Grenade resolution (1 packet of damage)

Art Improvements:
- Anim: Added Character anim for Reverse Polarity
- Art: Cover Art for v0.0.46
- Art: Evolved Riot Bot Card Animations
- Art: Improved New Map Node Placements
- New Art: Brand New Cyberpunk City Map art from Dystoth

UI Improvements:
- UI: Added Unlock Draft Mode Cheat
- UI: Augments with associated cards show the card on Hover
- UI: Can advance Tutorial Victory screen using a button on screen
- UI: Draft Heroes start with random selected heroes
- UI: Draft Mode - Augment UI has hovers and sounds
- UI: Draft Mode - Basic Card always shown at the top of the card list
- UI: Draft Rules panel added
- UI: Hero Augments panel is larger to support Draft extra augments
- UI: Hides Mouse Cursor when right-click-dragging a card
- UI: Improved Hero Panel Stats icon
- UI: Map has Persistent Node Locations across Save/Load
- UI: Minor tweak to Map Hero Panel
- UI: New Map View Layout
- UI: Opening the menu resets the Cursor and Mouse Drag
- UI: Prevents most mouse click actions while dragging. Cleaned up battle keyboard
- UI: Progress Screen shows unlock requirements
- UI: Progress View V2
- UI: Resets Mouse Drag State on Scene change or Menu Open
- UI: Starting a Draft from Main Menu when you have an existing game will now prompt to confirm
- UI: Updated Vulnerable Rule Panel
- UI: Victory Screen doesn't shows Credits Panel if you didn't earn any

Bug Fixes:
- Bug Fix: 3 Hero Party extra Level Up screen triggered in the UI
- Bug Fix: Combat Drone V2 now correctly Disable hero with Shock Blast
- Bug Fix: Draft Mode Load now can fit up to your 6 Augments
- Bug Fix: Draft Save/Load after level up now working correctly
- Bug Fix: Editor Find Cards String Comparison Null-Ref Fixed
- Bug Fix: Hero damage taken wasn't persisting between Boss Phases. Fixed
- Bug Fix: Missing Animation Shot-Rifle fixed in all cards
- Bug Fix: Prevent entering Deck Builder from Tutorial Card Shop
- Bug Fix: Reduced requirements to leave Deck Builder in Tutorial
- Bug Fix: Starting Permanent Gear can crash Hero Details View

Project:
- Project: Added Dystoth to the Credits
- Project: Started Coverage document

Miscellaneous:
- Coding: Adding Playtest Damage Competition Tracker
- Coding: Can Track Overkill damage amounts
- Coding: Deterministic Rng for Clinic Service Generation
- Coding: Draft Odds Tweaks
- Coding: Enemy Encounter Generation is now Deterministic
- Coding: Pick 3 Hero is now Persistent Deteministic
- Coding: Progressively Unlocks Draft Adventures
- Coding: Retrofitted Encounter Builder V5 to be Persistent Deterministic
- Coding: Shop Selection Determinism
