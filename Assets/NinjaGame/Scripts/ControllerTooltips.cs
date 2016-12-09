using UnityEngine;
using UnityEngine.UI;

namespace Assets.NinjaGame.Scripts
{

    public class ControllerTooltips : MonoBehaviour
    {


        [Tooltip("The text that is displayed on the tooltip.")]
        public string displayText;
        [Tooltip("The size of the text that is displayed.")]
        public int fontSize = 14;

        private LineRenderer line;
        // Use this for initialization
        void Start()
        {
            SetContainer();
            SetLine();
        }

        private void SetContainer()
        {
            transform.FindChild("TooltipCanvas").GetComponent<RectTransform>().sizeDelta = containerSize;
            var tmpContainer = transform.FindChild("TooltipCanvas/UIContainer");
            tmpContainer.GetComponent<RectTransform>().sizeDelta = containerSize;
            tmpContainer.GetComponent<Image>().color = containerColor;
        }

        private void SetLine()
        {
            line = transform.FindChild("Line").GetComponent<LineRenderer>();
            line.material = Resources.Load("TooltipLine") as Material;
            line.material.color = lineColor;
            line.SetColors(lineColor, lineColor);
            line.SetWidth(lineWidth, lineWidth);
            if (drawLineFrom == null)
            {
                drawLineFrom = transform;
            }
        }

        private void DrawLine()
        {
            if (drawLineTo)
            {
                line.SetPosition(0, drawLineFrom.position);
                line.SetPosition(1, drawLineTo.position);
            }
        }

        private void Update()
        {
            DrawLine();
        }
    }
}