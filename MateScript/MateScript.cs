using System;
using System.Collections.Generic;
using GTA;


namespace MateScript {
    class MateScript : Script {
        private GTA.Object mateObj = null;
        private GTA.Object thermosObj = null;
        // TODO: Use custom models.
        private readonly Model mateModel = "amb_icecone01";
        private readonly Model thermosModel = "amb_juice_bot";
        object matePtfxHandle = null;
        object thermosPtfxHandle = null;
        private readonly AnimationSet defaultSet = new AnimationSet("amb@icecream_default");
        private readonly AnimationSet destroySet = new AnimationSet("amb@icecream_destroy");
        private readonly AnimationSet idleSet = new AnimationSet("amb@icecream_idles");
        private readonly string defaultStandAnim = "stand_hold";
        private readonly string destroyWalkAnim = "walk_destroy";
        private readonly string idleWalkAnim = "walk_eat";
        private bool drinkingFlag = false;
        private Random cRandom = new Random();


        public MateScript() {
            this.Interval = 0;
            this.Tick += new EventHandler(this.MateScript_Tick);
        }

        private void MateScript_Tick(object sender, EventArgs e) {
            if (IsPlayerDrinking() && !drinkingFlag) {
                Game.Console.Print("This should not have happened");
                drinkingFlag = true;
                Player.Character.CanSwitchWeapons = false;
            } else if (!IsPlayerDrinking() && drinkingFlag) {
                Game.Console.Print("Drinking interrupted");
                drinkingFlag = false;
                Player.Character.CanSwitchWeapons = true;
                Player.Character.SayAmbientSpeech("ATTACK_ANY_GANGSTER");
                DestroyObjects();
            }

            if (IsPlayerAvailable() && !IsPlayerDrinking() && Game.isGameKeyPressed(GameKey.NavUp) && Game.isGameKeyPressed(GameKey.Sprint)) {
                StartDrinking();
                return;
            }

            if (IsPlayerDrinking() && Game.isGameKeyPressed(GameKey.NavLeft)) {
                TakeASip();
                return;
            }

            if (IsPlayerDrinking() && Game.isGameKeyPressed(GameKey.NavUp) && Game.isGameKeyPressed(GameKey.Sprint)) {
                StopDrinking();
                return;
            }
        }

        private bool IsPlayerAvailable() {
            return (
                Player.Character.isIdle
                && Player.Character.isAlive
                && !Player.Character.isInVehicle()
                && !Player.Character.isInWater
            );
        }

        private bool IsPlayerDrinking() {
            return (
                defaultSet.isPedPlayingAnimation(Player.Character, defaultStandAnim)
                || destroySet.isPedPlayingAnimation(Player.Character, destroyWalkAnim)
                || idleSet.isPedPlayingAnimation(Player.Character, idleWalkAnim)
            );
        }

        private void StartDrinking() {
            Game.Console.Print("Starting to drink");
            InitObjects();
            
            Player.Character.CanSwitchWeapons = false;
            Player.Character.Animation.Play(
                defaultSet, defaultStandAnim, 8.0f, AnimationFlags.Unknown05 | AnimationFlags.Unknown09 | AnimationFlags.Unknown11
            );
            drinkingFlag = true;
            Wait(2000);
        }

        private void TakeASip() {
            Game.Console.Print("Taking a sip");
            if (idleSet.isPedPlayingAnimation(Player.Character, idleWalkAnim)) return;

            Player.Character.Animation.Play(
                idleSet, idleWalkAnim, 8.0f, AnimationFlags.Unknown09 | AnimationFlags.Unknown11
            );

            // Manual waiting because waiting for anim to finish does not work.
            Wait(5000);

            Player.Character.Health += 5;
            if (cRandom.Next() % 3 == 0) {
                Player.Character.SayAmbientSpeech("THANKS");
            }

            StartDrinking();
        }

        private void StopDrinking() {
            Game.Console.Print("Stop Drinking");
            Player.Character.Animation.Play(
                destroySet, destroyWalkAnim, 8.0f, AnimationFlags.Unknown09 | AnimationFlags.Unknown11
            );
            Player.Character.Animation.WaitUntilFinished(destroySet, destroyWalkAnim);
            
            // Manual waiting because waiting for anim to finish does not work.
            Wait(6500);
            DestroyObjects();

            drinkingFlag = false;
            Player.Character.CanSwitchWeapons = true;
            Wait(2000);
        }

        private void InitObjects() {
            Game.Console.Print(String.Format(
                "Initializing Objects. mate: {0}, thermos: {1}",
                (uint)mateModel.Hash,
                (uint)thermosModel.Hash
            ));

            if (mateObj == null) {
                mateObj = World.CreateObject(mateModel, Player.Character.Position.Around(99.0F));
            }

            if (thermosObj == null) {
                thermosObj = World.CreateObject(thermosModel, Player.Character.Position.Around(99.0F));
            }

            while (!mateObj.Exists() || !thermosObj.Exists()) {
                Wait(0);
            }

            Game.Console.Print(String.Format(
                "mate and thermos spawned. Mate exists: {0}, Thermos exists: {1}",
                mateObj.Exists(), thermosObj.Exists()
            ));

            /* Adapted position for my models. With dummy models the possitions were:
            - Mate (ice cone) position and rotation:
                new Vector3(-0.02f, 0.02f, 0)
                new Vector3(0, 0, 0)
            - Thermos (bottle) position:
                new Vector3(0.05f, 0.05f, -0.05f)
            */

            mateObj.AttachToPed(
                Player.Character,
                Bone.RightHand,
                new Vector3(-0.02f, 0.12f, 0.01f),
                new Vector3(0, (float)(Math.PI / 9), (float)(-Math.PI / 4))
            );

            thermosObj.AttachToPed(
                Player.Character,
                Bone.RightUpperarmRoll,
                new Vector3(0.05f, 0f, -0.13f),
                new Vector3(0, 0, 0)
            );

            matePtfxHandle = GTA.Native.Function.Call<int>(
                "START_PTFX_ON_OBJ",
                "ambient_cig_smoke",
                mateObj,
                0.125f,
                -0.02f,
                0.01f,
                0.0f,
                0.0f,
                0.0f,
                0.0f
            );

            thermosPtfxHandle = GTA.Native.Function.Call<int>(
                "START_PTFX_ON_OBJ",
                "ambient_cig_smoke",
                thermosObj,
                0.125f,
                -0.02f,
                0.01f,
                0.0f,
                0.0f,
                0.0f,
                0.0f
            );
        }

        private void DestroyObjects() {
            if (mateObj != null && mateObj.Exists()) {
                mateObj.Detach();
            }

            if (thermosObj != null && thermosObj.Exists()) {
                thermosObj.Detach();
            }

            Wait(4000);

            if (mateObj != null && mateObj.Exists()) {
                mateObj.NoLongerNeeded();
                mateObj = null;
            }

            if (thermosObj != null && thermosObj.Exists()) {
                thermosObj.NoLongerNeeded();
                thermosObj = null;
            }

            if (matePtfxHandle != null && matePtfxHandle is int) {
                GTA.Native.Function.Call("STOP_PTFX", (int)matePtfxHandle);
                matePtfxHandle = null;
            }

            if (thermosPtfxHandle != null && thermosPtfxHandle is int) {
                GTA.Native.Function.Call("STOP_PTFX", (int)thermosPtfxHandle);
                thermosPtfxHandle = null;
            }
        }
    }
}