using System;
using GTA;


namespace MeleeScript {
    class MeleeScript : Script {
        private Ped enemy;
        private int ogWantedLevel;
        private bool fightStarted = false;

        public MeleeScript() {
            ogWantedLevel = Player.WantedLevel;
            Interval = 50;
            Tick += new EventHandler(MeleeScript_Tick);
        }


        private void MeleeScript_Tick(object sender, EventArgs e) {
            if (!fightStarted && IsPlayerTargeting() && Player.Character.isInMeleeCombat) {
                enemy = GetTargetedPed();
                if (enemy == null || !enemy.isInMeleeCombat) return;

                fightStarted = true;
                enemy.Enemy = true;
                Game.PlayCreditsMusic();
            }

            if (!fightStarted) {
                ogWantedLevel = Player.WantedLevel;
                return;
            }

            if (!enemy.Exists() || Player.Character.isDead || enemy.isInjured) {
                fightStarted = false;
                enemy = null;
                Game.StopCreditsMusic();
                return;
            }

            MonitorPed(enemy);
            MonitorPed(Player);
        }

        // private void MeleeScript_Tick(object sender, EventArgs e) {
        //     if (!IsPlayerTargeting() || !Player.Character.isInMeleeCombat) {
        //         ogWantedLevel = Player.WantedLevel;
                
        //         if (playingMusic) Game.StopCreditsMusic();
        //         playingMusic = false;
        //         return;
        //     }
                
        //     enemy = GetTargetedPed();
        //     if (enemy == null || !enemy.isInMeleeCombat) {
        //         ogWantedLevel = Player.WantedLevel;

        //         if (playingMusic) Game.StopCreditsMusic();
        //         playingMusic = false;
        //         return;
        //     }

        //     Game.Console.Print(
        //         String.Format(
        //             "[INFO] - P. H.: {0}; E. H.: {1}; Music: {2}; WantedLevel: {3}",
        //             Player.Character.Health,
        //             enemy.Health,
        //             playingMusic,
        //             Player.WantedLevel
        //         )
        //     );

        //     if (!playingMusic) {
        //         Game.PlayCreditsMusic();
        //         playingMusic = true;
        //     }

        //     MonitorPed(enemy);
        //     MonitorPed(Player);
        // }

        private bool IsPlayerTargeting() {
            return GTA.Native.Function.Call<bool>(
                "IS_PLAYER_TARGETTING_ANYTHING",
                Player.ID
            );
        }

        private Ped GetTargetedPed() {
            Ped[] surrounding = World.GetPeds(Player.Character.Position, 20.0f);
            if (surrounding.Length == 0) return null;

            float distToPlayer;
            float distToExpected;
            Vector3 expectedPos;
            foreach (Ped ped in surrounding) {
                if (ped == Player.Character) continue;

                distToPlayer = ped.Position.DistanceTo(Player.Character.Position);
                expectedPos = Player.Character.Position + distToPlayer * Player.Character.Direction;
                distToExpected = expectedPos.DistanceTo(ped.Position);
                if (distToExpected < 0.5f) return ped;
            }

            return null;
        }

        private void MonitorPed(Ped ped) {
            if (ped.Health < 80) {
                ped.Health++;
            }

            ped.WantedByPolice = false;
        }

        private void MonitorPed(Player player) {
            player.WantedLevel = ogWantedLevel;
            MonitorPed(Player.Character);
        }
    }
}