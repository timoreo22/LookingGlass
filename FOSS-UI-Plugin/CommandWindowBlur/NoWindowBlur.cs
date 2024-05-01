﻿using BepInEx.Configuration;
using FOSSUI.Base;
using LeTai.Asset.TranslucentImage;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOSSUI.CommandWindowBlur
{
    internal class NoWindowBlur : BaseThing
    {
        public static ConfigEntry<bool> disable;
        private static Hook overrideHook;
        public NoWindowBlur()
        {
            Setup();
        }
        public void Setup()
        {
            disable = BasePlugin.instance.Config.Bind<bool>("Settings", "Disable Command Window Blur", true, "Disable the background blur effect from the command window");
            InitHooks();
        }
        void InitHooks()
        {
            var targetMethod = typeof(RoR2.PickupPickerController).GetMethod(nameof(RoR2.PickupPickerController.OnDisplayBegin), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(NoWindowBlur).GetMethod(nameof(OnDisplayBegin), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        void OnDisplayBegin(Action<RoR2.PickupPickerController, NetworkUIPromptController, LocalUser, CameraRigController> orig, RoR2.PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {
            orig(self, networkUIPromptController, localUser, cameraRigController);
            TranslucentImage t = self.panelInstance.gameObject.GetComponentInChildren<TranslucentImage>();
            if (t is not null)
            {
                t.enabled = !disable.Value;
            }
        }
    }
}
