using System;
using System.Collections.Generic;
using GTA;


namespace TommyWardrobe {
    public enum SkinHandle {
        DEFAULT,
        SUIT,
        SPANDEX,
        GOLF,
        HAITIAN,
        POLICE,
        JASON,
        TSHIRT,
        VSUIT,
        SPORTB,
        SPORTR,
        LANCE,
        FORELI,
        COMPLETEHD,
        COMPLETE,
        _SENTINEL_VALUE
    }

    class TommyWardrobe : Script {
        const int TORSO_PEDCOMP = 1;
        const int BASE_TEXTURE = 0;
        
        private readonly Vector3 WARDROBE_POS = new Vector3(664.13f, -729.47f, 15.07f);
        private readonly float WARDROBE_HEADING = 169.59f;
        private readonly Dictionary<SkinHandle, string> SKIN_LABELS = new Dictionary<SkinHandle, string> {
            {SkinHandle.DEFAULT, "Default Outfit"},
            {SkinHandle.SUIT, "Semi Formal Suit"},
            {SkinHandle.SPANDEX, "Spand Ex Uniform"},
            {SkinHandle.GOLF, "Golfing Outfit"},
            {SkinHandle.HAITIAN, "Haitian Outfit"},
            {SkinHandle.POLICE, "Police Uniform"},
            {SkinHandle.JASON, "Heist Outfit"},
            {SkinHandle.TSHIRT, "Casual Outfit"},
            {SkinHandle.VSUIT, "Vercetti Suit"},
            {SkinHandle.SPORTB, "Black Sport Outfit"},
            {SkinHandle.SPORTR, "Red Sport Outfit"},
            {SkinHandle.LANCE, "Lance Outfit"},
            {SkinHandle.FORELI, "Foreli Outfit"},
            {SkinHandle.COMPLETE, "Complete Game Outfit"},
            {SkinHandle.COMPLETEHD, "Complete Game HD Outfit"}
        };

        private bool menuOpen = false;
        private SkinHandle currentClothes = SkinHandle.DEFAULT;
        private SkinHandle selectedOption = SkinHandle.DEFAULT;

        public TommyWardrobe() {
            Interval = 1;
            Tick += new EventHandler(TommyWardrobe_Tick);
            BindConsoleCommand(
                "SetClothes",
                new ConsoleCommandDelegate(SetClothes),
                String.Format(
                    "Provide an index from 0 to {0}, to select a clothes set",
                    (int)(SkinHandle._SENTINEL_VALUE - 1)
                )
            );
        }

        private void TommyWardrobe_Tick(object sender, EventArgs e) {
            if (IsPlayerAroundWardrobe() && !menuOpen) {
                ShowMenuPopup();                
            } else if (!IsPlayerAroundWardrobe() && menuOpen) {
                CloseWardrobe();
            } else if (menuOpen) {
                MenuFunctionality();
            }
        }

        private bool IsPlayerAroundWardrobe() {
            return Player.Character.Position.DistanceTo(WARDROBE_POS) < 2;
        }

        private void ShowMenuPopup() {
            // TODO: Add something nicer
            Game.DisplayText("Open Wardrobe: Y");

            if (Game.isGameKeyPressed(GameKey.EnterCar)) {
                OpenWardrobe();
            }
        }

        private void CloseWardrobe() {
            menuOpen = false;
            Player.Character.CanSwitchWeapons = true;
        }

        private void CloseWardrobe(bool keepSelected) {
            if (!keepSelected) {
                Game.FadeScreenOut(500, true);
                SetClothes(currentClothes);
                Game.FadeScreenIn(500, true);
            }

            CloseWardrobe();
        }

        private void MenuFunctionality() {
            // TODO: Add some drawing
            Game.DisplayText(String.Format(
                "Currently Selected: {0}\n"
                + "Press <-, -> to navigate options\n"
                + "Press A to select option\n"
                + "Press Y to cancel",
                SKIN_LABELS[selectedOption]
            ));

            if (Game.isGameKeyPressed(GameKey.NavRight)) {
                selectedOption = (SkinHandle)((int)(selectedOption + 1) % (int)SkinHandle._SENTINEL_VALUE);
                Game.FadeScreenOut(500, true);
                SetClothes(selectedOption);
                Game.FadeScreenIn(500);
                return;
            }

            if (Game.isGameKeyPressed(GameKey.NavLeft)) {
                selectedOption = selectedOption == SkinHandle.DEFAULT ? SkinHandle._SENTINEL_VALUE - 1 : selectedOption - 1;
                Game.FadeScreenOut(500, true);
                SetClothes(selectedOption);
                Game.FadeScreenIn(500);
                return;
            }

            if (Game.isGameKeyPressed(GameKey.Sprint)) {
                CloseWardrobe(true);
                return;
            }

            if (Game.isGameKeyPressed(GameKey.EnterCar)) {
                CloseWardrobe(false);
                return;
            }
        }

        private void OpenWardrobe() {
            Game.FadeScreenOut(1000, true);

            Player.TeleportTo(WARDROBE_POS);
            Player.Character.Heading = WARDROBE_HEADING;
            Player.Character.CanSwitchWeapons = false;
            selectedOption = currentClothes = (SkinHandle)GetCurrentClothes();

            menuOpen = true;

            Game.FadeScreenIn(1000, true);
        }

        public void SetClothes(ParameterCollection strings) {
            string param;

            try {
                param = strings[0];
            } catch(Exception exc) {
                Game.Console.Print(String.Format("ERROR: {0}", exc.Data));
                return;
            }

            if (param == null || param.Length == 0) {
                Game.Console.Print(String.Format("Invalid Param: {0}", param));
                return;
            }

            int idx = 0;
            try {
                idx = int.Parse(param);
            } catch (Exception exc) {
                Game.Console.Print(String.Format("ERROR: {0}", exc.Data));
                return;
            }

            if (idx >= (int)SkinHandle._SENTINEL_VALUE) {
                Game.Console.Print(
                    String.Format(
                        "Provided index is bigger than expected. Max: {0}",
                        (int)(SkinHandle._SENTINEL_VALUE - 1)
                    )
                );
            }

            SetClothes((SkinHandle)idx);
        }

        private void SetClothes(SkinHandle handle) {
            GTA.Native.Function.Call(
                "SET_CHAR_COMPONENT_VARIATION",
                Player.Character,
                TORSO_PEDCOMP,
                (int)handle,
                BASE_TEXTURE
            );
        }

        private int GetCurrentClothes() {
            return GTA.Native.Function.Call<int>(
                "GET_CHAR_DRAWABLE_VARIATION",
                Player.Character,
                TORSO_PEDCOMP
            );
        }
    }
}