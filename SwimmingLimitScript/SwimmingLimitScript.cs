using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTA;


namespace SwimmingLimitScript {
    class SwimmingLimitScript : Script {
        private float stamina = 100.0f;
        private float multiplier = 0.0f;
        private readonly  AnimationSet swimmingSet = new AnimationSet("swimming");
        private readonly Dictionary<string, float> animMultipliers = new Dictionary<string, float> {
            { "idle", 0.05f },
            { "wstart", 0.05f },
            { "walk", 0.25f },
            { "walk_strafe_b", 0.25f },
            { "walk_strafe_l", 0.25f },
            { "walk_strafe_r", 0.25f },
            { "walk_turn_l", 0.25f },
            { "walk_turn_r", 0.25f },
            { "wstop_l", 0.25f },
            { "wstop_r", 0.25f },
            { "runstart_fwd", 0.25f },
            { "run", 0.5f },
            { "rstop_l", 0.5f },
            { "rstop_r", 0.5f },
            { "run_turn_l", 0.5f },
            { "run_turn_r", 0.5f },
            { "sprint", 1.75f },
            { "sprint_turn_l", 1.75f },
            { "sprint_turn_r", 1.75f },
        };
        private readonly string statsPath = "scripts\\swimming_stats.txt";
        private readonly float maxMinutesSwam = 60 * 12;
        private float minutesSwam;

        public SwimmingLimitScript() {
            this.minutesSwam = LoadMinutesSwam();
            this.Interval = 100;
            this.Tick += new EventHandler(this.SwimmingLimitScript_Tick);
        }

        private void SwimmingLimitScript_Tick(object sender, EventArgs e) {
            DrawStaminaBar();
            multiplier = GetMultiplier();
            if (Player.Character.isSwimming && minutesSwam < maxMinutesSwam * 0.1f) {
                Player.Character.Velocity = 2 * minutesSwam / maxMinutesSwam * Player.Character.Velocity;
            }

            if (Player.Character.isInWater && stamina > 0) {
                SaveMinutesSwam();
                stamina -= multiplier;
                stamina = stamina < 0 ? 0 : stamina;
                if (minutesSwam < maxMinutesSwam) {
                    minutesSwam += (float)(Interval > 0 ? Interval : 1) / (1000 * 60);
                } else {
                    minutesSwam = maxMinutesSwam;
                }
                return;
            }

            if (Player.Character.isInWater && Player.Character.Health > 0) {
                Player.Character.Health -= (int)Math.Ceiling(multiplier);
                return;
            }

            if (Player.Character.isInWater) {
                Player.Character.Die();
                return;
            }

            stamina = stamina >= 100 ? 100.0f : stamina + 2.0f;
        }

        private float GetMultiplier() {
            // Linear: At 1% the factor is aprox 1.0, at 100% is aprox 0.02
            float slope = (0.02f - 1.0f) / (maxMinutesSwam - maxMinutesSwam * 0.01f);
            float intercept = 1.0f - slope * maxMinutesSwam * 0.01f;
            float factor = minutesSwam > maxMinutesSwam * 0.01f ? slope * minutesSwam + intercept : 1;

            foreach (string anim in animMultipliers.Keys) {
                if (swimmingSet.isPedPlayingAnimation(Player.Character, anim)) {
                    return animMultipliers[anim] * factor;
                }
            }
            return 0.0f;
        }

        private void DrawStaminaBar() {
            if (!Player.Character.isInWater && stamina > 99.9f) return;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < (int)Math.Ceiling(stamina / 4); i++) {
                stringBuilder.Append("█");
            }
            Game.DisplayText(String.Format(
                "{0}\nAbility: {1}%",
                stringBuilder.ToString(),
                (100 * minutesSwam  / maxMinutesSwam).ToString("F2")
            ));
        }

        private float LoadMinutesSwam() {
            float minutes = 0;
            Game.Console.Print("Loading swimming config file");
            try {
                StreamReader reader = new StreamReader(statsPath);
                string data = reader.ReadLine();
                minutes = float.Parse(data.Split(':')[1].Trim());
                reader.Close();
            } catch (Exception e) {
                Game.Console.Print(String.Format(
                    "[ERROR] Could not open swimming stats: {0} {1}",
                    e.GetType(),
                    e.Data
                ));
            }
        
            return minutes;
        }

        private void SaveMinutesSwam() {
            if (!Player.Character.isSwimming) return;

            Game.Console.Print(
                String.Format("[INFO] Saving swimming config file. Current minutesSwam: {0}", minutesSwam)
            );
            try {
                StreamWriter writter = new StreamWriter(statsPath);
                writter.WriteLine(String.Format("minutes: {0}", minutesSwam));
                writter.Close();
            } catch (Exception e) {
                Game.Console.Print(
                    String.Format("[ERROR] There was an issue writing stats: {0} {1}", e.GetType(), e.Data)
                );
            }
        }
    }
}