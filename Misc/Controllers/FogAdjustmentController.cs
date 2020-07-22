﻿#if UNITY_EDITOR
using System;
using UdonSharp;
using UnityEngine;


namespace UdonToolkit {
  [CustomName("Fog Adjustment")]
  [ControlledBehaviour(typeof(FogAdjustment))]
  [HelpURL("https://github.com/orels1/UdonToolkit/wiki/Misc-Behaviours#fog-adjustment")]
  public class FogAdjustmentController : UTController {
    [HideInInspector] public bool isLinear;
    [SectionHeader("Defaults")] [UdonPublic]
    public Color defaultFogColor;

    [HideIf("@isLinear")][UdonPublic] public float defaultFogDensity;
    [HideIf("@!isLinear")][UdonPublic]
    public float defaultFogStart;
    [HideIf("@!isLinear")][UdonPublic]
    public float defaultFogEnd;

    [SectionHeader("Active State")] [UdonPublic]
    public Color activeFogColor;

    [HideIf("@isLinear")][UdonPublic] public float activeFogDensity;
    [HideIf("@!isLinear")][UdonPublic]
    public float activeFogStart;
    [HideIf("@!isLinear")][UdonPublic]
    public float activeFogEnd;
    [UdonPublic] [Tooltip("0 - Instant")] public float fogFadeTime;

    [SectionHeader("Extras")] [UdonPublic] public bool startActive;

    [HelpBox("Will set the fog color and density from the Default or Active state on start", "@setInitialState")]
    [UdonPublic]
    public bool setInitialState;

    public override void SetupController() {
      isLinear = RenderSettings.fogMode == FogMode.Linear;
    }

    [Button("ActivateFog")]
    public void ActivateFog() {
      if (uB != null) {
        uB.SendCustomEvent("ActivateFog");
      }
    }
    
    [Button("DeactivateFog")]
    public void DeactivateFog() {
      if (uB != null) {
        uB.SendCustomEvent("DeactivateFog");
      }
    }
    
    [Button("Trigger")]
    public void Trigger() {
      if (uB != null) {
        uB.SendCustomEvent("Trigger");
      }
    }
  }
}

#endif