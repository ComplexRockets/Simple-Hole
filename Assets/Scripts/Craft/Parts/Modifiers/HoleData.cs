namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using System;
    using ModApi.Craft.Parts;
    using ModApi.Craft.Parts.Attributes;
    using ModApi.Design.PartProperties;
    using UnityEngine;

    [Serializable]
    [DesignerPartModifier("Hole")]
    [PartModifierTypeId("SimpleHole.Hole")]
    public class HoleData : PartModifierData<HoleScript>
    {
        [SerializeField]
        [DesignerPropertySpinner(0.01f, 10f, 0.5f, Label = "Width", AllowManualInput = true, ValidateManualInput = false)]
        public float width = 1;

        [SerializeField]
        [DesignerPropertySpinner(0.01f, 10f, 0.5f, Label = "Depth", AllowManualInput = true, ValidateManualInput = false)]
        public float height = 1;

        [SerializeField]
        [DesignerPropertySlider(Label = "CornerRadius", MinValue = 0f, MaxValue = 1f, NumberOfSteps = 1000)]
        private float _cornerRadius = 0;
        public float cornerRadius => _cornerRadius;

        [SerializeField]
        [DesignerPropertySlider(Label = "CornerRadius 1", MinValue = 0f, MaxValue = 1f, NumberOfSteps = 1000)]
        public float cornerRadius1 = 0;

        [SerializeField]
        [DesignerPropertySlider(Label = "CornerRadius 2", MinValue = 0f, MaxValue = 1f, NumberOfSteps = 1000)]
        public float cornerRadius2 = 0;

        [SerializeField]
        [DesignerPropertySlider(Label = "CornerRadius 3", MinValue = 0f, MaxValue = 1f, NumberOfSteps = 1000)]
        public float cornerRadius3 = 0;

        [SerializeField]
        [DesignerPropertySlider(Label = "CornerRadius 4", MinValue = 0f, MaxValue = 1f, NumberOfSteps = 1000)]
        public float cornerRadius4 = 0;

        [SerializeField]
        [DesignerPropertySlider(Label = "Resolution", MinValue = 3, MaxValue = 32, NumberOfSteps = 1000)]
        private int _cornerRes = 8;
        public int cornerRes => _cornerRes;

        [SerializeField]
        [DesignerPropertyToggleButton(Label = "Double Sided")]
        private bool _doubleSided = false;
        public bool doubleSided => _doubleSided;

        [SerializeField]
        [DesignerPropertyToggleButton(Label = "Auto Resize")]
        private bool _autoResize = false;
        public bool autoResize => _autoResize;

        protected override void OnDesignerInitialization(IDesignerPartPropertiesModifierInterface d)
        {
            d.OnAnyPropertyChanged(() => Script.UpdateMesh());
            d.OnPropertyChanged(() => _cornerRadius, (newVal, oldVal) =>
            {
                cornerRadius1 = newVal;
                cornerRadius2 = newVal;
                cornerRadius3 = newVal;
                cornerRadius4 = newVal;
                d.Manager.RefreshUI();
                Script.UpdateMesh();
            });

            d.OnValueLabelRequested(() => _cornerRadius, (float x) => (x * 100).ToString("n0") + " %");
            d.OnValueLabelRequested(() => cornerRadius1, (float x) => (x * 100).ToString("n0") + " %");
            d.OnValueLabelRequested(() => cornerRadius2, (float x) => (x * 100).ToString("n0") + " %");
            d.OnValueLabelRequested(() => cornerRadius3, (float x) => (x * 100).ToString("n0") + " %");
            d.OnValueLabelRequested(() => cornerRadius4, (float x) => (x * 100).ToString("n0") + " %");

            d.OnValueLabelRequested(() => width, (float x) => x.ToString("n2") + " m");
            d.OnValueLabelRequested(() => height, (float x) => x.ToString("n2") + " m");
        }
    }
}