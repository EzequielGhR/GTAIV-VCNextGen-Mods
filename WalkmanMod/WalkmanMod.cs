using System;
using GTA;


namespace WalkmanMod {
    class AnimationBlob {
        public readonly AnimationSet animationSet;
        public readonly string animationName;
        public AnimationBlob(string animSetName, string animName) {
            animationSet = new AnimationSet(animSetName);
            animationName = animName;
        }

        public bool IsPlaying(Ped ped) {
            return animationSet.isPedPlayingAnimation(ped, animationName);
        }

        public void WaitUntilFinished(Ped ped) {
            ped.Animation.WaitUntilFinished(animationSet, animationName);
        }

        public void Play(Ped ped) {
            ped.Animation.Play(animationSet, animationName, 8.0f);
        }

        public void Play(Ped ped, AnimationFlags flag) {
            ped.Animation.Play(animationSet, animationName, 8.0f, flag);
        }

        public void Loop(Ped ped) {
            Play(ped, AnimationFlags.Unknown05);
        }

        public void PlayWalking(Ped ped) {
            Play(ped, AnimationFlags.Unknown09 | AnimationFlags.Unknown11);
        }

        public void LoopWalking(Ped ped) {
            Play(ped, AnimationFlags.Unknown05 | AnimationFlags.Unknown09 | AnimationFlags.Unknown11);
        }
    }

    static class AnimationEnum {
        public static AnimationBlob START = new AnimationBlob("amb@music_create", "walk_start_mp3");
        public static AnimationBlob IDLE = new AnimationBlob("amb@music_hold", "walk_hold_mp3");
        public static AnimationBlob END = new AnimationBlob("amb@music_destroy", "walk_put_mp3_away");
    }

    class RadioEntry {
        public readonly int index;
        public readonly string name;

        public RadioEntry(int index, string name) {
            this.index = index;
            this.name = name;
        }

        public static implicit operator int(RadioEntry r) {
            return r.index;
        }

        public static implicit operator string(RadioEntry r) {
            return r.name;
        }
    }

    static class RadioEnum {
        public static RadioEntry V_ROCK = new RadioEntry(0, "V-Rock");
        public static RadioEntry WAVE_103 = new RadioEntry(1, "Wave 103");
        public static RadioEntry EMOTION_98_3 = new RadioEntry(2, "Emotion 98.3");
        public static RadioEntry FLASH_FM = new RadioEntry(3, "Flash FM");
        public static RadioEntry WILDSTYLE = new RadioEntry(4, "Wildstyle");
        public static RadioEntry FEVER_105 = new RadioEntry(5, "Fever 105");
        public static RadioEntry RADIO_ESPANTOSO = new RadioEntry(6, "Radio Espantoso");
        public static RadioEntry K_CHAT = new RadioEntry(7, "K-Chat");
        public static RadioEntry VCPR = new RadioEntry(8, "Vice City Public Radio");
        public static RadioEntry MIN = V_ROCK;
        public static RadioEntry MAX = VCPR;

        public static RadioEntry FromIndex(int index) {
            int effectiveIndex = index;
            if (effectiveIndex < MIN || effectiveIndex > MAX) {
                effectiveIndex %= (MAX + 1);
                if (effectiveIndex < MIN) {
                    effectiveIndex = MAX + effectiveIndex + 1;
                }
                
                Game.Console.Print(
                    String.Format(
                        "Index '{0}' out of bounds, using modular equivalent {1}",
                        index,
                        effectiveIndex
                    )
                );
            }

            if (effectiveIndex == WAVE_103) return WAVE_103;
            if (effectiveIndex == EMOTION_98_3) return EMOTION_98_3;
            if (effectiveIndex == FLASH_FM) return FLASH_FM;
            if (effectiveIndex == WILDSTYLE) return WILDSTYLE;
            if (effectiveIndex == FEVER_105) return FEVER_105;
            if (effectiveIndex == RADIO_ESPANTOSO) return RADIO_ESPANTOSO;
            if (effectiveIndex == K_CHAT) return K_CHAT;
            if (effectiveIndex == VCPR) return VCPR;
            if (effectiveIndex == V_ROCK) return V_ROCK;

            throw new Exception("Should not have gotten to this point");
        } 
    }

    class WalkmanMod : Script {
        private enum TuneOptions {
            NO_TUNE,
            TUNE_UP,
            TUNE_DOWN
        } 
        private int cooldown = 0;
        private GTA.Object walkmanObj = null;
        private readonly Model walkmanModel = "amb_mp3player";
        
        public WalkmanMod() {
            Interval = 1;
            Tick += new EventHandler(WalkmanMod_Tick);
        }

        private void WalkmanMod_Tick(object sender, EventArgs e) {
            if (ShouldToggleRadio()) {
                ToggleRadio();
            }

            TuneOptions tune = GetRadioTune();
            if (IsRadioEnabled() && tune != TuneOptions.NO_TUNE) {
                TuneRadio(tune);
            }

            cooldown = Math.Max(0, cooldown - 1);
        }

