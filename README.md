Square One
===============

You can play Square One here: https://harlig.itch.io/squareone
 
Square One is an isometric puzzle game built for the [2022 Game Off](https://itch.io/jam/game-off-2022).

Want to play from the source? Head over to [Releases](/Releases/) and downloaded the latest release. Just unzip the file and run the SquareOne executable.

------------

 
- [Square One](#square-one)
- [Demo](#demo)
- [Ideas](#ideas)
  - [MVP](#mvp)
    - [TODO](#todo)
    - [Stretch](#stretch)
    - [Done](#done)
  - [Level ideas](#level-ideas)
  - [Mechanic ideas](#mechanic-ideas)
  - [Meta ideas](#meta-ideas)
- [Contributing](#contributing)
- [Notes](#notes)
- [Help](#help)
  - [OmniSharp givin' ya trouble?](#omnisharp-givin-ya-trouble)

# Demo

Here's what it looks like in its current state

![LevelOneDemo.pdf](/ReadMeAssets/Level%20One%20Demo.png)

# Ideas

- [ ] Saving. Use PlayerPrefs and it's super easy
  - https://www.red-gate.com/simple-talk/development/dotnet-development/saving-game-data-with-unity/
- [ ] Leaderboard per-level
- [ ] Leaderboard for total turns remaining summation golf
- [ ] Stars per level based on how well they do

## Bug Reports
- [ ] On MacOS in Firefox, the ice level with 2 moving non-aggressive cubes was allowing the player to roll (not slide) and was not counting turns

## MVP

### TODO

### Stretch

- [ ] Add `z` within level to restart level 
- [ ] Control remapping?
- [ ] Movement tied to realtime seconds, not frames
  - Won't do, need to remove physics in a later version

### Done
- [x] Basic level or two that introduce concepts with little popups as you hit waypoints
- [x] Full ice map with obstacles that stop you from sliding
- [x] Maze with stationary obstacles
- [x] Maze with moving obstacle(s)
- [x] Different all ice puzzles, or mostly ice
- [x] Fix screen scale
- [x] Cleanup Main Menu (remove options)
- [x] Control directions feel clunky
  - Added compass but still could be updated
- [x] Credits page
- [x] Esc functionality for when in game also should be clickable
- [x] Intro for turn limit, waypoint, ice tiles, and obstacles
- [x] should slide on ice once in terminal state
- [x] Shouldn't hit waypoints once level in terminal state

## Level ideas

Should consider accessibility and write letters or some symbol on tiles to differentiate. 

- [ ] Bigger wall moving level that continues to move towards you
- [ ] enemy is blocked away from you and moving it like in x direction causes it to eventually come closer in y direction

## Mechanic ideas

- [x] Snake
   - You leave behind a trail and hitting own trail makes you lose level
- [ ] Double colored tiles
   - Like two green tiles and when you hit one, you get a green snake trail, then when you hit the other it stops but persists. Then you hit one of the two red tiles and get a red trail, then hit the second one. If you hit an already painted tile, you lose
- [x] Static terrain
- [ ] Dynamic terrain
  - [x] Objects that move on a schedule and can move you around the map
  - [ ] Some levels require usage of those moving objects to push you to complete in level limit
- [ ] Enemy chase
  - [x] Every time you move, enemy moves or tries to block you
  - [ ] Can use terrain to reroute them
  - [x] Can hit certain tiles to disable them
- [ ] Irrelevant colors
- [ ] Wall (similar to dynamic terrain?)
  - This theoretical level has a wall, and rotating the wall into view adds a certain mechanic that aids/allows you to complete the level - Evan Lane (all royalties are awarded to Evan Lane)
- [ ] Off the grid
  - end up on diagonal grid
- [x] Ice
  - Tiles that make you slide until you hit end of ice path
- [ ] Traps
  - Tiles that knock you away from next waypoint or something
- [ ] Portal
  - Tiles that teleport you across the map, think going from one corner to another in the board game Clue
- [ ] Tiles below map, you have to go hit one beneath map
- [ ] Moveable object that you can push onto ice and such

## Meta ideas

- [ ] Shoes on wrong foot
- [ ] Don't judge a book by its cover
  - Irrelevant title page

# Contributing

## TODO code stuff

- [ ] Hard wrap lines, can't be bothered rn

Unity editor version: 2021.3.13f1

Please enable C# auto-formatting in your text editor, and avoid checking in unformatted code.

I added some notes into the help section below if OmniSharp is being a pain (more of a note for myself since I'm new to Unity). 

# Notes

Need assets? Check out https://itch.io/game-assets

# Help

## OmniSharp givin' ya trouble?
If OmniSharp is being a pain, then it's possible that it's loading a .sln file that's incorrect, and therefore not discovering your .csproj files. When OmniSharp starts up, check the log and ensure it's loading the root .sln file.

If there's no root .sln file, go into Unity->Edit->Preferences, then ensure that your External Script Editor is set to VSCode, and you can "Regenerate Project Files" without any boxes ticked
