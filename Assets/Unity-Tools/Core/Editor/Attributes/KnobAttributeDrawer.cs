#if ODIN_INSPECTOR
using System.Globalization;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    public class KnobAttributeDrawer : OdinAttributeDrawer<KnobAttribute, float>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Get a sufficiently sized rect.
            var rect = EditorGUILayout.GetControlRect(true, 94f);

            // Handle the actual interaction.
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDrag && Event.current.IsHovering(rect))
            {
                this.ValueEntry.SmartValue += Event.current.delta.x;
            }

            // The normal prefix label.
            rect = EditorGUI.PrefixLabel(rect, label);

            // Define the rects needed for the dots along the perimeter and the knob itself.
            var pointsRect = rect.AlignLeft(90f);
            var knobRect = pointsRect.AlignCenterXY(80f);

            // Draw the knob background
            GUI.DrawTexture(knobRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(0.16f, 0.16f, 0.16f), 0f, float.MaxValue);
            GUI.DrawTexture(knobRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(0.05f, 0.05f, 0.05f), 1.25f, float.MaxValue);

            // Draw the knob itself.
            GUI.DrawTexture(knobRect.Padding(6f), Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(0.2f, 0.2f, 0.2f), 0f, float.MaxValue);
            GUI.DrawTexture(knobRect.Padding(6f), Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(0.4f, 0.4f, 0.4f), 1.25f, float.MaxValue);

            // Step settings.
            const int pointCount = 20; // Number of steps around the circle.
            var radius = knobRect.width * 0.5f + 4f;
            var currentValueAngle = Mathf.Lerp(0f, 360f, Mathf.InverseLerp(this.Attribute.Min, this.Attribute.Max, this.ValueEntry.SmartValue));

            // Draw point indicators.
            for (var i = 0; i < pointCount; i++)
            {
                var angle = i * (360f / pointCount) - 90f;
                var radianAngle = angle * Mathf.Deg2Rad;
                var pointColor = angle + 90f <= currentValueAngle ? Color.white : new Color(0.4f, 0.4f, 0.4f);

                // Calculate step position along the circle's perimeter.
                var pointPosition = new Vector2(
                    knobRect.center.x + Mathf.Cos(radianAngle) * radius,
                    knobRect.center.y + Mathf.Sin(radianAngle) * radius
                );

                // adjust position slightly to account for small errors.
                pointPosition.x += 1.5f;
                pointPosition.y += 1f;

                // Define the point's rect.
                var pointRect = new Rect(pointPosition.x - 2, pointPosition.y - 2, 2, 2);

                // Draw the point.
                GUI.DrawTexture(pointRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, pointColor, 0f, float.MaxValue);
            }

            // Define the rect for the value.
            var knobValueRect = knobRect.AlignCenterXY(32f, 16f);

            // Draw background and border for the value.
            GUI.DrawTexture(knobValueRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(0.16f, 0.16f, 0.16f), 0f, 3f);
            GUI.DrawTexture(knobValueRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(0.40f, 0.40f, 0.40f), 1.25f, 3f);

            // Draw the value.
            GUI.Label(knobValueRect, this.ValueEntry.SmartValue.ToString("0", CultureInfo.InvariantCulture), SirenixGUIStyles.MiniLabelCentered);

            // Clamp the value to ensure it stays within bounds.
            this.ValueEntry.SmartValue = Mathf.Clamp(this.ValueEntry.SmartValue, this.Attribute.Min, this.Attribute.Max);

            // Save the current GUI matrix and apply rotation.
            var originalMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(currentValueAngle, knobRect.center);

            // Draw the pointer / indicator.
            GUI.DrawTexture(knobRect.AlignTop(8f).AlignCenterX(4f).AddY(12f), Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1f, Color.white, 0f, float.MaxValue);

            // Restore the original matrix after drawing the rotated knob.
            GUI.matrix = originalMatrix;
        }
    }
}
#endif