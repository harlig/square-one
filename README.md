# SquareOne
 
SquareOne is an isometric puzzle game. This game was built for the [2022 Game Off](https://itch.io/jam/game-off-2022)

## Ideas

### Level ideas

* Snake
  * You leave behind a trail and hitting own trail makes you lose level

### Meta ideas

* Shoes on wrong foot

## Notes

Need assets? Check out https://itch.io/game-assets

## Help

### OmniSharp givin' ya trouble?
If OmniSharp is being a pain, then it's possible that it's loading a .sln file that's incorrect, and therefore not discovering your .csproj files. When OmniSharp starts up, check the log and ensure it's loading the root .sln file.

If there's no root .sln file, go into Unity->Edit->Preferences, then ensure that your External Script Editor is set to VSCode, and you can "Regenerate Project Files" without any boxes ticked
