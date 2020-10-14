## Card Battle Mode

  ![Combat Example](https://s3.amazonaws.com/prod-media.gameinformer.com/styles/full/s3/2019/03/01/40592c55/swq6.jpg)

  **Setup:**  
  - Shuffle player deck, and each individual enemy deck
  - Player's party characters are shown on the left-side of the screen in traditional JRPG battle view
  - Enemy character are shown on the right-right of the screen in traditional JRPG battle view
  - Player visibly draws a hand of 6 cards from his deck

  **Action Selection:**  
  - Each enemy secretly selects the correct number of cards from its hand
  - Player selects 3 cards and confirms the attack

  **Action Resolution:**  
  - Once the attack is confirmed, enemy cards are shown on screen
  - Resolve each of the player's cards, one at a time
  - If an enemy dies, remove that enemy's card from the queue
  - Once the player's cards are resolved, resolve each of the enemy's cards, one at a time
   
  **Turn Wrapup:**
  - If all the player's characters are knocked out, fade out to a Game Over graphic
  - If all the enemies are dead, display a celebration animation, award XP, Loot, and Money and return to Game Progression Mode
  - Otherwise, player and enemy should draw up to their hand maximum and another turn is played
  - If trying to draw cards from an empty deck, reshuffle the discard pile and put it in the deck before drawing card.

![Victory Example](https://i1.wp.com/www.geeksundergrace.com/wp-content/uploads/2019/04/steamworldquest2.jpg)

## Technical Design Notes
- Every part of the battle rules should be modular, and loosely coupled.
- There should be a different script/component for each of these behaviors.
- Flowing through the Battle should be guided by GameEvents

