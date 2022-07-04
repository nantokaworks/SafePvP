# SafePvP

## What is This?

Mod for Rust(Oxide)

Switchable PvP per Player :)
Players in PvE mode are protected by player confidence and buildings.
	

## Links

[Oxide Lib](https://umod.org/games/rust)

[Oxide API Reference](https://umod.org/documentation/games/rust)

## Configuration

```
{
  "fireInterval": 600000
}
```

### FireInterval

Interval (msec) to prevent consecutive commands from being issued.

## Chat Commands

/sp on - Pve Mode
/sp off - PvP Mode
/sp - current state

## Localization

```
{
	"Prefix": "<color=#66ff66>[</color>SafePvP<color=#66ff66>]</color>",
	"Already safe": "<color=yellow>{0}</color> already safe :)",
	"Already unsafe": "<color=yellow>{0}</color> already unsafe XD",
	"Switch to safe": "<color=yellow>{0}</color> switch to safe :)",
	"Switch to unsafe": "<color=yellow>{0}</color> switch to unsafe XD",
	"Command Interval Error": "It takes <color=yellow>{0}</color> seconds until the command can be used :(",
	"Warning": "<color=red>{0} is PvE Player :)</color>",
	"WarningCantbuild": "<color=red>PvE Player's Building :)</color>",
	"Help message": "\n<color=yellow>safe</color>\n<color=#66ff66>{0}</color>\n\n<color=yellow>command usage</color>\n<color=#36a1d8>/sp on</color> switch to safe.\n<color=#36a1d8>/sp off</color> switch to unsafe.\n<color=#36a1d8>/sp</color> this help."
}
```