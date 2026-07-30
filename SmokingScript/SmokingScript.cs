using System;
using GTA;


namespace SmokingScript {
    class SmokingScript : Script {
        private GTA.Object cigarObj = null;
        private readonly Model cigarModel = "bm_char_fag_f";
        private readonly int cooldown = 100;
        private int hold = 0;
        private readonly int holdThreshold = 200;
        private int iterations = 0;
        private readonly AnimationFlags flagEnableWalk = AnimationFlags.Unknown09 | AnimationFlags.Unknown11;
        private readonly AnimationFlags flagLoopAndWalk = AnimationFlags.Unknown05 | AnimationFlags.Unknown09 | AnimationFlags.Unknown11;
        private object pfxHandle = null;
        private bool smoking = false;
        private readonly AnimationSet smkCreateSet = new AnimationSet("amb@smoking_create");
        private readonly string smkCreateAnimName = "walk_create";
        private readonly AnimationSet smkDestroySet = new AnimationSet("amb@smoking_destroy");
        private readonly string smkDestroyAnimName = "walk_destroy";

        public SmokingScript() {
            this.Interval = 1;
            this.Tick += new EventHandler(this.SmokingScript_Tick);
        }

        private void SmokingScript_Tick(object sender, EventArgs e) {
            iterations = (iterations + 1) % 1000;
            if (!IsPlayerSmoking(true) && IsPlayerAvailable() && hold > holdThreshold) {
                StartSmoking();
                return;
            }
            
            if (IsPlayerSmoking() && hold > holdThreshold) {
                StopSmoking();
                return;
            }
            
            if (Game.isGameKeyPressed(GameKey.NavDown) && hold <= holdThreshold) {
                hold += this.Interval > 0 ? this.Interval : 1;
                return;
            }

            hold = 0;
            if (IsPlayerSmoking(true) && iterations % 200 == 0) {
                Player.Character.Health += this.Interval > 0 ? this.Interval : 1;
            } else if (!IsPlayerSmoking(true) && smoking) {
                Game.Console.Print("Interrupted Smoking");
                smoking = false;
                Player.Character.SayAmbientSpeech("GENERIC_CURSE");
                DestroyCigar();
            }
        }

        private bool IsPlayerSmoking() {
            return smoking && smkCreateSet.isPedPlayingAnimation(Player.Character, smkCreateAnimName);
        }

        private bool IsPlayerSmoking(bool checkStopping) {
            bool condition = IsPlayerSmoking();
            if (checkStopping) {
                condition = condition || smoking && smkDestroySet.isPedPlayingAnimation(Player.Character, smkDestroyAnimName);
            }
            return condition;
        }

        private bool IsPlayerAvailable() {
            return (
                Player.Character.isIdle
                && Player.Character.isAlive
                && !Player.Character.isInVehicle()
                && !Player.Character.isInWater
            );
        }

        private void StartSmoking() {
            Player.Character.SayAmbientSpeech("GENERIC_BUY");
            cigarObj = GetCigar();
            cigarObj.AttachToPed(
                Player.Character,
                Bone.RightHand,
                new Vector3(0.015f, 0.015f, -0.021f), // OG: 0.015f, -0.005f, -0.021f
                new Vector3(0.0f, 0.0f, 0.0f)
            );

            pfxHandle = GTA.Native.Function.Call<int>(
                "START_PTFX_ON_OBJ",
                "ambient_cig_smoke",
                cigarObj,
                0.125f,
                -0.02f,
                0.01f,
                0.0f,
                0.0f,
                0.0f,
                1.1f
            );
            
            Player.Character.Animation.Play(
                smkCreateSet,
                smkCreateAnimName,
                8.0f,
                flagLoopAndWalk
            );

            hold = 0;
            smoking = true;

            Game.Console.Print("Smoking started...");
            WaitForCooldown();
        }

        private void StopSmoking() {
            cigarObj = GetCigar();
                
            Player.Character.Animation.Play(
                smkDestroySet,
                smkDestroyAnimName,
                8.0f,
                flagEnableWalk
            );
            
            Player.Character.Animation.WaitUntilFinished(smkDestroySet, smkDestroyAnimName);
            
            Wait(10000);
            DestroyCigar();

            hold = 0;
            smoking = false;

            Game.Console.Print("Smoking Stopped...");
            Player.Character.SayAmbientSpeech("THANKS");
            WaitForCooldown();
        }

        private void WaitForCooldown() {
            for (int i = 0; i < cooldown; i++) {
                Wait(this.Interval);
            }
        }

        private GTA.Object GetCigar() {
            if (cigarObj != null && cigarObj.Exists()) return cigarObj;
            cigarObj = World.CreateObject(cigarModel, Player.Character.Position.Around(99.0F));
            while (!cigarObj.Exists()) {
                Wait(0);
            }
            return cigarObj;
        }

        private void DestroyCigar() {
            if (cigarObj == null) return;
            
            if (!cigarObj.Exists()) {
                cigarObj = null;
                return;
            }

            if (pfxHandle != null && pfxHandle is int) {
                GTA.Native.Function.Call("STOP_PTFX", (int)pfxHandle);
            }
            
            cigarObj.Detach();
            Wait(2000);

            cigarObj.Delete();
            cigarObj = null;
        }
    }
}