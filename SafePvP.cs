using System;
using System.Collections.Generic;
using Oxide.Core;

namespace Oxide.Plugins {
  [Info("SafePvP", "snicyme", "0.1.4")]
  [Description("Allows safe PvP mode per player :)")]
  public class SafePvP : RustPlugin {

    #region [Fields]

    //////////////////////////////////////////
    //// Config variables                 ////
    //////////////////////////////////////////

    /// <summary>
    /// Allow PvP Player List
    /// </summary>
    List<ulong> SafePlayer { get; set; }

    Dictionary<ulong, long> PrevCmdMs = new Dictionary<ulong, long>();


    //////////////////////////////////////////
    //// Strings                          ////
    //////////////////////////////////////////
    const string M_PREFIX = "Prefix";
    const string M_ALREADY_SAFE = "Already safe";
    const string M_ALREADY_UNSAFE = "Already unsafe";
    const string M_SWITCH_TO_SAFE = "Switch to safe";
    const string M_SWITCH_TO_UNSAFE = "Switch to unsafe";
    const string M_WARNING = "Warning";
    const string M_FIRE_INTERVAL = "Command Interval Error";
    const string M_HELP = "Help message";


    #endregion


    #region Config
    private class ModConfig {
      public int FireInterval;
    }

    private ModConfig config;

    protected override void LoadDefaultConfig() {
      Config.WriteObject(GetDefaultConfig(), true);
    }

    private ModConfig GetDefaultConfig() {
      return new ModConfig {
        FireInterval = 60000,
      };
    }
    #endregion

    #region Localization


    protected override void LoadDefaultMessages() => lang.RegisterMessages(defmessages, this, "en");

    void SendMsg(BasePlayer player, string message, params string[] args) => player.ChatMessage($"{lang.GetMessage(M_PREFIX, this)} {string.Format(lang.GetMessage(message, this), args)}");



    Dictionary<string, string> defmessages = new Dictionary<string, string> {
      [M_PREFIX] = "<color=#66ff66>[</color>SafePvP<color=#66ff66>]</color>",
      [M_ALREADY_SAFE] = "<color=yellow>{0}</color> already safe :)",
      [M_ALREADY_UNSAFE] = "<color=yellow>{0}</color> already unsafe XD",
      [M_SWITCH_TO_SAFE] = "<color=yellow>{0}</color> switch to safe :)",
      [M_SWITCH_TO_UNSAFE] = "<color=yellow>{0}</color> switch to unsafe XD",
      [M_FIRE_INTERVAL] = "It takes <color=yellow>{0}</color> seconds until the command can be used :(",
      [M_WARNING] = "<color=red>{0} is PvE Player :)</color>",
      [M_HELP] = "\n<color=yellow>safe</color>\n<color=#66ff66>{0}</color>\n\n<color=yellow>command usage</color>\n<color=#36a1d8>/sp on</color> switch to safe.\n<color=#36a1d8>/sp off</color> switch to unsafe.\n<color=#36a1d8>/sp</color> this help.",
    };


    #endregion

    #region Commands


    [ChatCommand("sp")]
    void CmdSP(BasePlayer player, string command, string[] args) {
      switch (args.Length) {
        case 1:
          long currentTicks = Convert.ToInt64(Math.Floor(DateTime.Now.Ticks / 10000.0));
          if (PrevCmdMs.ContainsKey(player.userID)) {
            long prevTicks = PrevCmdMs[player.userID];
            long diffTicks = currentTicks - prevTicks;
            if (diffTicks < config.FireInterval) {
              double interval = Math.Floor((config.FireInterval - diffTicks) / 100.0) / 10.0;
              SendMsg(player, M_FIRE_INTERVAL, interval.ToString());
              return;
            }
          }
          PrevCmdMs[player.userID] = currentTicks;

          Boolean handled = false;
          String subCmd = args[0];
          // safe mode
          if (subCmd == "on" || subCmd == "1") {
            if (!SafePlayer.Contains(player.userID)) {
              SafePlayer.Add(player.userID);
              SendMsg(player, M_SWITCH_TO_SAFE, player.displayName);
            } else {
              SendMsg(player, M_ALREADY_SAFE, player.displayName);
            }
            handled = true;
          } else if (subCmd == "off" || subCmd == "0") {
            if (SafePlayer.Contains(player.userID)) {
              SafePlayer.Remove(player.userID);
              SendMsg(player, M_SWITCH_TO_UNSAFE, player.displayName);
            } else {
              SendMsg(player, M_ALREADY_UNSAFE, player.displayName);
            }
            handled = true;
          }

          if (!handled) {
            SendMsg(player, M_HELP, SafePlayer.Contains(player.userID).ToString());
          }
          break;
        default:
          SendMsg(player, M_HELP, SafePlayer.Contains(player.userID).ToString());
          break;
      }
    }


    #endregion

    #region Hooks

    void Init() {
      config = Config.ReadObject<ModConfig>();
      Puts("FireInterval: {0}", config.FireInterval.ToString());
      // make safe players
      if (Interface.Oxide.DataFileSystem.ExistsDatafile("SafePvP")) {
        try {
          SafePlayer = Interface.Oxide.DataFileSystem.GetFile("SafePvP").ReadObject<List<ulong>>();
        } catch {
          SafePlayer = new List<ulong>();
        }
      } else {
        SafePlayer = new List<ulong>();
      }
    }


    void OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info) {
      if (info == null || info.InitiatorPlayer == null || entity == null) return;
      BasePlayer player = info.InitiatorPlayer;
      if (!player.IsConnected || !player.userID.IsSteamId()) return;

      // vs player
      if (!(entity is BasePlayer)) return;

      BasePlayer opponent = entity as BasePlayer;
      // SendMsg(player, "attack to " + opponent.userID.ToString());
      if (!opponent.IsConnected || !opponent.userID.IsSteamId()) return;
      // SendMsg(player, "attack from " + player.userID.ToString());

      // if (SafePlayer.Contains(player.userID)) { SendMsg(player, "SafePlayer Player Contains"); }
      // if (SafePlayer.Contains(opponent.userID)) { SendMsg(player, "SafePlayer Opponent Contains"); }

      if (SafePlayer.Contains(player.userID) || SafePlayer.Contains(opponent.userID)) {
        if (player.userID != opponent.userID) {
          SendMsg(player, M_WARNING, opponent.displayName);
          info.damageTypes.ScaleAll(0);
        }
      }
    }


    // save safe players when unload
    void Unload() { Interface.Oxide.DataFileSystem.GetFile("SafePvP").WriteObject(SafePlayer); }


    #endregion

  }
}
