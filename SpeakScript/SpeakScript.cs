using System;
using GTA;


namespace SpeakScript {
    class SpeakScript : Script {
        private Ped target = null;
        private readonly Random cRandom = new Random();

        public SpeakScript() {
            this.Interval = 0;
            this.Tick += new EventHandler(this.SpeakScript_Tick);
        }

        private void SpeakScript_Tick(object sender, EventArgs e) {
            target = null;
            if (IsPlayerAvailable() && IsPlayerTargeting()) {
                target = GetTargetedPed();
            }

            if (target == null) return;

            if (Game.isGameKeyPressed(GameKey.NavDown)) {
                AntagonizeTarget();
                return;
            }

            if (Game.isGameKeyPressed(GameKey.NavUp)) {
                GreetTarget();
                return;
            }
        }

        private bool IsPlayerAvailable() {
            return (
                Player.Character.isAlive
                && !Player.Character.isInAir
                && !Player.Character.isInWater
                && !Player.Character.isInVehicle()
            );
        }

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

        private void AntagonizeTarget() {
            if (target == null) {
                Game.Console.Print("[ERROR] (AntagonizeTarget) Target should not be null at this point");
                return;
            }

            bool targetIsCop = target.RelationshipGroup == RelationshipGroup.Cop;
            if (Player.Character.isInMeleeCombat) {
                Player.Character.SayAmbientSpeech("ATTACK_ANY_GANGSTER");
            } else {
                string phraseId = targetIsCop ? "CHASED" : "GENERIC_INSULT";
                Game.Console.Print("Player Phrase ID: " + phraseId);

                Player.Character.SayAmbientSpeech(phraseId);
            }

            SetTargetHate();

            if (target.isInMeleeCombat) {
                WaitForAmbientSpeech(Player.Character);
                target.SayAmbientSpeech("FIGHT");
            } else {
                target.Task.TurnTo(Player.Character);
                WaitForAmbientSpeech(Player.Character);
                target.SayAmbientSpeech(targetIsCop ? "FIGHT" : "GENERIC_DEJECTED");
            }
            WaitForAmbientSpeech(target);
        }

        private void GreetTarget() {
            if (target == null) {
                Game.Console.Print("[ERROR] (GreetTarget) Target should not be null at this point");
                return;
            }

            if (Player.Character.isInMeleeCombat) return;

            string playerPhraseId, tActivePhraseId;
            string tBasicPhraseId = "THANKS";
            string tSpecialPhraseId = "THANKS";
            bool isTargetCop = false;
            bool shouldIgnorePlayer = false;

            switch (target.RelationshipGroup) {
                case RelationshipGroup.Civillian_Female:
                    playerPhraseId = "HOOKER_QUIET";
                    shouldIgnorePlayer = cRandom.Next() % 3 != 0; // 2/3 chances of 
                    tSpecialPhraseId = "GENERIC_HI";
                    break;
                case RelationshipGroup.Prostitute:
                    playerPhraseId = "HOOKER_RELIEF";
                    shouldIgnorePlayer = cRandom.Next() % 3 == 0; // 1/3 chances of ignoring
                    tSpecialPhraseId = "SOLICIT";
                    break;
                case RelationshipGroup.Cop:
                    playerPhraseId = "PULL_GUN";
                    isTargetCop = true;
                    tBasicPhraseId = "BLOCKED_PED";
                    break;
                default:
                    playerPhraseId = "THANKS";
                    break;
            }

            Game.Console.Print(String.Format(
                "Player phrase ID: {0}; Target is a Cop: {1}; Target Ignore: {2}",
                playerPhraseId, isTargetCop, shouldIgnorePlayer
            ));

            Player.Character.SayAmbientSpeech(playerPhraseId);

            if (isTargetCop || shouldIgnorePlayer) {
                tActivePhraseId = tBasicPhraseId;
            } else {
                target.Task.TurnTo(Player.Character);
                tActivePhraseId = tSpecialPhraseId;
            }

            Game.Console.Print(String.Format("Target Phrase ID: {0}", tActivePhraseId));

            WaitForAmbientSpeech(Player.Character);
            target.SayAmbientSpeech(tActivePhraseId);
            WaitForAmbientSpeech(target);
        }

        private void SetTargetHate() {
            if (cRandom.Next() % 3 != 0) return;

            bool targetIsCop = target.RelationshipGroup == RelationshipGroup.Cop;
            if (targetIsCop) {
                Player.WantedLevel++;
            } else {
                target.Task.FightAgainst(Player.Character);
                Wait(500);
            }
        }

        private void WaitForAmbientSpeech(Ped ped) {
            while (GTA.Native.Function.Call<bool>("IS_AMBIENT_SPEECH_PLAYING", ped)) {
                Wait(0);
            }
        }
        
    }
}