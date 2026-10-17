using System;
using GTA;
using NAudioAdvancedIV;

namespace MateScript {
    class MateScript : Script {
        /* Adapted position for my models. With dummy models the possitions were:
            - Mate (ice cone) position and rotation:
                new Vector3(-0.02f, 0.02f, 0)
                new Vector3(0, 0, 0)
            - Thermos (bottle) position:
                new Vector3(0.05f, 0.05f, -0.05f)
        */
        private readonly GTA.Vector3 NIKO_MATE_POS = new GTA.Vector3(0.02f, 0.1f, 0.01f);
        private readonly GTA.Vector3 NIKO_MATE_ROT = new GTA.Vector3(0, (float)(Math.PI / 8), (float)(-Math.PI / 3));
        private readonly GTA.Vector3 NIKO_THERMOS_POS = new GTA.Vector3(0.05f, 0f, -0.13f);
        private readonly GTA.Vector3 NIKO_THERMOS_ROT = new GTA.Vector3(0, 0, 0);

        // Sound(s) initialization
        private Sound ruiditoDeMate = null;

        // Object initialization
        private GTA.Object mateObj = null;
        private GTA.Object thermosObj = null;
        
        // TODO: Add custom custom models instead of replacing? Might not be possible.
        private readonly Model mateModel = "amb_icecone01";
        private readonly Model thermosModel = "amb_juice_bot";

        // Other classvar initializations
        object matePtfxHandle = null;
        object thermosPtfxHandle = null;
        private readonly AnimationSet defaultSet = new AnimationSet("amb@icecream_default");
        private readonly AnimationSet destroySet = new AnimationSet("amb@icecream_destroy");
        private readonly AnimationSet idleSet = new AnimationSet("amb@icecream_idles");
        private readonly string defaultStandAnim = "stand_hold";
        private readonly string destroyWalkAnim = "walk_destroy";
        private readonly string idleWalkAnim = "walk_eat";
        private bool drinkingFlag = false;
        private readonly Random cRandom = new Random();


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
                DestroyObjectsAndSounds();
            }

            if (IsPlayerAvailable() && !IsPlayerDrinking() && Game.isGameKeyPressed(GameKey.NavDown) && Game.isGameKeyPressed(GameKey.Sprint)) {
                StartDrinking();
                return;
            }

            if (IsPlayerDrinking() && Game.isGameKeyPressed(GameKey.NavLeft)) {
                TakeASip();
                return;
            }

            if (IsPlayerDrinking() && Game.isGameKeyPressed(GameKey.NavDown) && Game.isGameKeyPressed(GameKey.Sprint)) {
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
            InitObjectsAndSounds();
            
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
            bool bSpecial = cRandom.Next() % 3 == 0;
            if (bSpecial) ruiditoDeMate.Play(Player.Character);

            // Manual waiting because waiting for anim to finish does not work.
            Wait(5000);

            Player.Character.Health += 5;
            if (bSpecial) Player.Character.SayAmbientSpeech("THANKS");

            StartDrinking();
        }

        private void StopDrinking() {
            Game.Console.Print("Stop Drinking");
            Player.Character.Animation.Play(
                destroySet, destroyWalkAnim, 8.0f, AnimationFlags.Unknown09 | AnimationFlags.Unknown11
            );
            Player.Character.Animation.WaitUntilFinished(destroySet, destroyWalkAnim);
            ruiditoDeMate.Play(Player.Character);
            
            // Manual waiting because waiting for anim to finish does not work.
            Wait(6500);
            DestroyObjectsAndSounds();

            drinkingFlag = false;
            Player.Character.CanSwitchWeapons = true;
            Wait(2000);
        }

        private void InitObjectsAndSounds() {
            Game.Console.Print(String.Format(
                "Initializing Objects. mate: {0}, thermos: {1}",
                (uint)mateModel.Hash,
                (uint)thermosModel.Hash
            ));

            if (ruiditoDeMate == null) {
                ruiditoDeMate = new Sound("ruiditodemate.wav", "scripts/MateSounds");
            }

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

            mateObj.AttachToPed(
                Player.Character,
                Bone.RightHand,
                NIKO_MATE_POS,
                NIKO_MATE_ROT
            );

            thermosObj.AttachToPed(
                Player.Character,
                Bone.RightUpperarmRoll,
                NIKO_THERMOS_POS,
                NIKO_THERMOS_ROT
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

        private void DestroyObjectsAndSounds() {
            if (mateObj != null && mateObj.Exists()) {
                mateObj.Detach();
            }

            if (thermosObj != null && thermosObj.Exists()) {
                thermosObj.Detach();
            }

            ruiditoDeMate?.Dispose();
            ruiditoDeMate = null;

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