# SquareOne
 
SquareOne is an isometric puzzle game. This game was built for the [2022 Game Off](https://itch.io/jam/game-off-2022)

## Demo

Here's what it looks like in its current state

![LevelOneDemo.pdf](/ReadMeAssets/Level%20One%20Demo.png)

and with using the camera controls to rotate

![LevelOneDemoAltCamera.pdf](/ReadMeAssets/Level%20One%20Demo%20Alt%20Camera%20Angle.png)

## Contributing

Unity editor version: 2021.3.13f1

Please enable C# auto-formatting in your text editor, and avoid checking in unformatted code.

I added some notes into the help section below if OmniSharp is being a pain (more of a note for myself since I'm new to Unity). 

## Ideas

### Level ideas

I'm liking the idea of modules, so like having levels 1-3 build on top of the lighting up tiles mechanic. Could this actually be a legit game and we can make a meme mode in the theme of the cliche? 

Should consider accessibility and write letters or some symbol on tiles to differentiate. 

* Snake
  * You leave behind a trail and hitting own trail makes you lose level
* This theoretical level has a wall, and rotating the wall into view adds a certain mechanic that aids/allows you to complete the level - Evan Lane (all royalties are awarded to Evan Lane)

### Meta ideas

* Shoes on wrong foot

## Notes

Need assets? Check out https://itch.io/game-assets

## Help

### OmniSharp givin' ya trouble?
If OmniSharp is being a pain, then it's possible that it's loading a .sln file that's incorrect, and therefore not discovering your .csproj files. When OmniSharp starts up, check the log and ensure it's loading the root .sln file.

If there's no root .sln file, go into Unity->Edit->Preferences, then ensure that your External Script Editor is set to VSCode, and you can "Regenerate Project Files" without any boxes ticked
