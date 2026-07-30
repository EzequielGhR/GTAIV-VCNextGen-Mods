using System;
using System.Collections.Generic;
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

        public SwimmingLimitScript() {
            this.Interval = 100;
            this.Tick += new EventHandler(this.SwimmingLimitScript_Tick);
        }

        private void SwimmingLimitScript_Tick(object sender, EventArgs e) {
            DrawStaminaBar();
            multiplier = GetMultiplier();
            if (Player.Character.isInWater && stamina > 0) {
                stamina -= multiplier;
                stamina = stamina < 0 ? 0 : stamina;
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
            foreach (string anim in animMultipliers.Keys) {
                if (swimmingSet.isPedPlayingAnimation(Player.Character, anim)) {
                    return animMultipliers[anim];
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
            Game.DisplayText(stringBuilder.ToString());
        }
    }
}