        private bool ShouldToggleRadio() {
            return (
                Player.Character.isIdle
                && Game.isGameKeyPressed(GameKey.Action)
                && Game.isGameKeyPressed(GameKey.NavDown)
                && cooldown == 0
            );
        }

        private void ToggleRadio() {
            if (IsRadioEnabled()) {
                AttachWalkmanToHand();
                AnimationEnum.START.PlayWalking(Player.Character);
                AnimationEnum.START.WaitUntilFinished(Player.Character);
                AnimationEnum.END.PlayWalking(Player.Character);
                AnimationEnum.END.WaitUntilFinished(Player.Character);
                Wait(1500);
                
                GTA.Native.Function.Call("SET_MOBILE_PHONE_RADIO_STATE", 0);
                DestroyWalkman();
            } else {
                SpawnWalkman();
                AnimationEnum.START.PlayWalking(Player.Character);
                AnimationEnum.START.WaitUntilFinished(Player.Character);
                AnimationEnum.END.PlayWalking(Player.Character);
                AnimationEnum.END.WaitUntilFinished(Player.Character);
                Wait(1500);

                AttachWalkmanToHip();

                GTA.Native.Function.Call("SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY", 1);
                GTA.Native.Function.Call("SET_MOBILE_PHONE_RADIO_STATE", 1);

                Game.DisplayText(GetRadioEntry());
            }
            cooldown = 100;
        }

        private bool IsRadioEnabled() {
            return GTA.Native.Function.Call<bool>("IS_MOBILE_PHONE_RADIO_ACTIVE");
        }

        private TuneOptions GetRadioTune() {
            if (cooldown != 0) return TuneOptions.NO_TUNE;
            if (Game.isGameKeyPressed(GameKey.Action) && Game.isGameKeyPressed(GameKey.NavLeft)) return TuneOptions.TUNE_DOWN;
            if (Game.isGameKeyPressed(GameKey.Action) && Game.isGameKeyPressed(GameKey.NavRight)) return TuneOptions.TUNE_UP;
            return TuneOptions.NO_TUNE;
        }

        private void TuneRadio(TuneOptions tune) {
            RadioEntry radio = GetRadioEntry();
            AttachWalkmanToHand();
            AnimationEnum.START.PlayWalking(Player.Character);
            AnimationEnum.START.WaitUntilFinished(Player.Character);


            if (tune == TuneOptions.TUNE_UP) {
                Game.Console.Print("Tunning radio up");
                radio = RadioEnum.FromIndex(radio + 1);
            } else if (tune == TuneOptions.TUNE_DOWN) {
                Game.Console.Print("Tunning radio down");
                radio = RadioEnum.FromIndex(radio - 1);
            }

            GTA.Native.Function.Call("RETUNE_RADIO_TO_STATION_INDEX", radio.index);

            AnimationEnum.END.PlayWalking(Player.Character);
            AnimationEnum.END.WaitUntilFinished(Player.Character);
            
            Wait(1500);

            AttachWalkmanToHip();

            Game.DisplayText(GetRadioEntry());
            cooldown = 100;
        }

        private RadioEntry GetRadioEntry() {
            Wait(100);

            int radio_index = GTA.Native.Function.Call<int>("GET_PLAYER_RADIO_STATION_INDEX");
            Game.Console.Print(String.Format("[DEBUG] Index returned: {0}", radio_index));
            return RadioEnum.FromIndex(radio_index);
        }

        private void SpawnWalkman() {
            if (walkmanObj == null) {
                walkmanObj = World.CreateObject(walkmanModel, Player.Character.Position.Around(99.0F));
            }

            while (!walkmanObj.Exists()) {
                Wait(0);
            }

            AttachWalkmanToHand();
        }

        private void DestroyWalkman() {
            if (walkmanObj != null && walkmanObj.Exists()) {
                walkmanObj.Detach();
                walkmanObj.NoLongerNeeded();
                walkmanObj = null;
            }
        }

        private void AttachWalkmanToHand() {
            if (walkmanObj != null && walkmanObj.Exists()) {
                walkmanObj.AttachToPed(
                    Player.Character,
                    Bone.RightHand,
                    new Vector3(0.07f, 0, 0.1f),
                    new Vector3(0, (float)Math.PI / 2.0f, 0)
                );
            }
        }

        private void AttachWalkmanToHip() {
            if (walkmanObj != null && walkmanObj.Exists()) {
                walkmanObj.AttachToPed(
                    Player.Character,
                    Bone.RightThigh,
                    new Vector3(-0.05f, 0, 0.05f),
                    new Vector3((float)Math.PI / 2.0f, 0, -(float)Math.PI / 6.0f)
                );
            }
        }
    }
}